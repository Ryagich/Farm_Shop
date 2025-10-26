using System;
using System.Collections.Generic;
using Inventory.Item;
using UnityEngine;

namespace Products
{
    [CreateAssetMenu(fileName = "ProductConfig", menuName = "configs/Product/ProductConfig")]
    public class ProductConfig : ScriptableObject
    {
        [field: SerializeField, Min(.0f)] public float Time { get; private set; } = 1.0f;
        [field: SerializeField] public List<ProductInfo> Materials { get; private set; } = new();
        [field: SerializeField, Min(1)] public int MaxCount { get; private set; } = 1;
        [field: SerializeField] public ItemConfig ItemConfig { get; private set; } = null!;
        [field: SerializeField] public GameObject MaterialHolder { get; private set; } = null!;
        [field: SerializeField, Min(.0f)] public float SpaceBetweenItemsY { get; private set; } = 1.0f;
        [field: SerializeField, Min(.0f)] public float SpaceBetweenItemHolders { get; private set; } = 1.0f;
        //Creator Animator
        [field: SerializeField] public CreatorAnimationConfig CreatorAnimationConfig { get; private set; }
    }
    
    [Serializable]
    public class ProductInfo
    {
        [field: SerializeField] public ItemConfig ItemConfig { get; private set; } = null!;
        [field: SerializeField, Min(1)] public int CountForProduct { get; private set; } = 1;
        [field: SerializeField, Min(1)] public int MaxCount { get; private set; } = 1;
        [field: SerializeField, Min(.0f)] public float SpaceBetweenItemsY { get; private set; } = 1.0f;
    }
}