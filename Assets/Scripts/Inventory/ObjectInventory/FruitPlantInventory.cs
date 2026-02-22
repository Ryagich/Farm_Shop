using System.Linq;
using Landings.Plants;
using UniRx;

namespace Inventory.ObjectInventory
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class FruitPlantInventory
    {
        public readonly ReactiveCollection<Fruit> Fruits = new();

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