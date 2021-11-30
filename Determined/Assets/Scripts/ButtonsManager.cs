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

    private void Awake()
    {
        signElementsUI = FindObjectsOfType<TMP_Text>().Where(x => x.text == "?").OrderBy(x => x.gameObject.name).ToList();
        matrixObjects = FindObjectsOfType<MatrixObject>();
        BlockSignButtons();
    }

    public void SetMinusSign()
    {
        var uiELement = signElementsUI.Where(x => x.text == "?").First();
        uiELement.text = "-";
        uiELement.color = Color.blue;
        BlockSignButtons();
    }

    public void SetPlusSign()
    {
        var uiELement = signElementsUI.Where(x => x.text == "?").First();
        uiELement.text = "+";
        uiELement.color = Color.red;
        BlockSignButtons();
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
}
