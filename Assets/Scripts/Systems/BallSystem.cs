﻿using Leopotam.EcsLite;
using UnityEngine;
using Zenject;

namespace AmazingTrack
{
    public class BallSystem : ITickable
    {
        private readonly EcsWorld world;
        private readonly AudioPlayer audioPlayer;
        private readonly AudioSettings audioSettings;
        private readonly GameSettings gameSettings;

        private readonly EcsPool<BallComponent> ballPool;
        private readonly EcsPool<BlockComponent> blockPool;
        private readonly EcsPool<CrystalComponent> crystalPool;
        private readonly EcsPool<ViewLinkComponent> viewLinkPool;
        private readonly EcsPool<FallingComponent> fallingPool;
        private readonly EcsPool<BallPassedComponent> ballPassedPool;
        private readonly EcsPool<BallHitComponent> ballHitCrystalPool;
        private readonly EcsFilter ballFilter;

        private readonly Collider[] hitColliders = new Collider[1];

        public BallSystem(EcsWorld world, AudioPlayer audioPlayer, AudioSettings audioSettings,
            GameSettings gameSettings)
        {
            this.world = world;
            this.audioPlayer = audioPlayer;
            this.audioSettings = audioSettings;
            this.gameSettings = gameSettings;

            ballPool = world.GetPool<BallComponent>();
            blockPool = world.GetPool<BlockComponent>();
            crystalPool = world.GetPool<CrystalComponent>();
            viewLinkPool = world.GetPool<ViewLinkComponent>();
            fallingPool = world.GetPool<FallingComponent>();
            ballPassedPool = world.GetPool<BallPassedComponent>();
            ballHitCrystalPool = world.GetPool<BallHitComponent>();
            ballFilter = world.Filter<BallComponent>().End();
        }

        public void ChangeDirection(int ball)
        {
            ref var viewLinkComponent = ref viewLinkPool.Get(ball);
            var position = viewLinkComponent.Transform.position;
            
            if (!CheckEntityUnder(position, out _))
                return; // can't change direction when fall

            ref var ballComponent = ref ballPool.Get(ball);
            if(ballComponent.LastFallingDirection != Vector3.zero) {
                ballComponent.Direction = ballComponent.LastFallingDirection == Vector3.forward ? -Vector3.left : Vector3.forward;
                ballComponent.LastFallingDirection = Vector3.zero;
            }else 
            ballComponent.Direction = ballComponent.Direction == Vector3.forward ? -Vector3.left : Vector3.forward;

            audioPlayer.Play(audioSettings.BallTurnSound);
        }

        public void Tick()
        {
            foreach (var ball in ballFilter)
                Update(ball);
        }

        private void Update(int ball)
        {
            ref var ballComponent = ref ballPool.Get(ball);

            if (ballComponent.Direction == Vector3.zero)
                return;
                
            
            ref var viewLinkComponent = ref viewLinkPool.Get(ball);
            // var position = viewLinkComponent.Transform.position;
            
            if (!fallingPool.Has(ball))
            {
                var position = viewLinkComponent.Transform.position;
                if (CheckCollisionWithCrystal(position, out int crystal) && crystalPool.Has(crystal))
                    ballHitCrystalPool.Add(crystal);

                if (CheckEntityUnder(position, out var block) && blockPool.Has(block))
                {
                    if (block != ballComponent.PreviousHitEntity)
                    {
                        if (ballComponent.PreviousHitEntity != null)
                            ballPassedPool.Add(ballComponent.PreviousHitEntity.Value);
                        ballComponent.PreviousHitEntity = block;
                    }
                }
                else
                {
                    ballComponent.LastFallingDirection = ballComponent.Direction;
                    fallingPool.Add(ball) = new FallingComponent { FallingDelay = 0.0f, FallingTime = 12.0f };
                }
            }

            float speed = !fallingPool.Has(ball) ? ballComponent.Speed : 1;
            viewLinkComponent.Transform.Translate(ballComponent.Direction * speed * Time.deltaTime);
        }

        public void ResetBall(int ball)
        {
            Debug.Log("Resetting ball position to the center of the last block.");
            ref var ballComponent = ref ballPool.Get(ball);
            ref var viewLinkComponent = ref viewLinkPool.Get(ball);

            if (fallingPool.Has(ball))
            {
                // Remove the falling component since the ball is being reset
                fallingPool.Del(ball);
            }
            

            // If the ball has a previous hit block, reset its position to the center of that block
            if (ballComponent.PreviousHitEntity.HasValue)
            {
                int blockEntity = ballComponent.PreviousHitEntity.Value;

                if (viewLinkPool.Has(blockEntity))
                {
                     ref var blockViewLink = ref viewLinkPool.Get(blockEntity);

                    // Get the child count of the block
                    int childCount = blockViewLink.Transform.childCount;

                    if (childCount > 0)
                    {
                        // Determine the middle child
                        int middleChildIndex = childCount / 2;

                        // Get the middle child's transform
                        Transform middleChild = blockViewLink.Transform.GetChild(middleChildIndex);

                        // Log the position of the middle child
                        Debug.Log($"Middle Child - Name: {middleChild.name}, Position: {middleChild.position}");

                        // Reset ball position to the position of the middle child
                        viewLinkComponent.Transform.position = new Vector3(middleChild.position.x, middleChild.position.y + 0.75f, middleChild.position.z); // Add height offset if needed to place above the child
                    }
                    else
                    {
                        Debug.LogWarning("Block entity has no children, unable to reset ball position.");
                    }
                }
                else
                {
                    Debug.LogWarning("Block entity does not have a view link, unable to reset ball position.");
                }
            }
            else
            {
                Debug.LogWarning("No previous block entity found, unable to reset ball position.");
            }

            // Reset direction to zero or a default direction if needed
            ballComponent.Direction = Vector3.zero;
            // Time.timeScale = 0f;
            viewLinkComponent.Transform.GetComponent<Rigidbody>().isKinematic = true;
            Debug.Log("Ball has been reset to the center of the last block.");
        }

        private bool CheckEntityUnder(Vector3 position, out int hitEntity)
        {
            const float SphereCastRadius = 0.1f;
            if (Physics.SphereCast(position, SphereCastRadius, Vector3.down, out var hit, 0.3f))
            {
                var linkView = hit.transform.parent.GetComponent<EntityLinkView>();
                if (linkView != null)
                    return linkView.Entity.Unpack(world, out hitEntity);
            }

            hitEntity = -1;
            return false;
        }

        private bool CheckCollisionWithCrystal(Vector3 position, out int hitEntity)
        {
            const float BallOverlapRadius = 0.4f;
            if (Physics.OverlapSphereNonAlloc(position, BallOverlapRadius, hitColliders, gameSettings.CrystalMask) != 0)
            {
                var linkView = hitColliders[0].transform.GetComponent<EntityLinkView>();
                return linkView.Entity.Unpack(world, out hitEntity);
            }
            
            hitEntity = -1;
            return false;
        }
    }
}