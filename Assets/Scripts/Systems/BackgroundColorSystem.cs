// using UnityEngine;
// using Zenject;

// namespace AmazingTrack
// {
//     public class BackgroundColorSystem : ITickable, IInitializable
//     {
//         private readonly GameSettings gameSettings;
//         private Camera cam;
//         private Material skyboxMaterial;

//         public BackgroundColorSystem(GameSettings gameSettings)
//         {
//             this.gameSettings = gameSettings;
//         }

//         public void Initialize()
//         {
//             cam = Camera.main;
            
//             // Use the skybox material defined in GameSettings
//             skyboxMaterial = gameSettings.SkyboxMaterial;
            
//             // Set the camera to use the skybox
//             cam.clearFlags = CameraClearFlags.Skybox;
            
//             // Apply the skybox material to the scene
//             RenderSettings.skybox = skyboxMaterial;
//         }

//         public void Tick()
//         {
//             float duration = gameSettings.BackgroundChangeDuration;
//             float t = Mathf.PingPong(Time.time, duration) / duration;
            
//             // Interpolate between the two background colors
//             Color gradientColor1 = Color.Lerp(gameSettings.BackgroundColorTop1, gameSettings.BackgroundColorTop2, t);
//             Color gradientColor2 = Color.Lerp(gameSettings.BackgroundColorBottom1, gameSettings.BackgroundColorBottom2, t);

//             // Update the skybox material properties
//             skyboxMaterial.SetColor("_Color1", gradientColor1);
//             skyboxMaterial.SetColor("_Color2", gradientColor2);
//         }

//         public void ChangeColor(){
//             // Change the color properties of the skybox over time
//             Color gradientColor = Color.Lerp(gameSettings.BackgroundColorTop1, gameSettings.BackgroundColorTop2, 0.5f);
            
//             // Assuming the skybox shader has properties for top and bottom gradient colors
//             skyboxMaterial.SetColor("_Tint", gradientColor); // For example, setting tint for the skybox
//         }
//     }
// }

//======================================================================================================================================================================
//======================================================================================================================================================================

// using UnityEngine;
// using Zenject;

// namespace AmazingTrack
// {
//     public class BackgroundColorSystem : ITickable, IInitializable
//     {
//         private readonly GameSettings gameSettings;
//         private readonly PlayerStatService playerStatService;
//         private Camera cam;
//         private Material skyboxMaterial;

//         // Variables to store the transition state
//         private Color currentTopColor;
//         private Color currentBottomColor;
//         private bool isTransitioning;
//         private float transitionProgress;

//         private int previousLevel; // Store the previous level for comparison

//         public BackgroundColorSystem(GameSettings gameSettings, PlayerStatService playerStatService)
//         {
//             this.gameSettings = gameSettings;
//             this.playerStatService = playerStatService;
//         }

//         public void Initialize()
//         {
//             cam = Camera.main;
            
//             // Use the skybox material defined in GameSettings
//             skyboxMaterial = gameSettings.SkyboxMaterial;
            
//             // Set the camera to use the skybox
//             cam.clearFlags = CameraClearFlags.Skybox;
            
//             // Apply the skybox material to the scene
//             RenderSettings.skybox = skyboxMaterial;

//             // Initialize the current colors
//             currentTopColor = gameSettings.BackgroundColorTop1;
//             currentBottomColor = gameSettings.BackgroundColorBottom1;

//             // Set initial colors for the skybox
//             skyboxMaterial.SetColor("_Color1", currentTopColor);
//             skyboxMaterial.SetColor("_Color2", currentBottomColor);

//             // Set initial previousLevel to current level
//             // ref var playerStatComponent = ref playerStatService.GetPlayerStat();
//             previousLevel = gameSettings.Level;
//         }

//         public void Tick()
//         {
//             // // Check if the level has changed
//             // ref var playerStatComponent = ref playerStatService.GetPlayerStat();
           
//             // if (playerStatComponent.Level != previousLevel)
//             // {
//             //     // Level has changed, initiate color change
//             //     ChangeColor();
//             //     previousLevel = playerStatComponent.Level; // Update previousLevel to the new level
//             // }

//             // if (isTransitioning)
//             // {
//             //     // Continue color transition if active
//             //     transitionProgress += Time.deltaTime / gameSettings.BackgroundChangeDuration;

//             //     // Lerp top and bottom colors
//             //     currentTopColor = Color.Lerp(currentTopColor, GetTargetTopColor(playerStatComponent), transitionProgress);
//             //     currentBottomColor = Color.Lerp(currentBottomColor, GetTargetBottomColor(playerStatComponent), transitionProgress);

//             //     // Apply the interpolated colors to the skybox
//             //     skyboxMaterial.SetColor("_Color1", currentTopColor);
//             //     skyboxMaterial.SetColor("_Color2", currentBottomColor);

