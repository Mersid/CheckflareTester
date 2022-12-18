namespace CheckflareTester;

public class ScraperTask
{

	public ScraperTask(string url)
	{
		Guid = Guid.NewGuid();
		Url = url;
	}
	
	public Guid Guid { get; set; }
	public string Url { get; set; }

	/// <summary>
	/// Status is 0 if it's not complete yet
	/// </summary>
	public int Status { get; set; }
	public DateTime CompletionTime { get; set; }
	public string? Html { get; set; }

	public string GuidString => Guid.ToString();
}