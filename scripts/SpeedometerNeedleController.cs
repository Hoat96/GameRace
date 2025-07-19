using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedometerNeedleController : MonoBehaviour
{
    [Header("References")]
    public Rigidbody targetRigidbody; // Rigidbody của xe
    private RectTransform needleRectTransform; // RectTransform của kim 

    [Header("Speedometer Configuration")]
    public float minSpeed = 0f;        // Tốc độ ứng với góc nhỏ nhất 
    public float maxSpeed = 200f;      // Tốc độ  ứng với góc lớn nhất )
    public float displayUnitsMultiplier = 3.6f; // Hệ số chuyển đổi từ m/s (tốc độ Rigidbody) sang đơn vị hiển thị
                                                // 3.6f cho km/h
                                                // 2.237f cho mph
    [Header("Needle Rotation")]
    public float minAngle = 135f;      // Góc quay Z minSpeed 
    public float maxAngle = -135f;     // Góc quay Z maxSpeed 
                                       
    public float smoothingFactor = 5.0f; // Độ mượt khi kim di chuyển 

    private float currentAngle; // Lưu góc hiện tại để làm mượt
    void Start()
    {
        needleRectTransform = GetComponent<RectTransform>();
        // Khởi tạo vị trí kim ban đầu
        currentAngle = minAngle;
        needleRectTransform.localEulerAngles = new Vector3(0, 0, currentAngle);
    }

    // Update is called once per frame
    void Update()
    {
        // 1. Lấy tốc độ hiện tại từ Rigidbody (đơn vị: mét/giây - m/s)
        float speedMPS = targetRigidbody.velocity.magnitude;
        // 2. Chuyển đổi tốc độ sang đơn vị hiển thị ( km/h hoặc mph)
        float speedDisplayUnits = speedMPS * displayUnitsMultiplier;
        // Dựa trên giá trị minSpeed và maxSpeed đã cấu hình.
        // Mathf.InverseLerp tự động kẹp giá trị trong khoảng 0-1.
        float normalizedSpeed = Mathf.InverseLerp(minSpeed, maxSpeed, speedDisplayUnits);
        // 4. Tính toán góc quay mục tiêu dựa trên tốc độ đã chuẩn hóa
        // Nội suy tuyến tính (Lerp) giữa góc nhỏ nhất và góc lớn nhất.
        float targetAngle = Mathf.Lerp(minAngle, maxAngle, normalizedSpeed);
        //5.Làm mượt chuyển động của kim
        // Nội suy góc hiện tại về phía góc mục tiêu theo thời gian.
        currentAngle = Mathf.Lerp(currentAngle, targetAngle, Time.deltaTime * smoothingFactor);
        // 6. Áp dụng góc quay cho RectTransform của kim
        needleRectTransform.localEulerAngles = new Vector3(0, 0, currentAngle);

    }
    public void SetTargetRigidbody(Rigidbody newRb)
    {
        targetRigidbody = newRb;
        if (targetRigidbody == null)
        {
            Debug.LogWarning("Target Rigidbody trong Speedometer được đặt thành null. Đồng hồ có thể không cập nhật.", this);
            
        }
    
    }
}
