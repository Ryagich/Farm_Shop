using System;
using System.Collections.Generic;
using Localization;
using UI.Hover.PopupLogics.Holders;
using UnityEngine;
using UnityEngine.Localization;
using Object = UnityEngine.Object;

namespace UI.Hover.PopupLogics.Popups
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class OnlyTitlePopup : IObjectPopup
    {
        public event Action CloseButton;
        public event Action Clicked;
        
        public RectTransform Root { get; private set; }
        public List<RectTransform> Children { get; private set; } = new();

        private readonly LocalizedString localizedString;
        private readonly PopupHolders popupHolders;

        public OnlyTitlePopup
            (
                PopupHolders popupHolders,
                LocalizedString localizedString
            )
        {
            this.localizedString = localizedString;
            this.popupHolders = popupHolders;
        }
        
        public IObjectPopup DrawPopup(Canvas canvas)
        {
            var popup = Object.Instantiate(popupHolders.OnlyTitleHolder, canvas.transform);
            Root = popup.GetComponent<RectTransform>();

            popup.Text.text = localizedString.GetLocalizedStringCached();
                
            return this;
        }

        public void ClickOnObject()
        {
            Debug.Log($"Click On UI Obj");
            Clicked?.Invoke();
        }
        
        public void Redraw() { }

        public void Subscribe() { }
    }
}