using UnityEngine;

namespace BuildingsAndGrid.Buildings
{
    [CreateAssetMenu(fileName = "HighlightConfig", menuName = "configs/Buildings/HighlightConfig")]
    public class HighlightConfig : ScriptableObject  
    {
        [field: SerializeField] public Material NormalMaterial { get; private set; }
        [field: SerializeField] public Material RedMaterial { get; private set; }
    }
}