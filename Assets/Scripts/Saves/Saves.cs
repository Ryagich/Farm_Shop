using System.Collections.Generic;
using BuildingsAndGrid.Buildings;
using UnityEngine;

namespace YG
{
    public partial class SavesYG
    {
        public int money = 45;
        public string[,] Tiles;
        public Vector2Int Offset;
        public List<BuildingSave> BuildingSaves = new();
        public List<BuildingInStorageSave> BuildingInStorageSave = new();
    }
}