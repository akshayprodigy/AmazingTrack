using Leopotam.EcsLite;
namespace AmazingTrack
{
    public struct PlayerLevelUpEventComponent
    {
        public int PlayerId;  // Optional, useful if you have multiple players
        public int NewLevel;  // New level value
    }
}
