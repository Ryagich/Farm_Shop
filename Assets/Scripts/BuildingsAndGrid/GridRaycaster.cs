using Input;
using UnityEngine;

namespace BuildingsAndGrid
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class GridRaycaster
    {
        private readonly InputConfig inputConfig;
        private readonly GridSettings gridSettings;
        private readonly Camera camera;
        private readonly Plane plane = new (Vector3.up, Vector3.zero);
        
        public GridRaycaster(InputConfig inputConfig, GridSettings gridSettings, Camera camera)
        {
            this.inputConfig = inputConfig;
            this.gridSettings = gridSettings;
            this.camera = camera;
        }

        public Vector2Int GetRaycastPositionOnGrid()
        {
            var ray = camera.ScreenPointToRay(inputConfig.PointerPosition.action.ReadValue<Vector2>());
            plane.Raycast(ray, out var enter);
            var hitPoint = ray.GetPoint(enter);
            var point = new Vector2(hitPoint.x / gridSettings.TileSize.x, hitPoint.z / gridSettings.TileSize.z);
            // Debug.Log($"{(int)point.x} {(int)point.y}");
            return new Vector2Int((int)point.x, (int)point.y);
        }
    }
}