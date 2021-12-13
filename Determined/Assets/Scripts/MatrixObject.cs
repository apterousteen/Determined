using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MatrixObjectState
{
    Active,
    Chosen,
    Blocked
}

public abstract class MatrixObject : MonoBehaviour
{
    public MatrixObjectState currentState = MatrixObjectState.Active;
    public int value;
    protected LineController lineController;
    protected BoxCollider2D boxCollider;
    protected bool fingerInCollider = false;

    public int x;
    public int y;

    protected virtual void Awake()
    {
        lineController = FindObjectOfType<LineController>();
        boxCollider = GetComponent<BoxCollider2D>();
    }

    protected void Update()
    {
        HandleTouchControl();
    }

    public virtual void ChooseMatrixObject()
    {
        currentState = MatrixObjectState.Chosen;
        lineController.AddPoint(gameObject.transform);
    }

    public virtual void MakeMatrixObjectActive()
    {
        currentState = MatrixObjectState.Active;
        lineController.DeletePoint(gameObject.transform);
    }

    public virtual void BlockMatrixObject()
    {
        currentState = MatrixObjectState.Blocked;
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
            if (currentState == MatrixObjectState.Active)
                ChooseMatrixObject();
            else MakeMatrixObjectActive();
        }
    }

    protected void PointerDown()
    {
        if (currentState != MatrixObjectState.Blocked)
            ChooseMatrixObject();
    }
}
