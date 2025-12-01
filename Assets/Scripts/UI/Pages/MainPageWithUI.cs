using UI.Configs;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace UI.Pages
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class MainPageWithUI : BasePage
    {
        public override PageType Type { get; } = PageType.GameWithUI;
        
        private readonly UIConfig uiConfig;
        private readonly RectTransform canvasRect;
        private readonly IObjectResolver resolver;
        private readonly UIUtils uiUtils;

        private RectTransform contentRect = null!;
        
        public MainPageWithUI
            (
                UIConfig uiConfig,
                Canvas canvas,
                IObjectResolver resolver,
                UIUtils uiUtils
            )
        {
            this.resolver = resolver;
            this.uiUtils = uiUtils;
            this.uiConfig = uiConfig;
            
            canvasRect = canvas.GetComponent<RectTransform>();
        }
        
        public override void Draw()
        {
            contentRect = resolver.Instantiate(uiConfig.ContentPref, canvasRect);
            contentRect.name = $"{uiConfig.ContentPref.name} | {Type}";
            uiUtils.DrawFinanceDrawer(contentRect);
            uiUtils.DrawGameModesSwitchButtons(contentRect, uiConfig.OffsetForGameMenuButtons, Type);
        }

        public override void Hide()
        {
            if (contentRect)
                Object.Destroy(contentRect.gameObject);
        }
    }
}