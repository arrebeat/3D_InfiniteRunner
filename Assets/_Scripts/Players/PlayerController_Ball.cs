using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController_Ball : MonoBehaviour
{
    public Vector2 pastPosition;
    public float speed;
    public float speedRun;
    public float speedRunMultiplier = 1;

    public GameManager_Runner gameManager { get; private set; }
    public Animator animator { get; private set; }
    public GameObject marble { get; private set; }
    public Material[] materials;

    private Vector3 _startPosition;
    private bool _canRun = false;
    private bool _isIntangible = false;

    private void Awake()
    {
        GameObject gameManagerObject = GameObject.Find("GameManager");
        gameManager = gameManagerObject.GetComponent<GameManager_Runner>();
        marble = GameObject.Find("Marble");
        animator = marble.GetComponent<Animator>();

        _startPosition = transform.position;
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
            transform.Translate(transform.forward * speedRun * speedRunMultiplier * Time.deltaTime);
        }
    }

    public void Move(float delta)
    {
        transform.position += Vector3.right * Time.deltaTime * delta * speed;
    }

    public void Hit()
    {
        //Debug.Log("WALL HIT");
        if (!_isIntangible)
        {
            _canRun = false;
            animator.SetTrigger("Idle");

            gameManager.Lose();
        }
    }

    public void StartRun()
    {
        _canRun = true;

        animator.SetTrigger("Move");
    }

    #region POWERUPS

    public void PowerUpSpeedStart(float duration, float speedMultiplier)
    {
        speedRunMultiplier = speedMultiplier;
        Invoke("PowerUpSpeedEnd", duration);
    }

    public void PowerUpSpeedEnd()
    {
        speedRunMultiplier = 1;
    }

    public void PowerUpIntangibleStart(float duration)
    {
        _isIntangible = true;
        MeshRenderer marbleRender = marble.GetComponent<MeshRenderer>();
        marbleRender.material = materials[1];
        Invoke("PowerUpIntangibleEnd", duration);
    }

    public void PowerUpIntangibleEnd()
    {
        _isIntangible = false;
        MeshRenderer marbleRender = marble.GetComponent<MeshRenderer>();
        marbleRender.material = materials[0];
    }

    public void PowerUpHoverStart(float duration, float height)
    {
        var hoverPos = transform.position;
        hoverPos.y = height;
        transform.position = hoverPos;
        Invoke("PowerUpHoverEnd", duration);
    }

    public void PowerUpHoverEnd()
    {
        var groundPos = transform.position;
        groundPos.y = _startPosition.y;
        transform.position = groundPos;
    }

    #endregion


}
