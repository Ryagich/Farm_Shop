using System.Collections.Generic;
using BuildingsAndGrid.Buildings;
using Landings.Plants;
using UnityEngine;

namespace YG
{
    public partial class SavesYG
    {
        public int money = 4500;
        public Vector2Int Offset;
        public List<BuildingSave> BuildingSaves = new();
        public List<ShelfSave> ShelvesSave = new();
        public List<BuildingInStorageSave> BuildingsInStorageSave = new();
        public List<PlantInStorageSave> PlantsInStorageSave = new();
        public List<PlantSave> PlantsSave = new();
        public string[,] Tiles;

        public bool StorageReadyMetricSend;
        public bool GameReadyMetricSend;
    }
}