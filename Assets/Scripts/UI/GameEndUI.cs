using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace AmazingTrack
{
    public class GameEndUI : MonoBehaviour
    {
        [Inject] 
        private GameSystem gameSystem;
        [Inject] private PlayerStatService playerStatService;

        [SerializeField] Text scoreText;
        [SerializeField] TextMeshProUGUI coinText;
        [SerializeField] TextMeshProUGUI healthText;

        
        private void OnEnable()
        {
            ref var playerStatComponent = ref playerStatService.GetPlayerStat();
            
            string text = "Your score: " + playerStatComponent.Score;
            bool newRecord = playerStatComponent.Score == playerStatComponent.HighScore;
            if (newRecord)
                text += "\nNew record !";
            else
                text += "\nHigh score: " + playerStatComponent.HighScore;


            scoreText.text = text;

            coinText.text = "" + playerStatComponent.TotalCrystalScore;
            healthText.text = "" + playerStatComponent.HealthScore;
        }

        public void OnRestartButton()
        {
                ref var playerStatComponent = ref playerStatService.GetPlayerStat();
                playerStatComponent.HealthScore--;
                if (playerStatComponent.HealthScore <= 0)
                {
                    // Show video ads to buy health points or show game over screen
                }else{
                   AdjustDifficultyBasedOnPlayerPerformance();
                }
        }

         public void OnEasyButtonClick()
        {
            gameSystem.GameStartToRecreate(GameMode.Easy);
            ref var playerStatComponent = ref playerStatService.GetPlayerStat();
            if(playerStatComponent.LastGameMode == GameMode.Easy)
            {
                playerStatComponent.GameModeCount++;
            }else{
                playerStatComponent.GameModeCount = 1;
            }
            playerStatComponent.LastGameMode = GameMode.Easy;
        }

        public void OnNormalButtonClick()
        {
            gameSystem.GameStartToRecreate(GameMode.Normal);

            ref var playerStatComponent = ref playerStatService.GetPlayerStat();
            if(playerStatComponent.LastGameMode == GameMode.Normal)
            {
                playerStatComponent.GameModeCount++;
            }else{
                playerStatComponent.GameModeCount = 1;
            }
            playerStatComponent.LastGameMode = GameMode.Normal;
        }

        public void OnHardButtonClick()
        {
            gameSystem.GameStartToRecreate(GameMode.Hard);
            ref var playerStatComponent = ref playerStatService.GetPlayerStat();
            if(playerStatComponent.LastGameMode == GameMode.Hard)
            {
                playerStatComponent.GameModeCount++;
            }else{
                playerStatComponent.GameModeCount = 1;
            }
            playerStatComponent.LastGameMode = GameMode.Hard;
        }

        public void OnHolesButtonClick()
        {
            gameSystem.GameStartToRecreate(GameMode.Holes);
            ref var playerStatComponent = ref playerStatService.GetPlayerStat();
            if(playerStatComponent.LastGameMode == GameMode.Holes)
            {
                playerStatComponent.GameModeCount++;
            }else{
                playerStatComponent.GameModeCount = 1;
            }
            playerStatComponent.LastGameMode = GameMode.Holes;
        }

        private void AdjustDifficultyBasedOnPlayerPerformance()
        {
            // Get the player's stats
            ref var playerStatComponent = ref playerStatService.GetPlayerStat();

            // Assume we are tracking last few game scores in a list
            if(playerStatComponent.UserLevel >= 7){
                Debug.Log("User is a pro player");
                // user is a pro player
                if(playerStatComponent.GameModeCount >= 3 && playerStatComponent.LastGameMode == GameMode.Hard){
                    int randomInt = Random.Range(1,10);
                    if(randomInt > 7)
                    {
                        OnHolesButtonClick();
                    }
                    else{
                        OnNormalButtonClick();
                    }
                        
                }else{
                    OnHardButtonClick();
                }
                
            }else if(playerStatComponent.UserLevel >= 3){
                // user is a medium player  
                Debug.Log("User is a medium player"+playerStatComponent.UserLevel);
                if(playerStatComponent.GameModeCount >= 2 && playerStatComponent.LastGameMode == GameMode.Normal){
                    OnHolesButtonClick();
                }else{
                OnNormalButtonClick();
                }
            }else{
                Debug.Log("User is a beginner");
                // user is a beginner
                OnEasyButtonClick();
            }
            
        }

    }
}