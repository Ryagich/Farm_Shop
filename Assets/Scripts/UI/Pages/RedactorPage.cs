using GameModes;
using UI.Configs;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace UI.Pages
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class RedactorPage : BasePage, AreaDrawer
    {
        private readonly UIConfig uiConfig;
        private readonly UIUtils uiUtils;
        private readonly HelpInfoDrawer helpInfoDrawer;
        private readonly IObjectResolver resolver;
        public override PageType Type { get; } = PageType.Redactor;
        public Area CurrentArea { get; private set; } = Area.None;

        private readonly RectTransform canvasRect;
        private RectTransform contentRect = null!;

        public RedactorPage
            (
                UIConfig uiConfig,
                UIUtils uiUtils,
                Canvas canvas,
                HelpInfoDrawer helpInfoDrawer,
                IObjectResolver resolver
            )
        {
            this.uiConfig = uiConfig;
            this.uiUtils = uiUtils;
            this.helpInfoDrawer = helpInfoDrawer;
            this.resolver = resolver;
            canvasRect = canvas.GetComponent<RectTransform>();
        }

        public override void Draw()
        {
            contentRect = resolver.Instantiate(uiConfig.ContentPref, canvasRect);
            contentRect.name = $"{uiConfig.ContentPref.name} | {Type}";
            
            uiUtils.DrawFinanceDrawer(contentRect);
            helpInfoDrawer.DrawMouseHelpForRedactorPage(contentRect);
        }

        public void SetArea(Area area)
        {
        }

        public override void Hide()
        {
            if (contentRect)
                Object.Destroy(contentRect.gameObject);
        }
    }
}