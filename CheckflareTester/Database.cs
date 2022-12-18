using System.Data.SQLite;
using Dapper;

namespace CheckflareTester;

public class Database
{
	private SQLiteConnection connection;
	public Database(string filePath)
	{
		connection = new SQLiteConnection($"Data Source = {filePath}; Version = 3");
	}

	public void Open()
	{
		connection.Open();
	}

	public void Initialize()
	{
		const string createTableSql = @"CREATE TABLE Scrapes (
									UUID	TEXT NOT NULL,
									URL	TEXT NOT NULL,
									Status	INTEGER NOT NULL,
									CompletionTime	TEXT NOT NULL,
									HTML	BLOB
									)";

		connection.Execute(createTableSql);
	}

	public void Insert(ScraperTask task)
	{
		const string insertSql = @"INSERT INTO Scrapes VALUES (@GuidString, @Url, @Status, @CompletionTime, @Html);";

		connection.Execute(insertSql, task);
	}
}