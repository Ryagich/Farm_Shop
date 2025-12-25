using GameModes;
using Input;
using Input.Cursor;
using MessagePipe;
using Messages;
using UnityEngine;
using Utils;
using VContainer.Unity;
using Screen = UnityEngine.Device.Screen;

namespace UI.Hover.PopupLogics
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class ObjectInfoPopupsController : ITickable
    {
        private readonly InputConfig inputConfig;
        private readonly HoverRaycaster hoverRaycaster;
        private readonly CursorHandler cursorHandler;
        private readonly Canvas canvas;
        private readonly GameModesController gameModesController;

        private HoverTrigger currentHover;
        private RectTransform currentPopup;

        public bool HavePopup => currentPopup != null;
        public bool IsFixed;
        private Vector2 position;

        public ObjectInfoPopupsController
            (
                InputConfig inputConfig,
                HoverRaycaster hoverRaycaster,
                CursorHandler cursorHandler,
                Canvas canvas,
                GameModesController gameModesController,
                ISubscriber<ClickMessage> clickSubscriber,
                ISubscriber<RightClickMessage> rightClickSubscriber,
                ISubscriber<GameModeChangedMessage> gameModeChangedSubscriber
            )
        {
            this.inputConfig = inputConfig;
            this.hoverRaycaster = hoverRaycaster;
            this.cursorHandler = cursorHandler;
            this.canvas = canvas;
            this.gameModesController = gameModesController;

            clickSubscriber.Subscribe(OnClick);
            rightClickSubscriber.Subscribe(OnRightClick);
            gameModeChangedSubscriber.Subscribe(OnGameModeChanged);
        }

        private void OnGameModeChanged(GameModeChangedMessage msg)
        {
            IsFixed = false;
        }

        private void OnClick(ClickMessage msg)
        {
            if (!currentPopup)
                return;
            var pos = inputConfig.PointerPosition.action.ReadValue<Vector2>();
            var isPopup = RectTransformUtility.RectangleContainsScreenPoint(currentPopup, pos, null);
            if (isPopup)
                return;
            if (IsFixed)
                IsFixed = false;
        }

        private void OnRightClick(RightClickMessage msg)
        {
            if (currentHover && (gameModesController.GameMode is GameMode.Game
                              || gameModesController.GameMode is GameMode.Inventory))
                IsFixed = !IsFixed;
        }

        public void Tick()
        {
            if (IsFixed)
            {
                if (currentPopup)
                    UpdatePosition();
                return;
            }

            position = inputConfig.PointerPosition.action.ReadValue<Vector2>();
            var hoverTrigger = hoverRaycaster.GetHoveredObject(position);

            if (!currentHover && !hoverTrigger || (gameModesController.GameMode != GameMode.Game
                                                && gameModesController.GameMode != GameMode.Inventory))
            {
                ClosePopup();
            }
            else if (!currentHover && hoverTrigger)
            {
                SetHover(hoverTrigger);
                DrawPopup();
            }
            else if (currentHover && !hoverTrigger)
            {
                SetHover(null);
                ClosePopup();
            }
            else if (currentHover.gameObject == hoverTrigger.gameObject && currentPopup)
            {
                UpdatePosition();
            }
            else if (currentHover.gameObject != hoverTrigger.gameObject)
            {
                SetHover(hoverTrigger);
                DrawPopup();
            }
        }

        private void SetHover(HoverTrigger hoverTrigger)
        {
            if (currentHover)
            {
                currentHover.Disposabled -= Subscribe;
            }
            currentHover = hoverTrigger;
            if (currentHover)
                currentHover.Disposabled += Subscribe;
        }

        private void Subscribe()
        {
            IsFixed = false;
            currentHover = null;
            ClosePopup();
        }

        private void OnClosePopup()
        {
            IsFixed = false;
            currentHover.ObjectPopup.CloseButton -= OnClosePopup;
        }

        private void UpdatePosition()
        {
            var popupSize = currentPopup.rect.size;
            var screenPos = position;
            if (screenPos.x + popupSize.x > Screen.width)
            {
                screenPos = screenPos.WithX(screenPos.x - popupSize.x);
            }
            if (screenPos.y + popupSize.y > Screen.height)
            {
                screenPos = screenPos.WithY(screenPos.y - popupSize.y);
            }
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                                                                    canvas.transform as RectTransform,
                                                                    screenPos,
                                                                    null,
                                                                    out var localPoint
                                                                   );
            currentPopup.anchoredPosition = localPoint;
        }
        
        private void DrawPopup()
        {
            if (currentPopup)
            {
                Object.Destroy(currentPopup.gameObject);
            }
            currentHover.ObjectPopup.CloseButton += OnClosePopup;
            if (!cursorHandler.IsVisible)
            {
                return;
            }
            currentPopup = currentHover.ObjectPopup.DrawPopup();
            
            UpdatePosition();
        }
        
        private void ClosePopup()
        {
            if (currentPopup)
            {
                Object.Destroy(currentPopup.gameObject);
            }
            currentPopup = null;
        }
    }
}