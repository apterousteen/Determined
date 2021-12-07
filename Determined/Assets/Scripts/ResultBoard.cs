using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using System;

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
    public MatrixObject[] matrixObjects;
    private ButtonsManager buttonsManager;

    //public MatrixObject[] objects;
    public DeterminantType typeOfResult;

    public Health health;

    private void Awake()
    {
        terms = new List<int>();
        resultElementsUI = FindObjectsOfType<TMP_Text>().Where(x => x.gameObject.tag == "Term").OrderBy(x => x.gameObject.name).ToList();
        signElementsUI = FindObjectsOfType<TMP_Text>().Where(x => x.text == "?").OrderBy(x => x.gameObject.name).ToList();
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
            if (typeOfResult == DeterminantType.DoubleMatrix) UpdateForDoubleMatrix();

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

    private void UpdateForDoubleMatrix()
    {
        var objects = GetChosenMatrixObjects();
        if (objects.Length == 0) return;
        MakeChosenMatrixObjectsActive(objects);
        CalculateAnswerForDouble(objects);
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
            resultElementsUI[i].text = terms[i].ToString();
        if (signElementsUI.Where(x => x.text == "?").Count() != 0)
            buttonsManager.ActivateSignButtons();
    }

    public void LoseHealth()
    {
        Debug.Log("health--");
        health.healthValue--;
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
    }

    private void GetResultForDoubleMatrix()
    {
        terms.Add(CalculateDoubleMatrixDeterminant());
        UpdateUI();
    }
}
