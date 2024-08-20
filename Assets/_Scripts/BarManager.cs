using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;
using Dreamteck.Splines;
using MoreMountains.Feedbacks;
using TMPro;

public class BarManager : MonoBehaviour
{
    
    public PlayerController player { get; private set; }
    public PlayerControllerCowboy cowboy { get; private set; }
    private PlayerControls playerControls;

    public bool holdToFill;
    public bool tapToFill;
    public bool autoRecharge;
    public Image fill;

    public enum Button
    {
        B1,
        B2,
        B3,
        B4
    }
    public Button selectedButton;
    public bool selectedButtonPressed;

    public bool B1Pressed { get; private set; }
    public bool B2Pressed { get; private set; }
    public bool B3Pressed { get; private set; }
    public bool B4Pressed { get; private set; }
    
    [Header("Values")]
    public int barMaxParameter;
    public int barChangeAmount;
    public float rechargeTime;
    [Space(5)]
    public float barAccelTime;
    public AnimationCurve barAccelCurve;
    public float barDeccelTime;
    public AnimationCurve barDeccelCurve;
    [Space(5)]
    //public float barUpdateTime;

    [Header("Timers")]
    public float barTimer;
    [Range(0, 1)]
    public float lerpTime;
    [Range(0, 1)]
    public float curveLerpTime;
    
    [Header("Debug")]
    public float barSpeed;

    private float barCurrentSpeed;
    private float barCurrentAccelTime;
    private float barCurrentDeccelTime;

    [Range(0, 1)]
    public float barCurrentPct;
    public int barCurrentValue;

    public event Action<float, float> OnBarPctChanged = delegate {};

    private Coroutine _coroutine;
    
    private void Awake()
    {
        playerControls = new PlayerControls();
        cowboy = GetComponent<PlayerControllerCowboy>();

        playerControls.Interface.Button1.started += Button1_started;
        playerControls.Interface.Button1.performed += Button1_performed;
        playerControls.Interface.Button1.canceled += Button1_canceled;

        playerControls.Interface.Button2.started += Button2_started;
        playerControls.Interface.Button2.performed += Button2_performed;
        playerControls.Interface.Button2.canceled += Button2_canceled;

        playerControls.Interface.Button3.started += Button3_started;
        playerControls.Interface.Button3.performed += Button3_performed;
        playerControls.Interface.Button3.canceled += Button3_canceled;

        playerControls.Interface.Button4.started += Button4_started;
        playerControls.Interface.Button4.performed += Button4_performed;
        playerControls.Interface.Button4.canceled += Button4_canceled;

        OnBarPctChanged += HandleValueChanged;
    }

    private void OnEnable() 
    {
        playerControls.Enable();        
    }
    
    private void OnDisable() 
    {
        playerControls.Disable();        
    }

    // Start is called before the first frame update
    void Start()
    {
        barCurrentDeccelTime = barDeccelTime;

        if (tapToFill)
        {
            barCurrentValue = barMaxParameter;
            BarPctUpdate();
        }
    }

    // Update is called once per frame
    void Update()
    {
        //BarPctUpdate();
        
        if (barCurrentValue < barMaxParameter)
        {
            //Recharge();
        }

        if (holdToFill)
        {
        HoldToFill();
        }        
    }

    private void BarPctUpdate()
    {
        if (holdToFill)
        {
            barCurrentPct = barSpeed / barMaxParameter;
        }

        if (tapToFill)
        {
            barCurrentPct = (float) barCurrentValue / (float) barMaxParameter;
        }

        barCurrentPct = Mathf.Clamp(barCurrentPct, 0, 1);
        barCurrentValue = Mathf.Clamp(barCurrentValue, 0, barMaxParameter);
        //fill.fillAmount = barCurrentPct;
    }

    private IEnumerator Recharge(float pct)
    {
        barTimer = pct * rechargeTime;
        
        //Debug.Log(preChangePct);
        //Debug.Log(pct);
        //Debug.Log(barCurrentPct);
        //Debug.Log(barTimer);

        while (barTimer < rechargeTime)
        {
            barTimer += Time.deltaTime;
            fill.fillAmount = Mathf.Lerp(0, 1, barTimer / rechargeTime);
            
            barCurrentValue = Mathf.FloorToInt(fill.fillAmount * barMaxParameter);
            BarPctUpdate();
            yield return null;
        }
    }
    
    private void HoldToFill()
    {
        if (selectedButtonPressed)
        {
            lerpTime = barTimer / barCurrentAccelTime;
            
            curveLerpTime = barAccelCurve.Evaluate(lerpTime);
            barTimer -= Time.deltaTime;
            barTimer = Mathf.Clamp(barTimer, 0, barAccelTime);

            barSpeed = Mathf.Lerp(barMaxParameter, barCurrentSpeed, curveLerpTime);
        }
        else
        {
            lerpTime = barTimer / barCurrentDeccelTime;
            
            curveLerpTime = barDeccelCurve.Evaluate(lerpTime);
            barTimer -= Time.deltaTime;
            barTimer = Mathf.Clamp(barTimer, 0, barDeccelTime);

            barSpeed = Mathf.Lerp(0, barCurrentSpeed, curveLerpTime);
        }
    }

