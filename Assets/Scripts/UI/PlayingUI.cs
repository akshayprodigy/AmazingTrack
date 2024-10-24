using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace AmazingTrack
{
    public class PlayingUI : MonoBehaviour
    {

        [SerializeField] TextMeshProUGUI coinText;
        [SerializeField] TextMeshProUGUI healthText;
        [SerializeField] TextMeshProUGUI scoreText;
        [SerializeField] GameObject TutorialText;
        
        private int score;
        private int crystalScore;
        private int highScore;
        private int health;
        private int level;

        [Inject] private PlayerStatService playerStatService;

        void OnEnable()
        {
            TutorialText.SetActive(true);
        }
        
        private void Update()
        {
            ref var playerStatComponent = ref playerStatService.GetPlayerStat();
            
            if (playerStatComponent.Score != score)
            {
                if(TutorialText.activeSelf){
                    TutorialText.SetActive(false);
                }
                scoreText.text = "" + playerStatComponent.Score;
                score = playerStatComponent.Score;
                
            }

            if (playerStatComponent.CrystalScore != crystalScore)
            {
                coinText.text = "" + playerStatComponent.CrystalScore;
                crystalScore = playerStatComponent.CrystalScore;
            }

            if (playerStatComponent.HealthScore != health)
            {
                healthText.text = "" + playerStatComponent.HealthScore;
                health = playerStatComponent.HealthScore;
            }

        }
    }
}