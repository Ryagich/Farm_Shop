using UnityEngine;

namespace Sounds
{
    [CreateAssetMenu(fileName = "Sounds Config", menuName = "configs/Sounds/Sounds Config")]
    public class SoundsConfig : ScriptableObject
    {
        [field: SerializeField] public AudioSource AudioSourcePrefab { get; private set; }
        [field: SerializeField] public SoundConfig StepOnGroundSoundSettings { get; private set; }
        [field: SerializeField] public SoundConfig StepOnWoodSoundSettings { get; private set; }
        [field: SerializeField] public SoundConfig StepOnStoneSoundSettings { get; private set; }
    }
}