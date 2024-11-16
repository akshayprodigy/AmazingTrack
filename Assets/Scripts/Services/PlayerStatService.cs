using Leopotam.EcsLite;
using UnityEngine;

namespace AmazingTrack
{
    public class PlayerStatService
    {
        private readonly EcsWorld world;
        
        public const int ScoreForCrystal = 10;
        public const int ScoreForStep = 1;
        private const int ScoreForNextLevel = 100;

        public int RetryCrystals = 50;
        
        private readonly EcsPool<PlayerStatComponent> playerStatPool;
        private readonly EcsPool<PlayerLevelUpComponent> playerLevelUpPool;
        private readonly EcsFilter playerStatFilter;
        
        public PlayerStatService(EcsWorld world)
        {
            this.world = world;

            playerStatPool = world.GetPool<PlayerStatComponent>();
            playerLevelUpPool = world.GetPool<PlayerLevelUpComponent>();
            playerStatFilter = world.Filter<PlayerStatComponent>().End();
        }

        public ref PlayerStatComponent GetPlayerStat()
        {
            var playerStat = playerStatFilter.GetRawEntities()[0];
            return ref playerStatPool.Get(playerStat);
        }

        public void AddScore(int score)
        {
            var playerStat = playerStatFilter.GetRawEntities()[0];
            ref var playerStatComponent = ref playerStatPool.Get(playerStat);
            playerStatComponent.Score += score;
            if (playerStatComponent.Score > playerStatComponent.Level * ScoreForNextLevel)
            {
                playerStatComponent.Level++;
                playerLevelUpPool.Add(playerStat);
            }
        }

        public void AddCrystal(int score){
            var playerStat = playerStatFilter.GetRawEntities()[0];
            ref var playerStatComponent = ref playerStatPool.Get(playerStat);
            playerStatComponent.CrystalScore += score;
        }

        public void GameStart(int level)
        {
            var playerStat = world.NewEntity();
            
            ref var playerStatComponent = ref playerStatPool.Add(playerStat);
            playerStatComponent.Level = level;
            playerStatComponent.Score = 0;
            playerStatComponent.CrystalScore = 0;
            RestoreResult(ref playerStatComponent);
            playerStatComponent.GamesPlayed += 1;
            SaveHealth(playerStatComponent);
        }

        public void GameEnd()
        {
            // new record
            var playerStat = playerStatFilter.GetRawEntities()[0];
            ref var playerStatComponent = ref playerStatPool.Get(playerStat);
            playerStatComponent.TotalCrystalScore += playerStatComponent.CrystalScore;
            if (playerStatComponent.Score > playerStatComponent.HighScore){

                playerStatComponent.HighScore = playerStatComponent.Score;
            }
                

            AdjustUserlevel(playerStatComponent);
           
        }

        public void Clear()
        {
            var playerStat = playerStatFilter.GetRawEntities()[0];
            world.DelEntity(playerStat);
        }

        public void AdjustUserlevel( PlayerStatComponent playerStatComponent)
        {
            int playerLevel = playerStatComponent.UserLevel;
            int suberbPerformanceThreshold = 500;
            int goodPerformanceThreshold = 300;
            int lowPerformanceThreshold = 30;
            int mediumPerformanceLowerBound = 100;

             // If player scores above 300, increase user level
            if (playerStatComponent.Score >= goodPerformanceThreshold)
            {
                playerLevel++;
                if(playerStatComponent.Score >= suberbPerformanceThreshold)
                {
                    playerLevel++;
                }
            }
            // If player scores below 50, decrease user level but give them another chance
            else if (playerStatComponent.Score <= lowPerformanceThreshold)
            {
                // If already at low performance, decrease level
                if (playerLevel > 1)
                {
                    playerLevel--;
                }
            }
            // If score is between 100 and 300, keep the user level the same or slightly adjust
            else if (playerStatComponent.Score >= mediumPerformanceLowerBound && playerStatComponent.Score <= goodPerformanceThreshold)
            {
                // Minor adjustments to the level can be added here, or leave it stable 
                if(playerStatComponent.UserLevel >= 7)
                {
                    // check last level and player conistency logic
                    playerLevel--;
                }else if(playerStatComponent.UserLevel <= 3)
                {
                    playerLevel++;
                }else{
                    // check last level and player conistency logic
                    playerLevel++;
                }

            }

            // Ensure the user level doesn't go below 1 or above 10
            playerLevel = Mathf.Clamp(playerLevel, 0, 10);
            playerStatComponent.UserLevel = playerLevel;

            StoreResult(playerStatComponent);
            
        }

