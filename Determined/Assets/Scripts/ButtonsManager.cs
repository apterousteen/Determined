using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;

public class ButtonsManager : MonoBehaviour
{
    private List<TMP_Text> signElementsUI;
    private MatrixObject[] matrixObjects;
    private ResultBoard resultBoard;

    public bool signPlus = true;

    private void Awake()
    {
        signElementsUI = FindObjectsOfType<TMP_Text>().Where(x => x.text == "?").OrderBy(x => x.gameObject.name).ToList();
        matrixObjects = FindObjectsOfType<MatrixObject>();
        resultBoard = FindObjectOfType<ResultBoard>();
        BlockSignButtons();
    }

    public void SetMinusSign()
    {
        if (resultBoard.typeOfResult == DeterminantType.Leibniz && resultBoard.terms.Count != 2)
        {
            resultBoard.LoseHealth();
            return;
        }
        if(resultBoard.typeOfResult == DeterminantType.Triangles && resultBoard.triangleSequences["DiagonalNegative"]
            && resultBoard.triangleSequences["UpperTriangleNegative"] && resultBoard.triangleSequences["BottomTriangleNegative"])
        {
            resultBoard.LoseHealth();
            return;
        }
        var uiELement = signElementsUI.Where(x => x.text == "?").First();
        uiELement.text = "-";
        uiELement.color = new Color(0.5490196f, 0.7960785f, 0.9333334f, 1);
        BlockSignButtons();
        ActivateMatrix();

        signPlus = false;
    }

    public void SetPlusSign()
    {
        if(resultBoard.typeOfResult == DeterminantType.DoubleMatrix || 
            (resultBoard.typeOfResult == DeterminantType.Leibniz && resultBoard.terms.Count != 4))
        {
            resultBoard.LoseHealth();
            return;
        }
        if (resultBoard.typeOfResult == DeterminantType.Triangles && resultBoard.triangleSequences["DiagonalPositive"]
            && resultBoard.triangleSequences["UpperTrianglePositive"] && resultBoard.triangleSequences["BottomTrianglePositive"])
        {
            resultBoard.LoseHealth();
            return;
        }
        var uiELement = signElementsUI.Where(x => x.text == "?").First();
        uiELement.text = "+";
        uiELement.color = new Color(0.9333334f, 0.5490196f, 0.5490196f, 1);
        BlockSignButtons();
        ActivateMatrix();

        signPlus = true;
    }

    public void BlockSignButtons()
    {
        var buttons = GameObject.FindGameObjectsWithTag("Sign Button");
        foreach (var button in buttons)
            button.GetComponent<Button>().interactable = false;
    }

    public void ActivateSignButtons()
    {
        var buttons = GameObject.FindGameObjectsWithTag("Sign Button");
        foreach (var button in buttons)
            button.GetComponent<Button>().interactable = true;
    }

    public void ActivateResultButton()
    {
        var line = GameObject.Find("lineBetweenSigns");
        line.gameObject.SetActive(false);

        var buttons = GameObject.FindGameObjectsWithTag("Sign Button");
        foreach (var button in buttons)
            button.SetActive(false);
        Resources.FindObjectsOfTypeAll<Button>()
            .Where(x => x.gameObject.tag == "Result Button")
            .FirstOrDefault().gameObject.SetActive(true);
    }

    public void BLockMatrix()
    {
        foreach (var matrixObject in matrixObjects)
            matrixObject.BlockMatrixObject();
    }

    public void ActivateMatrix()
    {
        foreach (var matrixObject in matrixObjects)
            matrixObject.MakeMatrixObjectActive();
    }
}
