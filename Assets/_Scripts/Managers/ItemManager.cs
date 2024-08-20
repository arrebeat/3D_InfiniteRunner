using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Image = UnityEngine.UI.Image;

public class ItemManager : MonoBehaviour
{
    public SOUIManager soUIManager;

    public TextMeshProUGUI textOnionAmount;
    public Image imageKey;
    public bool reset;

    public static ItemManager instance;
    
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
            Destroy(this.gameObject); 
    }

    void Start()
    {
        if (reset)
            Reset();

        imageKey.enabled = false;
    }

    void Update()
    {
        UpdateTextOnions();
    }

    public void Reset()
    {
        soUIManager.onionsCollected = 0;
        soUIManager.onionsConsumed = 0;
    }

    public void UpdateTextOnions()
    {
        textOnionAmount.text = soUIManager.onionsCollected.ToString();
    }

    public void CollectOnion()
    {
        soUIManager.onionsCollected += 1;
        textOnionAmount.text = soUIManager.onionsCollected.ToString();
    }

    public void ConsumeOnion()
    {
        soUIManager.onionsConsumed += 1;
    }

    public void CollectKey()
    {
        imageKey.enabled = true;
    }

}
