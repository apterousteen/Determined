using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using System;
using System.Threading;

public enum DeterminantType
{
    DoubleMatrix,
    Leibniz,
    Triangles,
    Any
}

public class ResultBoard : MonoBehaviour
{
    private List<int> terms;
    public List<TMP_Text> resultElementsUI;
    private List<TMP_Text> signElementsUI;
    public List<GameObject> resultBoxes;
    public MatrixObject[] matrixObjects;
    private ButtonsManager buttonsManager;

    //public MatrixObject[] objects;
    public DeterminantType typeOfResult;

    public Health health;
    // I hope it's ok this code is here
    [Header("Flickering of hint button")]
    public SpriteRenderer sprite = null;
    public float flickerDuration;
    public float flickerAmnt;

    public bool updateBox;
    public int boxIndex = 0;

    private Dictionary<string, bool> triangleSequences = new Dictionary<string, bool>
    {
        ["DiagonalPositive"] = false,
        ["UpperTrianglePositive"] = false,
        ["BottomTrianglePositive"] = false,
        ["DiagonalNegative"] = false,
        ["UpperTriangleNegative"] = false,
        ["BottomTriangleNegative"] = false
    };

    IEnumerator DamageFlicker()
    {
        for (int i = 0; i < flickerAmnt; i++)
        {
            sprite.color = new Color(1f, .9647059f, .3215686f, .2f);
            yield return new WaitForSeconds(flickerDuration);
            sprite.color = new Color(1f, 1f, 1f, 0f);
            yield return new WaitForSeconds(flickerDuration);
        }
    }

    private void Awake()
    {
        terms = new List<int>();
        resultElementsUI = FindObjectsOfType<TMP_Text>().Where(x => x.gameObject.tag == "Term").OrderBy(x => x.gameObject.name).ToList();
        signElementsUI = FindObjectsOfType<TMP_Text>().Where(x => x.text == "?").OrderBy(x => x.gameObject.name).ToList();
        //
        resultBoxes = FindObjectsOfType<GameObject>().Where(x => x.gameObject.tag == "Result Box").OrderBy(x => x.gameObject.name).ToList();
        matrixObjects = FindObjectsOfType<MatrixObject>();
        buttonsManager = GetComponent<ButtonsManager>();
        SetMatrixObjectsHiddenPositions();
    }

    private void SetMatrixObjectsHiddenPositions()
    {
        var matrixObjectsByX = matrixObjects.OrderBy(x => x.gameObject.GetComponent<Transform>().position.x);
        int counter = 0;
        foreach(var matrixObject in matrixObjectsByX)
        {
            counter++;
            matrixObject.x = counter / 3;
        }
        var matrixObjectsByY = matrixObjects.OrderByDescending(x => x.gameObject.GetComponent<Transform>().position.y);
        counter = 0;
        foreach (var matrixObject in matrixObjectsByY)
        {
            counter++;
            matrixObject.y = counter / 3;
        }
    }

    private void Update()
    {
        if(Input.GetMouseButtonUp(0))
        {
            var objects = GetChosenMatrixObjects();
            MakeChosenMatrixObjectsActive(objects);
            if (objects.Length <= 1) return;
            if (typeOfResult == DeterminantType.DoubleMatrix) CalculateAnswerForDouble(objects);
            if (typeOfResult == DeterminantType.Triangles) CalculateAnswerForTriangles(objects);
        }
    }

    private void MakeChosenMatrixObjectsActive(MatrixObject[] objects)
    {
        foreach (var matrixObject in objects)
            matrixObject.MakeMatrixObjectActive();
    }

    private void MakeChosenMatrixObjectsBlocked(MatrixObject[] objects)
    {
        foreach (var matrixObject in objects)
            matrixObject.BlockMatrixObject();
    }

    private void UpdateForLeibniz()
    {
        var objects = GetChosenMatrixObjects();
        if (objects.Length == 0) return;
        if (matrixObjects.Where(x => x.currentState == MatrixObjectState.Blocked).Count() == 0)
        {
            CheckForBlockingMatrixObjects(objects);
            return;
        }
    }

    private bool CheckForBlockingMatrixObjects(MatrixObject[] objects)
    {
        if (objects.Length != 5) return false;
        return true;
    }

    private void UpdateUI()
    {
        for (int i = 0; i < terms.Count; i++) 
        { 
            resultElementsUI[i].text = terms[i].ToString();
        }

        if (updateBox)
        {
            if (buttonsManager.signPlus)
                resultBoxes[boxIndex].GetComponent<Image>().color = new Color(0.9333334f, 0.5490196f, 0.5490196f, 1);
            else 
                resultBoxes[boxIndex].GetComponent<Image>().color = new Color(0.5490196f, 0.7960785f, 0.9333334f, 1);

            //resultBoxes[boxIndex].GetComponent<Image>().color = new Color(0.9333334f, 0.5490196f, 0.5490196f, 1);
            Debug.Log("Box Color Changed");
            updateBox = false;
            boxIndex++;
            
        }

        if (signElementsUI.Where(x => x.text == "?").Count() != 0)
            buttonsManager.ActivateSignButtons();
    }

    public void LoseHealth()
    {
        Debug.Log("health--");
        health.healthValue--;
        if (health.healthValue != 0)
            StartCoroutine(DamageFlicker());
    }

