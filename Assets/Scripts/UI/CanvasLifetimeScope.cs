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
            
            builder.Register<MainPage>(Lifetime.Scoped).As<MainPage>();
            builder.Register<MainPageWithUI>(Lifetime.Scoped).As<MainPageWithUI>();
            builder.Register<ShopPage>(Lifetime.Scoped).As<ShopPage>();
            
            builder.RegisterEntryPoint<PagesController>().AsSelf();
        }
    }
}