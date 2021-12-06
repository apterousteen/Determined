using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

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
    }

    private void Update()
    {
        if(Input.GetMouseButtonUp(0))
        {
            var objects = GetChosenMatrixObjects();
            foreach (var matrixObject in objects)
                matrixObject.MakeMatrixObjectActive();
            if (objects.Length == 0) Debug.Log("objects not captured");
            CalculateAnswerForDouble(objects);
        }
    }

    private void UpdateUI()
    {
        for (int i = 0; i < terms.Count; i++)
            resultElementsUI[i].text = terms[i].ToString();
        if (signElementsUI.Where(x => x.text == "?").Count() != 0)
            buttonsManager.ActivateSignButtons();
    }

    private MatrixObject[] GetChosenMatrixObjects()
    {
        return matrixObjects.Where(x => x.currentState == MatrixObjectState.Chosen).ToArray();
    }

    private void CalculateAnswerForDouble(MatrixObject[] objects)
    {
        if (objects.Length != 2 && objects.Length != 0)
        {
            Debug.Log("health--");
            health.healthValue--;
            return;
        }

        terms.Add(CalculateSimpleDiagonal(objects[0].value, objects[1].value));
        UpdateUI();
        if (terms.Count == 2)
        {
            buttonsManager.BLockMatrix();
            buttonsManager.ActivateResultButton();
        }
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
