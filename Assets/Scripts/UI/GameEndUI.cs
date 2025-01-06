using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using System.Collections;
using System.IO;

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
        [SerializeField] GameObject RewardButton;

        [SerializeField] GameObject RewardLiveButton;
        private int gameEndCount = 0;
        private bool isRevive = false;

        private string shareText = "Check out my high score! #MyHighScore";
        private string playStoreLink = "https://play.google.com/store/apps/details?id=com.yourcompany.yourgame";
        private string screenshotPath;

        public Image screenshotPreview;
        
        private void OnEnable()
        {
            gameEndCount++;
            ref var playerStatComponent = ref playerStatService.GetPlayerStat();
            RewardButton.SetActive(false);
            RewardLiveButton.SetActive(false);
            string text = "Your score: " + playerStatComponent.Score;
            bool newRecord = playerStatComponent.Score == playerStatComponent.HighScore;
            if (newRecord){
                text += "\nNew record !";
                StartCoroutine(TakeScreenshot());
            }
            else
                text += "\nHigh score: " + playerStatComponent.HighScore;


            scoreText.text = text;

            coinText.text = "" + playerStatComponent.TotalCrystalScore;
            healthText.text = "" + playerStatComponent.HealthScore;

            if(GoogleAdsManager.Instance.IsRewardedVideoReady()){
                RewardButton.SetActive(true);
            }
            GoogleAdsManager.Instance.OnRewardedAdRewarded += OnRewardedVideoCompleted;
             // Show interstitial ad after the first 3 instances and then after every 2 instances
            if (gameEndCount == 3 || (gameEndCount > 3 && (gameEndCount - 3) % 2 == 0))
            {
                if(GoogleAdsManager.Instance.IsInterstitialAdReady()){
                    GoogleAdsManager.Instance.InterstitialShowAd();
                }
            }
        }

        /// <summary>
        /// This function is called when the behaviour becomes disabled or inactive.
        /// </summary>
        void OnDisable()
        {
            GoogleAdsManager.Instance.OnRewardedAdRewarded -= OnRewardedVideoCompleted;
        }

        public void OnRewardedVideoCompleted()
        {
            
            ref var playerStatComponent = ref playerStatService.GetPlayerStat();
            // TODO add coids to Total Coins
            if(isRevive){
                RewardLiveButton.SetActive(false);
                playerStatComponent.HealthScore+=2;
            }else{
                RewardButton.SetActive(false);
                playerStatComponent.TotalCrystalScore += playerStatComponent.CrystalScore;
                coinText.text = "" + playerStatComponent.TotalCrystalScore;
                //  add sfx effect for 2x reward
            }

        }

         IEnumerator TakeScreenshot()
        {
            yield return new WaitForEndOfFrame();
            
            Texture2D screenshot = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
            screenshot.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
            screenshot.Apply();

            byte[] bytes = screenshot.EncodeToPNG();
            Destroy(screenshot);

            screenshotPath = Path.Combine(Application.temporaryCachePath, "screenshot.png");
            File.WriteAllBytes(screenshotPath, bytes);

            Debug.Log("Screenshot saved to: " + screenshotPath);

            // Display the screenshot in the UI
            Texture2D previewTexture = new Texture2D(2, 2);
            previewTexture.LoadImage(bytes);
            screenshotPreview.sprite = Sprite.Create(previewTexture, new Rect(0, 0, previewTexture.width, previewTexture.height), new Vector2(0.5f, 0.5f));
        }


        public void OnShareButtonPressed()
        {
            if (!string.IsNullOrEmpty(screenshotPath))
            {
                StartCoroutine(ShareScreenshot());
            }
            else
            {
                Debug.LogError("Screenshot not taken yet.");
            }
        }

        IEnumerator ShareScreenshot()
        {
            yield return new WaitForEndOfFrame();
            
            string shareSubject = "I just reached a new high score!";
            string shareMessage = shareText + "\nCheck out the game here: " + playStoreLink;

            // new NativeShare()
            //     .AddFile(screenshotPath)
            //     .SetSubject(shareSubject)
            //     .SetText(shareMessage)
            //     .Share();
        }
        
        public void OnRewardButtonClick(){
            GoogleAdsManager.Instance.ShowRewardedAd();
        }

        public void OnRewardLifeButtonClick(){
            isRevive = true;
            GoogleAdsManager.Instance.ShowRewardedAd();
        }

        public void OnRestartButton()
        {
                ref var playerStatComponent = ref playerStatService.GetPlayerStat();
                
                if (playerStatComponent.HealthScore <= 0)
                {
                    // Show video ads to buy health points or show game over screen
                    if(GoogleAdsManager.Instance.IsRewardedVideoReady()){
                        RewardLiveButton.SetActive(true);
                    }

                }else{
                    Debug.Log("Restarting game: " + playerStatComponent.HealthScore);
                    
                    Debug.Log("Restarting game: " + playerStatComponent.HealthScore);
                   AdjustDifficultyBasedOnPlayerPerformance();
                   playerStatComponent.HealthScore--;
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