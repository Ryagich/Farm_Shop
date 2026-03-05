using GameModes;
using Input;
using Input.Cursor;
using MessagePipe;
using Messages;
using UI.Hover.PopupLogics.Popups;
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
        private IObjectPopup currentPopup;

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
            var pos = inputConfig.PointerPosition.action.ReadValue<Vector2>();
            var hover = hoverRaycaster.GetHoveredObject(pos);
            if (hover)
            {
                hover.ObjectPopup.ClickOnObject();
            }
            if (!HavePopup)
            {
                return;
            }
            if (IsClickInsidePopup(currentPopup, pos))
            {
                return;
            }
            if (IsFixed)
            {
                IsFixed = false;
            }
        }
        
        private bool IsClickInsidePopup(IObjectPopup popup, Vector2 screenPos)
        {
            if (popup.Root &&
                RectTransformUtility.RectangleContainsScreenPoint(popup.Root, screenPos, null))
            {
                return true;
            }
            foreach (var child in popup.Children)
            {
                if (RectTransformUtility.RectangleContainsScreenPoint(child, screenPos, null))
                {
                    return true;
                }
            }

            return false;
        }

        private void OnRightClick(RightClickMessage msg)
        {
            if (currentHover && (gameModesController.GameMode is GameMode.Game
                              || gameModesController.GameMode is GameMode.Inventory))
            {
                IsFixed = !IsFixed;
            }
        }

        public void Tick()
        {
            if (IsFixed)
            {
                if (currentPopup == null || !currentPopup.Root)
                {
                    IsFixed = false;
                    currentPopup = null;
                    return;
                }
                UpdatePosition();
                return;
            }

            position = inputConfig.PointerPosition.action.ReadValue<Vector2>();
            var hoverTrigger = hoverRaycaster.GetHoveredObject(position);

            if (!currentHover && !hoverTrigger || (gameModesController.GameMode != GameMode.Game
                                                && gameModesController.GameMode != GameMode.Inventory
                                                && gameModesController.GameMode != GameMode.Dialogue))
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
            else if (currentHover.gameObject == hoverTrigger.gameObject && currentPopup != null)
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
            {
                currentHover.Disposabled += Subscribe;
            }
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
            var popupSize = currentPopup.Root.rect.size;
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
            currentPopup.Root.anchoredPosition = localPoint;
        }
        
        private void DrawPopup()
        {
            if (currentPopup != null)
            {
                Object.Destroy(currentPopup.Root.gameObject);
            }
            currentHover.ObjectPopup.CloseButton += OnClosePopup;
            if (!cursorHandler.IsVisible)
            {
                return;
            }
            currentPopup = currentHover.ObjectPopup.DrawPopup(canvas);
            
            UpdatePosition();
        }
        
        private void ClosePopup()
        {
            if (currentPopup != null && currentPopup.Root)
            {
                Object.Destroy(currentPopup.Root.gameObject);
            }
            currentPopup = null;
        }
    }
}