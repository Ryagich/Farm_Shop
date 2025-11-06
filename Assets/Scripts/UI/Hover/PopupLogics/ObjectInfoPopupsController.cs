using GameModes;
using Input;
using Input.Cursor;
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

        public ObjectInfoPopupsController
            (
                InputConfig inputConfig,
                HoverRaycaster hoverRaycaster,
                CursorHandler cursorHandler,
                Canvas canvas,
                GameModesController gameModesController
            )
        {
            this.inputConfig = inputConfig;
            this.hoverRaycaster = hoverRaycaster;
            this.cursorHandler = cursorHandler;
            this.canvas = canvas;
            this.gameModesController = gameModesController;
        }

        public void Tick()
        {
            var hover = hoverRaycaster.GetHoveredObject();
            
            if (!currentHover && !hover || gameModesController.GameMode is not GameModes.GameModes.Game)
            {
            }
            else if (!currentHover && hover)
            {
                currentHover = hover;
                DrawPopup();
            }
            else if (currentHover && !hover)
            {
                ClosePopup();
                currentHover = null;
            }
            else if (currentHover.gameObject == hover.gameObject)
            {
                DrawPopup();
            }
            else if (currentHover.gameObject != hover.gameObject)
            {
                currentHover = hover;
                DrawPopup();
            }
        }
        
        private void DrawPopup()
        {
            if (currentPopup)
            {
                Object.Destroy(currentPopup.gameObject);
            }

            if (!cursorHandler.IsVisible)
            {
                return;
            }
            var popupRect = currentHover.ObjectPopup.DrawPopup();
            var popupSize = popupRect.rect.size;
            var screenPos = inputConfig.PointerPosition.action.ReadValue<Vector2>();
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