using System.Linq;
using GameModes;
using Interactable;
using MessagePipe;
using Messages;
using TMPro;
using UI.Configs;
using UnityEngine;
using VContainer.Unity;
using UniRx;

namespace UI
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class InteractableInterface : IStartable
    {
        private readonly UIConfig uiConfig;
        private readonly Canvas canvas;
        private readonly PlayerInteractableLogic playerInteractableLogic;

        private bool canShow;
        private TMP_Text interactableText;
        
        public InteractableInterface
            (
                UIConfig uiConfig,
                Canvas canvas,
                PlayerInteractableLogic playerInteractableLogic,
                ISubscriber<GameModeChangedMessage> gameModeChangedMessageSubscriber
            )
        {
            this.uiConfig = uiConfig;
            this.canvas = canvas;
            this.playerInteractableLogic = playerInteractableLogic;

            playerInteractableLogic.Interactables
                                   .ObserveCountChanged()
                                   .Subscribe(_ => Update());
            gameModeChangedMessageSubscriber.Subscribe(OnGameModeChanged);
        }
        
        private void OnGameModeChanged(GameModeChangedMessage msg)
        {
            canShow = msg.GameMode is GameMode.Game;
            Update();
        }

        private void Update()
        {
            if (canShow && playerInteractableLogic.Interactables.Any(i => i.InteractionMode is InteractionMode.Manual))
            {
                if (!interactableText)
                {
                    interactableText = Object.Instantiate(uiConfig.InteractableText, canvas.GetComponent<RectTransform>());
                }
            }
            else if (interactableText)
            {
                Object.Destroy(interactableText.gameObject);
            }
        }
        
        public void Start() { }
    }
}