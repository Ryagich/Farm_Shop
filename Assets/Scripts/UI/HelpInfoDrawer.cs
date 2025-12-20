using Localization;
using TMPro;
using UI.Configs;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class HelpInfoDrawer
    {
        private readonly UIConfig uiConfig;
        private readonly SpritesConfig spritesConfig;
        private readonly HelpInfoConfig helpInfoConfig;
        private readonly LocalizationConfig localizationConfig;

        public HelpInfoDrawer
            (
                UIConfig uiConfig,
                SpritesConfig spritesConfig,
                HelpInfoConfig helpInfoConfig,
                LocalizationConfig localizationConfig
            )
        {
            this.uiConfig = uiConfig;
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
            RCMHelp.GetComponentInChildren<TMP_Text>().text = localizationConfig.RKMHelpForChoseUI.GetLocalizedString();
            
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
            RCMHelp.GetComponentInChildren<TMP_Text>().text = localizationConfig.RKMHelpForChoseUI.GetLocalizedString();
            
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
            RCMHelp.GetComponentInChildren<TMP_Text>().text = localizationConfig.CancelBuilding.GetLocalizedString();
            y += RCMHelp.sizeDelta.y;
            
            var LCMHelp = Object.Instantiate(helpInfoConfig.RightSimpleHelp, parentRect);

            LCMHelp.anchorMin = new Vector2(1f, 0.0f);
            LCMHelp.anchorMax = new Vector2(1f, 0.0f);
            
            LCMHelp.anchoredPosition = helpInfoConfig.RightHelpDefaultPosition + Vector2.up * (y + helpInfoConfig.RightHelpDistance.y);
            LCMHelp.GetComponentInChildren<Image>().sprite = spritesConfig.LCMIcon;
            LCMHelp.GetComponentInChildren<TMP_Text>().text = localizationConfig.HelpForPlaceBuilding.GetLocalizedString();
        }
    }
}