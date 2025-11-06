using MessagePipe;
using Messages;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer.Unity;

namespace GameModes
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class GameModesController : IStartable
    {
        public GameModes GameMode { get; private set; } = GameModes.Game;
      
        private readonly IPublisher<GameModeChangedMessage> gameModeChangedPublisher;
        private readonly IPublisher<ChangeCursorStateMessage> changeCursorStatePublisher;

        public GameModesController
            (
                IPublisher<GameModeChangedMessage> gameModeChangedPublisher,
                IPublisher<ChangeCursorStateMessage> changeCursorStatePublisher,
                ISubscriber<OpenGameModeMessage> openGameModeSubscriber,
                ISubscriber<OpenRedactorModeMessage> openRedactorModeSubscriber,
                ISubscriber<OpenShopModeMessage> openShopModeSubscriber
            )
        {
            this.gameModeChangedPublisher = gameModeChangedPublisher;
            this.changeCursorStatePublisher = changeCursorStatePublisher;

            openGameModeSubscriber.Subscribe(OpenGameMode);
            openRedactorModeSubscriber.Subscribe(OpenRedactorMode);
            openShopModeSubscriber.Subscribe(OpenShopMode);
        }

        public void Start()
        {
            gameModeChangedPublisher.Publish(new GameModeChangedMessage(GameModes.Game));
        }

        private void EnterMainGameMode()
        {
            if (GameMode is GameModes.Game)
            {
                changeCursorStatePublisher.Publish(new ChangeCursorStateMessage());
                return;
            }
            GameMode = GameModes.Game;
            gameModeChangedPublisher.Publish(new GameModeChangedMessage(GameMode));
        }

        private void OpenGameMode(OpenGameModeMessage msg)
        {
            EnterMainGameMode();    
        }
        
        private void OpenRedactorMode(OpenRedactorModeMessage msg)
        {
            if (GameMode is GameModes.Redactor)
            {
                EnterMainGameMode();
                return;
            }
            GameMode = GameModes.Redactor;
            gameModeChangedPublisher.Publish(new GameModeChangedMessage(GameMode));
        }

        private void OpenShopMode(OpenShopModeMessage msg)
        {
            if (GameMode is GameModes.Shop)
            {
                EnterMainGameMode();
                return;
            }
            GameMode = GameModes.Shop;
            gameModeChangedPublisher.Publish(new GameModeChangedMessage(GameMode));
        }
    }

    public enum GameModes
    {
        Game,
        Shop,
        Redactor
    }
}