using System;
using GameModes;
using UnityEngine;

namespace BuildingsAndGrid
{
    [Serializable]
    public class GridInfo
    {
        [field: SerializeField] public Vector2Int Position { get; private set; }
        [field: SerializeField] public Vector2Int Size { get; private set; }
        [field: SerializeField] public Area Type { get; private set; }
        
        public GridInfo(Vector2Int position,Vector2Int size, Area type)
        {
            Position = position;
            Size = size;
            Type = type;
        }
    }
}