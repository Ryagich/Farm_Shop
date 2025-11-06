using System.Diagnostics.CodeAnalysis;
using MessagePipe;
using Messages;
using UnityEngine;
using VContainer.Unity;

namespace Movement
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class VirtualPlayerMovement : ITickable
    {
        private readonly Camera cam;
        private readonly PlayerMovementConfig playerMovementConfig;
        
        private Transform target;
        private CharacterController controller;
        private Vector2 velocity;
        private bool canMove;
      
        public VirtualPlayerMovement
            (
                PlayerMovementConfig playerMovementConfig,
                Camera cam,
                ISubscriber<PlayerMoveMessage> playerMoveSubscriber
            )
        {
            this.playerMovementConfig = playerMovementConfig;
            this.cam = cam;
            
            playerMoveSubscriber.Subscribe(OnMove);
        }

        [SuppressMessage("ReSharper", "ParameterHidesMember")]
        public void Construct(Transform target, CharacterController controller)
        {
            this.target = target;
            this.controller = controller;
        }
        
        public void Tick()
        {
            if (!canMove || velocity is { x: 0, y: 0 })
            {
                return;
            }

            var moveDirection = Quaternion.Euler(0, cam.transform.rotation.eulerAngles.y, 0) *
                                new Vector3(velocity.x, 0, velocity.y);
            var angle = Mathf.Rad2Deg * Mathf.Atan2(moveDirection.x, moveDirection.z);
            target.rotation = Quaternion.Euler(0, angle, 0);
            controller.Move(target.forward * (playerMovementConfig.Speed * Time.deltaTime));
        }
        
        public void ChangeState(bool newState, Vector3 targetPosition)
        {
            controller.enabled = false;
            controller.transform.position = targetPosition;
            controller.enabled = true;
            canMove = newState;
        }
        
        private void OnMove(PlayerMoveMessage msg)
        {
            velocity = msg.Direction;
        }
    }
}