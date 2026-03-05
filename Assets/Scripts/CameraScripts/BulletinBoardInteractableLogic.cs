using Localization;
using MessagePipe;
using Messages;
using UI.Hover;
using UI.Hover.PopupLogics.Holders;
using UI.Hover.PopupLogics.Popups;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace CameraScripts
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class BulletinBoardInteractableLogic : IStartable
    {
        private readonly Transform cameraPosition;
        private readonly GameObject interactableZoneGO;
        private readonly GameObject exitUIZone;
        private readonly GameObject patchInfoUIZone;
        private readonly IPublisher<ChangeGameModeToDialogueRequest> changeGameModeToDialoguePublisher;
        private readonly IPublisher<OpenPatchNote> openPatchNotePublisher;

        public BulletinBoardInteractableLogic
            (
                LocalizationConfig localizationConfig,
                PopupHolders popupHolders,
                Interactable.Interactable interactable,
                [Key("CameraPosition")] Transform cameraPosition,
                [Key("UI Zone")] GameObject interactableZoneGO,
                [Key("ExitSign")] HoverTrigger exitHoverTrigger,
                [Key("PatchInfoSign")] HoverTrigger patchInfoHoverTrigger,
                [Key("Exit UI Zone")] GameObject exitUIZone,
                [Key("PatchInfo UI Zone")] GameObject patchInfoUIZone,

                IPublisher<ChangeGameModeToDialogueRequest> changeGameModeToDialoguePublisher,
                IPublisher<OpenPatchNote> openPatchNotePublisher
            )
        {
            this.cameraPosition = cameraPosition;
            this.interactableZoneGO = interactableZoneGO;
            this.exitUIZone = exitUIZone;
            this.patchInfoUIZone = patchInfoUIZone;
            this.changeGameModeToDialoguePublisher = changeGameModeToDialoguePublisher;
            this.openPatchNotePublisher = openPatchNotePublisher;

            var exitPopup = new OnlyTitlePopup(popupHolders, localizationConfig.Close);
            var patchInfoPopup = new OnlyTitlePopup(popupHolders, localizationConfig.PatchInfo);

            exitHoverTrigger.ObjectPopup = exitPopup;
            patchInfoHoverTrigger.ObjectPopup = patchInfoPopup;
            
            interactable.Interacted += Interact;
            interactable.EndInteracted += EndInteract;
            interactable.EndManualInteracted += EndManualInteract;

            exitPopup.Clicked += Close;
            patchInfoPopup.Clicked += OpenPatchNote;
        }

        public void Start()
        {
            exitUIZone.SetActive(false);
            patchInfoUIZone.SetActive(false);
        }

        private void Interact(LifetimeScope scope)
        {
            interactableZoneGO.SetActive(false);
            exitUIZone.SetActive(true);
            patchInfoUIZone.SetActive(true);

            changeGameModeToDialoguePublisher.Publish(new ChangeGameModeToDialogueRequest(cameraPosition));
        }

        private void Close()
        {
            interactableZoneGO.SetActive(true);
            exitUIZone.SetActive(false);
            patchInfoUIZone.SetActive(false);

            changeGameModeToDialoguePublisher.Publish(new ChangeGameModeToDialogueRequest(cameraPosition));
        }

        private void OpenPatchNote()
        {
            openPatchNotePublisher.Publish(new OpenPatchNote());
        }

        private void EndManualInteract(LifetimeScope scope)
        {
            Close();
        }
        
        private void EndInteract(LifetimeScope scope)
        {
            interactableZoneGO.SetActive(true);
            exitUIZone.SetActive(false);
            patchInfoUIZone.SetActive(false);
        }
    }
}