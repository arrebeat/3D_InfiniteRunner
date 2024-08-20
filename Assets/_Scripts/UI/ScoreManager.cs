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

public class ScoreManager : MonoBehaviour
{

    public TextMeshPro[] scoreTexts { get; private set; }

    public TextMeshProUGUI[] scoreValueTexts { get; private set; }
    public TextMeshProUGUI bountyValueText { get; private set;}
    public TextMeshProUGUI livesValueText { get; private set;}
    public TextMeshProUGUI wavesValueText { get; private set;}
    public TextMeshProUGUI comboValueText { get; private set;}

    public TextMeshProUGUI livesScoreText { get; private set; }
    public TextMeshProUGUI wavesScoreText { get; private set; }
    public TextMeshProUGUI comboScoreText { get; private set; }

    public TextMeshProUGUI comboText { get; private set; }

    public Image[] scoreRatingImages;
    private Image bountyRatingImage;
    private Image livesRatingImage;
    private Image wavesRatingImage;
    private Image comboRatingImage;

    public Sprite[] scoreSourceImages;

    public MMF_Player feedbacks { get; private set;}

    public float scoreUpdateDuration;
    public float score = 0;
    public int combo = 0;
    public int highCombo = 0;
    private float targetScore;
    private HealthBarManager healthBarManager;
    private ProgressManager progressManager;
    private ComboManager comboManager;
    private PlayerControllerCowboy player;
    private BadgeController badge;

    public GameObject containerEndObject;
    
    [Space(10)]
    public float waitScore;
    [Space(10)]
    public float finalBounty; 
    [Space(5)]
    public int finalLives;
    public float livesMultiplier;
    public float scoreLives;
    [Space(5)]
    public int finalWaves;
    public float wavesMultiplier;
    public float scoreWaves;
    [Space(5)]
    public int finalCombo;
    public float comboMultiplier;
    public float scoreCombo;

    [Space(10)]
    public float totalScore;


    [Header("Score Feedbacks")]
    public float delayScorePanels;
    public float delayScoreResults;
    
    [Space(10)]
    public TextMeshPro floatingText;
    public Transform floatingTextPosition;
    public float destroyTime;

    private void Awake()
    {
        GameObject canvas = GameObject.Find("Canvas");
        healthBarManager = canvas.GetComponent<HealthBarManager>();
        progressManager = canvas.GetComponent<ProgressManager>();
        comboManager = canvas.GetComponent<ComboManager>();

        containerEndObject = GameObject.Find("Container End");

        feedbacks = GetComponentInChildren<MMF_Player>();

        GameObject playerObject = GameObject.Find("Cowboy");
        player = playerObject.GetComponent<PlayerControllerCowboy>();

        GameObject badgeObject = GameObject.Find("Badge");
        badge = badgeObject.GetComponent<BadgeController>();
        
        scoreTexts = GetComponentsInChildren<TextMeshPro>();
        UpdateScoreText(); // Call this to set the initial score text.

        scoreRatingImages = GetComponentsInChildren<Image>();
        bountyRatingImage = scoreRatingImages[26];
        livesRatingImage = scoreRatingImages[29];
        wavesRatingImage = scoreRatingImages[32];
        comboRatingImage = scoreRatingImages[35];

        GameObject containerTallyObject = GameObject.FindGameObjectWithTag("ContainerTally");
        scoreValueTexts = containerTallyObject.GetComponentsInChildren<TextMeshProUGUI>();
        
        bountyValueText = scoreValueTexts[1];
        livesValueText = scoreValueTexts[3];
        wavesValueText = scoreValueTexts[6];
        comboValueText = scoreValueTexts[9];

        livesScoreText = scoreValueTexts[4];
        wavesScoreText = scoreValueTexts[7];
        comboScoreText = scoreValueTexts[10];

        comboText = scoreValueTexts[12];
    }

