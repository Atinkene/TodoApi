using MongoDB.Driver;
using TodoApi.Models;

namespace TodoApi.Services;

public class TodoItemsService
{
    private readonly IMongoCollection<TodoItem> _todoItemsCollection;

    public TodoItemsService(IConfiguration config)
    {
        var settings = config.GetSection("BookStoreDatabase").Get<TodoDatabaseSettings>()!;
        var client = new MongoClient(settings.ConnectionString);
        var database = client.GetDatabase(settings.DatabaseName);
        _todoItemsCollection = database.GetCollection<TodoItem>(settings.TodoItemsCollectionName);
    }

    public async Task<List<TodoItem>> GetAsync() =>
        await _todoItemsCollection.Find(_ => true).ToListAsync();

    public async Task<TodoItem?> GetAsync(string id) =>
        await _todoItemsCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

    public async Task CreateAsync(TodoItem newItem) =>
        await _todoItemsCollection.InsertOneAsync(newItem);

    public async Task UpdateAsync(string id, TodoItem updatedItem) =>
        await _todoItemsCollection.ReplaceOneAsync(x => x.Id == id, updatedItem);

    public async Task RemoveAsync(string id) =>
        await _todoItemsCollection.DeleteOneAsync(x => x.Id == id);
}