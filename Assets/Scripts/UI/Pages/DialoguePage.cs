using MessagePipe;
using Messages;
using Patch;
using UI.Configs;
using UI.Hover.PopupLogics;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace UI.Pages
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class DialoguePage : BasePage, IFixedTickable
    {
        public override PageType Type { get; } = PageType.Dialogue;

        private readonly UIConfig uiConfig;
        private readonly Patches patches;
        private readonly ObjectInfoPopupsController objectInfoPopupsController;
        private readonly IObjectResolver resolver;
        private readonly UIUtils uiUtils;
        
        private readonly RectTransform canvasRect;
        private RectTransform contentRect;

        private RectTransform helpRect;
        private int index;
        
        private bool needDrawPatchNote;
        
        public DialoguePage
            (
                UIConfig uiConfig,
                Patches patches,
                Canvas canvas,
                ObjectInfoPopupsController objectInfoPopupsController,
                IObjectResolver resolver,
                UIUtils uiUtils,
                ISubscriber<OpenPatchNote> openPatchNoteSubscriber
            )
        {
            this.resolver = resolver;
            this.uiUtils = uiUtils;
            this.uiConfig = uiConfig;
            this.patches = patches;
            this.objectInfoPopupsController = objectInfoPopupsController;

            canvasRect = canvas.GetComponent<RectTransform>();

            openPatchNoteSubscriber.Subscribe(OpenPatchNote);
        }
        
        public override void Draw()
        {
            contentRect = resolver.Instantiate(uiConfig.ContentPref, canvasRect);
            contentRect.name = $"{uiConfig.ContentPref.name} | {Type}";
            uiUtils.DrawFinanceDrawer(contentRect);

            if (needDrawPatchNote)
            {
                var patchInfo = patches.PatchInfos[index];
                var patchNote = resolver.Instantiate(patchInfo.PatchScroll, contentRect);
                var buttonLeft = resolver.Instantiate(uiConfig.ButtonArrowLeft, patchNote);
                var buttonRight = resolver.Instantiate(uiConfig.ButtonArrowRight, patchNote);
                
                buttonLeft.onClick.AddListener(SwitchLeft);
                buttonRight.onClick.AddListener(SwitchRight);
            }
        }

        private void SwitchRight()
        {
            Debug.Log($"SwitchRight");
            index++;
            if (index >= patches.PatchInfos.Count)
            {
                index = 0;
            }
            ReDrawWithNote();
        }
        
        private void SwitchLeft()
        {
            Debug.Log($"SwitchLeft");
            index--;
            if (index <= 0)
            {
                index = patches.PatchInfos.Count-1;
            }
            ReDrawWithNote();
        }
        
        private void OpenPatchNote(OpenPatchNote msg)
        {
            Debug.Log($"OpenPatchNote");
            needDrawPatchNote = true;
            ReDrawWithNote();
        }
        
        private void ReDraw()
        {
            Hide();
            Draw();
        }
        
        private void ReDrawWithNote()
        {
            HideWithNote();
            Draw();
        }
        
        private void HideWithNote()
        {
            if (contentRect)
            {
                Object.Destroy(contentRect.gameObject);
            }
        }
        
        public override void Hide()
        {
            if (contentRect)
            {
                Object.Destroy(contentRect.gameObject);
            }
            needDrawPatchNote = false;
        }

        public void FixedTick() { }
    }
}