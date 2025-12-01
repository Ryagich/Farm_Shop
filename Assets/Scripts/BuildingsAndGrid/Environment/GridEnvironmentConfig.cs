using BuildingsAndGrid.Buildings;
using UnityEngine;

namespace BuildingsAndGrid.Environment
{
    [CreateAssetMenu(fileName = "GridEnvironmentConfig", menuName = "configs/Buildings/GridEnvironmentConfig")]
    public class GridEnvironmentConfig : ScriptableObject
    {
        [field: SerializeField] public BuildingConfig ShopDoorConfig { get; private set; }
        [field: SerializeField] public BuildingConfig BackDoor { get; private set; }
        [field: SerializeField] public BuildingConfig Checkout { get; private set; }
    }
}