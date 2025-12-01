using GameModes;
using UnityEngine;
using VContainer.Unity;

namespace BuildingsAndGrid.Buildings
{
    [CreateAssetMenu(fileName = "BuildingConfig", menuName = "configs/Storage/Building")]
    public class BuildingConfig : ScriptableObject
    {
        [field: SerializeField] public Area Type { get; private set; }
        [field: SerializeField] public Sprite Icon { get; private set; }
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField] public LifetimeScope Building { get; private set; }
        [field: SerializeField] public HighlightBuilding HighlightBuilding { get; private set; }
        [field: SerializeField] public Vector2Int Size { get; private set; }
        [field: SerializeField] public int Price { get; private set; }
        [field: SerializeField] public bool ShowInShop { get; private set; } = true;
    }
}