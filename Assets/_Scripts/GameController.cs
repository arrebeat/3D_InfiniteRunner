using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public PlayerController player { get; private set; }
    private GameObject respawnPoint;
    public float respawnDelay;
    public float restartDelay;

    private void Awake()
    {
        GameObject playerObject = GameObject.Find("Teletustra");
        player = playerObject.GetComponent<PlayerController>();
        respawnPoint = GameObject.Find("Respawn Point");
    }

    
    void Update()
    {
        if (player.HasDied)
        {
            StartCoroutine(RestartLevel(restartDelay));
        }
    }

    IEnumerator Respawn(float respawnDelay)
    {
        yield return new WaitForSeconds(respawnDelay);
        player.transform.position = respawnPoint.transform.position;

        foreach (SpriteRenderer renderer in player.renderers)
        {
            renderer.enabled = true;
        }

        player.Invoke("OnEnable", 0);
        player.HasDied = false;
    }

    IEnumerator RestartLevel(float restarDelay)
    {
        yield return new WaitForSeconds(restartDelay);

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
