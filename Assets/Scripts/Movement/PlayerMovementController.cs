using CameraScripts;
using Gravity;
using MessagePipe;
using Messages;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Movement
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class PlayerMovementController : IStartable
    {
        private readonly PlayerMovementConfig playerMovementConfig;
        private readonly GravityConfig gravityConfig;
        private readonly IObjectResolver resolver;
        private readonly Transform transform;
        private readonly PlayerMovement playerMovement;
        private readonly VirtualPlayerMovement virtualPlayerMovement;
        private readonly PlayerParticleController playerParticleController;
        private readonly PlayerAnimationController playerAnimationController;
        private readonly CameraMovement cameraMovement;

        private Vector2 direction;
        private Transform vpTransform;
        private PlayerGravity vpPlayerGravity;
        
        public PlayerMovementController
            (
                PlayerMovementConfig playerMovementConfig,
                GravityConfig gravityConfig,
                IObjectResolver resolver,
                Transform transform,
                PlayerMovement playerMovement,
                VirtualPlayerMovement virtualPlayerMovement,
                PlayerParticleController playerParticleController,
                PlayerAnimationController playerAnimationController,
                CameraMovement cameraMovement,
                ISubscriber<GameModeChangedMessage> gameModeChangedSubscriber,
                ISubscriber<PlayerMoveMessage> playerMoveSubscriber
            )
        {
            this.playerMovementConfig = playerMovementConfig;
            this.gravityConfig = gravityConfig;
            this.resolver = resolver;
            this.transform = transform;
            this.playerMovement = playerMovement;
            this.virtualPlayerMovement = virtualPlayerMovement;
            this.playerParticleController = playerParticleController;
            this.playerAnimationController = playerAnimationController;
            this.cameraMovement = cameraMovement;

            gameModeChangedSubscriber.Subscribe(OnGameModeChanged);
            playerMoveSubscriber.Subscribe(OnMove);
        }

        public void Start()
        {
            var vp = resolver.Instantiate(playerMovementConfig.VirtualPlayerPrefab, transform.parent);
            vpTransform = vp.GetComponent<Transform>();
            virtualPlayerMovement.Construct(vpTransform, vp);
            vpPlayerGravity = new PlayerGravity(vp, gravityConfig);
        }

        private void OnGameModeChanged(GameModeChangedMessage msg)
        {
            if (msg.GameMode is GameModes.GameModes.Game)
            {
                playerMovement.ChangeState(true);
                virtualPlayerMovement.ChangeState(false, transform.position);
                playerParticleController.ChangeState(true, direction);
                playerAnimationController.ChangeState(true, direction);
                cameraMovement.ChangeTarget(transform);
            }
            else if (msg.GameMode is GameModes.GameModes.Redactor)
            {
                playerMovement.ChangeState(false);
                virtualPlayerMovement.ChangeState(true, transform.position);
                playerParticleController.ChangeState(false, direction);
                playerAnimationController.ChangeState(false, direction);
                cameraMovement.ChangeTarget(vpTransform);
            }
            else
            {
                playerMovement.ChangeState(false);
                virtualPlayerMovement.ChangeState(false, transform.position);
                playerParticleController.ChangeState(false, direction);
                playerAnimationController.ChangeState(false, direction);
                cameraMovement.ChangeTarget(transform);
            }
        }
        
        private void OnMove(PlayerMoveMessage msg) => direction = msg.Direction;
    }
}