using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell
{

    public int x;
    public int y;
    public float f = int.MaxValue;
    public float g = int.MaxValue;
    public float h = int.MaxValue;

    public Cell previousCell = null;

    public Cell(int x, int y) {
        this.x = x;
        this.y = y;
    }
    public void CalculateF() {
        f = g + h;
    }
}
