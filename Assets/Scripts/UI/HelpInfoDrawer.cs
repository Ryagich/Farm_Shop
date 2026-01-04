using Input;
using Localization;
using TMPro;
using UI.Configs;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace UI
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class HelpInfoDrawer
    {
        private readonly InputConfig inputConfig;
        private readonly SpritesConfig spritesConfig;
        private readonly HelpInfoConfig helpInfoConfig;
        private readonly LocalizationConfig localizationConfig;

        public HelpInfoDrawer
            (
                InputConfig inputConfig,
                SpritesConfig spritesConfig,
                HelpInfoConfig helpInfoConfig,
                LocalizationConfig localizationConfig
            )
        {
            this.inputConfig = inputConfig;
            this.spritesConfig = spritesConfig;
            this.helpInfoConfig = helpInfoConfig;
            this.localizationConfig = localizationConfig;
        }

        public RectTransform DrawMouseHelpForInventoryPage
            (
                RectTransform parentRect
            )
        {
            var RCMHelp = Object.Instantiate(helpInfoConfig.RightSimpleHelp, parentRect);
            RCMHelp.anchoredPosition = helpInfoConfig.RightHelpDefaultPosition;
            RCMHelp.GetComponentInChildren<Image>().sprite = spritesConfig.RCMIcon;
            RCMHelp.GetComponentInChildren<TMP_Text>().text = localizationConfig.RKMHelpForChoseUI.GetLocalizedStringCached();
            
            return RCMHelp;
        }
        
        public RectTransform DrawMouseHelpForMainWithUIPage
            (
                RectTransform parentRect
            )
        {
            var RCMHelp = Object.Instantiate(helpInfoConfig.RightSimpleHelp, parentRect);
          
            RCMHelp.anchorMin = new Vector2(1f, 0.0f);
            RCMHelp.anchorMax = new Vector2(1f, 0.0f);
            RCMHelp.anchoredPosition = helpInfoConfig.RightHelpDefaultPosition;
            
            RCMHelp.GetComponentInChildren<Image>().sprite = spritesConfig.RCMIcon;
            RCMHelp.GetComponentInChildren<TMP_Text>().text = localizationConfig.RKMHelpForChoseUI.GetLocalizedStringCached();
            
            return RCMHelp;
        }
        
        public void DrawMouseHelpForRedactorPage
            (
                RectTransform parentRect
            )
        {
            var y = 0.0f;
            var RCMHelp = Object.Instantiate(helpInfoConfig.RightSimpleHelp, parentRect);
            
            RCMHelp.anchorMin = new Vector2(1f, 0.0f);
            RCMHelp.anchorMax = new Vector2(1f, 0.0f);
            RCMHelp.anchoredPosition = helpInfoConfig.RightHelpDefaultPosition + Vector2.up * (y + helpInfoConfig.RightHelpDistance.y);

            RCMHelp.GetComponentInChildren<Image>().sprite = spritesConfig.RCMIcon;
            RCMHelp.GetComponentInChildren<TMP_Text>().text = localizationConfig.CancelBuilding.GetLocalizedStringCached();
            y += RCMHelp.sizeDelta.y;
            
            var LCMHelp = Object.Instantiate(helpInfoConfig.RightSimpleHelp, parentRect);

            LCMHelp.anchorMin = new Vector2(1f, 0.0f);
            LCMHelp.anchorMax = new Vector2(1f, 0.0f);
            LCMHelp.anchoredPosition = helpInfoConfig.RightHelpDefaultPosition + Vector2.up * (y + helpInfoConfig.RightHelpDistance.y);

            LCMHelp.GetComponentInChildren<Image>().sprite = spritesConfig.LCMIcon;
            LCMHelp.GetComponentInChildren<TMP_Text>().text = localizationConfig.HelpForPlaceBuilding.GetLocalizedStringCached();
            y += LCMHelp.sizeDelta.y;
           
            var RotateRightHelp = Object.Instantiate(helpInfoConfig.RightSimpleHelp, parentRect);
           
            RotateRightHelp.anchorMin = new Vector2(1f, 0.0f);
            RotateRightHelp.anchorMax = new Vector2(1f, 0.0f);
            RotateRightHelp.anchoredPosition = helpInfoConfig.RightHelpDefaultPosition + Vector2.up * (y + helpInfoConfig.RightHelpDistance.y);
            
            RotateRightHelp.GetComponentInChildren<Image>().sprite = spritesConfig.RotateRight;
            RotateRightHelp.GetComponentInChildren<TMP_Text>().text = $"{localizationConfig.RotateRight.GetLocalizedStringCached()} [{BuildingNames.GetEnglishKey(inputConfig.RightRotate)}] -";
            y += RotateRightHelp.sizeDelta.y;

            var RotateLeftHelp = Object.Instantiate(helpInfoConfig.RightSimpleHelp, parentRect);
           
            RotateLeftHelp.anchorMin = new Vector2(1f, 0.0f);
            RotateLeftHelp.anchorMax = new Vector2(1f, 0.0f);
            RotateLeftHelp.anchoredPosition = helpInfoConfig.RightHelpDefaultPosition + Vector2.up * (y + helpInfoConfig.RightHelpDistance.y);
            
            RotateLeftHelp.GetComponentInChildren<Image>().sprite = spritesConfig.RotateLeft;
            RotateLeftHelp.GetComponentInChildren<TMP_Text>().text = $"{localizationConfig.RotateLeft.GetLocalizedStringCached()} [{BuildingNames.GetEnglishKey(inputConfig.LeftRotate)}] -";
        }
    }
}