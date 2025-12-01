using UnityEngine;

namespace UI.Hover
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class HoverRaycaster
    {
        private readonly HoverSettings hoverSettings;
        private readonly Camera camera;

        public HoverRaycaster
            (
                HoverSettings hoverSettings,
                Camera camera
            )
        {
            this.hoverSettings = hoverSettings;
            this.camera = camera;
        }
        
        public HoverTrigger GetHoveredObject(Vector3 pointerPosition)
        {
            var ray = camera.ScreenPointToRay(pointerPosition);

            if (Physics.Raycast(ray, out var hit, hoverSettings.MaxDistance, hoverSettings.HoverLayer))
            {
                return hit.collider.GetComponentInParent<HoverTrigger>();
            }

            return null;
        }
    }
}