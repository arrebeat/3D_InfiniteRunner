using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager_Runner : MonoBehaviour
{
    public PlayerController_Ball player { get; private set; }
    public GameObject background;
    public GameObject buttonRestart;

    private void Awake()
    {
        GameObject playerObject = GameObject.Find("Player");
        player = playerObject.GetComponent<PlayerController_Ball>();
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
