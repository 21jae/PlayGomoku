using System;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    //그리드 격자를 15x15로 생성한다.

    //그리드의 크기는 GameObject(SpriteRenderer) 바둑판 이미지에 맞춰 생성한다.
    public SpriteRenderer boardRenderer;    //바둑판 spr
    private Vector2[,] gridPosition;    //격자

    private void Awake()
    {
        InitializeGrid();
    }

    #region Init
    private void InitializeGrid()
    {
        int gridSize = 15;
        gridPosition = new Vector2[gridSize, gridSize];

        //바둑판 이미지 측정
        float boardWidth = boardRenderer.bounds.size.x;
        float boardHeight = boardRenderer.bounds.size.y;

        //각 격자 사이의 간격 계산
        float cellWidth = boardWidth / gridSize;
        float cellHeight = boardHeight / gridSize;

        Vector3 boardPosition = boardRenderer.transform.position;

        for (int i = 0; i < gridSize;  i++)
        {
            for (int j = 0; j < gridSize; j++)
            {
                float posX = boardPosition.x + (i * cellWidth) - (boardWidth / 2) + (cellWidth / 2);
                float posY = boardPosition.y + (j * cellHeight) - (boardHeight / 2) + (cellHeight / 2);
                gridPosition[i, j] = new Vector2(posX, posY);
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (gridPosition != null)
        {
            Gizmos.color = Color.red;

            int gridSize = gridPosition.GetLength(0);
            for (int i = 0; i < gridSize; i++)
            {
                for (int j = 0; j < gridSize; j++)
                {
                    Gizmos.DrawSphere(new Vector3(gridPosition[i, j].x, gridPosition[i, j].y, 0), 0.1f);
                }
            }
        }
    }
    #endregion

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            PlaceStoneAtMousePosition();
        }
    }

    private void PlaceStoneAtMousePosition()
    {
        Vector2 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        int closestX = -1;
        int closestY = -1;
    }
}
