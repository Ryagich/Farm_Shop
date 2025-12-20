using UI.Configs;
using UI.Hover.PopupLogics;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace UI.Pages
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class MainPageWithUI : BasePage, IFixedTickable
    {
        public override PageType Type { get; } = PageType.GameWithUI;
        
        private readonly UIConfig uiConfig;
        private readonly ObjectInfoPopupsController objectInfoPopupsController;
        private readonly HelpInfoDrawer helpInfoDrawer;
        private readonly RectTransform canvasRect;
        private readonly IObjectResolver resolver;
        private readonly UIUtils uiUtils;

        private RectTransform contentRect = null!;
        private bool isActive;
        private RectTransform helpRect;

        public MainPageWithUI
            (
                UIConfig uiConfig,
                Canvas canvas,
                ObjectInfoPopupsController objectInfoPopupsController,
                HelpInfoDrawer helpInfoDrawer,
                IObjectResolver resolver,
                UIUtils uiUtils
            )
        {
            this.resolver = resolver;
            this.uiUtils = uiUtils;
            this.uiConfig = uiConfig;
            this.objectInfoPopupsController = objectInfoPopupsController;
            this.helpInfoDrawer = helpInfoDrawer;

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
             && contentRect)
                helpRect = helpInfoDrawer.DrawMouseHelpForMainWithUIPage(contentRect);
        }
        
        public override void Draw()
        {
            contentRect = resolver.Instantiate(uiConfig.ContentPref, canvasRect);
            contentRect.name = $"{uiConfig.ContentPref.name} | {Type}";
            uiUtils.DrawFinanceDrawer(contentRect);
            uiUtils.DrawGameModesSwitchButtons(contentRect, uiConfig.OffsetForGameMenuButtons, Type);
            isActive = true;
        }

        public override void Hide()
        {
            if (contentRect)
                Object.Destroy(contentRect.gameObject);
            isActive = false;
        }
    }
}