using System;
using BuildingsAndGrid.Buildings;
using GameModes;
using UnityEngine;

namespace BuildingsAndGrid
{
    [Serializable]
    public class Tile
    {
        [field: SerializeField] public Area Type { get; private set; }
        [field: SerializeField] public Vector2Int Index { get; private set; }
        public Building Building { get; private set; } = null;
        public bool IsFree => Building == null;
       
        public Tile(Vector2Int index, Area type, Building building = null)
        {
            Index = index;
            Type = type;
            Building = building;
        }
        
        public void SetBuilding(Building newBuilding)
        {
            Building = newBuilding;
        }

        public void SetType(Area type)
        {
            Type = type;
        }
    }
}