using TMPro;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace UI
{
    public class CanvasLifetimeScope : LifetimeScope
    {
        [field: SerializeField] public TMP_Text Finance { get; private set; } = null!;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterInstance(Finance).Keyed("Finance"); 

            builder.RegisterEntryPoint<FinanceDrawer>().AsSelf();
        }
    }
}