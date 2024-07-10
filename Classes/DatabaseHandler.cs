using System;
using System.Collections.Generic;
using System.IO;
using GunRush.Models;
using SQLite;
namespace GunRush.Classes;

public class DatabaseHandler
{
    public static DatabaseHandler Instance => _instance ?? new DatabaseHandler();
    private static DatabaseHandler _instance;
    private SQLiteConnection _connection;
    private string _dbPassword = "_not2_decrypt_m3_";

    private DatabaseHandler()
    {
        _instance = this;
        var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "gunrush");
        Directory.CreateDirectory(path);
        _connection = new SQLiteConnection(new SQLiteConnectionString(Path.Combine(path, "gunrush.db"), true, key: _dbPassword));
        _connection.CreateTable<Run>();
    }
    
    ~DatabaseHandler()
    {
        _connection?.Close();
    }

    public List<Run> GetRuns(int limit = 0)
    {
        return this._connection.Table<Run>().OrderByDescending(t => t.Id).Take(limit<=0?50:limit).ToList();
    }

    public int GetKills()
    {
        var kills = this._connection.Table<Run>().Where(t => t.Kills > 0).ToList();
        var total = 0;
        foreach (var run in kills) total += run.Kills;
        return total;
    }

    public Run GetShortest()
    {
        return this._connection.Table<Run>().Where(t=>t.Won == true).OrderBy(t => t.Duration).FirstOrDefault();
    }
    
    public Run GetLongest()
    {
        return this._connection.Table<Run>().Where(t=> t.Won == true).OrderByDescending(t => t.Duration).FirstOrDefault();
    }

    public int CountRuns(bool won = false)
    {
        if(!won) return this._connection.Table<Run>().Count();
        return this._connection.Table<Run>().Count(t => t.Won == true);
    }

    public void LogRun(Run run)
    {
        this._connection.Insert(run);
    }
}