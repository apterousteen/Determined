using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class SnapScroll : MonoBehaviour
{
    [Header("Controllers")]
    public int panCount = 3;
    [Range(0, 500)]
    public int panOffset;
    [Range(0f, 20f)]
    public float snapSpeed;
    [Range(0f, 10f)]
    public float scaleOffset;
    [Range(1f, 20f)]
    public float scaleSpeed;
    [Header("Other Objects")]
    public GameObject panPrefab;
 
    public GameObject[] instPans;
    private Vector2[] pansPos;
    private Vector2[] pansScale;
 
    private RectTransform contentRect;
    private Vector2 contentVector;
 
    private int selectedPanID;
    private bool isScrolling;

    public List<GameObject> dots;

    private void Start()
    {
        contentRect = GetComponent<RectTransform>();
        pansPos = new Vector2[panCount];
        pansScale = new Vector2[panCount];
        for (int i = 0; i < panCount; i++)
        {
            if (i == 0) continue;
            instPans[i].transform.localPosition = new Vector2(instPans[i - 1].transform.localPosition.x + panPrefab.GetComponent<RectTransform>().sizeDelta.x + panOffset,
                instPans[i].transform.localPosition.y);
            pansPos[i] = -instPans[i].transform.localPosition;
        }
    }

    private void Update()
    {
        float nearestPos = float.MaxValue;
        for (int i = 0; i < panCount; i++)
        {
            float distance = Mathf.Abs(contentRect.anchoredPosition.x - pansPos[i].x);
            if (distance < nearestPos)
            {
                nearestPos = distance;
                selectedPanID = i;
            }
            float scale = Mathf.Clamp(1 / (distance / panOffset) * scaleOffset, 0.8f, 1f);
            pansScale[i].x = Mathf.SmoothStep(instPans[i].transform.localScale.x, scale, scaleSpeed * Time.fixedDeltaTime);
            pansScale[i].y = Mathf.SmoothStep(instPans[i].transform.localScale.y, scale, scaleSpeed * Time.fixedDeltaTime);
            instPans[i].transform.localScale = pansScale[i];
        }

        dots[selectedPanID].GetComponent<Image>().color = new Color(1, 1, 1, 0.8f);
        var whiteDots = dots.Where((v, i) => i != selectedPanID).ToList();
        foreach (var dot in whiteDots)
        {
            dot.GetComponent<Image>().color = new Color(0.08235294f, 0.2392157f, 0.3647059f, 1);
        }

        if (isScrolling) return;
        contentVector.x = Mathf.SmoothStep(contentRect.anchoredPosition.x, pansPos[selectedPanID].x, snapSpeed * Time.fixedDeltaTime);
        contentRect.anchoredPosition = contentVector;
    }
 
    public void Scrolling(bool scroll)
    {
        isScrolling = scroll;
    }
}
