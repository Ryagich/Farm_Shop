using CameraScripts;
using Interactable;
using Inventory;
using Inventory.Movers;
using Inventory.ObjectInventory;
using Inventory.ObjectInventory.Sounds;
using Movement;
using Sounds;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Container
{
    public class PlayerLifetimeScope : LifetimeScope
    {
        [SerializeField] private StepSoundConfig stepSoundConfig;
        
        protected override void Configure(IContainerBuilder builder)
        {
            var hand = transform.Find("Hand");
            var particle = transform.Find("P").GetComponent<ParticleSystem>();
            var animator = transform.GetComponent<Animator>();

            builder.RegisterComponentInHierarchy<CharacterController>().AsSelf();
            builder.RegisterInstance(hand).As<Transform>().Keyed("Hand"); 
            builder.RegisterInstance(transform);
            builder.RegisterInstance(transform).Keyed("TransformForSound"); 
            builder.RegisterInstance(particle);
            builder.RegisterInstance(animator);
            
            builder.RegisterInstance(stepSoundConfig);

            var founder = gameObject.AddComponent<InteractableFounder>();
            builder.RegisterComponent(founder).AsSelf();
            
            builder.Register<InventoryPlayer>(Lifetime.Scoped)
                   .As<IInventory>() 
                   .AsSelf();        
            //builder.Register<PlayerMovement>(Lifetime.Scoped);
            builder.Register<PlayerParticleController>(Lifetime.Scoped);
            builder.Register<PlayerAnimationController>(Lifetime.Scoped);

            builder.RegisterEntryPoint<CameraMovement>().AsSelf();
            builder.RegisterEntryPoint<InventoryPlayer>().AsSelf();
            builder.RegisterEntryPoint<InventoryPlayerItemMover>().AsSelf();
            builder.RegisterEntryPoint<PlayerInteractableLogic>().AsSelf();
            
            builder.RegisterEntryPoint<PlayerGravity>().AsSelf();
            builder.RegisterEntryPoint<PlayerMovementController>().AsSelf();
            builder.RegisterEntryPoint<PlayerMovement>().AsSelf();
            builder.RegisterEntryPoint<VirtualPlayerMovement>().AsSelf();
            
            builder.RegisterEntryPoint<StepSound>().AsSelf();
            builder.RegisterEntryPoint<InventorySoundForRemoving>(Lifetime.Scoped).AsSelf();
        }
    }
}