using System.Linq;
using GameModes;
using Inventory.Finance;
using MessagePipe;
using Messages;
using TMPro;
using UI.Configs;
using UI.Pages;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using VContainer.Unity;

namespace UI
{
    //TODO: Идейно класс должен быть статическим, практически...
    // ReSharper disable once ClassNeverInstantiated.Global
    public class UIUtils
    {
        private readonly UIConfig uiConfig;
        private readonly FinanceManager financeManager;
        private readonly IObjectResolver resolver;
        private readonly IPublisher<ChangeGameModeRequest> openPageRequestPublisher;

        public UIUtils
            ( 
                UIConfig uiConfig, 
                FinanceManager financeManager,
                IObjectResolver resolver,
                IPublisher<ChangeGameModeRequest> openPageRequestPublisher
            )
        {
            this.uiConfig = uiConfig;
            this.financeManager = financeManager;
            this.resolver = resolver;
            this.openPageRequestPublisher = openPageRequestPublisher;
        }

        public FinanceDrawer DrawFinanceDrawer(RectTransform contentRect)
        {
            var financeRect = resolver.Instantiate(uiConfig.FinanceTextPrefab, contentRect);
            var financeText = financeRect.GetComponentInChildren<TMP_Text>();
            var financeDrawer = new FinanceDrawer(financeManager, financeText);

            return financeDrawer;
        }
        
        public void DrawGameModesSwitchButtons
            (
                RectTransform contentRect,
                Vector2 position,
                PageType type
            )
        {
            var buttonPosition = position;
            var buttonsParent = resolver.Instantiate(uiConfig.GameMenuButtonsParent, contentRect);
            var buttonsParentRect = buttonsParent.GetComponent<RectTransform>();
            
            var buttonToGame = resolver.Instantiate(uiConfig.GameMenuButton, buttonsParentRect);
            var buttonToGameRect = buttonToGame.GetComponent<RectTransform>();
            var buttonToGameIcon = buttonToGameRect.GetComponentsInChildren<Image>().First(i => i.name.Equals("Image"));
            buttonToGame.onClick.AddListener(() => OpenPage(GameModes.GameMode.Game));
            buttonToGame.name = $"Button To Game";
            buttonToGameIcon.sprite = uiConfig.SpritesConfig.GameIcon;
            buttonToGameRect.anchoredPosition = buttonPosition;
            buttonPosition += Vector2.down * (10 + buttonToGameRect.sizeDelta.y);
            
            var buttonToShop = resolver.Instantiate(uiConfig.GameMenuButton, buttonsParentRect);
            var buttonToShopRect = buttonToShop.GetComponent<RectTransform>();
            var buttonToShopIcon = buttonToShopRect.GetComponentsInChildren<Image>().First(i => i.name.Equals("Image"));
            buttonToShop.onClick.AddListener(() => OpenPage(GameModes.GameMode.Shop));
            buttonToShop.name = $"Button To Shop";
            buttonToShopRect.anchoredPosition = buttonPosition;
            buttonPosition += Vector2.down * (10 + buttonToShopRect.sizeDelta.y);
            buttonToShopIcon.sprite = uiConfig.SpritesConfig.ShopIcon;
            
            var buttonToRedactor = resolver.Instantiate(uiConfig.GameMenuButton, buttonsParentRect);
            var buttonToRedactorRect = buttonToRedactor.GetComponent<RectTransform>();
            var buttonToRedactorIcon = buttonToRedactorRect.GetComponentsInChildren<Image>().First(i => i.name.Equals("Image"));
            buttonToRedactor.onClick.AddListener(() => OpenPage(GameModes.GameMode.Inventory));
            buttonToRedactor.name = $"Button To Redactor";
            buttonToRedactorRect.anchoredPosition = buttonPosition;
            buttonToRedactorIcon.sprite = uiConfig.SpritesConfig.RedactorIcon;
            
            switch (type)
            {
                case PageType.GameWithUI or PageType.MainGame:
                    buttonToGame.interactable = false;
                    break;
                case PageType.Shop:
                    buttonToShop.interactable = false;
                    break;
                case PageType.Inventory:
                    buttonToRedactor.interactable = false;
                    break;
            }
        }
        
        public void InitSectionButtons(SectionButtons sectionButtons, Vector3 pos, Area area, AreaDrawer areaDrawer)
        {
            var sectionButtonsRect = sectionButtons.GetComponent<RectTransform>();
            sectionButtonsRect.anchoredPosition = pos;

            switch (area)
            {
                case Area.Garden:
                    sectionButtons.Garden.interactable = false;
                    break;
                case Area.Shop:
                    sectionButtons.ToShop.interactable = false;
                    break;
                case Area.Production:
                    sectionButtons.Production.interactable = false;
                    break;
            }

            sectionButtons.Garden.onClick.AddListener(() => areaDrawer.SetArea(Area.Garden));
            sectionButtons.ToShop.onClick.AddListener(() => areaDrawer.SetArea(Area.Shop));
            sectionButtons.Production.onClick.AddListener(() => areaDrawer.SetArea(Area.Production));
        }
        
        private void OpenPage(GameModes.GameMode mode) => openPageRequestPublisher.Publish(new ChangeGameModeRequest(mode));
    }
}