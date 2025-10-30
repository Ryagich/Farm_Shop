using System.Diagnostics.CodeAnalysis;
using MessagePipe;
using Messages;
using UniRx;
using UnityEngine;
using VContainer.Unity;

namespace Movement
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class PlayerMovement : ITickable
    {
        private Vector2 velocity;
        private readonly Camera cam;
        private readonly Transform playerTransform;
        private readonly CharacterController controller;
        private readonly PlayerMovementConfig config;

        private readonly CompositeDisposable disposables = new();

        [SuppressMessage("ReSharper", "ParameterHidesMember")]
        private PlayerMovement
            (
                Camera cam,
                ISubscriber<PlayerMoveMessage> subscriber,
                Transform playerTransform,
                CharacterController controller,
                PlayerMovementConfig config
            )
        {
            this.cam = cam;
            this.playerTransform = playerTransform;
            this.config = config;
            this.controller = controller;

            subscriber.Subscribe(OnMove).AddTo(disposables);
        }

        private void OnMove(PlayerMoveMessage msg)
        {
            velocity = msg.Direction;
        }

        public void Tick()
        {
            if (velocity is { x: 0, y: 0 })
            {
                return;
            }
            var moveDirection = Quaternion.Euler(0, cam.transform.rotation.eulerAngles.y, 0) *
                                new Vector3(velocity.x, 0, velocity.y);
            // направление, куда игрок должен смотреть
            var targetRotation = Quaternion.LookRotation(moveDirection, Vector3.up);
            // плавно поворачиваем игрока
            //playerTransform.rotation = Quaternion.RotateTowards(
            //                                                    playerTransform.rotation,
            //                                                    targetRotation,
            //                                                    config.RotationSpeed * Time.deltaTime // скорость поворота в градусах/сек
            //                                                   );
            var angle = Mathf.Rad2Deg * Mathf.Atan2(moveDirection.x, moveDirection.z);
            playerTransform.rotation = Quaternion.Euler(0, angle, 0);
            controller.Move(playerTransform.forward * (config.Speed * Time.deltaTime));
        }
    }
}
