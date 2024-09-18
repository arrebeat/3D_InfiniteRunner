using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using UnityEngine;

public class PlayerController_Ball : MonoBehaviour
{
    public Vector2 pastPosition { get; private set; }
    public float speedHorizontal;
    public float speedRun;
    public float speedRunMultiplier = 1;

    public GameManager_Runner gameManager { get; private set; }
    public Animator animator { get; private set; }
    public GameObject marble { get; private set; }
    public Material[] materials;
    public MMF_Player feedbacks { get; private set; }
    public Vector3 startPosition;

    private bool _canRun = false;
    private bool _isIntangible = false;

    private void Awake()
    {
        GameObject gameManagerObject = GameObject.Find("GameManager");
        gameManager = gameManagerObject.GetComponent<GameManager_Runner>();
        marble = GameObject.Find("Marble");
        animator = marble.GetComponentInChildren<Animator>();
        feedbacks = marble.GetComponentInChildren<MMF_Player>();

        startPosition = transform.position;
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
        transform.position += Vector3.right * Time.deltaTime * delta * speedHorizontal;
    }

    public void Hit()
    {
        //Debug.Log("WALL HIT");
        if (!_isIntangible)
        {
            _canRun = false;
            animator.SetTrigger("Hit");
            MMF_PositionShake positionShake = feedbacks.GetFeedbackOfType<MMF_PositionShake>();
            positionShake.Play(transform.position, 1);

            gameManager.Lose();
        }
    }

    public void StartRun()
    {
        _canRun = true;
        marble.GetComponent<BoxCollider>().enabled = true;
        animator.SetTrigger("Move");
    }

    public void Win()
    {
        _canRun = false;
        marble.GetComponent<BoxCollider>().enabled = false;
        animator.SetTrigger("Idle");
        
        gameManager.Win();
    }

    public void ResetPlayer()
    {
        transform.position = startPosition;
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
        MeshRenderer marbleRender = marble.GetComponentInChildren<MeshRenderer>();
        marbleRender.material = materials[1];
        Invoke("PowerUpIntangibleEnd", duration);
    }

    public void PowerUpIntangibleEnd()
    {
        _isIntangible = false;
        MeshRenderer marbleRender = marble.GetComponentInChildren<MeshRenderer>();
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
        groundPos.y = startPosition.y;
        transform.position = groundPos;
    }

    #endregion


}
