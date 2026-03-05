using GameModes;
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
    
    public readonly struct ChangeCursorStateMessage { }
    public readonly struct ClickMessage { }
    public readonly struct RightClickMessage { }
    public readonly struct LeftRotateMessage { }
    public readonly struct RightRotateMessage { }
    public readonly struct InteractableInputMessage { }

    public readonly struct ChangeGameModeRequest
    {
        public readonly GameMode Mode;

        public ChangeGameModeRequest(GameMode mode)
        {
            Mode = mode;
        }
    }
    
    public readonly struct ChangeGameModeToDialogueRequest
    {
        public readonly Transform CameraPoint;

        public ChangeGameModeToDialogueRequest(Transform cameraPoint)
        {
            CameraPoint = cameraPoint;
        }
    }
    
    public readonly struct OpenShopWithAreaRequest
    {
        public readonly Area Area;

        public OpenShopWithAreaRequest(Area area)
        {
            Area = area;
        }
    }
}
