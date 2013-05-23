using System;

namespace SteamParty.Core.SteamObjects
{
    public class Game : IEquatable<Game>
    {
        public int AppId { get; set; }
        public string Name { get; set; }
        public int? HoursPlayed { get; set; }
        public string IconUrl { get; set; }
        public string LogoUrl { get; set; }
        public string StoreUrl { get; set; }

        public override int GetHashCode()
        {
            return AppId;
        }
        public override bool Equals(object obj)
        {
            return Equals(obj as Game);
        }

        public bool Equals(Game obj)
        {
            return obj != null && obj.AppId == this.AppId;
        }
    }
}
