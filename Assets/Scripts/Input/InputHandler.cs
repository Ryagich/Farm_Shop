using System.Diagnostics.CodeAnalysis;
using MessagePipe;
using Messages;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer.Unity;

namespace Input
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class InputHandler : IStartable
    {
        private readonly InputConfig inputConfig;
        private readonly IPublisher<PlayerMoveMessage> publisher;
        
        [SuppressMessage("ReSharper", "ParameterHidesMember")]
        private InputHandler
            (
                InputConfig inputConfig, 
                IPublisher<PlayerMoveMessage> publisher
            )
        {
            this.inputConfig = inputConfig;
            this.publisher = publisher;
        }

        public void Start()
        {
            inputConfig.MoveInput.action.Enable();
            inputConfig.MoveInput.action.performed += OnMove;
            inputConfig.MoveInput.action.canceled += OnMove;
        }
        
        private void OnMove(InputAction.CallbackContext context)
        {
            var dir = context.ReadValue<Vector2>();
            publisher.Publish(new PlayerMoveMessage(dir));
        }
        
        public void Dispose()
        {
            inputConfig.MoveInput.action.performed -= OnMove;
            inputConfig.MoveInput.action.canceled -= OnMove;
        }
    }
}