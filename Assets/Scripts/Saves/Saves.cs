using System.Collections.Generic;
using BuildingsAndGrid.Buildings;
using Inventory.Item;
using UnityEngine;
using UnityEngine.Serialization;

namespace YG
{
    public partial class SavesYG
    {
        public int money = 4500;
        public string[,] Tiles;
        public Vector2Int Offset;
        public List<BuildingSave> BuildingSaves = new();
        public List<ShelfSave> ShelvesSave = new();
        public List<BuildingInStorageSave> BuildingInStorageSave = new();
        public List<ItemInStorageSave> ItemInStorageSave = new();
        
        public bool StorageReadyMetricSend;
        public bool GameReadyMetricSend;
    }
}