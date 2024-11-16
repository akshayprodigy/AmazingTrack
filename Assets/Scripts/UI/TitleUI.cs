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
        [SerializeField] GameObject RewardHealthButton;
        [SerializeField] GameObject IAPButton;

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
            playerStatComponent.IsGameFromClicked = true;
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
            playerStatComponent.IsGameFromClicked = true;
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
            playerStatComponent.IsGameFromClicked = true;
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
            playerStatComponent.IsGameFromClicked = true;
        }

        public void OnPlayButtonClick()
        {
            // UnityAdsManager.Instance.LoadRewardedVideo();
            // UnityAdsManager.Instance.LoadInterstitial();
            ref var playerStatComponent = ref playerStatService.GetPlayerStat();
            
            if(playerStatComponent.HealthScore>0)
                AdjustDifficultyBasedOnPlayerPerformance();
            else{
                // ask user to buy health
                 IAPButton.SetActive(true);
            }
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

        /// <summary>
        /// This function is called when the object becomes enabled and active.
        /// </summary>
        
        void OnEnable()
        {
            // UnityAdsManager.Instance.LoadRewardedVideo();
            // UnityAdsManager.Instance.LoadInterstitial();

            
            IAPButton.SetActive(false);
            RewardHealthButton.SetActive(false);
            //if(UnityAdsManager.Instance.IsRewardedVideoReady())
            GoogleAdsManager.Instance.OnRewardedAdLoaded += OnRewardedVideoReady;
            GoogleAdsManager.Instance.OnRewardedAdRewarded += OnRewardedVideoCompleted;
            
            
            // UnityAdsManager.Instance.OnRewardedVideoAvailable += OnRewardedVideoReady;
            // UnityAdsManager.Instance.OnRewardedVideoCompleted += OnRewardedVideoCompleted;
        }

        /// <summary>
        /// Start is called on the frame when a script is enabled just before
        /// any of the Update methods is called the first time.
        /// </summary>
        void Start()
        {
            ref var playerStatComponent = ref playerStatService.GetPlayerStat();
            GoogleAdsManager.Instance.LoadRewardedAd();
            if(playerStatComponent.HealthScore <= 2)
            {
                if(GoogleAdsManager.Instance.IsRewardedVideoReady())
                {
                    RewardHealthButton.SetActive(true);
                }
            }
            
        }

        private void OnRewardedVideoReady()
        {
            ref var playerStatComponent = ref playerStatService.GetPlayerStat();
            if(playerStatComponent.HealthScore <= 2)
            {
                RewardHealthButton.SetActive(true);
            }
        }

        public void OnRewardHealthButtonClick()
        {
            // UnityAdsManager.Instance.ShowRewardedVideo();
            GoogleAdsManager.Instance.ShowRewardedAd();
        }

        public void OnRewardedVideoCompleted(){
            ref var playerStatComponent = ref playerStatService.GetPlayerStat();
            playerStatComponent.HealthScore+=2;
        }

        private void OnDisable()
        {
            GoogleAdsManager.Instance.OnRewardedAdLoaded -= OnRewardedVideoReady;
            GoogleAdsManager.Instance.OnRewardedAdRewarded -= OnRewardedVideoCompleted;
            // UnityAdsManager.Instance.OnRewardedVideoAvailable -= OnRewardedVideoReady;
            // UnityAdsManager.Instance.OnRewardedVideoCompleted -= OnRewardedVideoCompleted;
        }

        private void Update() {
            ref var playerStatComponent = ref playerStatService.GetPlayerStat();
            coinText.text = "" + playerStatComponent.TotalCrystalScore;
            healthText.text = "" + playerStatComponent.HealthScore;
        }
    
    }
}