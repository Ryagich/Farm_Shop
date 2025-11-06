using MessagePipe;
using Messages;
using UniRx;
using UnityEngine;

namespace Movement
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class PlayerAnimationController
    {
        private readonly PlayerMovementConfig playerMovementConfig;
        private readonly Animator animator;
        private readonly CompositeDisposable disposables = new();

        private bool canPlay = true;
        public PlayerAnimationController
            (
                PlayerMovementConfig playerMovementConfig,
                Animator animator,
                ISubscriber<PlayerMoveMessage> subscriber
            )
        {
            this.playerMovementConfig = playerMovementConfig;
            this.animator = animator;

            subscriber.Subscribe(OnVelocityChanged).AddTo(disposables);  
        }

        public void ChangeState(bool newState, Vector2 direction)
        {
            canPlay = newState;
            Update(direction);
        }
        
        private void OnVelocityChanged(PlayerMoveMessage msg)
        {
            Update(msg.Direction);
        }

        private void Update(Vector2 direction)
        {
            animator.SetBool(playerMovementConfig.MovingName, canPlay && direction is not {x: 0, y: 0});
        }
    }
}