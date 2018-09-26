using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionClickDetector : MonoBehaviour
{
    public int positionId = 0;

    private void OnMouseDown()
    {
        BoardManager boardManager = GetComponentInParent<BoardManager>();
        boardManager.ClickBehavior(positionId);
    }
}
