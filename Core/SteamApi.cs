using System;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using SteamParty.Core.SteamObjects;

namespace SteamParty.Core
{
    public class SteamApi
    {
        private string Key { get; set; }
        private const string SteamApiBaseUrl = "https://api.steampowered.com";

        public SteamApi(string key)
        {
            Key = key;
        }

        public Player GetPlayerSummary(string steamId)
        {
            return GetPlayerSummaries(new [] { steamId })[0];
        }

        public IList<Player> GetPlayerSummaries(IEnumerable<string> steamIds)
        {
            const string serviceUrl = "/ISteamUser/GetPlayerSummaries/v0002/?key={0}&steamids={1}&format=json";

            var url = string.Format(SteamApiBaseUrl + serviceUrl, Key, string.Join(",", steamIds));
            var jsonData = new System.Net.WebClient().DownloadString(url);

            var o = JObject.Parse(jsonData);
            var a = o["response"]["players"].Select(p => new Player
                {
                    SteamId = (string) p["steamid"],
                    ProfileState = (bool) p["profilestate"],
                    DisplayName = (string)p["personaname"],
                    LastLogoff = DateTimeExtension.FromUnix((long) p["lastlogoff"]),
                    CommentPermission = (bool)(p["commentpermission"] ?? false),
                    ProfileUrl = (string) p["profileurl"],
                    AvatarUrl32 = (string) p["avatar"],
                    AvatarUrl64 = (string) p["avatarmedium"],
                    AvatarUrl184 = (string) p["avatarfull"],
                    Status = (PlayerStatusEnum)(int)p["personastate"]
                });

            return a.ToList();
        }

        public IList<Game> GetOwnedGames(Player player)
        {
            return GetOwnedGames(player.SteamId);
        }

        public IList<Game> GetOwnedGames(string steamId)
        {
            const string serviceUrl = "/IPlayerService/GetOwnedGames/v0001/?key={0}&steamId={1}&include_appinfo=1&include_played_free_games=1&format=json";
            const string pictureUrl = "http://media.steampowered.com/steamcommunity/public/images/apps/{0}/{1}.jpg";

            var url = string.Format(SteamApiBaseUrl + serviceUrl, Key, steamId);
            var jsonData = new System.Net.WebClient().DownloadString(url);

            var o = JObject.Parse(jsonData);
            var a = o["response"]["games"].Select(g => new Game
                {
                    AppId = (int)g["appid"],
                    Name = (string)g["name"],
                    Playtime = (int?)g["playtime_forever"],
                    IconUrl = string.Format(pictureUrl, g["appid"], g["img_icon_url"]),
                    LogoUrl = string.Format(pictureUrl, g["appid"], g["img_logo_url"])
                });

            return a.ToList();
        }
    }
}
