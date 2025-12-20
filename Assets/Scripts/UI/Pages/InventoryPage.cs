using System.Linq;
using GameModes;
using Localization;
using MessagePipe;
using Messages;
using UI.Configs;
using UI.Hover.PopupLogics;
using UnityEngine;
using Utils;
using VContainer;
using VContainer.Unity;

namespace UI.Pages
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class InventoryPage : BasePage, AreaDrawer, IFixedTickable
    {
        public override PageType Type { get; } = PageType.Inventory;
        public Area CurrentArea { get; private set; } = Area.Shop;

        private readonly UIConfig uiConfig;
        private readonly LocalizationConfig localizationConfig;
        private readonly Storage.Storage storage;
        private readonly HelpInfoDrawer helpInfoDrawer;
        private readonly ObjectInfoPopupsController objectInfoPopupsController;
        private readonly RectTransform canvasRect;
        private readonly IObjectResolver resolver;
        private readonly UIUtils uiUtils;
        private readonly SpritesConfig spritesConfig;
        private readonly IPublisher<OpenShopWithAreaRequest> openShopWithAreaRequestPublisher;
        private readonly IPublisher<ChangeGameModeRequest> changeGameModeRequestPublisher;
        private readonly IPublisher<ChoseBuildingMessage> choseBuildingPublisher;

        private RectTransform contentRect = null!;
        private RectTransform viewRect = null!;
        
        private bool isActive;
        private RectTransform helpRect;

        public InventoryPage
            (
                UIConfig uiConfig,
                LocalizationConfig localizationConfig,
                UIUtils uiUtils,
                SpritesConfig spritesConfig,
                Canvas canvas,
                Storage.Storage storage,
                HelpInfoDrawer helpInfoDrawer,
                ObjectInfoPopupsController objectInfoPopupsController,
                IObjectResolver resolver,
                IPublisher<OpenShopWithAreaRequest> openShopWithAreaRequestPublisher,
                IPublisher<ChangeGameModeRequest> changeGameModeRequestPublisher,
                IPublisher<ChoseBuildingMessage> choseBuildingPublisher
            )   
        {
            this.uiConfig = uiConfig;
            this.localizationConfig = localizationConfig;
            this.uiUtils = uiUtils;
            this.spritesConfig = spritesConfig;
            this.storage = storage;
            this.helpInfoDrawer = helpInfoDrawer;
            this.objectInfoPopupsController = objectInfoPopupsController;
            this.resolver = resolver;
            this.openShopWithAreaRequestPublisher = openShopWithAreaRequestPublisher;
            this.changeGameModeRequestPublisher = changeGameModeRequestPublisher;
            this.choseBuildingPublisher = choseBuildingPublisher;

            canvasRect = canvas.GetComponent<RectTransform>();
        }

        public void FixedTick()
        {
            if (helpRect)
            {
                Object.Destroy(helpRect.gameObject);
                helpRect = null;
            }
            if (isActive 
             && objectInfoPopupsController.HavePopup 
             && !objectInfoPopupsController.IsFixed
             && viewRect)
                helpRect = helpInfoDrawer.DrawMouseHelpForInventoryPage(viewRect);
        }

        public override void Draw()
        {
            if (contentRect)
                Object.Destroy(contentRect.gameObject);
            contentRect = resolver.Instantiate(uiConfig.ContentPref, canvasRect);
            contentRect.name = $"{uiConfig.ContentPref.name} | {Type}";

            viewRect = resolver.Instantiate(uiConfig.RedactorView, contentRect);
            var content = viewRect.GetComponentsInChildren<RectTransform>()
                                  .First(child => child.name.Equals("Content"));
            var sectionButtons = resolver.Instantiate(uiConfig.SectionButtons, viewRect);
            var sectionButtonsRect = sectionButtons.GetComponent<RectTransform>();
            var title = viewRect.GetComponentsInChildren<RectTransform>()
                                .First(child => child.name.Equals("Title"));
            sectionButtonsRect.SetParent(title);
            sectionButtonsRect.anchorMin = new Vector2(.0f, .5f);
            sectionButtonsRect.anchorMax = new Vector2(.0f, .5f);
            sectionButtonsRect.pivot = new Vector2(.0f, .5f);
            sectionButtonsRect.anchoredPosition = new Vector2(25.0f, .0f);

            var buildings = storage.GetBuildings(CurrentArea).Where(b => b.Count > 0).ToList();
            var cardSize = uiConfig.ProductCardPrefab.GetComponent<RectTransform>().sizeDelta;

            content.sizeDelta =
                content.sizeDelta.WithX((buildings.Count + 1) *
                                        (cardSize.x + uiConfig.SpaceBetweenProductionCards));
            uiUtils.DrawFinanceDrawer(contentRect);
            uiUtils.DrawGameModesSwitchButtons(contentRect, uiConfig.OffsetForGameMenuButtons, Type);
            uiUtils.InitSectionButtons(sectionButtons, uiConfig.SectionButtonsPositionForRedactorPage, CurrentArea,
                                       this);

            for (var i = 0; i < buildings.Count; i++)
            {
                var buildingConfig = buildings[i].BuildingConfig;
                var card = resolver.Instantiate(uiConfig.ProductCardPrefab, content);
                var cardRect = card.GetComponent<RectTransform>();
                cardRect.anchoredPosition = uiConfig.OffsetForProductionCards
                                          + new Vector2(i * (cardSize.x + uiConfig.SpaceBetweenProductionCards), 0);
                card.Icon.sprite = buildingConfig.Icon;
                card.SizeText.text = $"{buildingConfig.Size.x}x{buildingConfig.Size.y}";
                card.Name.text = $"{buildingConfig.Name.GetLocalizedString()}";
                card.InInventory.text = $"{localizationConfig.InInventory.GetLocalizedString()}: {buildings[i].Count}";
                var i1 = i;

                card.Button.onClick.AddListener(() =>
                                                    choseBuildingPublisher
                                                       .Publish(new
                                                                    ChoseBuildingMessage(buildings[i1].BuildingConfig,
                                                                         default, default, default, null)));
                card.Button.onClick.AddListener(() =>
                                                    changeGameModeRequestPublisher
                                                       .Publish(new ChangeGameModeRequest(GameMode.Redactor)));
            }
            DrawCardTo(content, buildings.Count);
            isActive = true;
        }

        private void DrawCardTo(RectTransform content, int cardCount)
        {
            var card = resolver.Instantiate(uiConfig.ProductCardPrefab, content);
            var cardRect = card.GetComponent<RectTransform>();
            cardRect.anchoredPosition = uiConfig.OffsetForProductionCards 
                                      + new Vector2(cardCount * (cardRect.sizeDelta.x + uiConfig.SpaceBetweenProductionCards), 0);

            if (CurrentArea is Area.Garden)
            {
                card.Icon.sprite = spritesConfig.ShopIcon;
                card.Button.onClick.AddListener(() => openShopWithAreaRequestPublisher
                                                   .Publish(new OpenShopWithAreaRequest(Area.Garden)));
            }
            else if (CurrentArea is Area.Shop)
            {
                card.Icon.sprite = spritesConfig.ShopIcon;
                card.Button.onClick.AddListener(() => openShopWithAreaRequestPublisher
                                                   .Publish(new OpenShopWithAreaRequest(Area.Shop)));
            }
            else if (CurrentArea is Area.Production)
            {
                card.Icon.sprite = spritesConfig.ShopIcon;
                card.Button.onClick.AddListener(() => openShopWithAreaRequestPublisher
                                                   .Publish(new OpenShopWithAreaRequest(Area.Production)));
            }
            card.SizeText.text = $"";
            card.Name.text = $"";
            card.InInventory.text = $"{localizationConfig.GoToShop.GetLocalizedString()}";
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
            if (contentRect)
                Object.Destroy(contentRect.gameObject);
        }
    }
}