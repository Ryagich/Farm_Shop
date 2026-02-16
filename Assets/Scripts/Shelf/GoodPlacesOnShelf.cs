using System;
using System.Collections.Generic;
using Inventory.Item;
using UnityEngine;

namespace Shelf
{
    [Serializable]
    public class GoodPlacesOnShelf
    {
        [field: SerializeField] public List<Transform> Places { get; private set; } = new();
    }
}