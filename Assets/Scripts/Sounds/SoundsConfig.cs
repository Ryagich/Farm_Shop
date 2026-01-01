using UnityEngine;

namespace Sounds
{
    [CreateAssetMenu(fileName = "Sounds Config", menuName = "configs/Sounds/Sounds Config")]
    public class SoundsConfig : ScriptableObject
    {
        [field: SerializeField] public AudioSource AudioSourcePrefab { get; private set; }
        [field: SerializeField] public SoundConfig ScannerSoundSettings { get; private set; }
        [field: SerializeField] public SoundConfig SwipeSoundSettings_1 { get; private set; }
    }
}