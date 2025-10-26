using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using NaughtyAttributes;
using VContainer;
using VContainer.Unity;

namespace Buyer
{
    public class BuyerCreator : MonoBehaviour
    {
        private List<Transform> spawnPoints = new();
        private BuyerLifetimeScope buyerPrefab;
        private IObjectResolver resolver;
        
        [SuppressMessage("ReSharper", "ParameterHidesMember")]
        [Inject]
        private void Construct(List<Transform> spawnPoints,
                               BuyerLifetimeScope buyerPrefab,
                               IObjectResolver resolver)
        {
            this.spawnPoints = spawnPoints;
            this.buyerPrefab = buyerPrefab;
            this.resolver = resolver;
        }
        
        [Button]
        private void InstantiateBuyer()
        {
            var spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Count - 1)];
            resolver.Instantiate(buyerPrefab, spawnPoint.position, spawnPoint.rotation);
        }
    }
}