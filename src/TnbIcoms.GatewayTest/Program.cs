using System.Net.Http.Json;
using System.Text.Json;
using System.Text;
try
{
    Console.Write("Gateway URL, e.g. http://10.52.9.21:5099: "); var url = Console.ReadLine()!.Trim().TrimEnd('/');
    Console.Write("Shared test key (hidden): "); var key = new StringBuilder();
    while (true) { var k = Console.ReadKey(true); if (k.Key == ConsoleKey.Enter) break; if (k.Key == ConsoleKey.Backspace) { if (key.Length > 0) key.Length--; } else if (!char.IsControl(k.KeyChar)) key.Append(k.KeyChar); }
    Console.WriteLine();
    Console.Write("AspNetUserId [f6c9faab-f249-4dd2-ae37-b32a904b7a3b]: "); var id = Console.ReadLine();
    if (string.IsNullOrWhiteSpace(id)) id = "f6c9faab-f249-4dd2-ae37-b32a904b7a3b";
    Console.Write("Repetitions [10]: "); var input = Console.ReadLine(); var count = string.IsNullOrWhiteSpace(input) ? 10 : int.Parse(input);
    if (count < 1 || count > 1000) throw new ArgumentException("Repetitions must be 1-1000.");
    using var client = new HttpClient { Timeout = TimeSpan.FromSeconds(90) };
    client.DefaultRequestHeaders.Add("X-Test-Key", key.ToString());
    var log = Path.Combine(AppContext.BaseDirectory, $"gateway-test-{DateTime.UtcNow:yyyyMMdd-HHmmss}.log");
    using var file = new StreamWriter(log) { AutoFlush = true };
    for (var i = 1; i <= count; i++)
    {
        var timer = System.Diagnostics.Stopwatch.StartNew(); string result;
        try
        {
            using var response = await client.PostAsJsonAsync(url + "/login-ad-query", new { aspNetUserId = id });
            var text = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                using var json = JsonDocument.Parse(text);
                result = $"HTTP {(int)response.StatusCode}; Found={json.RootElement.GetProperty("found")}; DB elapsed={json.RootElement.GetProperty("elapsedMs")}ms; Connection={json.RootElement.GetProperty("connectionId")}";
            }
            else result = $"HTTP {(int)response.StatusCode}; {text}";
        }
        catch (Exception ex) { result = ex.ToString(); }
        var line = $"{DateTime.UtcNow:O} Iteration={i}; HTTP total={timer.ElapsedMilliseconds}ms; {result}";
        Console.WriteLine(line); file.WriteLine(line);
    }
    Console.WriteLine($"Report: {log}");
}
catch (Exception ex) { Console.WriteLine(ex); }
finally { Console.WriteLine("Press Enter to close."); Console.ReadLine(); }
