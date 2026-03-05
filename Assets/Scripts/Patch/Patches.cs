using System.Collections.Generic;
using UnityEngine;

namespace Patch
{
    [CreateAssetMenu(fileName = "Patches", menuName = "configs/Patch/Patches")]
    public class Patches : ScriptableObject
    {
        [field: SerializeField] public  List<PatchInfo> PatchInfos { get; private set; } = new();
    }
}