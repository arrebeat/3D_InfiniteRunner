using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController_Ball : MonoBehaviour
{
    public Vector2 pastPosition;
    public float speed;
    public float speedRun;

    public GameManager_Runner gameManager { get; private set; }

    private bool _canRun = false;

    private void Awake()
    {
        GameObject gameManagerObject = GameObject.Find("GameManager");
        gameManager = gameManagerObject.GetComponent<GameManager_Runner>();
    }

    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            Move(Input.mousePosition.x - pastPosition.x);
        }

        pastPosition = Input.mousePosition;

        if (_canRun)
        {
            transform.Translate(transform.forward * speedRun * Time.deltaTime);
        }
    }

    public void Move(float delta)
    {
        transform.position += Vector3.right * Time.deltaTime * delta * speed;
    }

    public void Hit()
    {
        //Debug.Log("WALL HIT");
        _canRun = false;

        gameManager.Lose();
    }

    public void StartRun()
    {
        _canRun = true;

    }
}
