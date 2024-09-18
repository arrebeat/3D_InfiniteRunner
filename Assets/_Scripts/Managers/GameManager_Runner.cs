using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager_Runner : MonoBehaviour
{
    public PlayerController_Ball player { get; private set; }
    public LevelManager_Runner levelManager { get; private set; }
    public GameObject background;
    public GameObject buttonRestart;
    public GameObject buttonNext;

    private void Awake()
    {
        GameObject playerObject = GameObject.Find("Player");
        player = playerObject.GetComponent<PlayerController_Ball>();

        GameObject levelManagerObject = GameObject.Find("LevelManager");
        levelManager = levelManagerObject.GetComponent<LevelManager_Runner>();
    }

    public void Win()
    {
        Debug.Log("WIN");
        background.SetActive(true);
        
        if (levelManager.levelIndex < levelManager.levels.Count - 1)
        {
            buttonNext.SetActive(true);
        }
        else if (levelManager.levelIndex >= levelManager.levels.Count - 1)
        {
            buttonRestart.SetActive(true);
        }
    }
    
    public void Lose()
    {
        Debug.Log("LOSE");
        //GameObject background = GameObject.Find("Background");
        background.SetActive(true);

        //GameObject buttonRestart = GameObject.Find("Button: Restart");
        buttonRestart.SetActive(true);
    }

    void Update()
    {
        
    }
}
