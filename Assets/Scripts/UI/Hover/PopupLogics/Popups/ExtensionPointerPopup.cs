using System;
using System.Collections.Generic;
using BuildingsAndGrid.Extension;
using UI.Hover.PopupLogics.Holders;
using UniRx;
using UnityEngine;
using Object = UnityEngine.Object;

namespace UI.Hover.PopupLogics.Popups
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class ExtensionPointerPopup : IObjectPopup, IDisposable
    {
        public event Action CloseButton;
        public RectTransform Root { get; private set; }
        public List<RectTransform> Children { get; private set; } = new();

        private readonly PopupHolders popupHolders;
        private readonly ExtensionPointer extensionPointer;

        private CompositeDisposable disposables = new();
        
        public ExtensionPointerPopup
            (
                PopupHolders popupHolders,
                ExtensionPointer extensionPointer
            )
        {
            this.popupHolders = popupHolders;
            this.extensionPointer = extensionPointer;
        }
            
        public IObjectPopup DrawPopup(Canvas canvas)
        {
            var popup = Object.Instantiate(popupHolders.ExtensionPointerPopupHolder, canvas.transform);
            Root = popup.GetComponent<RectTransform>();
            disposables = new CompositeDisposable();

            popup.Size.text = $"{extensionPointer.Tiles.Count} Tiles";
            popup.Price.text = $"{extensionPointer.Price}$";
            
            return this;
        }

        public void Redraw() { }
        public void Subscribe() { }

        public void Dispose()
        {
            disposables.Dispose();
            if (Root)
                Object.Destroy(Root.gameObject);
        }
    }
}