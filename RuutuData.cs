using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RuutuData
{
    public Ruutu ruutu;
    public Vector2Int pos;
    public RuutuData[] neigbours = new RuutuData[4];
    public int sliding = 0;

    public void InitNeigbours()
    {
        //getting neigbour slots of this slot
        for (int i = 0; i < neigbours.Length; i++)
        {
            neigbours[i] = Ruudukko.Instance.GetRuutuData(pos + neigbourCheck[i]);
        }

    }

    public Vector2Int[] neigbourCheck = new Vector2Int[4]
    {
        new Vector2Int(0,1),
        new Vector2Int(1,0),
        new Vector2Int(0,-1),
        new Vector2Int(-1,0),
    };

    int v = 0;
    public int Value
    {
        get { return v; }
        set
        {
            if(v != value)
            {
                v = value;
                ruutu.UpdateValue(value);
                if (sliding == 1)
                {
                    //sliding right
                    CheckMovement(1);
                }
                else if (sliding == -1)
                {
                    //sliding left
                    CheckMovement(3);
                }
                sliding = 0;
                //falling
                CheckMovement(2);
            }
        }
    }

    public void CheckMovement(int dir)
    {
        int counterDir = dir > 1 ? dir - 2 : dir + 2;

        //moving object only if there is object(if value is 0 there is no object)
        if (v > 0)
        {
            if (neigbours[dir] != null)
            {
                Ruudukko.Instance.StartMove(this, dir);
            }

            if (neigbours[counterDir] != null && dir == 2)
            {
                if (neigbours[counterDir].Value > 0) Ruudukko.Instance.StartMove(neigbours[counterDir], dir);
            }
        }
        else if(neigbours[counterDir] != null)
        {
            sliding = 0;
            if (neigbours[counterDir].Value > 0) Ruudukko.Instance.StartMove(neigbours[counterDir], dir);
        }
        else
        {
            sliding = 0;
        }
    }


}
