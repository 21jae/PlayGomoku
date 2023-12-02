using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InGameConsole : MonoBehaviour
{
    public Text consoleText; // 인스펙터에서 할당
    private Queue<string> logQueue = new Queue<string>();

    private void OnEnable()
    {
        Application.logMessageReceived += HandleLog;
    }

    private void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
    }

    void HandleLog(string logString, string stackTrace, LogType type)
    {
        // 새 로그 메시지를 큐에 추가
        logQueue.Enqueue(logString);

        // 콘솔에 너무 많은 텍스트가 있으면 오래된 것부터 제거
        while (logQueue.Count > 50) // 50은 보관할 로그의 최대 수
        {
            logQueue.Dequeue();
        }

        consoleText.text = string.Join("\n", logQueue.ToArray());
    }

    // Player2가 돌을 놓을 수 있는지 확인하는 메서드 추가 (예시)
    public void CheckPlayer2CanPlaceStone(bool canPlace)
    {
        string message = canPlace ? "Player 2 can place a stone" : "Player 2 cannot place a stone";
        Debug.Log(message); // 이 메시지가 인게임 콘솔에 표시됩니다.
    }
}
