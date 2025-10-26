using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Hover.PopupLogics.Holders
{
    public class ShelfPopupHolder : MonoBehaviour
    {
        [field: SerializeField] public TMP_Text Name { get; private set; }
        [field: SerializeField] public TMP_Text ProductDescription { get; private set; }
        [field: SerializeField] public TMP_Text ProductsCount { get; private set; }
        [field: SerializeField] public TMP_Text BuyersCount { get; private set; }
        [field: SerializeField] public Image ProductsFillImage { get; private set; }
        [field: SerializeField] public Image BuyersFillImage { get; private set; }
    }
}