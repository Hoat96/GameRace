using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapFollow : MonoBehaviour
{
    public Transform playerTarget; 
    public float heightOffset = 20f; // Độ cao của camera minimap so với người chơi
    public bool followRotation = false; // biến kiểm tra xem có xoay theo người chơi không

    private Vector3 offset;
    
    // Hàm gọi từ script followPlayer
    public void SetTarget(Transform newTarget)
    {
        playerTarget = newTarget;
        if (playerTarget != null)
        {
            offset = new Vector3(0, heightOffset, 0);
            transform.rotation = Quaternion.Euler(90f, 0f, 0f); // Đảm bảo hướng nhìn ban đầu
        }
        
    }
    private void LateUpdate()
    {
        // Vị trí mới của camera minimap
        Vector3 newPosition = playerTarget.position + offset;
        transform.position = newPosition;
        // minimap xoay cùng hướng với người chơi
        if (followRotation)
        {   // Chỉ xoay quanh trục Y 
            transform.rotation = Quaternion.Euler(90f, playerTarget.eulerAngles.y, 0f);
        }
    }
}