        private void StoreResult(in PlayerStatComponent playerStatComponent)
        {
            PlayerPrefs.SetInt("Tap2Dash_HighScore", playerStatComponent.HighScore);
            PlayerPrefs.SetInt("Tap2Dash_LastScore", playerStatComponent.Score);
            PlayerPrefs.SetInt("Tap2Dash_LastLevel", playerStatComponent.Level);
            PlayerPrefs.SetInt("Tap2Dash_TotalCrystal", playerStatComponent.TotalCrystalScore);
            PlayerPrefs.SetInt("Tap2Dash_Health", playerStatComponent.HealthScore);
            PlayerPrefs.SetInt("Tap2Dash_GamesPlayed", playerStatComponent.GamesPlayed);//GameModeCount
            PlayerPrefs.SetInt("Tap2Dash_GameModeCount", playerStatComponent.GameModeCount);
            PlayerPrefs.SetInt("Tap2Dash_GameMode", (int)playerStatComponent.LastGameMode);
            PlayerPrefs.SetInt("Tap2Dash_UserLevel", playerStatComponent.UserLevel);
            PlayerPrefs.Save();
        }

        private void SaveHealth(in PlayerStatComponent playerStatComponent)
        {
            PlayerPrefs.SetInt("Tap2Dash_Health", playerStatComponent.HealthScore);
            PlayerPrefs.SetInt("Tap2Dash_GamesPlayed", playerStatComponent.GamesPlayed);
            PlayerPrefs.Save();
        }

        private void RestoreResult(ref PlayerStatComponent playerStatComponent)
        {
            //if (PlayerPrefs.HasKey("Tap2Dash_HighScore"))
                playerStatComponent.HighScore = PlayerPrefs.GetInt("Tap2Dash_HighScore",0);

            //if (PlayerPrefs.HasKey("Tap2Dash_LastScore"))    
                playerStatComponent.LastScore = PlayerPrefs.GetInt("Tap2Dash_LastScore",0);

            //if (PlayerPrefs.HasKey("Tap2Dash_LastLevel"))    
                playerStatComponent.LastLevel = PlayerPrefs.GetInt("Tap2Dash_LastLevel",0);

            //if (PlayerPrefs.HasKey("Tap2Dash_TotalCrystal"))
                playerStatComponent.TotalCrystalScore = PlayerPrefs.GetInt("Tap2Dash_TotalCrystal",0);

            //if (PlayerPrefs.HasKey("Tap2Dash_Health"))
                playerStatComponent.HealthScore = PlayerPrefs.GetInt("Tap2Dash_Health",5);

            //if(PlayerPrefs.HasKey("Tap2Dash_GamesPlayed"))
                playerStatComponent.GamesPlayed = PlayerPrefs.GetInt("Tap2Dash_GamesPlayed",0);

            //if(PlayerPrefs.HasKey("Tap2Dash_UserLevel"))
                playerStatComponent.UserLevel = PlayerPrefs.GetInt("Tap2Dash_UserLevel",5);

            //if(PlayerPrefs.HasKey("Tap2Dash_GameModeCount"))
                playerStatComponent.GameModeCount = PlayerPrefs.GetInt("Tap2Dash_GameModeCount",0);

            //if(PlayerPrefs.HasKey("Tap2Dash_GameMode"))
            //{
                int defaultGameMode = (int)GameMode.Normal;
                int savedGameMode = PlayerPrefs.GetInt("Tap2Dash_GameMode", defaultGameMode);
                playerStatComponent.LastGameMode = (GameMode)savedGameMode;
           //}

            
        }
    }
}