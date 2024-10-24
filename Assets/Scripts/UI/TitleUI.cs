using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Zenject;

namespace AmazingTrack
{
    public class TitleUI : MonoBehaviour
    {
        [Inject] 
        private GameSystem gameSystem;
        [Inject] private PlayerStatService playerStatService;


        [SerializeField] TextMeshProUGUI coinText;
        [SerializeField] TextMeshProUGUI healthText;

        public void OnEasyButtonClick()
        {
            gameSystem.GameStart(GameMode.Easy);
            ref var playerStatComponent = ref playerStatService.GetPlayerStat();
            if(playerStatComponent.LastGameMode == GameMode.Easy)
            {
                playerStatComponent.GameModeCount++;
            }
            playerStatComponent.LastGameMode = GameMode.Easy;
            playerStatComponent.HealthScore--;
        }

        public void OnNormalButtonClick()
        {
            gameSystem.GameStart(GameMode.Normal);

            ref var playerStatComponent = ref playerStatService.GetPlayerStat();
            if(playerStatComponent.LastGameMode == GameMode.Normal)
            {
                playerStatComponent.GameModeCount++;
            }
            playerStatComponent.LastGameMode = GameMode.Normal;
            playerStatComponent.HealthScore--;
        }

        public void OnHardButtonClick()
        {
            gameSystem.GameStart(GameMode.Hard);
            ref var playerStatComponent = ref playerStatService.GetPlayerStat();
            if(playerStatComponent.LastGameMode == GameMode.Hard)
            {
                playerStatComponent.GameModeCount++;
            }
            playerStatComponent.LastGameMode = GameMode.Hard;
            playerStatComponent.HealthScore--;
        }

        public void OnHolesButtonClick()
        {
            gameSystem.GameStart(GameMode.Holes);
            ref var playerStatComponent = ref playerStatService.GetPlayerStat();
            if(playerStatComponent.LastGameMode == GameMode.Holes)
            {
                playerStatComponent.GameModeCount++;
            }
            playerStatComponent.LastGameMode = GameMode.Holes;
            playerStatComponent.HealthScore--;
        }

        public void OnPlayButtonClick()
        {
            AdjustDifficultyBasedOnPlayerPerformance();
        }

        private void AdjustDifficultyBasedOnPlayerPerformance()
        {
            // Get the player's stats
            ref var playerStatComponent = ref playerStatService.GetPlayerStat();

            // Assume we are tracking last few game scores in a list
            if(playerStatComponent.UserLevel >= 7){
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
                if(playerStatComponent.GameModeCount >= 2 && playerStatComponent.LastGameMode == GameMode.Normal){
                    OnHolesButtonClick();
                }else{
                OnNormalButtonClick();
                }
            }else{
                // user is a beginner
                OnEasyButtonClick();
            }
            
        }

        private void Update() {
            ref var playerStatComponent = ref playerStatService.GetPlayerStat();
            coinText.text = "" + playerStatComponent.TotalCrystalScore;
            healthText.text = "" + playerStatComponent.HealthScore;
        }
    }
}