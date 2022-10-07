using System.Runtime.CompilerServices;
using System.Text.Json;
using DotaOpenApiBot.OpenDota;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using static DotaOpenApiBot.OpenDota.OpenDota;

namespace DotaOpenApiBot.EventHandling;

public static class MessageHandler {
	private static Dictionary<int, Hero> _heroes;
	private static bool _heroesLoaded;

	static MessageHandler() {
		const string heroesUrl = "https://raw.githubusercontent.com/odota/dotaconstants/master/build/heroes.json";
		try {
			_heroes = JsonSerializer.Deserialize<Dictionary<int, Hero>>(new HttpClient().GetStringAsync(heroesUrl).Result) ??
			          throw new NullReferenceException();
			_heroesLoaded = true;
		}
		catch (Exception e) {
			Console.WriteLine("Heroes could not be loaded: " + e.Message);
		}
	}

	public static async Task BotOnMessageReceived(this ITelegramBotClient bot, Message message) {
		try {
			var match = await GetMatch(long.Parse(message.Text));
			await bot.SendTextMessageAsync(message.Chat.Id, GameStatsMessageGenerator(match),
				replyMarkup: new InlineKeyboardMarkup(InlineKeyboardButton.WithUrl("Download replay", match.replay_url)),
				replyToMessageId: message.MessageId);
		}
		catch (Exception e) {
			await bot.SendTextMessageAsync(message.Chat.Id, "Match not found.", replyToMessageId: message.MessageId);
			Console.WriteLine(e);
		}
	}

	private static string GameStatsMessageGenerator(Match match) {
		var respond = "";
		if (match.radiant_team is not null && match.dire_team is not null)
			respond += match.radiant_team.name + " vs " + match.dire_team.name + "\n";
		var radiantTeam = $"Radiant{(match.radiant_team is not null ? $" ({match.radiant_team.name})" : "")}";
		var direTeam = $"Dire{(match.dire_team is not null ? $" ({match.dire_team.name})" : "")}";
		respond += $"{(match.radiant_win ? radiantTeam : direTeam)} win!\n" +
		           $"🌻:{match.radiant_score}  🧟‍♂️:{match.dire_score}   ⏳: {match.duration / 60}:{match.duration % 60}\n\n";
		respond += "Stats (🔪/💀/🍑   🐐/❗️   👨‍🎓   💰):\n\n";
		respond += $"{radiantTeam}:\n";
		respond += TeamStatsMessageGenerator(match, true) + "\n";
		respond += $"{direTeam}:\n";
		respond += TeamStatsMessageGenerator(match, false) + "\n";

		respond += $"ID: {match.match_id}";

		return respond;
	}

	private static string TeamStatsMessageGenerator(Match match, bool isRadiant) => match.players
		.Where(t => t.isRadiant == isRadiant).Aggregate("", (current, player) => current + PlayerStatsGenerator(player));

	private static string PlayerStatsGenerator(InGamePlayer player) =>
		$"{player.kills}/{player.death}/{player.assists}  {player.last_hits}/{player.denies}  {player.level}  {player.net_worth}" +
		$" {player.name} ({(_heroesLoaded ? _heroes[player.hero_id].localized_name : $"HeroId:{player.hero_id}")})\n";
}