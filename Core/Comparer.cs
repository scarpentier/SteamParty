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

        public IDictionary<Game, IList<Player>> Compare(IList<long> steamIds)
        {
            var players = _api.GetPlayerSummaries(steamIds);
            return Compare(players);
        }

        public IDictionary<Game, IList<Player>> Compare(IList<Player> players)
        {
            var gameCount = new Dictionary<Game, IList<Player>>();

            foreach (var player in players)
            {
                foreach (var game in _api.GetOwnedGames(player))
                {
                    if (!gameCount.ContainsKey(game))
                        gameCount.Add(game, new List<Player>());

                    gameCount[game].Add(player);
                }
            }

            // Sort the results
            gameCount = (from g in gameCount where g.Value.Count > 1 orderby g.Value.Count descending select g).ToDictionary(pair => pair.Key, pair => pair.Value);

            return gameCount;
        }
    }
}
