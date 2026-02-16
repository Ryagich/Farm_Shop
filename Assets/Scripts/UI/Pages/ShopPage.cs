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
        private readonly BuildingsStorage buildingsStorage;
        private readonly ItemsStorage itemsStorage;
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
                BuildingsStorage buildingsStorage,
                ItemsStorage itemsStorage,
                UIUtils uiUtils,
                Canvas canvas,
                IObjectResolver resolver
            )
        {
            this.uiConfig = uiConfig;
            this.localizationConfig = localizationConfig;
            this.financeManager = financeManager;
            this.buildingsStorage = buildingsStorage;
            this.itemsStorage = itemsStorage;
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
            
            var buildings = buildingsStorage.GetBuildings(CurrentArea).Where(b => b.BuildingConfig.ShowInShop).ToList();
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
                card.Icon.sprite = buildingConfig.Icon;
                card.SizeText.text = $"{buildingConfig.Size.x}x{buildingConfig.Size.y}";
                card.Name.text = $"{buildingConfig.Name.GetLocalizedStringCached()}";
                card.InInventory.text =
                    $"{localizationConfig.InInventory.GetLocalizedStringCached()}: {buildings[i].Count}";
                card.Button.GetComponentInChildren<TMP_Text>().text = $"{buildings[i].BuildingConfig.Price}$";
                
                var i1 = i;
                card.Button.onClick.AddListener(() => BuyBuilding(buildings[i1]));
            }
            if (CurrentArea == Area.Garden)
            {
                for (var i = 0; i < itemsStorage.Items.Count; i++)
                {
                    var itemConfig = itemsStorage.Items[i].ItemConfig;
                    var card = resolver.Instantiate(uiConfig.PurchaseCardPrefab, content);
                    card.Icon.sprite = itemConfig.Icon;
                    card.SizeText.text = $"";
                    card.Name.text = $"{itemConfig.Name.GetLocalizedStringCached()}";
                    card.InInventory.text =
                        $"{localizationConfig.InInventory.GetLocalizedStringCached()}: {itemsStorage.Items[i].Count}";
                    card.Button.GetComponentInChildren<TMP_Text>().text = $"{buildings[i].BuildingConfig.Price}$";
                    
                    var i1 = i;
                    card.Button.onClick.AddListener(() => BuyItem(itemsStorage.Items[i1]));
                }
            }
            uiUtils.DrawFinanceDrawer(contentRect);
            uiUtils.DrawGameModesSwitchButtons(contentRect, uiConfig.OffsetForGameMenuButtons, Type);
            uiUtils.InitSectionButtons(sectionButtons, uiConfig.SectionButtonsPositionForShopPage, CurrentArea, this);
        }
        
        private void BuyItem(ItemInStorage itemInStorage)
        {
            if (financeManager.TryChangeValue(-itemInStorage.ItemConfig.Price))
            {
                itemInStorage.Count++;
                var buildingInStorageSave = YG2.saves.ItemInStorageSave
                                               .First(b => b.Id.Equals(itemInStorage.ItemConfig.Id));
                buildingInStorageSave.Count = itemInStorage.Count;
                YG2.SaveProgress();
                ReDraw();
            }
        }
        
        private void BuyBuilding(BuildingInStorage buildingInStorage)
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