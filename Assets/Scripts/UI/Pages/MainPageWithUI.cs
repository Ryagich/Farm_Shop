using System.Linq;
using Inventory.Finance;
using MessagePipe;
using Messages;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using VContainer.Unity;

namespace UI.Pages
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class MainPageWithUI : BasePage
    {
        public override PageType Type { get; } = PageType.GameWithUI;
        
        private readonly UIConfig uiConfig;
        private readonly FinanceManager financeManager;
        private readonly RectTransform canvasRect;
        private readonly IObjectResolver resolver;
        private readonly IPublisher<OpenShopModeMessage> openShopModePublisher;
        private readonly IPublisher<OpenRedactorModeMessage> openRedactorModeMessage;

        private RectTransform contentRect = null!;
        
        public MainPageWithUI
            (
                // ReSharper disable once InconsistentNaming
                UIConfig UIConfig,
                Canvas canvas,
                FinanceManager financeManager,
                IObjectResolver resolver,
                IPublisher<OpenShopModeMessage> openShopModePublisher,
                IPublisher<OpenRedactorModeMessage> openRedactorModeMessage
            )
        {
            this.financeManager = financeManager;
            this.resolver = resolver;
            this.openShopModePublisher = openShopModePublisher;
            this.openRedactorModeMessage = openRedactorModeMessage;
            this.uiConfig = UIConfig;
            
            canvasRect = canvas.GetComponent<RectTransform>();
        }
        
        public override void Draw()
        {
            contentRect = resolver.Instantiate(uiConfig.ContentPref, canvasRect);
            contentRect.name = $"{uiConfig.ContentPref.name} | {Type}";
            var financeRect = resolver.Instantiate(uiConfig.FinanceTextPrefab, contentRect);
            var financeText = financeRect.GetComponentInChildren<TMP_Text>();
            var financeDrawer = new FinanceDrawer(financeManager, financeText);
            
            var buttonPosition = uiConfig.OffsetForGameMenuButtons;
            var buttonsParent = resolver.Instantiate(uiConfig.GameMenuButtonsParent, contentRect);
            var buttonsParentRect = buttonsParent.GetComponent<RectTransform>();
            
            var buttonToShop = resolver.Instantiate(uiConfig.GameMenuButton, buttonsParentRect);
            var buttonToShopRect = buttonToShop.GetComponent<RectTransform>();
            var buttonToShopIcon = buttonToShopRect.GetComponentsInChildren<Image>().First(i => i.name.Equals("Image"));
            
            var buttonToRedactor = resolver.Instantiate(uiConfig.GameMenuButton, buttonsParentRect);
            var buttonToRedactorRect = buttonToRedactor.GetComponent<RectTransform>();
            var buttonToRedactorIcon = buttonToRedactorRect.GetComponentsInChildren<Image>().First(i => i.name.Equals("Image"));
                
            buttonToShop.onClick.AddListener(OpenShop);
            buttonToShop.name = $"Button To Shop";
            buttonToShopRect.anchoredPosition = buttonPosition;
            buttonToShopIcon.sprite = uiConfig.ShopIcon;
            
            buttonPosition += Vector2.down * (10 + buttonToShopRect.sizeDelta.y);
            buttonToRedactor.onClick.AddListener(OpenRedactor);
            buttonToRedactor.name = $"Button To Redactor";
            buttonToRedactorRect.anchoredPosition = buttonPosition;
            buttonToRedactorIcon.sprite = uiConfig.RedactorIcon;
        }

        public override void Hide()
        {
            if (contentRect)
                Object.Destroy(contentRect.gameObject);
        }

        private void OpenShop() => openShopModePublisher.Publish(new OpenShopModeMessage());
        private void OpenRedactor() => openRedactorModeMessage.Publish(new OpenRedactorModeMessage());
    }
}