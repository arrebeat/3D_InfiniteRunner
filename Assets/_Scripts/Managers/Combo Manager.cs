using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using MoreMountains.Feedbacks;
using System;
using Unity.VisualScripting;
using MoreMountains.Tools;
using TMPro;
using Unity.Mathematics;
using UnityEngine.UIElements;
public class ComboManager : MonoBehaviour
{

    public MMProgressBar spiritBarL { get; private set; }
    public MMProgressBar spiritBarR { get; private set; }
    public PlayerControllerCowboy player { get; private set; }
    public ScoreManager scoreManager { get; private set; }
    
    public float decreaseFactor;
    public float decreaseWait;
    public float decreaseWaitHigh;
    [Space(5)]
    public float killIncrease;
    public float deadHitIncrease;
    public float parryIncrease;
    public float deflectIncrease;

    [Space(10)]
    public TextMeshPro floatingText;
    public Vector3 floatingTextOffset;
    public float destroyTime;


    [Space(10)]
    public UnityEvent OnBarUpdate;

    private Coroutine _coroutine;


    private void Awake()
    {
        GameObject[] spiritBarObjects = GameObject.FindGameObjectsWithTag("SpiritBar");
        spiritBarL = spiritBarObjects[0].GetComponent<MMProgressBar>();
        spiritBarR = spiritBarObjects[1].GetComponent<MMProgressBar>();

        GameObject canvas = GameObject.Find("Canvas");
        scoreManager = canvas.GetComponent<ScoreManager>();

        GameObject playerObject = GameObject.Find("Cowboy");
        player = playerObject.GetComponent<PlayerControllerCowboy>();
    }

    void Start()
    {
        
    }

    void Update()
    {
        //ConstantDecrease();
    }

    public void HandleBarUpdate()
    {
        if (_coroutine != null)
            StopCoroutine(_coroutine);

        _coroutine = StartCoroutine(ConstantDecrease());
    }

    public IEnumerator ConstantDecrease()
    {
        WaitForSeconds wait = new WaitForSeconds(decreaseWait);
        WaitForSeconds waitHigh = new WaitForSeconds(decreaseWaitHigh);

        if (spiritBarL.BarProgress >= 1)
        {
            //Debug.Log("GUN FIGHT HIGH!!!");
            
            player.GunFightHigh();

            yield return waitHigh;
                        
            player.animator.SetFloat("moveSpeed", player.initialSpeed);
            player.isGunFightHigh = false;
            scoreManager.combo = 0;
            scoreManager.UpdateComboText();
            Debug.Log(scoreManager.combo + "COMBO DROP");

            player.Renderers[4].material = player.materials[0];
            player.Renderers[5].material = player.materials[0];

            player.fullSpiritFillObjectL.SetActive(false);
            player.fullSpiritFillObjectR.SetActive(false);
        }
        else
        {
            player.animator.SetFloat("moveSpeed", player.initialSpeed);
            player.isGunFightHigh = false;
            
            player.Renderers[4].material = player.materials[0];
            player.Renderers[5].material = player.materials[0];
            
            player.fullSpiritFillObjectL.SetActive(false);
            player.fullSpiritFillObjectR.SetActive(false);
            yield return wait;
        }
        
        while (spiritBarL.BarProgress > 0)
        {
            spiritBarL.BarProgress -= Time.deltaTime*decreaseFactor;
            spiritBarL.BarProgress = Mathf.Clamp(spiritBarL.BarProgress, 0, 1);
            spiritBarL.UpdateBar01(spiritBarL.BarProgress);

            spiritBarR.BarProgress -= Time.deltaTime*decreaseFactor;
            spiritBarR.BarProgress = Mathf.Clamp(spiritBarR.BarProgress, 0, 1);
            spiritBarR.UpdateBar01(spiritBarR.BarProgress);

            yield return null;
        }
    }

    public void ShowFloatingText(string text)
    {
        var go = Instantiate(floatingText, player.transform.position, Quaternion.identity, player.transform);
        go.text = text;

        Destroy(go, destroyTime);
    }

    public void KillScored()
    {
        scoreManager.ShowFloatingText("+ 8.25");

        if (spiritBarL.BarProgress >= 0.85)
        {            
            spiritBarL.SetBar01(spiritBarL.BarProgress + killIncrease);
            spiritBarR.SetBar01(spiritBarR.BarProgress + killIncrease);

            scoreManager.combo += 1;
            scoreManager.UpdateComboText();

            if (scoreManager.combo > scoreManager.highCombo)
            {
                scoreManager.highCombo = scoreManager.combo;
            }

            //Debug.Log(scoreManager.combo + "X COMBO");
        }
        else
        {
            spiritBarL.SetBar01(spiritBarL.BarProgress + killIncrease);
            spiritBarR.SetBar01(spiritBarR.BarProgress + killIncrease);
        }

        OnBarUpdate?.Invoke();
    }

    public void PickupScored()
    {
        ShowFloatingText("BOUNTY");
        //scoreManager.ShowFloatingText("+ 25.00");
    }

    public void ParryScored()
    {
        //Debug.Log("PARRY!!!");
        if (!player.isGunSpinning)
        {
            ShowFloatingText("PARRY!!!");
        }

        spiritBarL.SetBar01(spiritBarL.BarProgress + parryIncrease);
        spiritBarR.SetBar01(spiritBarR.BarProgress + parryIncrease);

        OnBarUpdate?.Invoke();
    }

    public void DeflectScored()
    {
        //Debug.Log("DEFLECT!!!");

        ShowFloatingText("DEFLECT!!!");

        spiritBarL.SetBar01(spiritBarL.BarProgress + deflectIncrease);
        spiritBarR.SetBar01(spiritBarR.BarProgress + deflectIncrease);

        OnBarUpdate?.Invoke();
    }

    public void DeadHitScored()
    {
        //Debug.Log("DEAD HIT!!!");

        spiritBarL.SetBar01(spiritBarL.BarProgress + deadHitIncrease);
        spiritBarR.SetBar01(spiritBarR.BarProgress + deadHitIncrease);

        OnBarUpdate?.Invoke();
    }

    public void SpiritDeplete()
    {
        //Debug.Log("NO SPIRIT!!!");

        spiritBarL.SetBar01(0);
        spiritBarR.SetBar01(0);

        OnBarUpdate?.Invoke();
    }

    
}
