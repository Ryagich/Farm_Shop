using System;
using System.Linq;
using BuildingsAndGrid.Buildings;
using VContainer.Unity;
using YG;

namespace Shelf
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class ShelfDisposable : IStartable, IDisposable
    {
        private readonly Building building;
        
        public ShelfDisposable(Building building)
        {
            this.building = building;
        }
        
        public void Dispose()
        {
            var save = YG2.saves.ShelvesSave.FirstOrDefault(s => s.Cell.Equals(building.Cell) 
                                                              && s.Id.Equals(building.BuildingConfig.Id));
            if (save != null)
            {
                YG2.saves.ShelvesSave.Remove(save);
            }
        }

        public void Start() { }
    }
}