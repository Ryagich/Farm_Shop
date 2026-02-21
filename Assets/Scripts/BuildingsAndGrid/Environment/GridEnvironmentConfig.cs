using BuildingsAndGrid.Buildings;
using Inventory.Item;
using Landings.Plants.PlantConfigs;
using UnityEngine;

namespace BuildingsAndGrid.Environment
{
    [CreateAssetMenu(fileName = "GridEnvironmentConfig", menuName = "configs/Buildings/GridEnvironmentConfig")]
    // ReSharper disable once ClassNeverInstantiated.Global
    public class GridEnvironmentConfig : ScriptableObject
    {
        [field: SerializeField] public BuildingConfig ShopDoorConfig { get; private set; }
        [field: SerializeField] public BuildingConfig BackDoor { get; private set; }
        [field: SerializeField] public BuildingConfig Checkout { get; private set; }
        [field: SerializeField] public BuildingConfig Landing { get; private set; }
        [field: SerializeField] public BuildingConfig Shelf { get; private set; }
        [field: SerializeField] public ItemConfig DefaultItemConfig { get; private set; }
        [field: SerializeField] public PlantConfig DefaultPlantConfig { get; private set; }
        [field: SerializeField] public BuildingConfig Deleter { get; private set; }
    }
}