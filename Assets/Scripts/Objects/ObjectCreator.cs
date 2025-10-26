using MessagePipe;
using Messages;
using UniRx;
using VContainer;
using VContainer.Unity;

namespace Objects
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class ObjectCreator : IStartable
    {
        private readonly IObjectResolver resolver;
        private readonly IPublisher<CreatedNewObjectMessage> createdNewObjectPublisher;
        private readonly CompositeDisposable disposables = new();

        public ObjectCreator
            (
                ISubscriber<PlayerMadePurchaseMessage> playerMadePurchaseSubscriber,
                IObjectResolver resolver,
                IPublisher<CreatedNewObjectMessage> createdNewObjectPublisher
            )
        {
            this.resolver = resolver;
            this.createdNewObjectPublisher = createdNewObjectPublisher;
            
            playerMadePurchaseSubscriber.Subscribe(CreateObject).AddTo(disposables);
        }
        
        public void Start() { }

        private void CreateObject(PlayerMadePurchaseMessage msg)
        {
            var objScope = resolver.Instantiate(msg.Scope);
            var objTransform = objScope.gameObject.transform;
            createdNewObjectPublisher.Publish(new CreatedNewObjectMessage(objTransform, msg.Position, msg.Rotation));
        }
    }
}