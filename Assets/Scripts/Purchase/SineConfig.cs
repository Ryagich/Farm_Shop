using UnityEngine;

namespace Purchase
{
    [CreateAssetMenu(fileName = "SineConfig", menuName = "configs/Inventory/Sine")]
    public class SineConfig : ScriptableObject
    {
        [field: SerializeField] public float Speed { get; private set; } = 2.0f;
        [field: SerializeField] public float Min { get; private set; } = 1.5f;
        [field: SerializeField] public float Max { get; private set; } = 2;
        [field: SerializeField] public Vector3 Movement { get; private set; } = new Vector3(.0f, 0.01f, .0f);
        [field: SerializeField] public Vector3 Scale { get; private set; } = new Vector3(1.0f, 1.0f, .0f);
        [field: SerializeField] public bool RandomizeTime { get; private set; } = true;
    }
}