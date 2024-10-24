namespace AmazingTrack
{
    public struct PlayerStatComponent
    {
        public int Score;
        public int LastScore;

        public int HighScore;
        public int Level;
        public int LastLevel;
        public int CrystalScore;
        public int TotalCrystalScore; 
        public int HealthScore;

        public int UserLevel; // Range 1-3 Low 3-7 Medium 7-10 High
        public int GamesPlayed; 

        public int GameModeCount;
        public GameMode LastGameMode;

    }
}