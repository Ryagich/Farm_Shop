using BuildingsAndGrid.Buildings;
using BuildingsAndGrid.Environment;
using BuildingsAndGrid.Extension;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace BuildingsAndGrid
{
    public class GridLifetimeScope : LifetimeScope
    {
        [field: SerializeField] private MeshFilter meshFilter;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterInstance(meshFilter).AsSelf();
            builder.RegisterInstance(transform)
                   .As<Transform>()
                   .Keyed("GridRoot"); // <<< добавили
            
            builder.Register<GridRaycaster>(Lifetime.Scoped);
            builder.Register<TilesController>(Lifetime.Scoped);
            builder.Register<GridExtensionSpawner>(Lifetime.Scoped);

            builder.RegisterEntryPoint<VisualGridSeparation>().AsSelf();
            builder.RegisterEntryPoint<VisualFloor>().AsSelf();
            builder.RegisterEntryPoint<GridColliderBuilder>().AsSelf();
            builder.RegisterEntryPoint<BuildingMover>().AsSelf();
            builder.RegisterEntryPoint<ExtensionFounder>().AsSelf();
            builder.RegisterEntryPoint<GridWallsCreator>().AsSelf();
        }
    }
}