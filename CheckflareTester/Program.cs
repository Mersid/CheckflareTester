using System.Net;
using System.Text.Json;
using CheckflareTester;


Database database = new Database("Database.sqlite");
database.Open();
database.Initialize();

string[] urls = File.ReadAllLines("Urls.txt");

HttpClient client = new HttpClient();

List<HttpResponseMessage> responses1 = new List<HttpResponseMessage>();
List<HttpResponseMessage> responses2 = new List<HttpResponseMessage>();

const string addr1 = @"http://198.74.51.249:5000";
const string addr2 = @"http://45.79.81.175:5000";

// Send the requests to the two Checkflare servers
for (int i = 0; i < urls.Length; i++)
{
	if (i % 2 == 0)
	{
		HttpResponseMessage response = await client.GetAsync($"{addr1}/Scraper/SubmitTask?url={urls[i]}");
		responses1.Add(response);
	}
	else
	{
		HttpResponseMessage response = await client.GetAsync($"{addr2}/Scraper/SubmitTask?url={urls[i]}");
		responses2.Add(response);
	}

	Console.WriteLine($"Submitted task {i + 1} of {urls.Length}");
}

Random random = new Random();
int completed = 0;
	
while (true)
{
	Thread.Sleep(250);
	// Pick a random token and check if it is complete
	HttpResponseMessage? response1 = responses1.Count == 0 ? null : responses1[random.Next(0, responses1.Count)];
	HttpResponseMessage? response2 = responses2.Count == 0 ? null : responses2[random.Next(0, responses2.Count)];

	// If both are null, then we are finished processing all of them
	if (response1 == null && response2 == null)
		break;

	if (response1 != null)
	{
		ResponseToken token = JsonSerializer.Deserialize<ResponseToken>(response1.Content.ReadAsStringAsync().Result)!;
		HttpResponseMessage data = await client.GetAsync($"{addr1}/Scraper/GetResult/{token.token.ToString()}");

		if (data.StatusCode == HttpStatusCode.OK)
		{
			ScraperTask task = JsonSerializer.Deserialize<ScraperTask>(data.Content.ReadAsStringAsync().Result, new JsonSerializerOptions() {PropertyNameCaseInsensitive = true})!;
			if (task.Html is null)
			{
				int t = 8;
			}
			else
			{
				database.Insert(task);
				Console.WriteLine($"Response 1: Added {task.Guid} {task.Url} to database!");
				completed++;
				Console.WriteLine($"Completed: {completed}");
				responses1.Remove(response1);
			}
		}
	}
	
	if (response2 != null)
	{
		ResponseToken token = JsonSerializer.Deserialize<ResponseToken>(response2.Content.ReadAsStringAsync().Result)!;
		HttpResponseMessage data = await client.GetAsync($"{addr2}/Scraper/GetResult/{token.token.ToString()}");

		if (data.StatusCode == HttpStatusCode.OK)
		{
			ScraperTask task = JsonSerializer.Deserialize<ScraperTask>(data.Content.ReadAsStringAsync().Result, new JsonSerializerOptions() {PropertyNameCaseInsensitive = true})!;
			if (task.Html is null)
			{
				int t = 8;
			}
			else
			{
				database.Insert(task);
				Console.WriteLine($"Response 2: Added {task.Guid} {task.Url} to database!");
				completed++;
				Console.WriteLine($"Completed: {completed}");
				responses2.Remove(response2);
			}
			
		}
	}
}