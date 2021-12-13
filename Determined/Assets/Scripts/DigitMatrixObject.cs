using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DigitMatrixObject : MatrixObject
{
    private TMP_Text textWindow;

    protected override void  Awake()
    {
        base.Awake();
        textWindow = GetComponentInChildren<TMP_Text>();
        textWindow.text = value.ToString();
    }

    public override void ChooseMatrixObject()
    {
        base.ChooseMatrixObject();
        gameObject.GetComponent<SpriteRenderer>().color = Color.red;
    }

    public override void MakeMatrixObjectActive()
    {
        base.MakeMatrixObjectActive();
        gameObject.GetComponent<SpriteRenderer>().color = Color.white;
    }

    public override void BlockMatrixObject()
    {
        base.BlockMatrixObject();
        gameObject.GetComponent<SpriteRenderer>().color = Color.gray;
    }
}
