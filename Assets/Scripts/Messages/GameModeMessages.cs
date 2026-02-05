using GameModes;

namespace Messages
{
    public readonly struct GameModeChangedMessage
    {
        public readonly GameMode GameMode;
        public readonly Area Area;

        public GameModeChangedMessage(GameMode gameMode, Area area = Area.None)
        {
            GameMode = gameMode;
            Area = area;
        }
    }
    
    public readonly struct GridExtendMessage { }
}