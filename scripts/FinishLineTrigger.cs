using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishLineTrigger : MonoBehaviour
{
    
    private bool hasBeenTriggered = false;
    

    void OnTriggerEnter(Collider other)
    {
        // Kiểm tra xem có phải người chơi đã va chạm không
        if (!hasBeenTriggered && other.CompareTag("Player")) 
        {
            hasBeenTriggered = true; // Đánh dấu đã kích hoạt
            GameManagerLevel gameTimer = FindObjectOfType<GameManagerLevel>();
            if (gameTimer != null)
            {
                gameTimer.PlayerReachedFinishLine();
            }
            else
            {
                Debug.LogError("FinishLine: không tìm thấy GameManager!");
            }
        }
    }
}
