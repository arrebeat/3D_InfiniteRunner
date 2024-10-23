using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UIScreens
{
    public class ScreenManager : MonoBehaviour
    {
        public List<ScreenBase> screenBases;

        public ScreenType startScreenType = ScreenType.Main;


        private ScreenBase _currentScreen; 

        void Start()
        {
            HideAll();
            ShowByType(startScreenType);
        }

        public void ShowByType(ScreenType type)
        {
            if (_currentScreen != null) _currentScreen.Hide();
            
            var nextScreen = screenBases.Find(x => x.screenType == type);

            nextScreen.Show();
            _currentScreen = nextScreen;
        }

        public void HideAll()
        {
            screenBases.ForEach(i => i.Hide());
        }
    }
}