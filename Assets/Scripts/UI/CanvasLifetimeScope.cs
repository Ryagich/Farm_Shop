using UI.Hover.PopupLogics;
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
            
            builder.Register<MainPage>(Lifetime.Singleton);
            builder.Register<ShopPage>(Lifetime.Singleton);
            builder.Register<RedactorPage>(Lifetime.Singleton);
            builder.Register<UIUtils>(Lifetime.Singleton);
            builder.Register<HelpInfoDrawer>(Lifetime.Singleton);
            
            builder.RegisterEntryPoint<PagesController>();
            builder.RegisterEntryPoint<InventoryPage>().AsSelf();
            builder.RegisterEntryPoint<MainPageWithUI>().AsSelf();
            builder.RegisterEntryPoint<DialoguePage>().AsSelf();
            builder.RegisterEntryPoint<ObjectInfoPopupsController>().AsSelf();
            builder.RegisterEntryPoint<InteractableInterface>().AsSelf();
        }
    }
}