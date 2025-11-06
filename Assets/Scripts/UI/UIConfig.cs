using Storage;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    [CreateAssetMenu(fileName = "UI Config", menuName = "configs/UI/UIConfig")]
    public class UIConfig : ScriptableObject
    {
        [field: SerializeField] public RectTransform ContentPref {get; private set;} = null!;
        [field: SerializeField] public RectTransform FinanceTextPrefab { get; private set; } = null!;
        [field: SerializeField] public RectTransform GameMenuButtonsParent { get; private set; } = null!;
        [field: SerializeField] public Button GameMenuButton { get; private set; } = null!;
        [field: SerializeField] public Vector2 OffsetForGameMenuButtons { get; private set; } = new Vector2(10, 0);
        [field: SerializeField] public Sprite ShopIcon { get; private set; }
        [field: SerializeField] public Sprite RedactorIcon { get; private set; }
        [field: SerializeField] public RectTransform ShopView { get; private set; } = null!;
        [field: SerializeField] public PurchaseCard PurchaseCardPrefab { get; private set; } = null!;
        [field: SerializeField] public Vector2 SpaceBetweenPurchaseCards { get; private set; } = new Vector2(265f, 50);
    }
}