//             //     // Stop the transition when it's complete
//             //     if (transitionProgress >= 1.0f)
//             //     {
//             //         isTransitioning = false;
//             //         transitionProgress = 0.0f;
//             //     }
//             // }
//         }

//         // Trigger color change when level changes
//         private void ChangeColor()
//         {
//             // Start transitioning colors based on the current level
//             Debug.Log("Changing background color for level " + gameSettings.Level);
//             isTransitioning = true;
//             transitionProgress = 0.0f;

//             // Store current colors as the start point
//             currentTopColor = skyboxMaterial.GetColor("_Color1");
//             currentBottomColor = skyboxMaterial.GetColor("_Color2");
//         }

//         // Get the target top color based on the current level (lerped between ColorTop1 and ColorTop2)
//         private Color GetTargetTopColor(PlayerStatComponent playerStatComponent)
//         {
//             float t = Mathf.Clamp01((playerStatComponent.Level - 1) / 9f); // Normalized value between 0 (level 1) and 1 (level 10)
//             return Color.Lerp(gameSettings.BackgroundColorTop1, gameSettings.BackgroundColorTop2, t);
//         }

//         // Get the target bottom color based on the current level (lerped between ColorBottom1 and ColorBottom2)
//         private Color GetTargetBottomColor(PlayerStatComponent playerStatComponent)
//         {
//             float t = Mathf.Clamp01((playerStatComponent.Level - 1) / 9f); // Normalized value between 0 (level 1) and 1 (level 10)
//             return Color.Lerp(gameSettings.BackgroundColorBottom1, gameSettings.BackgroundColorBottom2, t);
//         }
//     }
// }

//======================================================================================================================================================================
//======================================================================================================================================================================


using Leopotam.EcsLite;
using UnityEngine;
using Zenject;

namespace AmazingTrack
{
    public class BackgroundColorSystem : ITickable, IInitializable
    {
        private readonly GameSettings gameSettings;
        private readonly PlayerStatService playerStatService;
        private readonly EcsWorld world; 
        private Camera cam;
        private Material skyboxMaterial;

        private bool isTransitioning;
        private float transitionProgress;

        private readonly EcsFilter levelUpEventFilter;
        private readonly EcsPool<PlayerLevelUpEventComponent> playerLevelUpEventPool;

        public BackgroundColorSystem(EcsWorld world, GameSettings gameSettings, PlayerStatService playerStatService)
        {
            this.world = world; 
            this.gameSettings = gameSettings;
            this.playerStatService = playerStatService;

            levelUpEventFilter = world.Filter<PlayerLevelUpEventComponent>().End();
            playerLevelUpEventPool = world.GetPool<PlayerLevelUpEventComponent>();
        }

        public void Initialize()
        {
            cam = Camera.main;

            // Use the skybox material defined in GameSettings
            skyboxMaterial = gameSettings.SkyboxMaterial;

            // Set the camera to use the skybox
            cam.clearFlags = CameraClearFlags.Skybox;

            // Apply the skybox material to the scene
            RenderSettings.skybox = skyboxMaterial;

            // Initialize the current colors
            skyboxMaterial.SetColor("_Color1", gameSettings.BackgroundColorTop1);
            skyboxMaterial.SetColor("_Color2", gameSettings.BackgroundColorBottom1);
        }

        public void Tick()
        {
            foreach (var i in levelUpEventFilter)
            {
                var entity = i;

                // Since we are iterating entities that match the PlayerLevelUpEventComponent,
                // we can safely get the component.
                ref var levelUpEvent = ref playerLevelUpEventPool.Get(entity);

                Debug.Log($"Handling level up event for player {levelUpEvent.PlayerId} to level {levelUpEvent.NewLevel}");

                // Trigger color change when level changes
                ChangeColor();

                // Destroy the event after handling it
                world.DelEntity(entity); 
            }

            if (isTransitioning)
            {
                // Continue color transition if active
                transitionProgress += Time.deltaTime / gameSettings.BackgroundChangeDuration;

                // Lerp top and bottom colors
                var t = Mathf.Clamp01(transitionProgress);
                var topColor = Color.Lerp(gameSettings.BackgroundColorTop1, gameSettings.BackgroundColorTop2, t);
                var bottomColor = Color.Lerp(gameSettings.BackgroundColorBottom1, gameSettings.BackgroundColorBottom2, t);

                // Apply the interpolated colors to the skybox
                skyboxMaterial.SetColor("_Color1", topColor);
                skyboxMaterial.SetColor("_Color2", bottomColor);

                // Stop the transition when it's complete
                if (transitionProgress >= 1.0f)
                {
                    isTransitioning = false;
                    transitionProgress = 0.0f;
                }
            }
        }

        private void ChangeColor()
        {
            // Start transitioning colors based on the current level
            Debug.Log("Changing background color for new level");
            isTransitioning = true;
            transitionProgress = 0.0f;
        }
    }
}