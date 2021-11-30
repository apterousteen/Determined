using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineController : MonoBehaviour
{
    private MatrixObject[] objects;
    private LineRenderer lr;
    public List<Vector3> points;

    private void Awake()
    {
        objects = FindObjectsOfType<MatrixObject>();
        points = new List<Vector3>();
        lr = GetComponent<LineRenderer>();
    }

    private void Update()
    {
        if (Input.GetMouseButton(0) && AnyMatrixObjectChosen())
            UpdateLine();
        else ClearLine();
    }

    private bool AnyMatrixObjectChosen()
    {
        foreach (var matrixObject in objects)
            if (matrixObject.currentState == MatrixObjectState.Chosen)
                return true;
        return false;
    }

    public void AddPoint(Transform point)
    {
        points.Add(point.position);
    }

    public void DeletePoint(Transform point)
    {
        points.Remove(point.position);
    }

    private int CountChosenObjects()
    {
        int res = 0;
        foreach (var matrixObject in objects)
            if (matrixObject.currentState == MatrixObjectState.Chosen)
                res++;
        return res;
    }

    private void UpdateLine()
    {
        var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        var numPoints = points.Count;
        lr.positionCount = numPoints + 1;
        for (int i = 0; i < numPoints; i++)
            lr.SetPosition(i, points[i]);
        lr.SetPosition(numPoints, mousePos);
    }

    private void ClearLine()
    {
        lr.positionCount = 0;
        points.Clear();
        foreach (var matrixObject in objects)
            matrixObject.MakeMatrixObjectActive();
    }
}
