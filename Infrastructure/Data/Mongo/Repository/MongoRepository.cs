using MongoDB.Bson;
using MongoDB.Driver;
using static Dapper.SqlMapper;
using System.Collections.Generic;
using System.Collections;


using Microsoft.VisualBasic;
using Infrastructure.Data.Mongo.Entity;

namespace Infrastructure.Data.Mongo.Repository;

public class MongoRepository<T> :IMongoRepository<T> where T : class
{
    private readonly IMongoDatabase _database;
    private readonly string _collectionName = typeof(T).Name;
    private readonly IMongoCollection<User> _users;
    private readonly IMongoCollection<Conversation> _conversations;
    private readonly IMongoCollection<Messages> _messages;
    public IMongoCollection<T> Collection => _database.GetCollection<T>(_collectionName);
    private readonly IMongoClient _mongoClient;
    public MongoRepository(IMongoClient client, string databaseName)
    {
        _mongoClient = client;
        _database = client.GetDatabase(databaseName);
        _users = _database.GetCollection<User>("Users");
        _conversations = _database.GetCollection<Conversation>("Conversations");
        _messages = _database.GetCollection<Messages>("Messages");
    }

    public async Task<List<T>> GetByIdAsync(string field,string id)
    {

            var filter = Builders<T>.Filter.Eq(field, id);
            return await Collection.Find(filter).ToListAsync();

    }

    public async Task<List<T>> FindAsync(string field, string keyword)
    {
        var filter = Builders<T>.Filter.Eq(field, keyword);
        return await Collection.Find(filter).ToListAsync();
    }


    public async Task<List<T>> GetAllAsync()
    {
        return await Collection.Find(_ => true).ToListAsync();
    }

    public async Task CreateAsync(T entity)
    {
        await Collection.InsertOneAsync(entity);
    }

    public async Task CreateRangeAsync(IEnumerable<T> entities)
    {
        await Collection.InsertManyAsync(entities);
    }

    public async Task UpdateAsync(string id, T entity)
    {
        var filter = Builders<T>.Filter.Eq("_id", ObjectId.Parse(id));
        await Collection.ReplaceOneAsync(filter, entity);
    }

    public async Task UpdateAllAsync(UpdateDefinition<T> updateDefinition)
    {
        var filter = Builders<T>.Filter.Empty; // 匹配所有文件
        var result = await Collection.UpdateManyAsync(filter, updateDefinition);

    }



    public async Task DeleteAsync(string id)
    {
        var filter = Builders<T>.Filter.Eq("_id", ObjectId.Parse(id));
        await Collection.DeleteOneAsync(filter);
    }



    //Linebot會用到的


    public async Task CreateUserAsync(User user)
    {
        await _users.InsertOneAsync(user);
    }

    public async Task<User> GetUserByLineUserIdAsync(string UserId)
    {
        var filter = Builders<User>.Filter.Eq(u => u.UserId, UserId);
        return await _users.Find(filter).FirstOrDefaultAsync();
    }
    public async Task<User> GetUserAsync(string userId)
    {
        var filter = Builders<User>.Filter.Eq(u => u.UseObjectrId, userId);
        return await _users.Find(filter).FirstOrDefaultAsync();
    }

    public async Task<Conversation> CreateConversationByUserIdAsync(string userId, Conversation conversation)
    {
        conversation.UserId = userId;
        await _conversations.InsertOneAsync(conversation);
        return conversation;
    }

    public async Task<Conversation> GetConversationAsync(string conversationId)
    {
        var filter = Builders<Conversation>.Filter.Eq(c => c.ConversationId, conversationId);
        return await _conversations.Find(filter).FirstOrDefaultAsync();
    }

    public async Task<Conversation> UpdateConversationAsync(Conversation conversation)
    {
        var filter = Builders<Conversation>.Filter.Eq(c => c.ConversationId, conversation.ConversationId);
        await _conversations.ReplaceOneAsync(filter, conversation);
        return conversation;
    }

    public async Task<List<Conversation>> GetConversationsByUserIdAsync(string userId)
    {
        var filter = Builders<Conversation>.Filter.Eq(c => c.UserId, userId);
        var sort = Builders<Conversation>.Sort.Descending(c => c.CreateAt);
        return await _conversations.Find(filter).Sort(sort).ToListAsync();
    }

    public async Task<Conversation> GetLatestConversationByUserIdAsync(string userId)
    {
        var filter = Builders<Conversation>.Filter.Eq(c => c.UserId, userId);
        var sort = Builders<Conversation>.Sort.Descending(c => c.CreateAt);
        return await _conversations.Find(filter).Sort(sort).FirstOrDefaultAsync();
    }

    public async Task<Messages> CreateMessageByConversationIdAsync(string conversationId, Messages message)
    {
        message.ConversationId = conversationId;
        message.Timestamp = DateTime.UtcNow;
        await _messages.InsertOneAsync(message);
        return message;
    }

    public async Task<List<Messages>> GetMessagesByConversationIdAsync(string conversationId)
    {
        var filter = Builders<Messages>.Filter.Eq(m => m.ConversationId, conversationId);
        var sort = Builders<Messages>.Sort.Ascending(m => m.Timestamp);
        return await _messages.Find(filter).Sort(sort).ToListAsync();
    }

    public async Task<Conversation> CreateConversationWithMessageAsync(string userId, Conversation conversation, Messages message)
    {
        using var session = await _mongoClient.StartSessionAsync();
        session.StartTransaction();

        try
        {
            conversation.UserId = userId;
            await _conversations.InsertOneAsync(session, conversation);

            message.ConversationId = conversation.ConversationId;
            message.Timestamp = DateTime.UtcNow;
            await _messages.InsertOneAsync(session, message);

            await session.CommitTransactionAsync();
            return conversation;
        }
        catch (Exception)
        {
            await session.AbortTransactionAsync();
            throw;
        }
    }
}