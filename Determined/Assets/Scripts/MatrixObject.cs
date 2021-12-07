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

    public int x;
    public int y;

    protected virtual void Awake()
    {
        lineController = FindObjectOfType<LineController>();
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
        {
            if (currentState == MatrixObjectState.Active)
                ChooseMatrixObject();
            else MakeMatrixObjectActive();
        }
    }

    public void OnMouseDown()
    {
        if (Input.GetMouseButtonDown(0))
            ChooseMatrixObject();
    }
}
