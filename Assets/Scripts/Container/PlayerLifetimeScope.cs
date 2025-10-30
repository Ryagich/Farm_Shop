using CameraScripts;
using Interactable;
using Inventory;
using Inventory.Movers;
using Movement;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Container
{
    public class PlayerLifetimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            var hand = transform.Find("Hand");
            var particle = transform.Find("P").GetComponent<ParticleSystem>();
            var animator = transform.GetComponent<Animator>();

            builder.RegisterComponentInHierarchy<CharacterController>().AsSelf();
            builder.RegisterInstance(hand).As<Transform>().Keyed("Hand"); 
            builder.RegisterInstance(transform);
            builder.RegisterInstance(particle);
            builder.RegisterInstance(animator);
            
            var founder = gameObject.AddComponent<InteractableFounder>();
            builder.RegisterComponent(founder).AsSelf();
            
            builder.Register<InventoryPlayer>(Lifetime.Scoped)
                   .As<IInventory>() 
                   .AsSelf();        

            builder.UseEntryPoints(ep =>
                                   {
                                       ep.Add<PlayerMovement>().AsSelf();
                                       ep.Add<PlayerGravity>().AsSelf();
                                       ep.Add<CameraMovement>().AsSelf();
                                       ep.Add<InventoryPlayer>().AsSelf();
                                       ep.Add<InventoryPlayerItemMover>().AsSelf();
                                       ep.Add<PlayerInteractableLogic>().AsSelf();
                                       ep.Add<PlayerParticleController>().AsSelf();
                                       ep.Add<PlayerAnimationController>().AsSelf();
                                   });
        }
    }
}