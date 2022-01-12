using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SimplerLineController : MonoBehaviour
{
    public LineRenderer lr1;
    public LineRenderer lr2;

    private void Awake()
    {
        var renderers = GetComponentsInChildren<LineRenderer>();
        lr1 = renderers[0];
        lr2 = renderers[1];
    }

    public void AddPoints(MatrixObject[] objects)
    {
        UpdateFirstLine(objects);
    }

    public void AddPoints(MatrixObject[] objects, MatrixObject cornerObject)
    {
        if(objects.Length == 2)
        {
            var temp = objects.ToList();
            temp.Add(cornerObject);
            temp = temp.OrderBy(obj => obj.x).ThenBy(obj => obj.y).ToList();
            UpdateSecondLine(temp.ToArray());
        }
        if(objects.Length == 5)
        {
            var temp = objects.Where(obj => obj.x == cornerObject.x).
                OrderBy(obj => obj.y).ToArray();
            UpdateFirstLine(temp);
            temp = objects.Where(obj => obj.y == cornerObject.y).
                OrderBy(obj => obj.x).ToArray();
            UpdateSecondLine(temp);
        }
    }

    public void UpdateFirstLine(MatrixObject[] objects)
    {
        var points = objects.Select(x => x.transform.position).ToList();
        var numPoints = objects.Length;
        lr1.positionCount = numPoints;
        for (int i = 0; i < numPoints; i++)
            lr1.SetPosition(i, points[i]);
    }

    public void UpdateSecondLine(MatrixObject[] objects)
    {
        var points = objects.Select(x => x.transform.position).ToList();
        var numPoints = objects.Length;
        lr2.positionCount = numPoints;
        for (int i = 0; i < numPoints; i++)
            lr2.SetPosition(i, points[i]);
    }

    public void ClearLines()
    {
        lr1.positionCount = 0;
        lr2.positionCount = 0;
    }
}
