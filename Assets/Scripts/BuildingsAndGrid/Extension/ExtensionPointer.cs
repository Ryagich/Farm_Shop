using System.Collections.Generic;
using UnityEngine;

namespace BuildingsAndGrid.Extension
{
    public class ExtensionPointer : MonoBehaviour
    {
        [field: SerializeField] public Vector2Int Direction { get; private set; }
        [field: SerializeField] public List<Tile> Tiles { get; private set; } = new();

        public int Price;
        
        public void SetValues
            (
                GridSettings gridSettings,
                Vector2Int direction,
                List<Tile> tiles,
                int tilesCount
            )
        {
            Tiles = tiles;
            Direction = direction;
            
            Price = (int)(Tiles.Count * gridSettings.PriceForNewTile + tilesCount * gridSettings.PriceForExistingTile);
        }
    }
}