using ApplicationCore.Interfaces;
using Infrastructure.Data.Mongo;
using Infrastructure.Data.Mongo.Entity;
using Infrastructure.Data.Mongo.Repository;
using Infrastructure.Service.MongoDB;
using Infrastructure.Services.Linebot.SemanticProductSearch.Dtos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel.Connectors.MongoDB;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel.Memory;
using MongoDB.Driver;
using System.Diagnostics.CodeAnalysis;

namespace Infrastructure.Services.Linebot.SemanticProductSearch
{
    [Experimental("SKEXP0020")]
    public class SemanticProductSearchService
    {
        private readonly SemanticKernelSearchService _mongoDBService;
        private readonly MemoryBuilder _memoryBuilder;
        private readonly ISemanticTextMemory _semanticTextMemory;
        private readonly MongoDbVectorSettings _mongoDbSettings;
        private readonly MongoDBMemoryStore _mongoDBMemoryStore;
        private readonly string _openAiApiKey;
        private readonly string _connectionString;
        private readonly string _searchIndexName;
        private readonly string _databaseName;
        private readonly string _collectionName;
        private readonly IMongoClient _mongoClient;
        private readonly ILogger<SemanticProductSearchService> _logger;

        public SemanticProductSearchService(MongoDbVectorSettings mongoDbSettings, IMongoClient mongoClient,
            IConfiguration configuration, ILogger<SemanticProductSearchService> logger, MongoRepository<Products> productInMongoRepository, SemanticKernelSearchService mongoDBService)
        {
            // Initialize the openAI API key for text embedding generation
            _openAiApiKey = configuration["OpenAIApiKey"];
            // Initialize the mongodb settings: connection string, search index name, database name, collection name
            _mongoDbSettings = mongoDbSettings;
            _connectionString = _mongoDbSettings.ConnectionString;
            _searchIndexName = _mongoDbSettings.SearchIndexName;
            _databaseName = _mongoDbSettings.DatabaseName;
            //_collectionName = _mongoDbSettings.VectorCollectionName;
            // Initialize the memory store: MongoDBMemoryStore(or you can use other memory store like QdrantMemoryStore, etc.)
            _mongoDBMemoryStore = new MongoDBMemoryStore(_connectionString, _databaseName, _searchIndexName);
            // Initialize the memory builder: set up text embedding generation and memory store
            _memoryBuilder = new MemoryBuilder();
            _memoryBuilder.WithOpenAITextEmbeddingGeneration("text-embedding-ada-002", _openAiApiKey);
            _memoryBuilder.WithMemoryStore(_mongoDBMemoryStore);
            // Build the memory: create the semantic text memory
            _semanticTextMemory = _memoryBuilder.Build();
            _mongoClient = mongoClient;
            _logger = logger;
            _mongoDBService = mongoDBService;
        }

        public async Task<List<ProductSearchResult>> GetRecommendationsAsync(string userInput)
        {
            try
            {
                var memories = _semanticTextMemory.SearchAsync("Products", userInput, limit: 10, minRelevanceScore: 0.6);


            var result = new List<ProductSearchResult>();
            await foreach (var memory in memories)
            {
                var productSearchResult = new ProductSearchResult
                {
                    Id = memory.Metadata.Id,
                    Description=memory.Metadata.Description,
                    Name=memory.Metadata.AdditionalMetadata,
                    //Relevance：通常在 0 到 1 之間，表示這個查詢結果與你的查詢的相關性
                    Relevance = memory.Relevance.ToString("0,00"),
                };
                result.Add(productSearchResult);
            }

            return result;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error during memory search: {ex.Message}");
            }
            return null;
        }
        public async Task FetchAndSaveProductDocumentsAsync()
        {
            // Fetch and save product documents to the semantic text memory
            await FetchAndSaveProductDocuments(_semanticTextMemory);
        }

        private async Task FetchAndSaveProductDocuments(ISemanticTextMemory memory)
        {
            IEnumerable<Products> products =await _mongoDBService.GetFetchData();
            foreach (var product in products)
            {
                try
                {
                    _logger.LogInformation($"Processing {product.ProductId}, {product.ProductName}...");
                    await memory.SaveInformationAsync(
                        collection: "Products",
                        text: product.Description,
                        id: product.ProductId.ToString(),
                        description: $"""
                        價錢：{product.ProductPrice},
                        上市時間：{product.ProductShelfTime},
                        描述：{product.Description}
                        """,
                        additionalMetadata: $"""
                        遊戲名稱：{product.ProductName},
                        分類：{product.CategoryName},
                        標籤：{product.TagNames},
                        """
                    );
                    _logger.LogInformation($"Done {product.ProductId}...");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message);
                }
            }
        }
    }
}
