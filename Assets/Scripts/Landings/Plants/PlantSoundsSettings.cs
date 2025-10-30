using Sounds;
using UnityEngine;

namespace Landings.Plants.PlantConfigs
{
    [CreateAssetMenu(fileName = "PlantSoundsSettings", menuName = "configs/Plants/PlantSoundsSettings")]
    public class PlantSoundsSettings : ScriptableObject
    {
        [field: SerializeField] public SoundsSettings GrownUpSoundsSettings { get; private set; }
        [field: SerializeField] public SoundsSettings GrownStageSoundsSettings { get; private set; }
    }
}
