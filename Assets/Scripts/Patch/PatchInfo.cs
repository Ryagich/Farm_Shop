using UnityEngine;

namespace Patch
{
    [CreateAssetMenu(fileName = "PatchInfo", menuName = "configs/Patch/Patch Info")]
    public class PatchInfo : ScriptableObject
    {
        [field: SerializeField] public RectTransform PatchScroll { get; private set; }
    }
}