using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace AmazingTrack
{
    public class GameOverUI : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI coinText;
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
[       Inject]
        private GameSystem gameSystem;
        void OnEnable()
        {
            TutorialText.SetActive(true);
            ref var playerStatComponent = ref playerStatService.GetPlayerStat();
            scoreText.text = "" + playerStatComponent.Score;
            coinText.text = "" + playerStatComponent.CrystalScore;
            healthText.text = "" + playerStatComponent.HealthScore;
        }
        
        private void  FixedUpdate()
        {
            var gameStateComponent = gameSystem.GetGameState();
            CountDownText.text = "" + (int)gameStateComponent.ReviveTimer;
        }

        public void Revive(){
            gameSystem.OnReviveButtonPressed();
        }
    }
}
