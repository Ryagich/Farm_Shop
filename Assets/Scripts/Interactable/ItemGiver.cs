using Inventory;
using Inventory.Item;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Interactable
{
    public class ItemGiver : MonoBehaviour
    {
        [SerializeField] private ItemConfig itemConfig;

        private void Awake()
        {
            var interactable = GetComponent<Interactable>();
            interactable.Interacted += Interact;
        }

        private void Interact(LifetimeScope scope)
        {
            var inventory = scope.Container.Resolve<IInventory>();
            inventory.Add(itemConfig, transform.localToWorldMatrix);
        }
    }
}