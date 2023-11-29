using System;
using Unity.Mathematics;
using UnityEngine;

public class BoardManager : Singleton<BoardManager>
{
    private bool isBlackTurn = true;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 clickPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            PlaceStone(clickPos);
            isBlackTurn = !isBlackTurn;
        }
    }

    private void PlaceStone(Vector2 clickedPosition)
    {
        Vector2 gridPosition = GetNearestPointOnGrid(clickedPosition);
        string prefabPath = isBlackTurn ? "Prefabs/R_Black" : "Prefabs/R_White";
        GameObject stonePrefab = Resources.Load<GameObject>(prefabPath);
        Instantiate(stonePrefab, gridPosition, Quaternion.identity);
    }

    private Vector2 GetNearestPointOnGrid(Vector2 clickedPosition)
    {
        int gridX = Mathf.RoundToInt(clickedPosition.x);
        int gridY = Mathf.RoundToInt(clickedPosition.y);

        gridX = Mathf.Clamp(gridX, 0, 14);
        gridY = Mathf.Clamp(gridY, 0, 14);

        Vector2 gridPosition = new Vector2(gridX, gridY);

        return gridPosition;
    }

    //만약 gridX,Y 범위 밖으로 나간다면(Mathf.Clamp)범위를 벗어난다면 Debug.ErrorLog("그곳은 불가능합니다")
    //현재 0,0부터 오목판이 되고있는데, 
}
