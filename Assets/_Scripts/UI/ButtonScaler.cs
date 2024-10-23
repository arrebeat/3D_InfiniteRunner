using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

public class ButtonScaler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public float enterScale = 1.2f;
    public float enterScaleDuration = 0.1f;
    public float exitScaleDuration = 0.2f;
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.DOScale(enterScale, enterScaleDuration);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        transform.DOScale(1, exitScaleDuration);
    }

    public void OnClick()
    {
        transform.DOScale(1, exitScaleDuration);
        transform.DOScale(enterScale, enterScaleDuration);
    }
}
