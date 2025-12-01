using GameModes;

namespace UI.Pages
{
    public abstract class BasePage
    {
        public abstract PageType Type { get; }

        public abstract void Draw();

        public abstract void Hide();
    }

    public interface AreaDrawer
    {
        public  Area CurrentArea { get; }
        public void SetArea(Area area);
    }
}