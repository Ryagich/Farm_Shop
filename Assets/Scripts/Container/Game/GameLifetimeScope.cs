using BuildingsAndGrid;
using Buyer;
using Checkout;
using Doors;
using GameModes;
using Input;
using Input.Cursor;
using Inventory.Finance;
using MessagePipe;
using Messages;
using Objects;
using Shelf;
using Sounds;
using Storage;
using UI.Hover;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Container.Game
    {
        public class GameLifetimeScope : LifetimeScope
        {
            [field: SerializeField] public PlayerLifetimeScope PlayerPrefab { get; private set; } = null!;
          
            private PlayerLifetimeScope playerScope;

            protected override void Configure(IContainerBuilder builder)
            {
                builder.RegisterInstance(Camera.main).AsSelf();

                // Core
                builder.Register<ShelvesController>(Lifetime.Singleton).AsSelf();
                builder.Register<DoorsController>(Lifetime.Singleton).AsSelf();
                builder.Register<CheckoutsController>(Lifetime.Singleton).AsSelf();
                builder.Register<FinanceManager>(Lifetime.Singleton);
                builder.Register<CursorHandler>(Lifetime.Singleton);
                builder.Register<CursorController>(Lifetime.Singleton);
                builder.Register<HoverRaycaster>(Lifetime.Singleton);
               
                builder.Register<TilesController>(Lifetime.Singleton);
                builder.Register<SurfaceController>(Lifetime.Singleton)
                       .AsSelf()
                       .As<IStartable>();
                // === MessagePipe ===
                var options = builder.RegisterMessagePipe();
                builder.RegisterMessageBroker<PlayerMoveMessage>(options);
                builder.RegisterMessageBroker<CreatedNewObjectRequest>(options);
                builder.RegisterMessageBroker<CreatedNewBuildingOnGridRequest>(options);
                builder.RegisterMessageBroker<CreatedNewObjectMessage>(options);
                builder.RegisterMessageBroker<CreatedNewObjectOnGridMessage>(options);
                builder.RegisterMessageBroker<DeleteBuildingOnGridRequest>(options);
                builder.RegisterMessageBroker<DeleteBuildingOnGridMessage>(options);
                builder.RegisterMessageBroker<NewShelfCreatedMessage>(options);
                builder.RegisterMessageBroker<ShelfDeletedMessage>(options);
                builder.RegisterMessageBroker<PlaySoundMessage>(options);
                builder.RegisterMessageBroker<InteractableMessage>(options);
                builder.RegisterMessageBroker<InteractableEndMessage>(options);
                builder.RegisterMessageBroker<ChoseBuildingMessage>(options);
                builder.RegisterMessageBroker<AddBuildingToStorageRequest>(options);
                builder.RegisterMessageBroker<AddItemToStorageRequest>(options);
                builder.RegisterMessageBroker<ChangeGameModeRequest>(options);
                builder.RegisterMessageBroker<GridExtendMessage>(options);
                builder.RegisterMessageBroker<BuyerIsOverMessage>(options);
                
                // === InputHandler ===
                builder.Register<InputHandler>(Lifetime.Singleton).AsSelf().As<IStartable>();


                builder.RegisterBuildCallback(container =>
                                              {
                                                  GlobalMessagePipe.SetProvider(container.AsServiceProvider());
                                                  playerScope = CreateChildFromPrefab(PlayerPrefab, _ => { });
                                                  container.Resolve<SoundsManager>().PlayerTransform = playerScope.transform;
                                              });
                // builder.RegisterEntryPoint<TilesController>(Lifetime.Scoped).AsSelf();
                //Buyers
                builder.Register<ShoppingListGenerator>(Lifetime.Singleton).AsSelf();
                builder.RegisterEntryPoint<SoundsManager>().AsSelf();
                builder.RegisterEntryPoint<BuyerSpawnPoints>().AsSelf();
                builder.RegisterEntryPoint<BuyersSpawner>().AsSelf();

                builder.RegisterEntryPoint<ObjectCreator>().AsSelf();
                builder.RegisterEntryPoint<ObjectMoverInHisPlace>().AsSelf();
                builder.RegisterEntryPoint<GameModesController>().AsSelf();
                
                //Storage
                builder.RegisterEntryPoint<BuildingsStorage>().AsSelf();
                builder.RegisterEntryPoint<ItemsStorage>().AsSelf();
                builder.RegisterEntryPoint<StorageBootstrapper>().AsSelf();
            }
        }
    }