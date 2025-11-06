using UnityEngine;

namespace Messages
{
    public readonly struct PlayerMoveMessage
    {
        public readonly Vector2 Direction;

        public PlayerMoveMessage(Vector2 direction)
        {
            Direction = direction;
        }
    }
    
    public readonly struct OpenGameModeMessage { }
    public readonly struct ChangeCursorStateMessage { }
    public readonly struct OpenRedactorModeMessage { }
    public readonly struct OpenShopModeMessage { }
}