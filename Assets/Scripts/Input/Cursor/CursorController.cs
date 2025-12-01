using GameModes;
using MessagePipe;
using Messages;

namespace Input.Cursor
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class CursorController
    {
        private readonly CursorHandler cursorHandler;
        public bool IsVisibleInPlayMode;

        public CursorController
            (
                CursorHandler cursorHandler,
                ISubscriber<ChangeCursorStateMessage> changeCursorSubscriber,
                ISubscriber<GameModeChangedMessage> gameModeChangeSubscriber
            )
        {
            this.cursorHandler = cursorHandler;

            changeCursorSubscriber.Subscribe(OnChangeCursor);
            gameModeChangeSubscriber.Subscribe(OnChangeGameMode);
        }

        private void OnChangeGameMode(GameModeChangedMessage msg)
        {
            switch (msg.GameMode)
            {
                case GameMode.Game:
                    cursorHandler.SetCursorState(IsVisibleInPlayMode);
                    break;
                case GameMode.Inventory or GameMode.Shop or GameMode.Redactor:
                    cursorHandler.ShowCursor();
                    break;
            }
        }
        
        private void OnChangeCursor(ChangeCursorStateMessage msg)
        {
            cursorHandler.ChangeCursorState();
            IsVisibleInPlayMode = cursorHandler.IsVisible;
        }
    }
}   