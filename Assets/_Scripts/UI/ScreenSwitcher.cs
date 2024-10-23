using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

namespace UIScreens
{
    public class ScreenSwitcher : MonoBehaviour
    {
        public ScreenManager screenManager { get; private set; }
        public ScreenType screenType;

        void Awake()
        {
            GameObject screenManagerObject = GameObject.Find("ScreenManager");
            screenManager = screenManagerObject.GetComponent<ScreenManager>();
        }

        public void ScreenSwitch()
        {
            screenManager.ShowByType(screenType);
        }        
    }
}
