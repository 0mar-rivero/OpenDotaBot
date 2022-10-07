using System.Text.Json;

namespace DotaOpenApiBot.OpenDota;

public static class OpenDota {
	public static async Task<Match?> GetMatch(long matchId) =>
		JsonSerializer.Deserialize<Match>(
			await new HttpClient().GetStringAsync($"https://api.opendota.com/api/matches/{matchId}"));

	// public static async Task<TeamPlayer[]?> GetPlayersByTeam(long teamId) =>
	// 	JsonSerializer.Deserialize<TeamPlayer[]>(
	// 		await new HttpClient().GetStringAsync($"https://api.opendota.com/api/teams/{teamId}/players"));
}

public record Match(long match_id, int radiant_score, int dire_score, int duration, int game_mode, int lobby_type,
	bool radiant_win, long replay_salt, Team? dire_team, Team? radiant_team, InGamePlayer[] players, string replay_url);

public record Team(long team_id, double? rating, int? wins, int? losses, string name, string tag, string logo_url);

public record InGamePlayer(long? account_id, int player_slot, int assists, int death, int denies, int kills,
	int last_hits, int net_worth, int hero_id, string name, bool isRadiant, int level);

public record Hero(string localized_name);

// public record TeamPlayer(long account_id, string name, int games_played, int wins, bool is_current_team_member);