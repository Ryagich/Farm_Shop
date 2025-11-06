using Inventory.Finance;
using TMPro;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace UI.Pages
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class MainPage : BasePage
    {
        // ReSharper disable once InconsistentNaming
        private readonly UIConfig UIConfig;
        private readonly FinanceManager financeManager;
        private readonly RectTransform canvasRect;
        private readonly IObjectResolver resolver;
        public override PageType Type { get; } = PageType.MainGame;

        private RectTransform contentRect = null!;
        
        public MainPage
            (
                // ReSharper disable once InconsistentNaming
                UIConfig UIConfig,
                Canvas canvas,
                FinanceManager financeManager,
                IObjectResolver resolver
            )
        {
            this.financeManager = financeManager;
            this.resolver = resolver;
            this.UIConfig = UIConfig;
            
            canvasRect = canvas.GetComponent<RectTransform>();
        }

        public override void Draw()
        {
            contentRect = resolver.Instantiate(UIConfig.ContentPref, canvasRect);
            var financeRect = resolver.Instantiate(UIConfig.FinanceTextPrefab, contentRect);
            var financeText = financeRect.GetComponentInChildren<TMP_Text>();

            var financeDrawer = new FinanceDrawer(financeManager, financeText);
        }

        public override void Hide()
        {
            if (contentRect)
               Object.Destroy(contentRect.gameObject);
        }
    }
}