    private MatrixObject[] GetChosenMatrixObjects()
    {
        return matrixObjects.Where(x => x.currentState == MatrixObjectState.Chosen).ToArray();
    }

    private void CalculateAnswerForDouble(MatrixObject[] objects)
    {
        if (objects.Length != 2 || objects[0].x == objects[1].x || objects[0].y == objects[1].y)
        {
            LoseHealth();
            return;
        }

        if(terms.Count == 0 && objects.Where(a => a.x == 0 && a.y ==0).Count() == 0)
        {
            LoseHealth();
            return;
        }

        if (terms.Count == 1 && objects.Where(a => a.x == 0 && a.y == 1).Count() == 0)
        {
            LoseHealth();
            return;
        }

        terms.Add(CalculateSimpleDiagonal(objects[0].value, objects[1].value));
        updateBox = true;
        UpdateUI();
        buttonsManager.BLockMatrix();
        if (terms.Count == 2)
            buttonsManager.ActivateResultButton();
    }

    private int CalculateSimpleDiagonal(int first, int second)
    {
        return first * second;
    }

    private int CalculateDoubleMatrixDeterminant()
    {
        return terms[0] - terms[1];
    }

    public void GetResult()
    {
        if (typeOfResult == DeterminantType.DoubleMatrix)
            GetResultForDoubleMatrix();
        if (typeOfResult == DeterminantType.Triangles)
            GetResultForTriangleMatrix();
    }

    private void GetResultForDoubleMatrix()
    {
        terms.Add(CalculateDoubleMatrixDeterminant());
        UpdateUI();
    }

    private void CalculateAnswerForTriangles(MatrixObject[] objects)
    {
        if (objects.Count() == 3)
        {
            if (buttonsManager.signPlus)
            {
                if (objects.Any(a => a.x == 0 && a.y == 0) && objects.Any(a => a.x == 1 && a.y == 1) &&
                    objects.Any(a => a.x == 2 && a.y == 2) && !triangleSequences["DiagonalPositive"])
                {
                    triangleSequences["DiagonalPositive"] = true;
                    terms.Add(CalculateTriangleOperations(objects[0].value, objects[1].value, objects[2].value));
                }
                else if (objects.Any(a => a.x == 1 && a.y == 0) && objects.Any(a => a.x == 2 && a.y == 1) &&
                         objects.Any(a => a.x == 0 && a.y == 2) && !triangleSequences["UpperTrianglePositive"])
                {
                    triangleSequences["UpperTrianglePositive"] = true;
                    terms.Add(CalculateTriangleOperations(objects[0].value, objects[1].value, objects[2].value));
                }
                else if (objects.Any(a => a.x == 2 && a.y == 0) && objects.Any(a => a.x == 0 && a.y == 1) &&
                         objects.Any(a => a.x == 1 && a.y == 2) && !triangleSequences["BottomTrianglePositive"])
                {
                    triangleSequences["BottomTrianglePositive"] = true;
                    terms.Add(CalculateTriangleOperations(objects[0].value, objects[1].value, objects[2].value));
                }
                else
                {
                    LoseHealth();
                    return;
                }
            }
            else if (!buttonsManager.signPlus)
            {
                if (objects.Any(a => a.x == 2 && a.y == 0) && objects.Any(a => a.x == 1 && a.y == 1) &&
                    objects.Any(a => a.x == 0 && a.y == 2) && !triangleSequences["DiagonalNegative"])
                {
                    triangleSequences["DiagonalNegative"] = true;
                    terms.Add(CalculateTriangleOperations(objects[0].value, objects[1].value, objects[2].value));
                }

                else if (objects.Any(a => a.x == 1 && a.y == 0) && objects.Any(a => a.x == 0 && a.y == 1) &&
                         objects.Any(a => a.x == 2 && a.y == 2) && !triangleSequences["UpperTriangleNegative"])
                {
                    triangleSequences["UpperTriangleNegative"] = true;
                    terms.Add(CalculateTriangleOperations(objects[0].value, objects[1].value, objects[2].value));
                }

                else if (objects.Any(a => a.x == 2 && a.y == 1) && objects.Any(a => a.x == 1 && a.y == 2) &&
                         objects.Any(a => a.x == 0 && a.y == 0) && !triangleSequences["BottomTriangleNegative"])
                {
                    triangleSequences["BottomTriangleNegative"] = true;
                    terms.Add(CalculateTriangleOperations(objects[0].value, objects[1].value, objects[2].value));
                }
                else
                {
                    LoseHealth();
                    return;
                }
            }
        }
        else
        {
            LoseHealth();
            return;
        }

        if (terms.Count() == 3)
        {
            UpdateUI();
            buttonsManager.BLockMatrix();
        }

        if (terms.Count == 6)
            buttonsManager.ActivateResultButton();
    }

    private int CalculateTriangleOperations(int first, int second, int third)
    {
        if (!buttonsManager.signPlus)
            return -1 * first * second * third;
        else
            return first * second * third;
    }

    private int CalculateTriangleDeterminant()
    {
        return terms.Sum();
    }

    private void GetResultForTriangleMatrix()
    {
        terms.Add(CalculateTriangleDeterminant());
        UpdateUI();
    }
}
