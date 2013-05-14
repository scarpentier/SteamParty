using System;

namespace SteamParty.Core.SteamObjects
{
    public class Player : IEquatable<Player>
    {
        public string SteamId { get; set; }
        public string DisplayName { get; set; }
        public string ProfileUrl { get; set; }
        public string AvatarUrl32 { get; set; }
        public string AvatarUrl64 { get; set; }
        public string AvatarUrl184 { get; set; }
        public bool IsPublic { get; set; } // CommunityVisibility VisibilityState 1 = private, friendsonly; 3 = public
        public PlayerStatusEnum Status { get; set; }
        public DateTime LastLogoff { get; set; }
        public bool CommentPermission { get; set; }

        public override int GetHashCode()
        {
            return SteamId.GetHashCode();
        }
        public override bool Equals(object obj)
        {
            return Equals(obj as Player);
        }

        public bool Equals(Player obj)
        {
            return obj != null && obj.SteamId == this.SteamId;
        }
    }

    public enum PlayerStatusEnum
    {
        Offline = 0,
        Online = 1,
        Busy = 2,
        Away = 3,
        Snooze = 4,
        LookingToTrade = 5,
        LookingToPlay = 6
    }
}
