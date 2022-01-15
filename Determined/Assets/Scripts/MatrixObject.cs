using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public enum MatrixObjectState
{
    Active,
    Chosen,
    Blocked
}

public abstract class MatrixObject : MonoBehaviour
{
    [SerializeField] Sprite chosenMatrixObject = null;
    [SerializeField] Sprite activeMatrixObject = null;
    [SerializeField] Sprite blockedMatrixObject = null;

    public MatrixObjectState currentState = MatrixObjectState.Active;
    public int value;
    protected LineController lineController;
    protected BoxCollider2D boxCollider;
    private AudioManager audioManager;
    protected bool fingerInCollider = false;

    public int x;
    public int y;

    protected virtual void Awake()
    {
        lineController = FindObjectOfType<LineController>();
        boxCollider = GetComponent<BoxCollider2D>();
        audioManager = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioManager>();
    }

    protected void Update()
    {
        HandleTouchControl();
    }

    public virtual void ChooseMatrixObject()
    {
        currentState = MatrixObjectState.Chosen;
        lineController.AddPoint(gameObject.transform);
        gameObject.GetComponent<SpriteRenderer>().sprite = chosenMatrixObject;
    }

    public virtual void MakeMatrixObjectActive()
    {
        currentState = MatrixObjectState.Active;
        lineController.DeletePoint(gameObject.transform);
        gameObject.GetComponent<SpriteRenderer>().sprite = activeMatrixObject;
    }

    public virtual void BlockMatrixObject()
    {
        currentState = MatrixObjectState.Blocked;
        gameObject.GetComponent<SpriteRenderer>().sprite = blockedMatrixObject;
    }

    public void OnMouseEnter()
    {
        if (Input.GetMouseButton(0))
            PointerEntered();
    }

    public void OnMouseDown()
    {
        if (Input.GetMouseButtonDown(0))
            PointerDown();
    }

    protected void HandleTouchControl()
    {
        if(Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            var touchPos = Camera.main.ScreenToWorldPoint(touch.position);
            var touchInCollider = boxCollider.bounds.Contains(touchPos);
            if (touch.phase == TouchPhase.Began && touchInCollider)
            {
                fingerInCollider = true;
                PointerEntered();
            }
            if(touch.phase == TouchPhase.Moved && touchInCollider && !fingerInCollider)
            {
                fingerInCollider = true;
                PointerEntered();
            }
            if (touch.phase == TouchPhase.Moved && !touchInCollider && fingerInCollider)
                fingerInCollider = false;
        }
    }

    protected void PointerEntered()
    {
        if (currentState != MatrixObjectState.Blocked)
        {
            if(lineController.points.Last() == gameObject.transform.position) 
                MakeMatrixObjectActive();
            else
            {
                ChooseMatrixObject();
                audioManager.Play("MatrixObjectChained");
            }
        }
    }

    protected void PointerDown()
    {
        if (currentState != MatrixObjectState.Blocked)
        {
            ChooseMatrixObject();
            audioManager.Play("MatrixObjectClicked");
        }
    }
}
