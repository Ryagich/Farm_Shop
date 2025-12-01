using System.Linq;
using GameModes;
using Inventory.Finance;
using Storage;
using UI.Configs;
using UnityEngine;
using Utils;
using VContainer;
using VContainer.Unity;

namespace UI.Pages
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class ShopPage : BasePage, AreaDrawer
    {
        public override PageType Type { get; } = PageType.Shop;
        public  Area CurrentArea { get; private set; } = Area.Shop;
        
        private readonly UIConfig uiConfig;
        private readonly Storage.Storage storage;
        private readonly FinanceManager financeManager;
        private readonly RectTransform canvasRect;
        private readonly IObjectResolver resolver;
        private readonly UIUtils uiUtils;

        private RectTransform contentRect = null!;

        public ShopPage
            (
                UIConfig uiConfig,
                FinanceManager financeManager,
                Storage.Storage storage,
                UIUtils uiUtils,
                Canvas canvas,
                IObjectResolver resolver
            )   
        {
            this.uiConfig = uiConfig;
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
            var sectionButtons = resolver.Instantiate(uiConfig.SectionButtons, viewRect);
            var buildings = storage.GetBuildings(CurrentArea).Where(b => b.BuildingConfig.ShowInShop).ToList();
            var spaceToOneCard = viewRect.sizeDelta.x / uiConfig.CardsRowCount;
            var cardSize = uiConfig.PurchaseCardPrefab.GetComponent<RectTransform>().sizeDelta;
            var columnCount = buildings.Count / uiConfig.CardsRowCount;
                
            content.sizeDelta = content.sizeDelta.WithY(uiConfig.SpaceBetweenPurchaseCards.y * columnCount 
                                                              + uiConfig.SpaceBetweenPurchaseCards.y / 2
                                                              + cardSize.y * columnCount);
            for (var i = 0; i < buildings.Count; i++)
            {
                var buildingConfig = buildings[i].BuildingConfig;
                var card = resolver.Instantiate(uiConfig.PurchaseCardPrefab, content);
                var cardRect = card.GetComponent<RectTransform>();
                var x = (i % uiConfig.CardsRowCount) * spaceToOneCard - ((uiConfig.CardsRowCount - 1) * spaceToOneCard) / 2;
                cardRect.anchoredPosition = new Vector2(x,
                                                    - uiConfig.SpaceBetweenPurchaseCards.y * (i / uiConfig.CardsRowCount) 
                                                      - uiConfig.SpaceBetweenPurchaseCards.y / 2 
                                                      - cardSize.y * (i / uiConfig.CardsRowCount));
                card.Icon.sprite = buildingConfig.Icon;
                card.SizeText.text = $"{buildingConfig.Size.x}x{buildingConfig.Size.y}";
                card.Name.text = $"{buildingConfig.Name}\n" +
                                 $"Price: {buildings[i].BuildingConfig.Price}";
                card.InInventory.text = $"In Inventory: {buildings[i].Count}";

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
                ReDraw();
            }
        }
        
        public void SetArea(Area area)
        {
            CurrentArea = area;
            Hide();
            Draw();
        }

        private void ReDraw()
        {
            Hide();
            Draw();
        }
        
        public override void Hide()
        {
            if (contentRect)
                Object.Destroy(contentRect.gameObject);
        }
    }
}