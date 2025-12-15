using System.Linq;
using GameModes;
using Localization;
using MessagePipe;
using Messages;
using UI.Configs;
using UnityEngine;
using Utils;
using VContainer;
using VContainer.Unity;

namespace UI.Pages
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class InventoryPage : BasePage, AreaDrawer
    {
        public override PageType Type { get; } = PageType.Inventory;
        public Area CurrentArea { get; private set; } = Area.Shop;

        private readonly UIConfig uiConfig;
        private readonly LocalizationConfig localizationConfig;
        private readonly Storage.Storage storage;
        private readonly RectTransform canvasRect;
        private readonly IObjectResolver resolver;
        private readonly UIUtils uiUtils;
        private readonly IPublisher<OpenShopWithAreaRequest> openShopWithAreaRequestPublisher;
        private readonly IPublisher<ChangeGameModeRequest> changeGameModeRequestPublisher;
        private readonly IPublisher<ChoseBuildingMessage> choseBuildingPublisher;

        private RectTransform contentRect = null!;
        
        public InventoryPage
            (
                UIConfig uiConfig,
                LocalizationConfig localizationConfig,
                Canvas canvas,
                Storage.Storage storage,
                UIUtils uiUtils,
                IObjectResolver resolver,
                IPublisher<OpenShopWithAreaRequest> openShopWithAreaRequestPublisher,
                IPublisher<ChangeGameModeRequest> changeGameModeRequestPublisher,
                IPublisher<ChoseBuildingMessage> choseBuildingPublisher
            )   
        {
            this.uiConfig = uiConfig;
            this.localizationConfig = localizationConfig;
            this.uiUtils = uiUtils;
            this.storage = storage;
            this.resolver = resolver;
            this.openShopWithAreaRequestPublisher = openShopWithAreaRequestPublisher;
            this.changeGameModeRequestPublisher = changeGameModeRequestPublisher;
            this.choseBuildingPublisher = choseBuildingPublisher;

            canvasRect = canvas.GetComponent<RectTransform>();
        }

        public override void Draw()
        {
            contentRect = resolver.Instantiate(uiConfig.ContentPref, canvasRect);
            contentRect.name = $"{uiConfig.ContentPref.name} | {Type}";
          
            var viewRect = resolver.Instantiate(uiConfig.RedactorView, contentRect);
            var content = viewRect.GetComponentsInChildren<RectTransform>()
                                                  .First(child => child.name.Equals("Content"));
            var sectionButtons = resolver.Instantiate(uiConfig.SectionButtons, viewRect);
            var buildings = storage.GetBuildings(CurrentArea).Where(b => b.Count > 0).ToList();
            var cardSize = uiConfig.ProductCardPrefab.GetComponent<RectTransform>().sizeDelta;
            
            content.sizeDelta = content.sizeDelta.WithX((buildings.Count + 1) * (cardSize.x + uiConfig.SpaceBetweenProductionCards));
            uiUtils.DrawFinanceDrawer(contentRect);
            uiUtils.DrawGameModesSwitchButtons(contentRect, uiConfig.OffsetForGameMenuButtons, Type);
            uiUtils.InitSectionButtons(sectionButtons, uiConfig.SectionButtonsPositionForRedactorPage, CurrentArea, this);
            
            for (var i = 0; i < buildings.Count; i++)
            {
                var buildingConfig = buildings[i].BuildingConfig;
                var card = resolver.Instantiate(uiConfig.ProductCardPrefab, content);
                var cardRect = card.GetComponent<RectTransform>();
                cardRect.anchoredPosition = uiConfig.OffsetForProductionCards 
                                          + new Vector2(i * (cardSize.x + uiConfig.SpaceBetweenProductionCards), 0);
                card.Icon.sprite = buildingConfig.Icon;
                card.SizeText.text = $"{buildingConfig.Size.x}x{buildingConfig.Size.y}";
                card.Text.text = $"{buildingConfig.Name.GetLocalizedString()}\n" +
                                 $"{localizationConfig.InInventory.GetLocalizedString()}: {buildings[i].Count}";
                var i1 = i;
                
                card.Button.onClick.AddListener(() => choseBuildingPublisher.Publish(new ChoseBuildingMessage(buildings[i1].BuildingConfig, default, default, default, null)));
                card.Button.onClick.AddListener(() => changeGameModeRequestPublisher.Publish(new ChangeGameModeRequest(GameMode.Redactor)));
            }
            DrawCardTo(content, buildings.Count);
        }

        private void DrawCardTo(RectTransform content, int cardCount)
        {
            var card = resolver.Instantiate(uiConfig.ProductCardPrefab, content);
            var cardRect = card.GetComponent<RectTransform>();
            cardRect.anchoredPosition = uiConfig.OffsetForProductionCards 
                                      + new Vector2(cardCount * (cardRect.sizeDelta.x + uiConfig.SpaceBetweenProductionCards), 0);

            if (CurrentArea is Area.Garden)
            {
                card.Icon.sprite = uiConfig.SpritesConfig.ShopIcon;
                card.Button.onClick.AddListener(() => openShopWithAreaRequestPublisher
                                                   .Publish(new OpenShopWithAreaRequest(Area.Garden)));
            }
            else if (CurrentArea is Area.Shop)
            {
                card.Icon.sprite = uiConfig.SpritesConfig.ShopIcon;
                card.Button.onClick.AddListener(() => openShopWithAreaRequestPublisher
                                                   .Publish(new OpenShopWithAreaRequest(Area.Shop)));
            }
            else if (CurrentArea is Area.Production)
            {
                card.Icon.sprite = uiConfig.SpritesConfig.ShopIcon;
                card.Button.onClick.AddListener(() => openShopWithAreaRequestPublisher
                                                   .Publish(new OpenShopWithAreaRequest(Area.Production)));
            }
            card.Text.text = $"{localizationConfig.GoToShop.GetLocalizedString()}";
        }
        
        public void SetArea(Area area)
        {
            CurrentArea = area;
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