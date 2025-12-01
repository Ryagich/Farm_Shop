using System.Collections.Generic;
using UnityEngine;

namespace BuildingsAndGrid.Buildings
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class Building : MonoBehaviour
    {
        public BuildingConfig BuildingConfig;
        public Transform Content { get; private set; }
        public List<Tile> Tiles { get; private set; } = new();
        
        // ReSharper disable once ParameterHidesMember
        public void SetContent(Transform content)
        {
            Content = content;
        }
        
        public void SetTiles(List<Tile> tiles)
        {
            Tiles = tiles;
        }
        
        public void SetContentRotation(Quaternion rotation) => Content.rotation = rotation;

        private void OnDestroy()
        {
            Tiles?.ForEach(tile => tile.SetBuilding(null));
        }
    }
}