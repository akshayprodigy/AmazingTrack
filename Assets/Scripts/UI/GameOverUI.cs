using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace AmazingTrack
{
    public class GameOverUI : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI coinText;
        [SerializeField] TextMeshProUGUI totalCoinText;
        [SerializeField] TextMeshProUGUI healthText;
        [SerializeField] TextMeshProUGUI scoreText;
        [SerializeField] TextMeshProUGUI CountDownText;
        [SerializeField] GameObject TutorialText;
        
        private int score;
        private int crystalScore;
        private int highScore;
        private int health;
        private int level;

        [Inject] private PlayerStatService playerStatService;
        [Inject]
        private GameSystem gameSystem;
        private readonly GameSettings gameSettings;
        void OnEnable()
        {
            TutorialText.SetActive(true);
            ref var playerStatComponent = ref playerStatService.GetPlayerStat();
            scoreText.text = "" + playerStatComponent.Score;
            coinText.text = "+ " + playerStatComponent.CrystalScore;
            totalCoinText.text = "" + playerStatComponent.TotalCrystalScore;
            healthText.text = "" + playerStatComponent.HealthScore;
            GoogleAdsManager.Instance.OnRewardedAdRewarded += OnRewardedVideoCompleted;
        }

        void OnDisable()
        {
            GoogleAdsManager.Instance.OnRewardedAdRewarded -= OnRewardedVideoCompleted;
        }

        private void OnRewardedVideoCompleted()
        {
            ref var playerStatComponent = ref playerStatService.GetPlayerStat();
            playerStatComponent.IsGameFromClicked = false;
            gameSystem.OnReviveButtonPressed();
        }

        private void  FixedUpdate()
        {
            var gameStateComponent = gameSystem.GetGameState();
            CountDownText.text = "" + (int)gameStateComponent.ReviveTimer;
        }

        public void Revive(){
            if(GoogleAdsManager.Instance.IsRewardedVideoReady()){
                GoogleAdsManager.Instance.ShowRewardedAd();
            }else{
                ref var playerStatComponent = ref playerStatService.GetPlayerStat();
                playerStatComponent.TotalCrystalScore -= playerStatService.RetryCrystals;
                totalCoinText.text = "+ " + playerStatComponent.TotalCrystalScore;
                playerStatComponent.IsGameFromClicked = false;
                gameSystem.OnReviveButtonPressed();
            }
            
        }
    }
}
