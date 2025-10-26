using MessagePipe;
using Messages;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using DG.Tweening;

namespace Products
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class ProductCreator : IFixedTickable
    {
        public bool IsWorking { get; private set; }

        private float t;

        private readonly ProductConfig productConfig;
        private readonly IPublisher<ProductCreated> publisher;
        private readonly Transform target;

        public ProductCreator
            (
                ProductConfig productConfig,
                IPublisher<ProductCreated> publisher,
                [Key("CreatorTransform")] Transform target
            )
        {
            this.productConfig = productConfig;
            this.publisher = publisher;
            this.target = target;
        }
        
        public void FixedTick()
        {
            if (!IsWorking)
                return;
            t += Time.fixedDeltaTime;
            if (t >= productConfig.Time)
            {
                t = 0;
                IsWorking = false;
                publisher.Publish(new ProductCreated(productConfig.ItemConfig, target.localToWorldMatrix));
            }
        }

        public void StartWork()
        {
            var targetScale = target.localScale;
            target.localScale = targetScale * productConfig.CreatorAnimationConfig.ScaleFactor;
            target.DOScale(targetScale, productConfig.CreatorAnimationConfig.AnimationTime)
                  .SetEase(productConfig.CreatorAnimationConfig.Ease, 
                           productConfig.CreatorAnimationConfig.Overshoot);
            t = 0;
            IsWorking = true;
        }
    }
}