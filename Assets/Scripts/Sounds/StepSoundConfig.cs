using UnityEngine;

namespace Sounds
{
    [CreateAssetMenu(fileName = "StepSound Config", menuName = "configs/Sounds/StepSound")]
    public class StepSoundConfig : ScriptableObject
    {
        [field: SerializeField] public float StepDistance { get; private set; } = 1.2f;
        [field: SerializeField] public SoundConfig StepOnGroundSoundSettings { get; private set; }
        [field: SerializeField] public SoundConfig StepOnWoodSoundSettings { get; private set; }
        [field: SerializeField] public SoundConfig StepOnStoneSoundSettings { get; private set; }
    }
}