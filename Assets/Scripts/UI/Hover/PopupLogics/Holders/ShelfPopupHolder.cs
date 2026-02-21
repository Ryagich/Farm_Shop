using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Hover.PopupLogics.Holders
{
    public class ShelfPopupHolder : MonoBehaviour
    {
        [field: SerializeField] public TMP_Text Name { get; private set; }
        [field: SerializeField] public TMP_Text BuyersCount { get; private set; }
        [field: SerializeField] public Image BuyersFillImage { get; private set; }
        [field: SerializeField] public Button ButtonMove { get; private set; } = null!;
        [field: SerializeField] public Button ButtonMoveToInventory { get; private set; } = null!;
        [field: SerializeField] public Button ButtonDisable { get; private set; } = null!;
        [field: SerializeField] public RectTransform CardParent { get; private set; } = null!;
    }
}