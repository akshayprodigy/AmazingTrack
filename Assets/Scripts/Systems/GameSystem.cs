using Leopotam.EcsLite;
using UnityEngine;
using Zenject;

namespace AmazingTrack
{
    public enum GameMode
    {
        Easy,
        Normal,
        Hard,
        Holes
    }

    public class GameSystem : IInitializable, ITickable
    {
        private readonly EcsWorld world;
        private readonly GameSettings gameSettings;
        private readonly AudioSettings audioSettings;
        private readonly BallSystem ballSystem;

        private readonly PlayerStatService playerStatService;
        private readonly ObjectSpawner spawner;
        private readonly BlockSystem blockSystem;
        private readonly AudioPlayer audioPlayer;
        
        private readonly EcsPool<BallComponent> ballPool;
        private readonly EcsFilter ballFilter;

        private readonly EcsFilter ballPassedFilter;
        private readonly EcsFilter ballHitCrystalFilter;
        private readonly EcsFilter ballFallingFilter;
        private readonly EcsFilter playerLevelUpFilter;
        
        private readonly EcsFilter gameStateFilter;
        private readonly EcsPool<GameStateComponent> gameStatePool;

        private const float BallSpawnHeight = 0.75f;

        public GameSystem(EcsWorld world, GameSettings gameSettings, AudioPlayer audioPlayer, 
            AudioSettings audioSettings, BallSystem ballSystem, PlayerStatService playerStatService, 
            ObjectSpawner spawner, BlockSystem blockSystem)
        {
            this.world = world;
            this.gameSettings = gameSettings;
            this.audioPlayer = audioPlayer;
            this.audioSettings = audioSettings;
            this.ballSystem = ballSystem;
            this.playerStatService = playerStatService;
            this.spawner = spawner;
            this.blockSystem = blockSystem;

            ballPool = world.GetPool<BallComponent>();
            ballFilter = world.Filter<BallComponent>().End();
            
            ballPassedFilter = world.Filter<BallPassedComponent>().End();
            ballHitCrystalFilter = world.Filter<BallHitComponent>().End();
            ballFallingFilter = world.Filter<BallComponent>().Inc<FallingComponent>().End();
            
            gameStatePool = world.GetPool<GameStateComponent>();
            gameStateFilter = world.Filter<GameStateComponent>().End();

            playerLevelUpFilter = world.Filter<PlayerLevelUpComponent>().End();
        }
        
        public ref GameStateComponent GetGameState()
        {
            var playerStat = gameStateFilter.GetRawEntities()[0];
            return ref gameStatePool.Get(playerStat);
        }

        public void Initialize()
        {
            ShowTitle(false);
        }

        public void Tick()
        {
            var gameState = gameStateFilter.GetRawEntities()[0];
            ref var gameStateComponent = ref gameStatePool.Get(gameState);
            
            switch (gameStateComponent.State)
            {
                case GameState.Title:
                    if (Application.platform == RuntimePlatform.Android)
                    {
                        if (Input.GetKeyDown(KeyCode.Escape))
                            Application.Quit();
                    }

                    break;
                case GameState.Playing:
                    if (Input.GetKeyDown(KeyCode.Escape))
                        ShowTitle();

                    if (Input.GetMouseButtonDown(0))
                        ballSystem.ChangeDirection(ballFilter.GetRawEntities()[0]);

                    foreach (var _ in ballFallingFilter)
                        GameOver();
                    // foreach (var ball in ballFallingFilter)
                    // {
                    //     gameStateComponent.State = GameState.Revive;
                    //     gameStateComponent.ReviveTimer = 10.0f; // Set revive timer to 10 seconds
                    //     ShowReviveUI(); // Show the revive UI to the user
                    //     break;
                    // }
                    break;
                case GameState.Revive:
                    // Countdown revive timer
                    gameStateComponent.ReviveTimer -= Time.deltaTime;

                    // If the revive timer reaches 0, go to GameOver state
                    if (gameStateComponent.ReviveTimer <= 0)
                    {
                        gameStateComponent.State = GameState.GameOver;
                        HideReviveUI(); // Hide the revive UI
                        ChangeState(GameState.GameEnd);
                        // GameOver(); // Show the game over UI
                    }
                    break;
                case GameState.GameOver:
                    gameStateComponent.GameOverTimer -= Time.deltaTime;
                    if (gameStateComponent.GameOverTimer <= 0){
                        gameStateComponent.ReviveTimer = 10.0f;
                        ChangeState(GameState.Revive);  
                    }
                        // ChangeState(GameState.GameEnd);
                    break;
                case GameState.GameEnd:
                {    

                    if (Input.GetKeyDown(KeyCode.Escape))
                        ShowTitle();
                }
                    break;
                default:
                    break;
            }
            
            foreach (var _ in ballPassedFilter)
            {
                playerStatService.AddScore(PlayerStatService.ScoreForStep);
            }
            
            foreach (var _ in ballHitCrystalFilter)
            {
                playerStatService.AddScore(PlayerStatService.ScoreForCrystal);
                playerStatService.AddCrystal(1);
                audioPlayer.Play(audioSettings.BallHitCrystalSound, audioSettings.BallHitCrystalVolume);
            }

            foreach (var _ in playerLevelUpFilter)
            {
                audioPlayer.Play(audioSettings.NextLevelSound);
            
                var ball = ballFilter.GetRawEntities()[0];
                ref var ballComponent = ref ballPool.Get(ball);
                ballComponent.Speed = GetBallSpeedForCurrentLevel();
                // Create the level-up event
                var eventEntity = world.NewEntity();
                var playerLevelUpEventPool = world.GetPool<PlayerLevelUpEventComponent>();
                ref var playerLevelUpEvent = ref playerLevelUpEventPool.Add(eventEntity);
                playerLevelUpEvent.PlayerId = 1;  // Assuming player ID is 1, adapt if needed
                playerLevelUpEvent.NewLevel = playerStatService.GetPlayerStat().Level;
            }
        }


