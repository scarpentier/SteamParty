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

        public Dictionary<Game, List<Player>> Compare(IList<long> steamIds)
        {
            var players = _api.GetPlayerSummaries(steamIds);
            return Compare(players);
        }

        public Dictionary<Game, List<Player>> Compare(IList<Player> players)
        {
            var gameCount = new Dictionary<int, Tuple<Game, List<Player>>>();

            // 1. Query Steam API and get list of owned games for everyone
            foreach (var player in players)
            {
                foreach (var game in _api.GetOwnedGames(player))
                {
                    if (!gameCount.ContainsKey(game.AppId))
                    {
                        gameCount.Add(game.AppId, Tuple.Create(game, new List<Player>()));
                        gameCount[game.AppId].Item1.Playtime = 0; // Reset playtime
                    }

                    gameCount[game.AppId].Item1.Playtime += game.Playtime;
                    gameCount[game.AppId].Item2.Add(player);
                }
            }

            // 4. Sort the results
            return (from g in gameCount
                         where g.Value.Item2.Count > 1
                         orderby g.Value.Item2.Count descending, g.Value.Item1.Playtime descending 
                         select g).ToDictionary(pair => pair.Value.Item1, pair => pair.Value.Item2);

        }
    }

    public struct GameComparaison
    {
        public Game Game { get; set; }
        public IList<Player> Players { get; set; }
        public int Totalplaytime { get; set; }
    }
}
