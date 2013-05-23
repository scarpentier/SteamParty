using System;
using System.Collections.Generic;
using System.Linq;
using SteamParty.Core.SteamObjects;

namespace SteamParty.Core
{
    public class Comparer
    {
        private readonly SteamApi _api;

        public Comparer(SteamApi api)
        {
            _api = api;
        }

        public List<GamePlayers> Compare(IList<string> steamIds)
        {
            var players = _api.GetPlayerSummaries(steamIds);
            return Compare(players);
        }

        public List<GamePlayers> Compare(IList<Player> players)
        {
            var gameCount = new Dictionary<int, Tuple<Game, List<Player>>>();

            foreach (var player in players)
            {
                foreach (var game in _api.GetOwnedGames(player))
                {
                    var gamePlayTime = game.HoursPlayed;

                    if (!gameCount.ContainsKey(game.AppId))
                    {
                        gameCount.Add(game.AppId, Tuple.Create(game, new List<Player>()));
                        gameCount[game.AppId].Item1.HoursPlayed = 0; // Reset playtime
                    }

                    gameCount[game.AppId].Item1.HoursPlayed += gamePlayTime;
                    gameCount[game.AppId].Item2.Add(player);
                }
            }

            return (from g in gameCount
                    where g.Value.Item2.Count > 1
                    orderby g.Value.Item2.Count descending, g.Value.Item1.HoursPlayed descending
                    select new GamePlayers()
                        {
                            AppId = g.Value.Item1.AppId,
                            IconUrl = g.Value.Item1.IconUrl,
                            LogoUrl = g.Value.Item1.LogoUrl,
                            StoreUrl = g.Value.Item1.StoreUrl,
                            Name = g.Value.Item1.Name,
                            HoursPlayed = g.Value.Item1.HoursPlayed,
                            Players = g.Value.Item2
                        }).ToList();
        }
    }

    public class GamePlayers: Game
    {
        public List<Player> Players;
    }
}
