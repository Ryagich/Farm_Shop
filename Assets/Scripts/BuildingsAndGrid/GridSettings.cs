using System.Collections.Generic;
using BuildingsAndGrid.Extension;
using UnityEngine;

namespace BuildingsAndGrid
{
    [CreateAssetMenu(fileName = "GridSettings", menuName = "configs/Buildings/GridSettings")]
    public class GridSettings : ScriptableObject
    {
        [field: SerializeField] public List<GridInfo> Info { get; private set; }
        [field: SerializeField] public Vector3 TileSize { get; private set; }
        [field: SerializeField] public float yOffset { get; private set; } = .01f;
        [field: SerializeField] public Vector2Int EnvironmentAddSizeSize { get; private set; }
        [field: SerializeField] public float ShopPlankSize { get; private set; } = .5f;
        [field: SerializeField] public ExtensionPointer ExpansionPref { get; private set; }
        [field: SerializeField] public LayerMask ExtensionLayer { get; private set; }
        [field: SerializeField] public GameObject HighlightTile { get; private set; }
        [field: SerializeField] public Material GhostMaterial { get; private set; }
        [field: SerializeField] public Material GhostGreenMaterial { get; private set; }
        [field: SerializeField] public Material GhostRedMaterial { get; private set; }
        [field: SerializeField] public Material ShopFloorMaterial { get; private set; }
        [field: SerializeField] public Material ProductionFloorMaterial { get; private set; }
        [field: SerializeField] public Material GardenFloorMaterial { get; private set; }
        [field: SerializeField] public Material WallFloorMaterial { get; private set; }
        [field: SerializeField] public Material GreenMaterial { get; private set; }
        [field: SerializeField] public LayerMask WallLayer { get; private set; }
        [field: SerializeField] public LayerMask WallForPlayerLayer { get; private set; }

        [field: SerializeField] public float PriceForNewTile { get; private set; }
        [field: SerializeField] public float PriceForExistingTile { get; private set; }
    }
}