    void Start()
    {
        containerEndObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowFloatingText(string text)
    {
        var go = Instantiate(floatingText, floatingTextPosition.transform.position, Quaternion.identity, floatingTextPosition);
        go.text = text;

        Destroy(go, destroyTime);
    }

    public void ScoreTally()
    {
        finalBounty = score;

        bountyValueText.text = string.Format("{0:0.00}", finalBounty);

        finalLives = player.currentHealth;
        finalWaves = progressManager.currentWave;
        finalCombo = highCombo;

        livesMultiplier = (float)finalLives/(float)player.maxHealth;
        wavesMultiplier = finalWaves * 0.1f;
        comboMultiplier = finalCombo * 0.1f;

        scoreLives = Mathf.Round((finalBounty * livesMultiplier) * 100f) / 100f;
        scoreWaves = Mathf.Round((finalBounty * wavesMultiplier) * 100f) / 100f;
        scoreCombo = Mathf.Round((finalBounty * comboMultiplier) * 100f) / 100f;

        //float roundedResult = Mathf.Round(result * 100f) / 100f;

        livesValueText.text = finalLives.ToString();
        wavesValueText.text = finalWaves.ToString();
        comboValueText.text = finalCombo.ToString();

        livesScoreText.text = "+" + string.Format("{0:0.00}", scoreLives);
        wavesScoreText.text = "+" + string.Format("{0:0.00}", scoreWaves);
        comboScoreText.text = "+" + string.Format("{0:0.00}", scoreCombo);

        totalScore = finalBounty + scoreLives + scoreWaves + scoreCombo;

        scoreValueTexts[11].text = string.Format("{0:0.00}", totalScore);

        ScoreRating(finalBounty, finalLives, finalWaves, finalCombo);
        StopAllCoroutines();
        StartCoroutine(ScoreFeedbacks());
    }

    public void ScoreRating(float finalBounty, float finalLives, float finalWaves, float finalCombo)
    {
        if (finalBounty < 250)
        {
            badge.finalBountyRating = BadgeController.BountyRating.Copper;
            bountyRatingImage.sprite = scoreSourceImages[3];
        }
        if (finalBounty >= 250 && finalBounty < 500)
        {
            badge.finalBountyRating = BadgeController.BountyRating.Bronze;
            bountyRatingImage.sprite = scoreSourceImages[2];
        }
        if (finalBounty >= 500 && finalBounty < 750)
        {
            badge.finalBountyRating = BadgeController.BountyRating.Silver;
            bountyRatingImage.sprite = scoreSourceImages[1];
        }
        if (finalBounty >= 750)
        {
            badge.finalBountyRating = BadgeController.BountyRating.Gold;
            bountyRatingImage.sprite = scoreSourceImages[0];
        }


        if (finalLives == 0)
        {
            badge.finalLivesRating = BadgeController.LivesRating.Bone;
            livesRatingImage.sprite = scoreSourceImages[7];
        }
        if (finalLives == 1 || finalLives == 2)
        {
            badge.finalLivesRating = BadgeController.LivesRating.Bronze;
            livesRatingImage.sprite = scoreSourceImages[6];
        }
        if (finalLives == 3 || finalLives == 4)
        {
            badge.finalLivesRating = BadgeController.LivesRating.Silver;
            livesRatingImage.sprite = scoreSourceImages[5];
        }
        if (finalLives == 5)
        {
            badge.finalLivesRating = BadgeController.LivesRating.Gold;
            livesRatingImage.sprite = scoreSourceImages[4];
        }


        if (finalWaves < 5)
        {
            badge.finalWavesRating = BadgeController.WavesRating.Bronze;
            wavesRatingImage.sprite = scoreSourceImages[10];
        }
        if (finalWaves >= 5 && finalWaves <= 9)
        {
            badge.finalWavesRating = BadgeController.WavesRating.Silver;
            wavesRatingImage.sprite = scoreSourceImages[9];
        }
        if (finalWaves > 9)
        {
            badge.finalWavesRating = BadgeController.WavesRating.Gold;
            wavesRatingImage.sprite = scoreSourceImages[8];
        }


        if (finalCombo == 0)
        {
            badge.finalComboRating = BadgeController.ComboRating.NoCombo;
        }
        if (finalCombo > 0 && finalCombo < 15)
        {
            badge.finalComboRating = BadgeController.ComboRating.Bronze;
            comboRatingImage.sprite = scoreSourceImages[13];
        }
        if (finalCombo >= 15 && finalCombo < 25)
        {
            badge.finalComboRating = BadgeController.ComboRating.Silver;
            comboRatingImage.sprite = scoreSourceImages[12];
        }
        if (finalCombo >= 25 && finalCombo < 30)
        {
            badge.finalComboRating = BadgeController.ComboRating.Gold;
            comboRatingImage.sprite = scoreSourceImages[11];
        }
    }

    public IEnumerator ScoreFeedbacks()
    {
        WaitForSeconds waitPanel = new WaitForSeconds(delayScorePanels);
        WaitForSeconds waitResult = new WaitForSeconds(delayScoreResults);

        WaitForSeconds wait01 = new WaitForSeconds(0);

        List<MMF_Position> feedbacksPanelEnter = feedbacks.GetFeedbacksOfType<MMF_Position>();
        List<MMF_TMPAlpha> feedbacksResultEnter = feedbacks.GetFeedbacksOfType<MMF_TMPAlpha>();
        List<MMF_PositionShake> feedbacksPositionShake = feedbacks.GetFeedbacksOfType<MMF_PositionShake>();
        List<MMF_ImageAlpha> feedbacksImageAlpha = feedbacks.GetFeedbacksOfType<MMF_ImageAlpha>();

        var badgeShake = badge.feedbacks.GetFeedbackOfType<MMF_PositionShake>();

        feedbacksImageAlpha[0].Play(transform.position, 1); //fade background

        feedbacksPanelEnter[0].Play(transform.position, 1);
        if (badge.finalBountyRating == BadgeController.BountyRating.Gold)
        {
            badge.animators[0].SetInteger("ringIndex", 10);
        }
        if (badge.finalBountyRating == BadgeController.BountyRating.Silver)
        {
            badge.animators[0].SetInteger("ringIndex", 20);
        }
        if (badge.finalBountyRating == BadgeController.BountyRating.Bronze)
        {
            badge.animators[0].SetInteger("ringIndex", 30);
        }
        if (badge.finalBountyRating == BadgeController.BountyRating.Copper)
        {
            badge.animators[0].SetInteger("ringIndex", 40);
        }

        yield return wait01;
        feedbacksImageAlpha[1].Play(transform.position, 1);

        yield return waitPanel;

        feedbacksPanelEnter[1].Play(transform.position, 1);      
        if (badge.finalLivesRating == BadgeController.LivesRating.Gold)
        {
            badge.animators[1].SetInteger("starIndex", 11);
        }
        if (badge.finalLivesRating == BadgeController.LivesRating.Silver)
        {
            badge.animators[1].SetInteger("starIndex", 21);
        }
        if (badge.finalLivesRating == BadgeController.LivesRating.Bronze)
        {
            badge.animators[1].SetInteger("starIndex", 31);
        }
        if (badge.finalLivesRating == BadgeController.LivesRating.Bone)
        {
            badge.animators[1].SetInteger("starIndex", 41);
        }          
        
        yield return wait01;
        feedbacksImageAlpha[2].Play(transform.position, 1);

        yield return waitPanel;
        
        feedbacksPanelEnter[2].Play(transform.position, 1);  
        if (badge.finalWavesRating == BadgeController.WavesRating.Gold)
        {
            badge.animators[2].SetInteger("ribbonIndex", 12);
        }
        if (badge.finalWavesRating == BadgeController.WavesRating.Silver)
        {
            badge.animators[2].SetInteger("ribbonIndex", 22);
        }
        if (badge.finalWavesRating == BadgeController.WavesRating.Bronze)
        {
            badge.animators[2].SetInteger("ribbonIndex", 32);
        }                      
                
        yield return wait01;
        feedbacksImageAlpha[3].Play(transform.position, 1);

        yield return waitPanel;
        //badgeShake.Play(transform.position, 1);

        feedbacksPanelEnter[3].Play(transform.position, 1);
        if (badge.finalComboRating == BadgeController.ComboRating.Gold)
        {
            badge.animators[3].SetInteger("wingsIndex", 13);
        }
        if (badge.finalComboRating == BadgeController.ComboRating.Silver)
        {
            badge.animators[3].SetInteger("wingsIndex", 23);
        }
        if (badge.finalComboRating == BadgeController.ComboRating.Bronze)
        {
            badge.animators[3].SetInteger("wingsIndex", 33);
        }
        
        if (finalCombo != 0)
        {
            yield return wait01;
            feedbacksImageAlpha[4].Play(transform.position, 1);
        }

        yield return waitResult;

        livesScoreText.enabled = true;
        feedbacksResultEnter[0].Play(transform.position, 1);
        feedbacksPanelEnter[4].Play(transform.position, 1);

        yield return waitResult;

        wavesScoreText.enabled = true;
        feedbacksResultEnter[1].Play(transform.position, 1);
        feedbacksPanelEnter[5].Play(transform.position, 1);

        yield return waitResult;

        comboScoreText.enabled = true;
        feedbacksResultEnter[2].Play(transform.position, 1);
        feedbacksPanelEnter[6].Play(transform.position, 1);

        yield return waitPanel;

        scoreValueTexts[11].enabled = true;
        feedbacksPositionShake[0].Play(transform.position, 1);

        yield return waitPanel;

        containerEndObject.SetActive(true);

        yield return null;
    }

    

    public void ChangeScoreGradually(float delta)
    {
        targetScore += delta;
        StartCoroutine(UpdateScoreGradually(targetScore, scoreUpdateDuration));
    }

    // Update the score and update the displayed score text.
    private void UpdateScoreText()
    {
        string formattedScore = score.ToString("0000.00");

        int decimalPointIndex = formattedScore.IndexOf(",");
        string integerPart = formattedScore.Substring(0, decimalPointIndex);
        string decimalPart = formattedScore.Substring(decimalPointIndex + 1);

        decimalPart = $"<size=65%>{decimalPart}</size>";

        formattedScore = integerPart + "." + decimalPart;

        scoreTexts[0].text = formattedScore;
    }

    private IEnumerator UpdateScoreGradually(float targetScore, float duration)
{
    float initialScore = score;
    float startTime = Time.time;

    while (Time.time - startTime < duration)
    {
        float elapsed = Time.time - startTime;
        score = Mathf.Lerp(initialScore, targetScore, elapsed / duration);
        UpdateScoreText();
        yield return null;
    }

    score = targetScore; // Ensure the score ends up exactly as the target.
    UpdateScoreText();
}

    public void UpdateComboText()
    {
        if (combo != 0)
        {
            List<MMF_PositionShake> feedbacksPositionShake = feedbacks.GetFeedbacksOfType<MMF_PositionShake>();
          
            comboText.text = "x" + combo.ToString() + "\nCOMBO";
            comboText.enabled = true;
            feedbacksPositionShake[1].Play(transform.position, 1);            
        }
        else
        {
            comboText.enabled = false;
        }
    }
    

}
