using TMPro;
using UnityEngine;

namespace UI.Hover.PopupLogics.Holders
{
    public class ProductionZoneHolder : MonoBehaviour
    {
        [field: SerializeField] public TMP_Text ProductionProductName { get; private set; } = null!;
        [field: SerializeField] public TMP_Text ReadyToTake { get; private set; } = null!;
        [field: SerializeField] public TMP_Text ProductionTime { get; private set; } = null!;
        [field: SerializeField] public TMP_Text MaterialsHeader { get; private set; } = null!;
    }
}