    private void HoldToFillCallbacks()
    {
        if (selectedButtonPressed)
        {
            barCurrentSpeed = barSpeed;
            barCurrentDeccelTime = barDeccelTime;

            if (barSpeed == 0)
            {
                barTimer = barAccelTime;
                barCurrentAccelTime = barAccelTime;
            }

            if (barSpeed > 0)
            {
                barTimer = barAccelTime * (1 - barCurrentPct);
                barCurrentAccelTime = barAccelTime * (1 - barCurrentPct);
            }
        }
        else
        {
            barCurrentSpeed = barSpeed;
            barCurrentAccelTime = barAccelTime;
 
            if (barSpeed == barMaxParameter)
            {
                barTimer = barDeccelTime;
                barCurrentDeccelTime = barDeccelTime;
            }

            if (barSpeed < barMaxParameter)
            {
                barTimer = barDeccelTime * barCurrentPct;
                barCurrentDeccelTime = barDeccelTime * barCurrentPct;
            }
        }
    }

    public void TapToFill(int amount, float updateTime)
    {
        //Debug.Log(barCurrentPct);
        //float updateTime;
        
        //if (selectedButtonPressed)
        //{
            barCurrentValue += amount;
            
            /*if (amount < 0)
            {
                Debug.Log(-1);
                updateTime = barAccelTime;
            }
            else
            {
                Debug.Log(+1);
                updateTime = barDeccelTime;
            }*/

            BarPctUpdate();

            OnBarPctChanged(barCurrentPct, updateTime);
            //Debug.Log(barCurrentPct);
        //}
    }

    private void HandleValueChanged(float pct, float updateTime)
    {
        //Debug.Log(pct);
        if (_coroutine != null)
            StopCoroutine(_coroutine);
        //_coroutine = null;
        _coroutine = StartCoroutine(ChangeToPct(pct, updateTime));
    }

    private IEnumerator ChangeToPct(float pct, float updateTime)
    {
        float preChangePct = fill.fillAmount;
        barTimer = 0f;

        while (barTimer < updateTime)
        {
            //curveLerpTime = barAccelCurve.Evaluate(barTimer / barAccelTime);
            barTimer += Time.deltaTime;
            //Debug.Log($"preChangePct: {preChangePct}, pct: {pct}");
            
            fill.fillAmount = Mathf.Lerp(preChangePct, pct, barTimer / updateTime);

            yield return null;
        }

        fill.fillAmount = pct;
        //StopCoroutine(_coroutine);
        //_coroutine = null;
        if (autoRecharge)
            _coroutine = StartCoroutine(Recharge(barCurrentPct));
    }

#region INPUT CALLBACKS

    private void Button1_started(InputAction.CallbackContext context)
    {
        B1Pressed = true;

        if (selectedButton == Button.B1)
        {
            selectedButtonPressed = true;
        }
        /*if (selectedButton == Button.B1)
        {
            selectedButtonPressed = true;

            if (holdToFill)
            {
                HoldToFillCallbacks();
            }

            if (tapToFill)
            {
                TapToFill(barChangeAmount);
            }
        }*/    
    }

    private void Button1_performed(InputAction.CallbackContext context)
    {
        if (selectedButton == Button.B1)
        {
            selectedButtonPressed = true;

            if (holdToFill)
            {
                HoldToFillCallbacks();
            }

            if (tapToFill)
            {
                //TapToFill(barChangeAmount);
            }
        }    
    }

    private void Button1_canceled(InputAction.CallbackContext context)
    {
        B1Pressed = false;

        if (selectedButton == Button.B1)
        {
            selectedButtonPressed = false;

            if (holdToFill)
            {
                HoldToFillCallbacks();
            }
        }
    }

    private void Button2_started(InputAction.CallbackContext context)
    {
        B2Pressed = true;

        if (selectedButton == Button.B2)
        {
            selectedButtonPressed = true;

            if (holdToFill)
            {
                HoldToFillCallbacks();
            }
        }
    }

    private void Button2_performed(InputAction.CallbackContext context)
    {
        
    }

    private void Button2_canceled(InputAction.CallbackContext context)
    {
        B2Pressed = false;

        if (selectedButton == Button.B2)
        {
            selectedButtonPressed = false;

            if (holdToFill)
            {
                HoldToFillCallbacks();
            }
        }
    }

    private void Button3_started(InputAction.CallbackContext context)
    {
        B3Pressed = true;

        if (selectedButton == Button.B3)
        {
            selectedButtonPressed = true;

            if (holdToFill)
            {
                HoldToFillCallbacks();
            }
        }
    }

    private void Button3_performed(InputAction.CallbackContext context)
    {
        
    }

    private void Button3_canceled(InputAction.CallbackContext context)
    {
        B3Pressed = false;

        if (selectedButton == Button.B3)
        {
            selectedButtonPressed = false;

            if (holdToFill)
            {
                HoldToFillCallbacks();
            }
        }
    }

    private void Button4_started(InputAction.CallbackContext context)
    {
        B4Pressed = true;

        if (selectedButton == Button.B4)
        {
            selectedButtonPressed = true;

            if (holdToFill)
            {
                HoldToFillCallbacks();
            }
        }
    }

    private void Button4_performed(InputAction.CallbackContext context)
    {
        
    }

    private void Button4_canceled(InputAction.CallbackContext context)
    {
        B4Pressed = false;

        if (selectedButton == Button.B4)
        {
            selectedButtonPressed = false;

            if (holdToFill)
            {
                HoldToFillCallbacks();
            }
        }
    }
#endregion
}
