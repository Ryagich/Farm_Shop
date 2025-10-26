using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Hover.PopupLogics.Holders
{
    public class BuyerProductInfo : MonoBehaviour
    {
        [field: SerializeField] public TMP_Text ProductName { get; private set; } = null!;
        [field: SerializeField] public TMP_Text ProductCounts { get; private set; } = null!;
        [field: SerializeField] public Image FillBack { get; private set; } = null!;
        [field: SerializeField] public Image Fill { get; private set; } = null!;
    }
}