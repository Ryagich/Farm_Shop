using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Hover.PopupLogics.Holders
{
    public class PurchaseObjectHolder : MonoBehaviour
    {
        [field: SerializeField] public TMP_Text PurchaseObjectName { get; private set; }
        [field: SerializeField] public TMP_Text Purchase { get; private set; }
        [field: SerializeField] public Image Fill { get; private set; } = null!;
    }
}