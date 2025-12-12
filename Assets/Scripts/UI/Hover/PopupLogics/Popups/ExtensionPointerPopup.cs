using System;
using BuildingsAndGrid.Extension;
using UI.Hover.PopupLogics.Holders;
using UnityEngine;
using Object = UnityEngine.Object;

namespace UI.Hover.PopupLogics.Popups
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class ExtensionPointerPopup : IObjectPopup
    {
        public event Action CloseButton;

        private readonly PopupHolders popupHolders;
        private readonly ExtensionPointer extensionPointer;
        private readonly Canvas canvas;

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
            popup.Size.text = $"{extensionPointer.Tiles.Count} Tiles";
            popup.Price.text = $"{extensionPointer.Price}$";
            return popup.GetComponent<RectTransform>();
        }
    }
}