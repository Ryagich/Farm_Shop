using TMPro;
using UnityEngine;

namespace UI.Hover.PopupLogics.Holders
{
    [CreateAssetMenu(fileName = "Popups list", menuName = "configs/Popups")]
    public class PopupHolders : ScriptableObject
    {
        [field: SerializeField] public ShelfPopupHolder ShelfPopupHolder { get; private set; } = null!;
        [field: SerializeField] public CheckoutPopupHolder CheckoutPopupHolder { get; private set; } = null!;
        [field: SerializeField] public BuyerPopupHolder BuyerPopupHolder { get; private set; } = null!;
        [field: SerializeField] public BuyerProductInfo BuyerProductInfo { get; private set; } = null!;
        [field: SerializeField] public PurchaseObjectHolder PurchaseObjectHolder { get; private set; } = null!;
        [field: SerializeField] public LandingPlantIsItemHolder LandingPlantIsItemHolder { get; private set; } = null!;
        [field: SerializeField] public LandingFruitPlantHolder LandingFruitPlantHolder { get; private set; } = null!;
        [field: SerializeField] public LandingFruitPlantInfoAboutFruits LandingFruitPlantInfoAboutFruits { get; private set; } = null!;
        [field: SerializeField] public ProductionZoneHolder ProductionZoneHolder { get; private set; } = null!;
        [field: SerializeField] public TMP_Text MaterialProductText { get; private set; } = null!;
    }
}