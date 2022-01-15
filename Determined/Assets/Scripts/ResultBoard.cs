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
    public bool ColoredLevel;
    public bool hinted;
    public List<Image> coloredAnswers;

    public List<int> terms;
    public List<TMP_Text> resultElementsUI;
    private List<TMP_Text> signElementsUI;
    public List<GameObject> resultBoxes;
    public MatrixObject[] matrixObjects;
    private ButtonsManager buttonsManager;
    private SimplerLineController lineController;

    
    public DeterminantType typeOfResult;

    public Health health;
    [Header("Flickering of hint button")]
    public SpriteRenderer sprite = null;
    public float flickerDuration;
    public float flickerAmnt;

    [Header("Corner Triangles")]
    [SerializeField] private Sprite DiagonalPositive = null;
    [SerializeField] private Sprite DiagonalNegative = null;
    [SerializeField] private Sprite UpperTrianglePositive = null;
    [SerializeField] private Sprite BottomTrianglePositive = null;
    [SerializeField] private Sprite UpperTriangleNegative = null;
    [SerializeField] private Sprite BottomTriangleNegative = null;

    public bool updateBox;
    public int boxIndex = 0;
    private bool twoTermsCalculated = false;
    public bool levelWasWon = false;

    private int xElementLocation;
    private int yElementLocation;
    private bool isXLineSelected = false;
    private bool isYLineSelected = false;
    private int coefficient = 0;
    private int leibnizSupportPositive = 0;
    private int leibnizSupportNegative = 0;
    public int[] leibnizX;
    public int[] leibnizY;
    private int leibnizSupportCount = 0;
    private List<(int, int)> usedAlgebraicComplements = new List<(int, int)>();

    private string cornerTriangleName;

    public Dictionary<string, bool> triangleSequences = new Dictionary<string, bool>
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
        if(ColoredLevel)
        {
            coloredAnswers = FindObjectsOfType<Image>().Where(x => x.gameObject.tag == "Term").OrderBy(x => x.gameObject.name).ToList();
            if (hinted)
                coloredAnswers[0].color = new Color(255, 255, 255, 0.6f);
        }
        else resultElementsUI = FindObjectsOfType<TMP_Text>().Where(x => x.gameObject.tag == "Term").OrderBy(x => x.gameObject.name).ToList();
        signElementsUI = FindObjectsOfType<TMP_Text>().Where(x => x.text == "?").OrderBy(x => x.gameObject.name).ToList();
        
        resultBoxes = FindObjectsOfType<GameObject>().Where(x => x.gameObject.tag == "Result Box").OrderBy(x => x.gameObject.name).ToList();
        matrixObjects = FindObjectsOfType<MatrixObject>();
        buttonsManager = GetComponent<ButtonsManager>();
        lineController = FindObjectOfType<SimplerLineController>();
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
            if (typeOfResult == DeterminantType.Leibniz &&
                matrixObjects.Where(a => a.currentState == MatrixObjectState.Blocked).Count() == 5) CalculateAnswerForLeibnizForDouble(objects);
            else if (typeOfResult == DeterminantType.Leibniz) CalculateAnswerForLeibniz(objects);
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
    
    private void MakeBlockedMatrixObjectsActive()
    {
        foreach (var matrixObject in matrixObjects)
            matrixObject.MakeMatrixObjectActive();
        lineController.ClearLines();
    }

    private bool CheckForBlockingMatrixObjects(MatrixObject[] objects)
    {
        if (objects.Length != 5) return false;
        return true;
    }

    private void ShowAnswer(int i)
    {
        if (ColoredLevel)
        {
            coloredAnswers[i].color = new Color(255, 255, 255, 100);
            if (hinted && i < coloredAnswers.Count - 2)
                coloredAnswers[i + 1].color = new Color(255, 255, 255, 0.6f);
        }
        else resultElementsUI[i].text = terms[i].ToString();
    }

    private void UpdateUI()
    {
        for (int i = 0; i < terms.Count; i++) 
        {
            ShowAnswer(i);
        }

        if (updateBox)
        {
            if (buttonsManager.signPlus)
                resultBoxes[boxIndex].GetComponent<Image>().color = new Color(0.9333334f, 0.5490196f, 0.5490196f, 1);
            else 
                resultBoxes[boxIndex].GetComponent<Image>().color = new Color(0.5490196f, 0.7960785f, 0.9333334f, 1);

            Debug.Log("Box Color Changed");

            if (cornerTriangleName == "DiagonalPositive")
            {
                resultBoxes[boxIndex].transform.GetChild(1).GetComponent<Image>().color = new Color(1, 1, 1, 0.8f);
                resultBoxes[boxIndex].transform.GetChild(1).GetComponent<Image>().sprite = DiagonalPositive;
            }
            else if (cornerTriangleName == "DiagonalNegative")
            {
                resultBoxes[boxIndex].transform.GetChild(1).GetComponent<Image>().color = new Color(1, 1, 1, 0.8f);
                resultBoxes[boxIndex].transform.GetChild(1).GetComponent<Image>().sprite = DiagonalNegative;
            }
            else if (cornerTriangleName == "UpperTrianglePositive")
            {
                resultBoxes[boxIndex].transform.GetChild(1).GetComponent<Image>().color = new Color(1, 1, 1, 0.8f);
                resultBoxes[boxIndex].transform.GetChild(1).GetComponent<Image>().sprite = UpperTrianglePositive;
            }

            else if (cornerTriangleName == "UpperTriangleNegative")
            {
                resultBoxes[boxIndex].transform.GetChild(1).GetComponent<Image>().color = new Color(1, 1, 1, 0.8f);
                resultBoxes[boxIndex].transform.GetChild(1).GetComponent<Image>().sprite = UpperTriangleNegative;
            }

            else if (cornerTriangleName == "BottomTrianglePositive")
            {
                resultBoxes[boxIndex].transform.GetChild(1).GetComponent<Image>().color = new Color(1, 1, 1, 0.8f);
                resultBoxes[boxIndex].transform.GetChild(1).GetComponent<Image>().sprite = BottomTrianglePositive;
            }

            else if (cornerTriangleName == "BottomTriangleNegative")
            {
                resultBoxes[boxIndex].transform.GetChild(1).GetComponent<Image>().color = new Color(1, 1, 1, 0.8f);
                resultBoxes[boxIndex].transform.GetChild(1).GetComponent<Image>().sprite = BottomTriangleNegative;
            }

            updateBox = false;
            boxIndex++;
            
        }

        if (signElementsUI.Where(x => x.text == "?").Count() != 0 &&
            (typeOfResult != DeterminantType.Leibniz || twoTermsCalculated))
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
        if (typeOfResult == DeterminantType.Leibniz)
            GetResultForLeibnizMatrix();
    }

    private void GetResultForDoubleMatrix()
    {
        levelWasWon = true;
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

                    cornerTriangleName = triangleSequences.Keys.First();
                }
                else if (objects.Any(a => a.x == 1 && a.y == 0) && objects.Any(a => a.x == 2 && a.y == 1) &&
                         objects.Any(a => a.x == 0 && a.y == 2) && !triangleSequences["UpperTrianglePositive"])
                {
                    triangleSequences["UpperTrianglePositive"] = true;
                    terms.Add(CalculateTriangleOperations(objects[0].value, objects[1].value, objects[2].value));

                    cornerTriangleName = triangleSequences.ElementAt(1).Key;
                }
                else if (objects.Any(a => a.x == 2 && a.y == 0) && objects.Any(a => a.x == 0 && a.y == 1) &&
                         objects.Any(a => a.x == 1 && a.y == 2) && !triangleSequences["BottomTrianglePositive"])
                {
                    triangleSequences["BottomTrianglePositive"] = true;
                    terms.Add(CalculateTriangleOperations(objects[0].value, objects[1].value, objects[2].value));

                    cornerTriangleName = triangleSequences.ElementAt(2).Key;
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

                    cornerTriangleName = triangleSequences.ElementAt(3).Key;
                }

                else if (objects.Any(a => a.x == 1 && a.y == 0) && objects.Any(a => a.x == 0 && a.y == 1) &&
                         objects.Any(a => a.x == 2 && a.y == 2) && !triangleSequences["UpperTriangleNegative"])
                {
                    triangleSequences["UpperTriangleNegative"] = true;
                    terms.Add(CalculateTriangleOperations(objects[0].value, objects[1].value, objects[2].value));

                    cornerTriangleName = triangleSequences.ElementAt(4).Key;
                }

                else if (objects.Any(a => a.x == 2 && a.y == 1) && objects.Any(a => a.x == 1 && a.y == 2) &&
                         objects.Any(a => a.x == 0 && a.y == 0) && !triangleSequences["BottomTriangleNegative"])
                {
                    triangleSequences["BottomTriangleNegative"] = true;
                    terms.Add(CalculateTriangleOperations(objects[0].value, objects[1].value, objects[2].value));

                    cornerTriangleName = triangleSequences.ElementAt(5).Key;
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
        updateBox = true;
        UpdateUI();
        buttonsManager.BLockMatrix();

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
        levelWasWon = true;
        var sum = terms[0];
        var signs = FindObjectsOfType<TMP_Text>().Where(x => x.gameObject.tag == "Sign").OrderBy(x => x.gameObject.name).ToList();
        for (int i = 0; i < signs.Count; i++)
        {
            var modificator = 1;
            if (signs[i].text == "-")
                modificator = -1;
            sum += terms[i + 1] * modificator;
        }
        return sum;
    }

    private void GetResultForTriangleMatrix()
    {
        terms.Add(CalculateTriangleDeterminant());
        UpdateUI();
    }
    
    private void CalculateAnswerForLeibniz(MatrixObject[] objects)
    {
        int tempX = 0, tempY = 0;
        int tempXMax = 0, tempYMax = 0;
        int xMaxPos = 0, yMaxPos = 0;

        twoTermsCalculated = false;
        if (objects.Count() == 3)
        {
            if (objects.All(a => a.x == objects[0].x) && !isXLineSelected)
            {
                xElementLocation = objects[0].x;
                isXLineSelected = true;
                lineController.AddPoints(objects);
                MakeChosenMatrixObjectsBlocked(objects);
            }
            else if (objects.All(a => a.y == objects[0].y) && !isYLineSelected)
            {
                yElementLocation = objects[0].y;
                isYLineSelected = true;
                lineController.AddPoints(objects);
                MakeChosenMatrixObjectsBlocked(objects);
            }
            else
            {
                LoseHealth();
                return;
            }
        }
        else if (objects.Count() == 2 && isXLineSelected && !isYLineSelected)
        {
            if (objects.All(a => a.y == objects[0].y))
            {
                yElementLocation = objects[0].y;
                var cornerValue = matrixObjects.Where(a => a.currentState == MatrixObjectState.Blocked).First(a => a.x == xElementLocation && a.y == yElementLocation);
                isYLineSelected = true;
                lineController.AddPoints(objects, cornerValue);
                if (CheckForDuplicates())
                {
                    LoseHealth();
                    MakeBlockedMatrixObjectsActive();
                    isXLineSelected = false;
                    isYLineSelected = false;
                    return;
                }
                coefficient = cornerValue.value;
                terms.Add(coefficient);
                updateBox = true;
                MakeChosenMatrixObjectsBlocked(objects);
            }
            else
            {
                LoseHealth();
                return;
            }
        }
        else if (objects.Count() == 2 && isYLineSelected && !isXLineSelected)
        {
            if (objects.All(a => a.x == objects[0].x))
            {
                xElementLocation = objects[0].x;
                var cornerValue = matrixObjects.Where(a => a.currentState == MatrixObjectState.Blocked).First(a => a.x == xElementLocation && a.y == yElementLocation);
                isXLineSelected = true;
                lineController.AddPoints(objects, cornerValue);
                if (CheckForDuplicates())
                {
                    LoseHealth();
                    MakeBlockedMatrixObjectsActive();
                    isXLineSelected = false;
                    isYLineSelected = false;
                    return;
                }
                coefficient = cornerValue.value;
                terms.Add(coefficient);
                updateBox = true;
                MakeChosenMatrixObjectsBlocked(objects);
            }
            else
            {
                LoseHealth();
                return;
            }
        }
        else if (objects.Count() == 5)
        {
            foreach (var e in objects)
            {
                tempX = objects.Where(a => a.x == e.x).Count();
                tempY = objects.Where(a => a.y == e.y).Count();
                if (tempX > tempXMax)
                {
                    tempXMax = tempX;
                    xMaxPos = e.x;
                }
                if (tempY > tempYMax)
                {
                    tempYMax = tempY;
                    yMaxPos = e.y;
                }
            }

            if (tempXMax == 3 && tempYMax == 3)
            {
                var cornerValue = objects.First(a => a.x == xMaxPos && a.y == yMaxPos);
                xElementLocation = xMaxPos;
                yElementLocation = yMaxPos;
                if (CheckForDuplicates())
                {
                    LoseHealth();
                    isXLineSelected = false;
                    isYLineSelected = false;
                    return;
                }
                else
                {
                    coefficient = cornerValue.value;
                    updateBox = true;
                    terms.Add(coefficient);
                    lineController.AddPoints(objects, cornerValue);
                    MakeChosenMatrixObjectsBlocked(objects);
                }
            }
            else
            {
                LoseHealth();
                return;
            }
        }
        else
        {
            LoseHealth();
            return;
        }
        UpdateUI();
    }

    private bool CheckForDuplicates()
    {
        if (leibnizX.Length == 0)
            return usedAlgebraicComplements.Count() != 0 &&
            usedAlgebraicComplements.Any(a => a.Item1 == xElementLocation && a.Item2 == yElementLocation);
        var rightLines = false;
        for(int i = 0; i < 3; i++)
        {
            if (leibnizX[i] == xElementLocation && leibnizY[i] == yElementLocation)
            {
                rightLines = true;
                Debug.Log("Good lines");
            }
        }
        return !rightLines || usedAlgebraicComplements.Count() != 0 &&
            usedAlgebraicComplements.Any(a => a.Item1 == xElementLocation && a.Item2 == yElementLocation);
    }

    private void CalculateAnswerForLeibnizForDouble(MatrixObject[] objects)
    {
        if (objects.Count() == 2)
        {
            if (objects[0].x != objects[1].x && objects[0].y != objects[1].y)
            {
                if (leibnizSupportCount < 2)
                {
                    objects = objects.OrderBy(that => that.y).ToArray();
                    if(objects[0].x < objects[1].x)
                    {
                        leibnizSupportPositive = CalculateSimpleDiagonal(objects[0].value, objects[1].value);
                        leibnizSupportCount++;

                    }
                    else
                    {
                        leibnizSupportNegative = CalculateSimpleDiagonal(objects[0].value, objects[1].value);
                        leibnizSupportCount++;
                    }
                }

                if (leibnizSupportCount == 2)
                {
                    leibnizSupportCount = 0;
                    terms.Add((leibnizSupportPositive - leibnizSupportNegative));
                    twoTermsCalculated = true;
                    usedAlgebraicComplements.Add((xElementLocation, yElementLocation));
                    isXLineSelected = false;
                    isYLineSelected = false;
                    
                    updateBox = true;
                    //lineController.ClearLines();
                    UpdateUI();

                    if (terms.Count() == 6)
                    {
                        MakeBlockedMatrixObjectsActive();
                        buttonsManager.ActivateResultButton(); //Null Reference Exception
                    }
                    else if (terms.Count() != 6)
                    {
                        MakeBlockedMatrixObjectsActive();
                        buttonsManager.ActivateSignButtons();
                        buttonsManager.BLockMatrix();
                    }
                }
            }
            else
            {
                LoseHealth();
                return;
            }
        }
        else
        {
            LoseHealth();
            return;
        }
    }

    private int CalculateLeibnizDeterminant()
    {
        levelWasWon = true;
        var sum = terms[0];
        var signs = FindObjectsOfType<TMP_Text>().Where(x => x.gameObject.tag == "Sign").OrderBy(x => x.gameObject.name).ToList();
        for (int i = 0; i < signs.Count; i++)
        {
            var modificator = 1;
            if (signs[i].text == "-")
                modificator = -1;
            sum += terms[i + 1] * modificator;
        }
        return sum;
    }

    private void GetResultForLeibnizMatrix()
    {
        var secondPanel = this.gameObject.transform.Find("Second Panel").gameObject;
        if (secondPanel.activeSelf)
            terms.Add(CalculateLeibnizDeterminant());
        else
        {
            var temp = new List<int>();
            for (int i = 0; i < terms.Count(); i += 2)
                temp.Add(terms[i] * terms[i + 1]);
            terms = temp;
            var signs = FindObjectsOfType<TMP_Text>().Where(x => x.gameObject.tag == "Sign").OrderBy(x => x.gameObject.name).ToList();
            this.gameObject.transform.Find("First Panel").gameObject.SetActive(false);
            secondPanel.SetActive(true);
            var newSigns = FindObjectsOfType<TMP_Text>().Where(x => x.gameObject.tag == "Sign").OrderBy(x => x.gameObject.name).ToList();
            if(ColoredLevel)
                coloredAnswers = FindObjectsOfType<Image>().Where(x => x.gameObject.tag == "Term").OrderBy(x => x.gameObject.name).ToList();
            else resultElementsUI = FindObjectsOfType<TMP_Text>().Where(x => x.gameObject.tag == "Term").OrderBy(x => x.gameObject.name).ToList();
            for (int i = 0; i < signs.Count; i++)
                newSigns[i].text = signs[i].text;
            resultBoxes = FindObjectsOfType<GameObject>().Where(x => x.gameObject.tag == "Result Box").OrderBy(x => x.gameObject.name).ToList();
            for (int i = 0; i < resultBoxes.Count; i++)
                if (i == 0)
                    resultBoxes[i].GetComponent<Image>().color = new Color(0.9333334f, 0.5490196f, 0.5490196f, 1);
                else if (newSigns[i - 1].text == "+")
                    resultBoxes[i].GetComponent<Image>().color = new Color(0.9333334f, 0.5490196f, 0.5490196f, 1);
                else
                    resultBoxes[i].GetComponent<Image>().color = new Color(0.5490196f, 0.7960785f, 0.9333334f, 1);

        }
        UpdateUI(); //Null Reference Exception
    }
}
