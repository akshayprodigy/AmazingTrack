using Leopotam.EcsLite;
using UnityEngine;
using Zenject;

namespace AmazingTrack
{
    public class CrystalSystem : ITickable
    {
        private readonly ObjectSpawner spawner;
        private readonly GameObject particleEffectPrefab; 
        private const float RotationSpeed = 60.0f;

        private readonly EcsPool<ViewLinkComponent> viewLinkPool;
        
        private readonly EcsFilter crystalFilter;
        private readonly EcsFilter ballHitCrystalFilter;

        public CrystalSystem(EcsWorld world, ObjectSpawner spawner, GameObject particleEffectPrefab)
        {
            this.spawner = spawner;
            this.particleEffectPrefab = particleEffectPrefab;
            viewLinkPool = world.GetPool<ViewLinkComponent>();
            
            crystalFilter = world.Filter<CrystalComponent>().End();
            ballHitCrystalFilter = world.Filter<CrystalComponent>().Inc<BallHitComponent>().End();
        }

        public void Tick()
        {
            foreach (var crystal in crystalFilter)
            {
                ref var viewLinkComponent = ref viewLinkPool.Get(crystal);
                viewLinkComponent.Transform.Rotate(Vector3.up, RotationSpeed * Time.deltaTime);
            }

            foreach (var crystal in ballHitCrystalFilter)
            {
                ref var viewLinkComponent = ref viewLinkPool.Get(crystal);
                Vector3 crystalPosition = viewLinkComponent.Transform.position;

                // Instantiate the particle effect at the crystal's position
                var particleEffect = GameObject.Instantiate(particleEffectPrefab, crystalPosition, Quaternion.identity);
                // createa a pool for partile effect instead of instantiating it
                GameObject.Destroy(particleEffect, 5.0f);
                spawner.DespawnObject(crystal);

            }
                
        }
    }
}