using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using DG.Tweening;
using Unity.VisualScripting;
using TMPro;


namespace UIScreens
{
    public enum ScreenType
    {
        Main,
        Info,
        Shop
    }

    public class ScreenBase : MonoBehaviour
    {
        public ScreenType screenType;
        public List<Transform> objects;
        public List<Typer> texts;
        public bool startHidden = false;

        [Header("Animation")]
        public float animationDuration = 0.3f;
        public float delayBetweenObjects = 0.3f;

        void Start()
        {
            if (startHidden)
                HideObjects();
        }

        [Button]
        public virtual void Show()
        {
            ShowObjects();
        }

        [Button]
        public virtual void Hide()
        {
            HideObjects();
        }

        private void ShowObjects()
        {
            for (int i = 0; i < objects.Count; i++)
            {
                var obj = objects[i];

                obj.gameObject.SetActive(true);
                obj.DOScale(0, animationDuration).From().SetDelay(i * delayBetweenObjects);
            }

            Invoke(nameof(StartTypes), delayBetweenObjects * objects.Count);
        }

        private void StartTypes()
        {
            for (int i = 0; i < texts.Count; i++)
            {
                texts[i].StartTyping();
            }
        }
        
        private void ForceShowObjects()
        {
            foreach (var obj in objects)
            {
                obj.gameObject.SetActive(true);
            }
        }
        private void HideObjects()
        {
            objects.ForEach(obj => obj.gameObject.SetActive(false));
        }

    }
    
}

