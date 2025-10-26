using MessagePipe;
using Messages;
using UniRx;
using UnityEngine;
using VContainer.Unity;

namespace Buyer
{
    //TODO: Пока просто удаляю - переделать на переиспользование
    // ReSharper disable once ClassNeverInstantiated.Global
    public class BuyersReleaser : IStartable
    {
        private readonly CompositeDisposable disposables = new();

        public BuyersReleaser(ISubscriber<BuyerIsOverMessage> BuyerIsOverSubscriber) 
        {
            BuyerIsOverSubscriber.Subscribe(OnBuyerIsOver).AddTo(disposables);
        }

        private void OnBuyerIsOver(BuyerIsOverMessage msg)
        {
            Object.Destroy(msg.BuyerLifetimeScope.gameObject);
        }

        public void Start() { }
    }
}