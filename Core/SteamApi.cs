using System;
using System.Linq;
using System.Collections.Generic;
using System.Xml.Linq;
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

        public string GetSteamIdFromName(string name)
        {
            const string steamCommunityProfileUrl = "http://steamcommunity.com/id/{0}/?xml=1";
            var url = string.Format(steamCommunityProfileUrl, name);

            try
            {
                var xml = XDocument.Load(url);
                return xml.Element("profile").Element("steamID64").Value;
            }
            catch
            {
                return null;
            }
        }

        public Player GetPlayerSummary(string steamId)
        {
            long n;
            if (!long.TryParse(steamId, out n))
            {
                steamId = GetSteamIdFromName(steamId);
                if (string.IsNullOrEmpty(steamId)) return null;
            }

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
                    IsPublic = ((string)p["communityvisibilitystate"]) == "3",
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
            const string storeUrl = "http://store.steampowered.com/app/{0}";

            var url = string.Format(SteamApiBaseUrl + serviceUrl, Key, steamId);
            var webclient = new System.Net.WebClient() { Encoding = System.Text.Encoding.UTF8 };

            var jsonData = webclient.DownloadString(url);

            var o = JObject.Parse(jsonData);

            if (!o["response"].Any()) return null; // Will happen if the profile isn't public

            if (o["response"]["game_count"].Value<int>() == 0) return new List<Game>(); // Will happen if the profile has no games at all

            var a = o["response"]["games"].Select(g => new Game
                {
                    AppId = (int)g["appid"],
                    Name = (string)g["name"],
                    HoursPlayed = g["playtime_forever"] == null ? null : (int?)Math.Round((double)g["playtime_forever"] / 60, 0), // Playtime is expressed in minutes but we want hours
                    IconUrl = string.Format(pictureUrl, g["appid"], g["img_icon_url"]),
                    LogoUrl = string.IsNullOrEmpty((string)g["img_logo_url"]) ? "Content/images/nocover.png" : string.Format(pictureUrl, g["appid"], g["img_logo_url"]),
                    StoreUrl = string.Format(storeUrl, g["appid"])
                });

            return a.ToList();
        }
    }
}
