using UnityEngine;

namespace UI.Hover
{
    [CreateAssetMenu(fileName = "HoverSettings", menuName = "configs/UI/HoverSettings")]
    public class HoverSettings : ScriptableObject
    {
        [field: SerializeField] public LayerMask HoverLayer { get; private set; }
        [field: SerializeField] public float MaxDistance { get; private set; } = 30;
    }
}