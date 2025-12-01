using VContainer.Unity;

namespace BuildingsAndGrid.Environment
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class GridEnvironment : IStartable
    {
        private readonly TilesController tilesController;

        public GridEnvironment
            (
                TilesController tilesController
            )
        {
            this.tilesController = tilesController;
        }
        
        public void Start() { }

        private void CreateRightEnvironment()
        {
            
        }
    }
}