using System;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
    //오목판의 시각적 구성 담당할 부분
    public GameObject cellPrefab;
    public GameObject linePrefab;
    public Transform boardTransform;

    public void CreateGomokuBoard(int width, int height)
    {
        float cellWidth = cellPrefab.GetComponent<RectTransform>().sizeDelta.x;
        float cellHeight = cellPrefab.GetComponent<RectTransform>().sizeDelta.y;
        Vector2 leftBottomPosition = new Vector2(-cellWidth * width / 2, -cellHeight * height / 2);

        for (int i = 0; i < width; i++)
        {
            for ( int j = 0; j < height; j++)
            {
                GameObject cell = Instantiate(cellPrefab, boardTransform);
                cell.name = "Cell_" + i + "_" + j;

                float posX = leftBottomPosition.x + cellWidth * i + cellWidth / 2;
                float posY = leftBottomPosition.y + cellHeight * j + cellHeight / 2;
                cell.GetComponent<RectTransform>().anchoredPosition = new Vector2(posX, posY);
                cell.GetComponent<Image>().color = new Color32(209, 129, 66, 255);
            }
        }

        CreateGridLines(width, height);
    }

    private void CreateGridLines(int width, int height)
    {
        float boardWdith = cellPrefab.GetComponent<RectTransform>().sizeDelta.x * width;
        float boardHeight = cellPrefab.GetComponent<RectTransform>().sizeDelta.y * height;

        Vector2 centerOffset = new Vector2(boardWdith / 2, boardHeight / 2);

        for (int i = 0; i <= width; i++)
        {
            GameObject line = Instantiate(linePrefab, boardTransform);
            line.GetComponent<RectTransform>().anchoredPosition =
                new Vector2((i * cellPrefab.GetComponent<RectTransform>().sizeDelta.x) - centerOffset.x, 0);
            line.GetComponent<RectTransform>().sizeDelta = new Vector2(1, boardHeight);
        }

        for (int j = 0; j <= height; j++)
        {
            GameObject line = Instantiate(linePrefab, boardTransform);
            line.GetComponent<RectTransform>().anchoredPosition =
                new Vector2(0, (j * cellPrefab.GetComponent<RectTransform>().sizeDelta.y) - centerOffset.y);
            line.GetComponent<RectTransform>().sizeDelta = new Vector2(boardWdith, 1);
        }
    }

    //이곳 아래는 나중에 버튼 등을 담당 (지금 구현하지않음)


}
