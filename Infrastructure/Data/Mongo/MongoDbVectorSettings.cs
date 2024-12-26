namespace Infrastructure.Data.Mongo
{
    public class MongoDbVectorSettings
    {
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
        public string SearchIndexName {  get; set; }
        public string LineMessageDatabaseName { get; set; }
        public string VectorCollectionName { get; set; }
    }
}
