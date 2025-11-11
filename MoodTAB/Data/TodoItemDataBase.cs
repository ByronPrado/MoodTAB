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
        _database.CreateTableAsync<Diario>().Wait();
        _database.CreateTableAsync<Medicamento>().Wait();
        //agregar + tablas
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
    public Task<int> DeleteQuestionAsync(Pregunta q) => _database.DeleteAsync(q);

    public Task<Pregunta> GetQuestionByIdAsync(int id)
    {
        return _database.Table<Pregunta>().Where(i => i.ID_Pregunta == id).FirstOrDefaultAsync();
    }

    public Task<List<Pregunta>> GetQuestionsByUserIdAsync(int id)
    {
        return _database.Table<Pregunta>().Where(p => p.Usuario_dirigido == id).ToListAsync();
    }



    public Task<int> SaveAnswerAsync(Respuestas a) => _database.InsertAsync(a);
    public Task<List<Respuestas>> GetAnswersAsync() => _database.Table<Respuestas>().ToListAsync();
    public Task<int> DeleteAnswersAsync(Respuestas a) => _database.DeleteAsync(a);

    public Task<int> SaveDiarioAsync(Diario d) => _database.InsertAsync(d);
    public Task<List<Diario>> GetDiarioAsync() => _database.Table<Diario>().ToListAsync();
    public Task<int> DeleteDiarioAsync(Diario d) => _database.DeleteAsync(d);

    public Task<Diario> GetDiarioByIdAsync(int id)
    {
        return _database.Table<Diario>().Where(i => i.Id == id).FirstOrDefaultAsync();
    }
    //nuevo
        // Devuelve todos los diarios que caen exactamente en 'date' (00:00:00 .. 23:59:59)
    public Task<List<Diario>> GetDiariosByDateAsync(DateTime date)
    {
        var start = date.Date;
        var end = start.AddDays(1);
        return _database.Table<Diario>()
                        .Where(d => d.CreatedAt >= start && d.CreatedAt < end)
                        .OrderByDescending(d => d.CreatedAt)
                        .ToListAsync();
    }

    // Devuelve diarios en el rango inclusive [start.Date, end.Date]
    public Task<List<Diario>> GetDiariosBetweenAsync(DateTime start, DateTime end)
    {
        var s = start.Date;
        var e = end.Date.AddDays(1);
        return _database.Table<Diario>()
                        .Where(d => d.CreatedAt >= s && d.CreatedAt < e)
                        .OrderByDescending(d => d.CreatedAt)
                        .ToListAsync();
    }
    public Task<List<Diario>> GetDiariosMesActualAsync()
    {
        var now = DateTime.Now;
        var start = new DateTime(now.Year, now.Month, 1);
        var end = start.AddMonths(1).AddDays(-1);
        return GetDiariosBetweenAsync(start, end);
    }

    public async Task<Diario?> GetDiarioAnteriorAsync(DateTime fechaActual)
    {
        return await _database.Table<Diario>()
            .Where(d => d.CreatedAt < fechaActual)
            .OrderByDescending(d => d.CreatedAt)
            .FirstOrDefaultAsync();
    }
    public Task<List<Diario>> GetDiariosDiasAnterioresAsync(int dias)
    {
        var end = DateTime.Now.Date;
        var start = end.AddDays(-dias);
        return GetDiariosBetweenAsync(start, end);
    }

    public Task<int> SaveMedAsync(Medicamento m) => _database.InsertAsync(m);
    public Task<List<Medicamento>> GetMedsAsync() => _database.Table<Medicamento>().ToListAsync();
    public Task<int> DeleteMedAsync(Medicamento m) => _database.DeleteAsync(m);

    public Task<Medicamento> GetMedByIdAsync(int id)
    {
        return _database.Table<Medicamento>().Where(i => i.ID_Medicamento == id).FirstOrDefaultAsync();
    }

    public Task<List<Medicamento>> GetMedsByUserIdAsync(int id)
    {
        return _database.Table<Medicamento>().Where(m => m.Usuario_dirigido == id).ToListAsync();
    }

    public Task<List<Medicamento>> GetMedsByDateAsync(DateTime date)
    {
        var start = date.Date;
        var end = start.AddDays(1);
        return _database.Table<Medicamento>()
                        .Where(m => m.CreatedAt >= start && m.CreatedAt < end)
                        .OrderByDescending(d => d.CreatedAt)
                        .ToListAsync();
    }

    public Task<List<Medicamento>> GetMedsBetweenAsync(DateTime start, DateTime end)
    {
        var s = start.Date;
        var e = end.Date.AddDays(1);
        return _database.Table<Medicamento>()
                        .Where(m => m.CreatedAt >= s && m.CreatedAt < e)
                        .OrderByDescending(d => d.CreatedAt)
                        .ToListAsync();
    }
    public Task<List<Medicamento>> GetMedsMesActualAsync()
    {
        var now = DateTime.Now;
        var start = new DateTime(now.Year, now.Month, 1);
        var end = start.AddMonths(1).AddDays(-1);
        return GetMedsBetweenAsync(start, end);
    }
    public Task<List<Medicamento>> GetMedsDiasAnterioresAsync(int dias)
    {
        var end = DateTime.Now.Date;
        var start = end.AddDays(-dias);
        return GetMedsBetweenAsync(start, end);
    }

    public async Task ClearAllTablesAsync()
    {
        // Borra todas las tablas locales relevantes
        await _database.DeleteAllAsync<TodoItem>();
        await _database.DeleteAllAsync<Pregunta>();
        await _database.DeleteAllAsync<Respuestas>();
        await _database.DeleteAllAsync<Diario>();
        await _database.DeleteAllAsync<Medicamento>();

        System.Diagnostics.Debug.WriteLine("[DB] Todas las tablas locales fueron limpiadas correctamente.");
    }

}