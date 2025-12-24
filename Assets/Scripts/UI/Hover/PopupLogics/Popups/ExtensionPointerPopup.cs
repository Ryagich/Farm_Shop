using System;
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

        private readonly PopupHolders popupHolders;
        private readonly ExtensionPointer extensionPointer;
        private readonly Canvas canvas;

        private CompositeDisposable disposables = new();
        private RectTransform popupRect;
        
        public ExtensionPointerPopup
            (
                PopupHolders popupHolders,
                ExtensionPointer extensionPointer,
                Canvas canvas
            )
        {
            this.popupHolders = popupHolders;
            this.extensionPointer = extensionPointer;
            this.canvas = canvas;
        }
            
        public RectTransform DrawPopup()
        {
            var popup = Object.Instantiate(popupHolders.ExtensionPointerPopupHolder, canvas.transform);
            popupRect = popup.GetComponent<RectTransform>();
            disposables = new CompositeDisposable();

            popup.Size.text = $"{extensionPointer.Tiles.Count} Tiles";
            popup.Price.text = $"{extensionPointer.Price}$";
            
            return popupRect;
        }

        public void Redraw() { }
        public void Subscribe() { }

        public void Dispose()
        {
            disposables.Dispose();
            if (popupRect)
                Object.Destroy(popupRect.gameObject);
        }
    }
}