        public void OnReviveButtonPressed()
        {
            var gameState = gameStateFilter.GetRawEntities()[0];
            ref var gameStateComponent = ref gameStatePool.Get(gameState);
            
            Debug.Log("Revive button pressed");
            if (gameStateComponent.State == GameState.Revive)
            {
                // Transition back to Playing state and reset the ball
                

                foreach (var ball in ballFilter)
                {
                    ballSystem.ResetBall(ball);
                }

                // Hide the revive UI
                gameStateComponent.State = GameState.Playing;
                HideReviveUI();
            }
        }
        private void ClearScene()
        {
            blockSystem.ClearBlocks();
            spawner.Clear();
            
            playerStatService.Clear();
        }
        
        private void InitScene()
        {
            var gameState = world.NewEntity();
            gameStatePool.Add(gameState);
            // gameSettings.GameMode = (GameMode)PlayerPrefs.GetInt("Tap2Dash_GameMode", (int)GameMode.Normal);
            playerStatService.GameStart(gameSettings.Level);

            blockSystem.CreateStartBlocks(GetPartsCountInBlock());
            spawner.SpawnBall(new Vector3(0, BallSpawnHeight, 0), GetBallSpeedForCurrentLevel());
        }
        
        private void ChangeState(GameState state)
        {
            var gameState = gameStateFilter.GetRawEntities()[0];
            ref var gameStateComponent = ref gameStatePool.Get(gameState);
            gameStateComponent.State = state;
         }

        private float GetBallSpeedForCurrentLevel()
        {
            ref var playerStatComponent = ref playerStatService.GetPlayerStat();
            return gameSettings.BallInitialSpeed + playerStatComponent.Level - 1;
        }

        private void ShowReviveUI(){
            // Show the revive UI
        }

        private void HideReviveUI(){    
            // Hide the revive UI  
            
        }

        private void ShowTitle(bool clearScene = true)
        {
            if (clearScene)
                ClearScene();
            InitScene();
            ChangeState(GameState.Title);
        }

        public void GameStart(GameMode gameMode)
        {
            bool recreate = gameSettings.GameMode != gameMode;
            gameSettings.GameMode = gameMode;
            GameStart(recreate);
        }

        public void GameStartToRecreate(GameMode gameMode)
        {
            gameSettings.GameMode = gameMode;
            GameStart(true);
        }

        private void GameStart(bool recreateScene)
        {
            if (recreateScene)
            {
                ClearScene();
                InitScene();
            }

            ChangeState(GameState.Playing);

            audioPlayer.Play(audioSettings.GameStartSound);
        }

        private void GameOver()
        {
            playerStatService.GameEnd();

            audioPlayer.Play(audioSettings.BallFallSound);

            var gameState = gameStateFilter.GetRawEntities()[0];
            ref var gameStateComponent = ref gameStatePool.Get(gameState);
            gameStateComponent.GameOverTimer = 1.0f;
            
            ChangeState(GameState.GameOver);
        }

        private int GetPartsCountInBlock()
        {
            switch (gameSettings.GameMode)
            {
                case GameMode.Easy:
                    return 3;
                case GameMode.Normal:
                    return 2;
                case GameMode.Hard:
                    return 1;
                case GameMode.Holes:
                    return 3;
                default:
                    return 2;
            }
        }
    }
}