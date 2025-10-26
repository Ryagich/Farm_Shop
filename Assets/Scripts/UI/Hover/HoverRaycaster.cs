using Input;
using UnityEngine;

namespace UI.Hover
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class HoverRaycaster
    {
        private readonly HoverSettings hoverSettings;
        private readonly InputConfig inputConfig;
        private readonly Camera camera;

        public HoverRaycaster
            (
                HoverSettings hoverSettings,
                InputConfig inputConfig,
                Camera camera
            )
        {
            this.hoverSettings = hoverSettings;
            this.inputConfig = inputConfig;
            this.camera = camera;
        }
        
        public HoverTrigger GetHoveredObject()
        {
            var pointerPosition = inputConfig.PointerPosition.action.ReadValue<Vector2>();
            var ray = camera.ScreenPointToRay(pointerPosition);

            if (Physics.Raycast(ray, out var hit, hoverSettings.MaxDistance, hoverSettings.HoverLayer))
            {
                return hit.collider.GetComponentInParent<HoverTrigger>();
            }

            return null;
        }
    }
}