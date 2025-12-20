using UI.Pages;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace UI
{
    public class CanvasLifetimeScope : LifetimeScope
    {
        [field: SerializeField] public Canvas Canvas { get; private set; }
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterInstance(Canvas).As<Canvas>();
            
            builder.Register<MainPage>(Lifetime.Scoped);
            // builder.Register<MainPageWithUI>(Lifetime.Scoped);
            builder.Register<ShopPage>(Lifetime.Scoped);
            // builder.Register<InventoryPage>(Lifetime.Scoped);
            builder.Register<RedactorPage>(Lifetime.Scoped);
            builder.Register<UIUtils>(Lifetime.Scoped);
            builder.Register<HelpInfoDrawer>(Lifetime.Scoped);
            
            builder.RegisterEntryPoint<PagesController>();
            builder.RegisterEntryPoint<InventoryPage>().AsSelf();
            builder.RegisterEntryPoint<MainPageWithUI>().AsSelf();
        }
    }
}