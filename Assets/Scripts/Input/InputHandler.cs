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
        private readonly IPublisher<ChangeGameModeRequest> changeGameModeRequestPublisher;
        private readonly IPublisher<ClickMessage> clickPublisher;
        private readonly IPublisher<RightClickMessage> rightClickPublisher;
        private readonly IPublisher<LeftRotateMessage> leftRotatePublisher;
        private readonly IPublisher<RightRotateMessage> rightRotatePublisher;
        private readonly IPublisher<InteractableInputMessage> interactableInputPublisher;

        [SuppressMessage("ReSharper", "ParameterHidesMember")]
        private InputHandler
            (
                InputConfig inputConfig, 
                IPublisher<PlayerMoveMessage> playerMovePublisher,
                IPublisher<ChangeGameModeRequest> changeGameModeRequestPublisher,
                IPublisher<ClickMessage> clickPublisher,
                IPublisher<RightClickMessage> rightClickPublisher,
                IPublisher<LeftRotateMessage> leftRotatePublisher,
                IPublisher<RightRotateMessage> rightRotatePublisher,
                IPublisher<InteractableInputMessage> interactableInputPublisher
            )
        {
            this.inputConfig = inputConfig;
            this.playerMovePublisher = playerMovePublisher;
            this.changeGameModeRequestPublisher = changeGameModeRequestPublisher;
            this.clickPublisher = clickPublisher;
            this.rightClickPublisher = rightClickPublisher;
            this.leftRotatePublisher = leftRotatePublisher;
            this.rightRotatePublisher = rightRotatePublisher;
            this.interactableInputPublisher = interactableInputPublisher;
        }

        public void Start()
        {
            inputConfig.Click.action.started += Click;
            inputConfig.RightClick.action.started += RightClick;
            inputConfig.MoveInput.action.performed += OnMove;
            inputConfig.MoveInput.action.canceled += OnMove;
            inputConfig.OpenGameMode.action.started += OpenGameMode;
            inputConfig.OpenRedactorMode.action.started += OpenInventory;
            inputConfig.OpenShopMode.action.started += OpenShopMode;
            inputConfig.RightRotate.action.started += RightRotate;
            inputConfig.LeftRotate.action.started += LeftRotate;
            inputConfig.Interactable.action.started += Interactable;
        }
        
        private void RightRotate(InputAction.CallbackContext context)
        {
            rightRotatePublisher.Publish(new RightRotateMessage());
        }
        
        private void LeftRotate(InputAction.CallbackContext context)
        {
            leftRotatePublisher.Publish(new LeftRotateMessage());
        }
        
        private void Click(InputAction.CallbackContext context)
        {
            clickPublisher.Publish(new ClickMessage());
        }
        
        private void RightClick(InputAction.CallbackContext context)
        {
            rightClickPublisher.Publish(new RightClickMessage());
        }
        
        private void OpenGameMode(InputAction.CallbackContext context)
        {
            changeGameModeRequestPublisher.Publish(new ChangeGameModeRequest(GameModes.GameMode.Game));
        }

        private void OpenInventory(InputAction.CallbackContext context)
        {
            changeGameModeRequestPublisher.Publish(new ChangeGameModeRequest(GameModes.GameMode.Inventory));
        }
        
        private void OpenShopMode(InputAction.CallbackContext context)
        {
            changeGameModeRequestPublisher.Publish(new ChangeGameModeRequest(GameModes.GameMode.Shop));
        }
        
        private void Interactable(InputAction.CallbackContext context)
        {
            interactableInputPublisher.Publish(new InteractableInputMessage());
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