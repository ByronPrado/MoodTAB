using SQLite;
using MoodTAB.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace MoodTAB.Data;

public class TodoItemDataBase
{   //config de la BD
    private readonly SQLiteAsyncConnection _database;

    public TodoItemDataBase(string dbPath)
    {
        _database = new SQLiteAsyncConnection(dbPath);
        _database.CreateTableAsync<TodoItem>().Wait();
        _database.CreateTableAsync<Pregunta>().Wait();
        _database.CreateTableAsync<Respuestas>().Wait();
    }

    public Task<List<TodoItem>> GetItemsAsync()
    {
        return _database.Table<TodoItem>().ToListAsync();
    }

    public Task<TodoItem> GetItemAsync(int id)
    {
        return _database.Table<TodoItem>().Where(i => i.Id == id).FirstOrDefaultAsync();
    }

    public Task<int> SaveItemAsync(TodoItem item)
    {
        if (item.Id != 0)
        {
            return _database.UpdateAsync(item);
        }
        else
        {
            return _database.InsertAsync(item);
        }
    }

    public Task<int> DeleteItemAsync(TodoItem item)
    {
        return _database.DeleteAsync(item);
    }
    // nuevas funciones para Preguntas y Respuestas
    public Task<int> SaveQuestionAsync(Pregunta q) => _database.InsertAsync(q);
    public Task<List<Pregunta>> GetQuestionsAsync() => _database.Table<Pregunta>().ToListAsync();
    public Task<int> DeleteQuestionAsync(Pregunta q)=> _database.DeleteAsync(q);

    public Task<Pregunta> GetQuestionByIdAsync(int id)
    {
        return _database.Table<Pregunta>().Where(i => i.Id == id).FirstOrDefaultAsync();
    }
    

    public Task<int> SaveAnswerAsync(Respuestas a) => _database.InsertAsync(a);
    public Task<List<Respuestas>> GetAnswersAsync() => _database.Table<Respuestas>().ToListAsync();
    public Task<int> DeleteAnswersAsync(Respuestas a)=> _database.DeleteAsync(a);

}