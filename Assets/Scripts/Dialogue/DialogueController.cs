using CameraScripts;
using GameModes;
using MessagePipe;
using Messages;
using VContainer.Unity;

namespace Dialogue
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class DialogueController : IStartable
    {
        private readonly CameraMotor cameraMotor;

        public DialogueController
            (
                CameraMotor cameraMotor,
                ISubscriber<GameModeChangedMessage> openPageRequestSubscriber,
                ISubscriber<ChangeGameModeToDialogueRequest> changeGameModeToDialogueSubscriber
            )
        {
            this.cameraMotor = cameraMotor;
            
            openPageRequestSubscriber.Subscribe(ChangeCameraState);
            changeGameModeToDialogueSubscriber.Subscribe(SetNewTarget);
        }

        private void SetNewTarget(ChangeGameModeToDialogueRequest msg)
        {
            cameraMotor.ChangeDialogueTarget(msg.CameraPoint);
        }
        
        private void ChangeCameraState(GameModeChangedMessage msg)
        {
            if (msg.GameMode is GameMode.Dialogue)
            {
                cameraMotor.ChangeCameraMode(CameraModes.Dialogue);
            }
            else
            {
                cameraMotor.ChangeCameraMode(CameraModes.Gameplay);
            }
        }

        public void Start() { }
    }
}