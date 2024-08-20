using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;
using Dreamteck.Splines;
using MoreMountains.Feedbacks;
using System;
using UnityEngine.UI;
using TMPro;

public class Item : MonoBehaviour, IDamageable
{
    public float lifetime;
    public float timer;
    [Space(5)]
    public float bounty;
    [Range(1, 6)]    
    public int bountyMultiplier;
//    public float multiplierUpdateDelay;
//    private int targetMultiplier;

//    public GameObject containerBountyMultiplier;
    public TextMeshPro multiplierText { get; private set; } 

    public Animator animator { get; private set; }
    private ScoreManager scoreManager;

    private void Awake()
    {
        animator = GetComponent<Animator>();

//        GameObject canvas = GameObject.Find("Canvas");
//        containerBountyMultiplier = GameObject.Find("ContainerBountyMultiplier");
        multiplierText = GetComponentInChildren<TextMeshPro>();
        
        GameObject scoreManagerObject = GameObject.Find("Canvas");
        scoreManager = scoreManagerObject.GetComponent<ScoreManager>();
    }
    
    void Start()
    {
        bountyMultiplier = 1;
        multiplierText.enabled = true;
        timer = lifetime;
    }

    // Update is called once per frame
    void Update()
    {
        bountyMultiplier = Mathf.Clamp(bountyMultiplier, 1, 6);

        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            Destroy(gameObject);
        }
    }

    public void TakeDamage(int Damage)
    {
        scoreManager.ChangeScoreGradually(bounty * bountyMultiplier);
        scoreManager.ShowFloatingText(string.Format("{0:0.00}", (bounty * bountyMultiplier)));
                Destroy(gameObject);
    }

    /*
    public void ChangeMultiplier()
    {
        targetMultiplier += 1;
        StartCoroutine(UpdateMultiplier(targetMultiplier));
    }

    private void UpdateMultiplierText()
    {
        string formattedMultiplier = bountyMultiplier.ToString();

        string xPart = formattedMultiplier.Substring(0);
        string integerPart = formattedMultiplier.Substring(1);

        integerPart = $"<size=150%>{integerPart}</size>";

        formattedMultiplier = "X" + integerPart;

        multiplierText.text = formattedMultiplier;
    }
    */

    public void MultiplierTextUpdate()
    {
        string formattedMultiplierText = $"X{bountyMultiplier}";

        multiplierText.text = formattedMultiplierText;

    }

    /*
    private IEnumerator UpdateMultiplier(int targetMultiplier)
    {
        WaitForSeconds wait = new WaitForSeconds(multiplierUpdateDelay);
        float initialMultiplier = bountyMultiplier;

        bountyMultiplier = targetMultiplier;
        yield return wait;

        UpdateMultiplierText();
    }
    */
}
