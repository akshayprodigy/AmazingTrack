namespace AmazingTrack
{
    public enum GameState
    {
        Title,
        Playing,
        GameOver,
        GameEnd,
        Revive
    }
    
    public struct GameStateComponent
    {
        public GameState State;
        public float GameOverTimer;
        public float ReviveTimer;
    }
}