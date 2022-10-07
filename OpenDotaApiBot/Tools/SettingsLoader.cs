using System.Text.Json;

namespace DotaOpenApiBot.Tools; 

public class SettingsLoader {
	public static BotSettings LoadBotSettings() {
		try {
			return JsonSerializer.Deserialize<BotSettings>(System.IO.File.ReadAllText("botsettings.json")) ?? throw new JsonException();
		}
		catch (FileNotFoundException) {
			Console.WriteLine("botsettings.json not found");
			throw;
		}
		catch (JsonException) {
			Console.WriteLine("botsettings.json is not valid");
			throw;
		}
		catch (Exception e) {
			Console.WriteLine(e);
			throw;
		}
	}
}