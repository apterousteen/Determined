using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DigitMatrixObject : MatrixObject
{
    private TMP_Text textWindow;

    public List<TMP_Text> matrixText;

    protected override void  Awake()
    {
        base.Awake();
        textWindow = GetComponentInChildren<TMP_Text>();
        textWindow.text = value.ToString();
    }
}
