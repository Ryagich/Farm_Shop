using Inventory.Finance;
using TMPro;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace UI.Pages
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class ShopPage : BasePage
    {
        public override PageType Type { get; } = PageType.Shop;

        private readonly UIConfig uiConfig;
        private readonly FinanceManager financeManager;
        private readonly RectTransform canvasRect;
        private readonly IObjectResolver resolver;
        
        private RectTransform contentRect = null!;

        public ShopPage
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
            this.uiConfig = UIConfig;

            canvasRect = canvas.GetComponent<RectTransform>();
        }

        //Начальная и конечная точки - -670 и 670
        //расстояние между карточками 270
        //помещается в строку 6 карточек
        public override void Draw()
        {
            contentRect = resolver.Instantiate(uiConfig.ContentPref, canvasRect);
            contentRect.name = $"{uiConfig.ContentPref.name} | {Type}";
            var financeRect = resolver.Instantiate(uiConfig.FinanceTextPrefab, contentRect);
            var financeText = financeRect.GetComponentInChildren<TMP_Text>();
            var financeDrawer = new FinanceDrawer(financeManager, financeText);
            
            var shopViewRect = resolver.Instantiate(uiConfig.ShopView, contentRect);
        }

        public override void Hide()
        {
            if (contentRect)
                Object.Destroy(contentRect.gameObject);
        }
    }
}