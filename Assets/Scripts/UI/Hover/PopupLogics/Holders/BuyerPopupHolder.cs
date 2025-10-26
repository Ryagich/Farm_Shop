using TMPro;
using UnityEngine;

namespace UI.Hover.PopupLogics.Holders
{
    public class BuyerPopupHolder : MonoBehaviour
    {
        [field: SerializeField] public TMP_Text BuyerStatus { get; private set; } = null!;
        [field: SerializeField] public TMP_Text ProductsListTitle { get; private set; } = null!;
    }
}