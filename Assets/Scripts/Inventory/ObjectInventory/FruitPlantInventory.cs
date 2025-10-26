using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Landings.Plants;
using VContainer;

namespace Inventory.ObjectInventory
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class FruitPlantInventory
    {
        private readonly List<Fruit> Fruits = new();

        private readonly IObjectResolver resolver;
      
        [SuppressMessage("ReSharper", "ParameterHidesMember")]
        public FruitPlantInventory(IObjectResolver resolver)
        {
            this.resolver = resolver;
        }
        
        public void Add(Fruit fruit)
        {
            Fruits.Add(fruit);
        }

        public bool CanGet() => Fruits.Count > 0;
        
        public Fruit Get()
        {
            var fruit = Fruits.First();
            Fruits.Remove(fruit);
            return fruit;
        }

        public int GetCount() => Fruits.Count;
    }
}