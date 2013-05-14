using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SteamParty.Core.Test
{
    [TestClass]
    public class UnitTest1
    {
        private readonly SteamApi _api = new SteamApi("89F7BE5A7EA33D7538DC6B7CB8FC9163");

        [TestMethod]
        public void GetSteamIdFromName()
        {
            var steamid = _api.GetSteamIdFromName("dubispacebar");
            Assert.AreEqual("76561197962208538", steamid);
        }

        [TestMethod]
        public void GetOwnedGames()
        {
            var games = _api.GetOwnedGames("76561197960434622");
            Assert.IsTrue(games.Count > 0);
        }

        [TestMethod]
        public void Compare()
        {
            var list = new List<string>
                {
                    "76561197975995523", // ice_mouton
                    "76561197962208538", // dubispacebar
                    "76561197965572012", // siliticx
                };

            var c = new Comparer(_api);
            var games = c.Compare(list);

            Assert.IsTrue(games.Count > 0);
        }

        [TestMethod]
        public void GetPlayerSummaries()
        {
            var player1 = _api.GetPlayerSummary("76561197975995523");
            Assert.AreEqual("ice_mouton", player1.DisplayName);

            var player2 = _api.GetPlayerSummary("76561197962208538");
            Assert.AreEqual("SPACEBAR", player2.DisplayName);
        }
    }
}
