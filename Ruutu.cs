using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Ruutu : MonoBehaviour
{
    RectTransform ruutuRect;
    RectTransform kuvaRect;
    public Image kuva;
    public TMP_Text teksti;


    private void Awake()
    {
        ruutuRect = GetComponent<RectTransform>();
        kuvaRect = kuva.GetComponent<RectTransform>();
    }

    public void UpdateValue(int v)
    {
        //updating object visually by it's new value
        //if value is 0, there is no object
        if(v > 0)
        {
            teksti.text = v.ToString();
        }
        else
        {
            teksti.text = "";
        }

        kuva.color = Ruudukko.Instance.GridColor(v);
    }

    public void UpdateRects(int x, int y)
    {
        float size = Ruudukko.Instance.gridSize;
        ruutuRect.anchoredPosition = new Vector2(x*size, y*size);
        kuvaRect.anchoredPosition = new Vector2(size / 2, size / 2);
        kuvaRect.sizeDelta = new Vector2(size * 0.98f, size * 0.98f);
        teksti.GetComponent<RectTransform>().sizeDelta = kuvaRect.sizeDelta * 0.8f;
    }
}
