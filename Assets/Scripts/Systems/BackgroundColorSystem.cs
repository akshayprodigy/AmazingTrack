

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