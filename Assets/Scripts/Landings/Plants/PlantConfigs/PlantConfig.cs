using System.Collections.Generic;
using UnityEngine;

namespace Landings.Plants.PlantConfigs
{
    [CreateAssetMenu(fileName = "PlantConfig", menuName = "configs/Plants/PlantConfig")]
    public class PlantConfig : ScriptableObject
    {
        [field: SerializeField] public List<GameObject> Stages { get; private set; } = new();
        [field: SerializeField] public Vector2 GrowTime { get; private set; } = new(1.0f, 2.0f);
        [field: SerializeField] public Vector2 TimeBetweenStages { get; private set; } = new(1.5f, 2.25f);
        [field: SerializeField] public Vector3 StartPosition { get; private set; }
        [field: SerializeField] public Vector3 TargetPosition { get; private set; }
    }
}