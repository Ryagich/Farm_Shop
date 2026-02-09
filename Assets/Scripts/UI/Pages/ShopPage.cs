using System.Linq;
using GameModes;
using Inventory.Finance;
using Localization;
using Storage;
using TMPro;
using UI.Configs;
using UnityEngine;
using UnityEngine.UI;
using Utils;
using VContainer;
using VContainer.Unity;
using YG;

namespace UI.Pages
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class ShopPage : BasePage, AreaDrawer
    {
        public override PageType Type { get; } = PageType.Shop;
        public Area CurrentArea { get; private set; } = Area.Shop;

        private readonly UIConfig uiConfig;
        private readonly LocalizationConfig localizationConfig;
        private readonly Storage.Storage storage;
        private readonly FinanceManager financeManager;
        private readonly RectTransform canvasRect;
        private readonly IObjectResolver resolver;
        private readonly UIUtils uiUtils;
        private RectTransform contentRect = null!;

        public ShopPage
            (
                UIConfig uiConfig,
                LocalizationConfig localizationConfig,
                FinanceManager financeManager,
                Storage.Storage storage,
                UIUtils uiUtils,
                Canvas canvas,
                IObjectResolver resolver
            )
        {
            this.uiConfig = uiConfig;
            this.localizationConfig = localizationConfig;
            this.financeManager = financeManager;
            this.storage = storage;
            this.uiUtils = uiUtils;
            this.resolver = resolver;
            canvasRect = canvas.GetComponent<RectTransform>();
        }

        public override void Draw()
        {
            contentRect = resolver.Instantiate(uiConfig.ContentPref, canvasRect);
            contentRect.name = $"{uiConfig.ContentPref.name} | {Type}";
            var viewRect = resolver.Instantiate(uiConfig.ShopView, contentRect);
            var content = viewRect.GetComponentsInChildren<RectTransform>()
                                  .First(child => child.name.Equals("Content"));
            var layout = content.GetComponent<GridLayoutGroup>(); 
            var title = viewRect.GetComponentsInChildren<RectTransform>().First(child => child.name.Equals("Title"));
            
            var sectionButtons = resolver.Instantiate(uiConfig.SectionButtons, viewRect);
            var sectionButtonsRect = sectionButtons.GetComponent<RectTransform>();
            sectionButtonsRect.SetParent(title);
            sectionButtonsRect.anchorMin = new Vector2(.0f, .5f);
            sectionButtonsRect.anchorMax = new Vector2(.0f, .5f);
            sectionButtonsRect.pivot = new Vector2(.0f, .5f);
            sectionButtonsRect.anchoredPosition = new Vector2(25.0f, .0f);
            
            var buildings = storage.GetBuildings(CurrentArea).Where(b => b.BuildingConfig.ShowInShop).ToList();
            var columnCount = buildings.Count / uiConfig.CardsRowCount is 0 ? 1 : buildings.Count / uiConfig.CardsRowCount;
            var cardSize = uiConfig.PurchaseCardPrefab.GetComponent<RectTransform>().rect.size;
            var spaceBetweenCardsX = 60;
            //var cardSize = uiConfig.PurchaseCardPrefab.GetComponent<RectTransform>().sizeDelta;

            layout.cellSize = cardSize;
            layout.padding.right = (int)(spaceBetweenCardsX / 2);
            layout.padding.left = (int)(spaceBetweenCardsX / 2);
            layout.padding.top = (int)(uiConfig.SpaceBetweenPurchaseCards.y / 2);
            layout.padding.bottom = (int)(uiConfig.SpaceBetweenPurchaseCards.y / 2);

            layout.spacing = new Vector2(spaceBetweenCardsX, uiConfig.SpaceBetweenPurchaseCards.y);
            
            content.sizeDelta = content.sizeDelta.WithY(uiConfig.SpaceBetweenPurchaseCards.y * columnCount 
                                                              + uiConfig.SpaceBetweenPurchaseCards.y / 2
                                                              + cardSize.y * columnCount);
            for (var i = 0; i < buildings.Count; i++)
            {
                var buildingConfig = buildings[i].BuildingConfig;
                var card = resolver.Instantiate(uiConfig.PurchaseCardPrefab, content);
                // var cardRect = card.GetComponent<RectTransform>();
                // var rows = uiConfig.CardsRowCount is 0 ? 1 : uiConfig.CardsRowCount;
                // var x = (i % uiConfig.CardsRowCount) * spaceToOneCardX - ((rows - 1) * spaceToOneCardX) / 2;
                // Debug.Log($"x {x}");
                // cardRect.anchoredPosition =
                //     new Vector2(x, 
                //                 // ReSharper disable once PossibleLossOfFraction
                //                 -uiConfig.SpaceBetweenPurchaseCards.y * (i / rows) -
                //                 uiConfig.SpaceBetweenPurchaseCards.y / 2
                //                 // ReSharper disable once PossibleLossOfFraction
                //               - cardSize.y * (i / rows));
                card.Icon.sprite = buildingConfig.Icon;
                card.SizeText.text = $"{buildingConfig.Size.x}x{buildingConfig.Size.y}";
                card.Name.text = $"{buildingConfig.Name.GetLocalizedStringCached()}";
                card.InInventory.text =
                    $"{localizationConfig.InInventory.GetLocalizedStringCached()}: {buildings[i].Count}";
                card.Button.GetComponentInChildren<TMP_Text>().text = $"{buildings[i].BuildingConfig.Price}$";
                var i1 = i;
                card.Button.onClick.AddListener(() => Buy(buildings[i1]));
            }
            uiUtils.DrawFinanceDrawer(contentRect);
            uiUtils.DrawGameModesSwitchButtons(contentRect, uiConfig.OffsetForGameMenuButtons, Type);
            uiUtils.InitSectionButtons(sectionButtons, uiConfig.SectionButtonsPositionForShopPage, CurrentArea, this);
        }

        private void Buy(BuildingInStorage buildingInStorage)
        {
            if (financeManager.TryChangeValue(-buildingInStorage.BuildingConfig.Price))
            {
                buildingInStorage.Count++;
                var buildingInStorageSave = YG2.saves.BuildingInStorageSave
                                               .First(b => b.Id.Equals(buildingInStorage.BuildingConfig.Id));
                buildingInStorageSave.Count = buildingInStorage.Count;
                YG2.SaveProgress();
                ReDraw();
            }
        }

        public void SetArea(Area area)
        {
            CurrentArea = area;
            ReDraw();
        }

        private void ReDraw()
        {
            Hide();
            Draw();
        }

        public override void Hide()
        {
            if (contentRect) Object.Destroy(contentRect.gameObject);
        }
    }
}