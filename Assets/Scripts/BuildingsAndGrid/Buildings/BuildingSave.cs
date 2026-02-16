using System;
using System.Collections.Generic;
using UnityEngine;

namespace BuildingsAndGrid.Buildings
{
    [Serializable]
    public class BuildingSave
    {
        public string Id;
        public Vector2Int Cell;
        
        public float RotX;
        public float RotY;
        public float RotZ;
        
        public BuildingSave(string id, Vector2Int cell, Quaternion rotation)
        {
            Id = id;
            Cell = cell;
            
            var euler = rotation.eulerAngles;
            RotX = euler.x;
            RotY = euler.y;
            RotZ = euler.z;
        }
    }

    [Serializable]
    public class ShelfSave
    {
        public string Id;
        public Vector2Int Cell;
        public List<(string, int)> inventoriesInfo = new();

        public ShelfSave(string id, Vector2Int cell)
        {
            Id = id;
            Cell = cell;
        }
    }

    [Serializable]
    public class BuildingInStorageSave
    {
        public string Id;
        public int Count;
        
        public BuildingInStorageSave(string id, int count)
        {
            Id = id;
            Count = count;
        }
    }
}