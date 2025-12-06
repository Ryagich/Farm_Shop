using System.Collections.Generic;
using Messages;

namespace Checkout
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class CheckoutsController
    {
        public List<CheckoutController> Checkouts = new();

        public void RegisterCheckout(NewCheckoutCreatedMessage msg)
        {
            Checkouts.Add(msg.CheckoutController);
        }

        public void UnregisterCheckout(CheckoutDeletedMessage msg)
        {
            Checkouts.Remove(msg.CheckoutController);
        }

        public int GetMaxPositionCount()
        {
            var r = 0;
            foreach (var checkout in Checkouts)
            {
                r += checkout.ByersQueue.queuePoints.Count;
            }
            return r;
        }
    }
}