using Storage;
using UI.Cards;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Configs
{
    [CreateAssetMenu(fileName = "UI Config", menuName = "configs/UI/UIConfig")]
    public class UIConfig : ScriptableObject
    {
        [field: SerializeField] public RectTransform ContentPref {get; private set;}
        [field: SerializeField] public RectTransform FinanceTextPrefab { get; private set; }
        [field: SerializeField] public RectTransform GameMenuButtonsParent { get; private set; }
        [field: SerializeField] public Button GameMenuButton { get; private set; }
        [field: SerializeField] public Vector2 OffsetForGameMenuButtons { get; private set; } = new (10, 0);
        [field: SerializeField] public RectTransform ShopView { get; private set; }
        [field: SerializeField] public RectTransform RedactorView { get; private set; }
        [field: SerializeField] public PurchaseCard PurchaseCardPrefab { get; private set; }
        [field: SerializeField] public ProductionCard ProductCardPrefab { get; private set; }
        [field: SerializeField] public ProductCard ProductCard { get; private set; }
        [field: SerializeField] public WindowToChangeProduct WindowToChangeProduct { get; private set; }
        [field: SerializeField] public Vector2 SpaceBetweenPurchaseCards { get; private set; } = new (265f, 50);
        [field: SerializeField] public int CardsRowCount { get; private set; } = 6;
        [field: SerializeField] public SectionButtons SectionButtons { get; private set; }
        [field: SerializeField] public Vector3 SectionButtonsPositionForShopPage { get; private set; }
        [field: SerializeField] public Vector3 SectionButtonsPositionForRedactorPage { get; private set; }
        [field: SerializeField] public Vector2 OffsetForProductionCards { get; private set; } = new (10.0f, -10.0f);
        [field: SerializeField] public float SpaceBetweenProductionCards { get; private set; } = 25.0f;
    }
}