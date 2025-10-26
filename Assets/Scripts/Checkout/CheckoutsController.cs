using System.Collections.Generic;
using Messages;

namespace Checkout
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class CheckoutsController
    {
        public List<CheckoutController> Checkouts = new();

        public void OnNewShelfCreated(NewCheckoutCreatedMessage msg)
        {
            Checkouts.Add(msg.CheckoutController);
        }
    }
}