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
        private readonly IPublisher<PlayerMoveMessage> playerMovePublisher;
        private readonly IPublisher<OpenGameModeMessage> openGameModePublisher;
        private readonly IPublisher<OpenRedactorModeMessage> openRedactorModePublisher;
        private readonly IPublisher<OpenShopModeMessage> openShopModePublisher;

        [SuppressMessage("ReSharper", "ParameterHidesMember")]
        private InputHandler
            (
                InputConfig inputConfig, 
                IPublisher<PlayerMoveMessage> playerMovePublisher,
                IPublisher<OpenGameModeMessage> openGameModePublisher,
                IPublisher<OpenRedactorModeMessage> openRedactorModePublisher,
                IPublisher<OpenShopModeMessage> openShopModePublisher
            )
        {
            this.inputConfig = inputConfig;
            this.playerMovePublisher = playerMovePublisher;
            this.openGameModePublisher = openGameModePublisher;
            this.openRedactorModePublisher = openRedactorModePublisher;
            this.openShopModePublisher = openShopModePublisher;
        }

        public void Start()
        {
            // inputConfig.MoveInput.action.Enable();
            inputConfig.MoveInput.action.performed += OnMove;
            inputConfig.MoveInput.action.canceled += OnMove;
            inputConfig.OpenGameMode.action.started += OpenGameMode;
            inputConfig.OpenRedactorMode.action.started += OpenRedactorMode;
            inputConfig.OpenShopMode.action.started += OpenShopMode;
        }
        
        private void OpenGameMode(InputAction.CallbackContext context)
        {
            openGameModePublisher.Publish(new OpenGameModeMessage());
        }

        private void OpenRedactorMode(InputAction.CallbackContext context)
        {
            openRedactorModePublisher.Publish(new OpenRedactorModeMessage());
        }
        
        private void OpenShopMode(InputAction.CallbackContext context)
        {
            openShopModePublisher.Publish(new OpenShopModeMessage());
        }
        
        private void OnMove(InputAction.CallbackContext context)
        {
            var dir = context.ReadValue<Vector2>();
            playerMovePublisher.Publish(new PlayerMoveMessage(dir));
        }
        
        public void Dispose()
        {
            inputConfig.MoveInput.action.performed -= OnMove;
            inputConfig.MoveInput.action.canceled -= OnMove;
        }
    }
}