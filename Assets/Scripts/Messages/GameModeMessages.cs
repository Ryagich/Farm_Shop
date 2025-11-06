namespace Messages
{
    public readonly struct GameModeChangedMessage
    {
        public readonly GameModes.GameModes GameMode;

        public GameModeChangedMessage(GameModes.GameModes gameMode)
        {
            GameMode = gameMode;
        }
    }
    
    public readonly struct GameModeChangedRequest
    {
        public readonly GameModes.GameModes GameMode;

        public GameModeChangedRequest(GameModes.GameModes gameMode)
        {
            GameMode = gameMode;
        }
    }
}