using System.Collections.Generic;
using UnityEngine;

namespace BuildingsAndGrid
{
    public class ExtensionPointer : MonoBehaviour
    {
        [field: SerializeField] public Vector2Int Direction { get; private set; }
        [field: SerializeField] public List<Tile> Tiles { get; private set; } = new();
        
        public void SetValues(Vector2Int direction, List<Tile> tiles)
        {
            Tiles = tiles;
            Direction = direction;
        }
    }
}