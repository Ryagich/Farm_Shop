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

        private bool IsFixed;
        private Vector2 position;
        private HoverTrigger hoverTrigger;
        
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
            // Получаем экранную позицию курсора
            var pos = inputConfig.PointerPosition.action.ReadValue<Vector2>();

            // Для canvas в Screen Space Overlay — камера не нужна
            var isPopup = RectTransformUtility.RectangleContainsScreenPoint(currentPopup, pos, null);
            if (isPopup)
                return;
            if (IsFixed)
                IsFixed = false;
        }
        
        private void OnRightClick(RightClickMessage msg)
        {
            if (hoverTrigger && (gameModesController.GameMode is GameMode.Game
                                  || gameModesController.GameMode is GameMode.Inventory))
                IsFixed = !IsFixed;
        }
        
        private void UpdatePosition()
        {
            if (!IsFixed)
            {     
                position = inputConfig.PointerPosition.action.ReadValue<Vector2>();
                hoverTrigger = hoverRaycaster.GetHoveredObject(position);
            }
        }
        
        public void Tick()
        {
            UpdatePosition();
            if (IsFixed)
                return;
            if (!currentHover && !hoverTrigger || (gameModesController.GameMode != GameMode.Game 
                                                && gameModesController.GameMode != GameMode.Inventory))
            {
                ClosePopup();
            }
            else if (!currentHover && hoverTrigger)
            {
                currentHover = hoverTrigger;
                DrawPopup();
            }
            else if (currentHover && !hoverTrigger)
            {
                ClosePopup();
                currentHover = null;
            }
            else if (currentHover.gameObject == hoverTrigger.gameObject)
            {
                DrawPopup();
            }
            else if (currentHover.gameObject != hoverTrigger.gameObject)
            {
                currentHover = hoverTrigger;
                DrawPopup();
            }
        }

        private void OnClosePopup()
        {
            IsFixed = false;
            currentHover.ObjectPopup.CloseButton -= OnClosePopup;
        }
        
        private void DrawPopup()
        {
            if (currentPopup)
            {
                Object.Destroy(currentPopup.gameObject);
            }
            Debug.Log($"currentHover {currentHover != null}");
            Debug.Log($"currentHover.ObjectPopup { currentHover.ObjectPopup != null}");
            currentHover.ObjectPopup.CloseButton += OnClosePopup;

            if (!cursorHandler.IsVisible)
            {
                return;
            }
            var popupRect = currentHover.ObjectPopup.DrawPopup();
            var popupSize = popupRect.rect.size;
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
            
            popupRect.anchoredPosition = localPoint;
            currentPopup = popupRect;
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