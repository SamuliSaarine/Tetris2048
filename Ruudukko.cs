using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class Ruudukko : MonoBehaviour
{
    public static Ruudukko Instance;

    public int width;
    public int height;

    public GameObject gridObject;

    public float fallSpeed;
    public float slideSpeed;

    public StartColor[] colors;

    private int score = 0;
    public TMP_Text scoreText;

    [HideInInspector]
    public RuutuData[,] ruudut;

    public float gridSize { get; private set; }

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        CalcGridSize();
        InitScoreText();
        GenerateGrids();
        scoreText.text = "0";
        InvokeRepeating("Spawn", 0f, 2f);
    }

    void Spawn()
    {
        //Choosing random slot from top row of the grid to spawn a new number object
        RuutuData spawnRuutu = ruudut[Random.Range(0, width), height - 1];

        if (spawnRuutu.Value != 0) GameOver();

        //Getting value for number object and adding that to players score
        int spawnValue = SpawnValue();
        spawnRuutu.Value = spawnValue;
        score += spawnValue;

        scoreText.text = score.ToString();
    }

    int SpawnValue()
    {
        int random = Random.Range(0, 6);

        if (random > 4) return 4;
        else if (random > 2) return 2;
        else return 1;
    }

    void GameOver()
    {
        //Restarting the game
        Debug.Log("Game Over!");
        SceneManager.LoadScene(0);
    }

    void InitScoreText()
    {
        //fitting score text to screen size
        float rectWidth = Screen.width * 0.9f;
        scoreText.rectTransform.sizeDelta = new Vector2(rectWidth, rectWidth / 4f);
        scoreText.rectTransform.anchoredPosition = new Vector2(0, -rectWidth / 5f);
    }

    void CalcGridSize()
    {
        //calculating grid size that it fit screen well
        RectTransform rect = GetComponent<RectTransform>();
        float layWidth = Screen.width*0.95f;
        gridSize = layWidth / width;
        float layHeight = gridSize * height;

        if(layHeight > Screen.height * 0.7f)
        {
            layHeight = Screen.height * 0.7f;
            gridSize = layHeight / height;
            layWidth = gridSize * width;        
        }

        rect.sizeDelta = new(layWidth ,layHeight);
    }

    void GenerateGrids()
    {
        ruudut = new RuutuData[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                //creating a new slot
                ruudut[x, y] = new();
                RuutuData r = ruudut[x, y];
                r.ruutu = Instantiate(gridObject, transform).GetComponent<Ruutu>();
                r.ruutu.UpdateRects(x, y);
                r.pos = new(x, y);
            }
        }

        foreach (var ruutu in ruudut)
        {
            ruutu.InitNeigbours();
        }
    }

    public void Slide(bool right)
    {
        Debug.Log("Sliding " + (right ? "Right" : "Left"));

        //sliding all numbers to the direction swiped
        if(right)
        {
            for (int x = width -1; x >= 0; x--)
            {
                for (int y = 0; y < height; y++)
                {
                    RuutuData r = ruudut[x, y];

                    if (r.Value != 0)
                    {
                        r.sliding = 1;
                        r.CheckMovement(1);
                    }
                }
            }
        }
        else
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    RuutuData r = ruudut[x, y];

                    if (r.Value != 0)
                    {
                        r.sliding = -1;
                        r.CheckMovement(3);
                    }
                }
            }
        }
    }

    public void StartMove(RuutuData r, int i)
    {
        StartCoroutine(Move(r, i));
    }

    public IEnumerator Move(RuutuData r, int neigbourIndex)
    {
        float moveSpeed = neigbourIndex == 2 ? fallSpeed : slideSpeed;
        yield return new WaitForSeconds(1 / moveSpeed);

        RuutuData n = r.neigbours[neigbourIndex];

        if(r.Value == 0)
        {
            //we are not doing anything to empty slots
            r.sliding = 0;
            yield break;
        }

        if (n.Value != 0)
        {
            if (n.Value != r.Value)
            {
                //object can't move if there is another object with different value in direction
                if(neigbourIndex != 2)
                {
                    r.sliding = 0;
                }
                yield break;
            }
        }

        if (neigbourIndex == 3)
        {
            n.sliding = -1;
        }
        else if (neigbourIndex == 1)
        {
            n.sliding = 1;
        }

        //adding object value to slot moved to and removing it from slot moved from
        n.Value += r.Value;
        r.Value = 0;
    }

    public RuutuData GetRuutuData(Vector2Int pos)
    {
        int x = pos.x;
        int y = pos.y;

        if(x < 0 || x >= width || y < 0 || y >= height)
        {
            return null;
        }
        else
        {
            return ruudut[x, y];
        }
    }

    public Color GridColor(int v)
    {
        //Calculating color of object
        for (int i = 0; i < colors.Length; i++)
        {
            StartColor sc = colors[i];
            if(v <= sc.limit)
            {
                Color c = sc.color;
                float change = (sc.limit - v) / (sc.changeAmount*100);
                if (c.r > 0.1f) c.r -= change;
                if (c.g > 0.1f) c.g -= change;
                if (c.b > 0.1f) c.b -= change;
                return c;
            }
        }

        return colors[0].color;
    }
}

[System.Serializable]
public class StartColor
{
    public int limit = 64;
    public Color color = new Color(40,40,40,255);
    public float changeAmount = 1;
}
