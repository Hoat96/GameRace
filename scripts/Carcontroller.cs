using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Carcontroller : MonoBehaviour
{
    [Header("Wheel Colliders")]
    public WheelCollider frontLeftWheelCollider;
    public WheelCollider frontRightWheelCollider;
    public WheelCollider rearLeftWheelCollider;
    public WheelCollider rearRightWheelCollider;

    [Header("Wheel Transforms (Visuals)")]
    public Transform frontLeftWheelTransform;
    public Transform frontRightWheelTransform;
    public Transform rearLeftWheelTransform;
    public Transform rearRightWheelTransform;

    [Header("Boost Settings")]
    public KeyCode boostKey = KeyCode.LeftShift; // Nút để kích hoạt tăng tốc
    public float boostForce = 100f;      // Lực tăng tốc bổ sung 
    public float boostDuration = 9.0f;   // Thời gian tăng tốc kéo dài (giây)
    public float boostCooldown = 2.0f;  // Thời gian chờ trước khi có thể tăng tốc lại (giây)

    [Header("Car Settings")]
    public float motorForce = 1500f;    // Lực đẩy của động cơ
    public float brakeForce = 3000f;    // Lực phanh
    public float maxSteerAngle = 30f;   // Góc lái tối đa (độ)
    public Transform centerOfMass;      // Điểm đặt trọng tâm (quan trọng cho ổn định)

    [Header("Drift Settings")]
    public KeyCode driftKey = KeyCode.LeftControl; // Nút kích hoạt drift
    public float driftSidewaysStiffness = 0.6f; // Hệ số nhân cho stiffness ngang khi drift 
    public float driftForwardStiffness = 0.8f;  // Hệ số nhân cho stiffness dọc khi drift

    [Header("Engine Sound Settings")]
    public AudioSource engineAudioSource; // Tham chiếu đến AudioSource
    public float minPitch = 0.7f;       // Pitch thấp nhất (khi xe đứng yên/chạy chậm)
    public float maxPitch = 2.5f;       // Pitch cao nhất (khi xe chạy nhanh)
    public float pitchChangeSpeed = 5.0f; // Tốc độ thay đổi pitch (làm mượt)

    [Header("Reset position car")]
    private Transform resetPoint;       // Điểm mà xe sẽ được reset về 
    private bool isResetting = false; // biến để tránh reset liên tục



    // Sử dụng tốc độ để điều chỉnh pitch/volume 
    public float maxSpeedForSound = 100f; // Tốc độ tối đa (m/s) tương ứng với maxPitch. 
    private float currentPitch = 1.0f; // Pitch hiện tại (dùng để làm mượt)
    // biến bật tắt audio
    private bool engineSoundPlaying = false;
    // biến kiểm soát bật tắt script
    private bool canControl = true;

    private Rigidbody rb;
    private float horizontalInput;
    private float verticalInput;
    private bool isBraking;
    private float currentSteerAngle;
    private float currentBrakeForce;
    public List<WheelCollider> rearWheels;

    
    //Biến Boost xe 
    private bool isBoosting = false;          // biến kiểm tra có đang tăng tốc ko
    private float currentBoostTimer = 0f;     // Đếm ngược thời gian tăng tốc còn lại
    private float currentCooldownTimer = 0f;  // Đếm ngược thời gian còn lại
    // Biến nội bộ để lưu ma sát gốc và trạng thái
    private List<WheelFrictionCurve> originalRearSidewaysFriction = new List<WheelFrictionCurve>();
    private List<WheelFrictionCurve> originalRearForwardFriction = new List<WheelFrictionCurve>();
    private bool isDrifting = false;


    private void Awake()
    {
        engineAudioSource = GetComponent<AudioSource>(); // Tự động lấy AudioSource

        
        rb = GetComponent<Rigidbody>();
        // Đặt trọng tâm cho Rigidbody. để xe không bị lật dễ dàng.
        if (centerOfMass != null && rb != null)
        {
            rb.centerOfMass = transform.InverseTransformPoint(centerOfMass.position);
        }
        

    }

    void Start()
    {
    
        // Lưu lại ma sát gốc của bánh sau
        StoreOriginalFriction();

        currentPitch = minPitch; // Khởi tạo pitch ban đầu
        engineAudioSource.pitch = currentPitch;

        // Tìm GameObject có Tag "ResetPoint" trong scene
        GameObject resetPointObject = GameObject.FindWithTag("ResetCarPoint");
        if(resetPointObject != null )
        {
            resetPoint = resetPointObject.transform;
        }
        

    }

    void Update()
    {
        CheckBoostInput();
        HandleBoostTimers();
        UpdateEngineSound();
        // Sử dụng GetKey để kiểm tra phím đang được giữ
        if (Input.GetKey(driftKey))
        {
            if (!isDrifting) // Chỉ gọi hàm StartDrift một lần khi bắt đầu nhấn
            {
                StartDrift();
            }
        }
        else
        {
            if (isDrifting) // Chỉ gọi hàm StopDrift một lần khi nhả phím
            {
                StopDrift();
            }
        }
        // điều kiện kiểm soát dừng khi game over/win
        if (!canControl)
        {
            // Đảm bảo xe thực sự dừng lại
            if (rb != null) 
            {
                // dừng ngay lập tức
                 rb.velocity = Vector3.zero;
                 rb.angularVelocity = Vector3.zero;
            }
            return; // Thoát khỏi hàm, không xử lý input bên dưới
        }
    }

    void FixedUpdate()
    {
        GetInput();
        HandleMotor();
        HandleSteering();
        ApplyBraking();
        UpdateWheels(); // Cập nhật vị trí/xoay của mesh bánh xe     
    }

    void GetInput()
    {
        // Lấy input từ người dùng
        horizontalInput = Input.GetAxis("Horizontal"); 
        verticalInput = Input.GetAxis("Vertical");     
        isBraking = Input.GetKey(KeyCode.Space);  
    }

    void HandleMotor()
    {
        // phép toán giúp bánh xe di chuyển và tăng tốc xe
        if (Mathf.Abs(verticalInput) > 0.01f)
        {
            float currentMotorForce = verticalInput * motorForce;
            rearLeftWheelCollider.motorTorque = currentMotorForce;
            rearRightWheelCollider.motorTorque = currentMotorForce;
            if (isBoosting && verticalInput > 0)
            {
                currentMotorForce += boostForce;
            }

            rb.AddForce(transform.forward * currentMotorForce, ForceMode.Acceleration);
        }
    }
    void CheckBoostInput()
    {
        // Kiểm tra: Nhấn nút boost? Đang không boost? Đã hết cooldown chưa?
        if (Input.GetKeyDown(boostKey) && !isBoosting && currentCooldownTimer <= 0f)
        {
            Debug.Log("Bắt đầu tăng tốc");
            StartBoost();
        }
        
    }

    // Hàm quản lý các bộ đếm thời gian boost và cooldown
    void HandleBoostTimers()
    {
        // Nếu đang boost, giảm thời gian boost còn lại
        if (isBoosting)
        {
            currentBoostTimer -= Time.deltaTime;
            if (currentBoostTimer <= 0f)
            {
                StopBoost();
            }
        }
        // Nếu không boost và đang trong thời gian cooldown, giảm thời gian cooldown còn lại
        else if (currentCooldownTimer > 0f)
        {
            currentCooldownTimer -= Time.deltaTime;
        }
    }
    void StopBoost()
    {
        isBoosting = false;
        currentBoostTimer = 0f; // Đảm bảo reset timer
        currentCooldownTimer = boostCooldown; // Bắt đầu đếm ngược cooldown
    }

    // Hàm bắt đầu quá trình boost
    void StartBoost()
    {
        isBoosting = true;
        currentBoostTimer = boostDuration; // Đặt lại thời gian boost
        
        Debug.Log("Boost Activated!");
    }

    void ApplyBraking()
    {

        // Phanh thường (khi nhấn Space)
        currentBrakeForce = isBraking ? brakeForce : 0f;
        frontLeftWheelCollider.brakeTorque = currentBrakeForce;
        frontRightWheelCollider.brakeTorque = currentBrakeForce;

        rearLeftWheelCollider.brakeTorque = currentBrakeForce;
        rearRightWheelCollider.brakeTorque = currentBrakeForce;
    }

    void HandleSteering()
    {
        // Tính toán và áp dụng góc lái cho bánh trước
        currentSteerAngle = maxSteerAngle * horizontalInput;
        frontLeftWheelCollider.steerAngle = currentSteerAngle;
        frontRightWheelCollider.steerAngle = currentSteerAngle;
    }
    void StoreOriginalFriction()
    {
        originalRearSidewaysFriction.Clear();
        originalRearForwardFriction.Clear();
        foreach (WheelCollider wheel in rearWheels)
        {
            // Phải tạo bản sao (copy) vì WheelFrictionCurve là struct
            originalRearSidewaysFriction.Add(wheel.sidewaysFriction);
            originalRearForwardFriction.Add(wheel.forwardFriction);
        }
        
    }
    void StartDrift()
    {
        isDrifting = true;
        Debug.Log("Start Drifting"); // Log để debug

        for (int i = 0; i < rearWheels.Count; i++)
        {
            WheelCollider wheel = rearWheels[i];

            // Lấy ma sát gốc đã lưu
            WheelFrictionCurve sidewaysFriction = originalRearSidewaysFriction[i];
            WheelFrictionCurve forwardFriction = originalRearForwardFriction[i];

            // Tạo ma sát mới bằng cách giảm stiffness
            // tạo một WheelFrictionCurve mới và gán lại
            WheelFrictionCurve newSidewaysFriction = sidewaysFriction; // Copy struct
            newSidewaysFriction.stiffness *= driftSidewaysStiffness; // Giảm stiffness
            wheel.sidewaysFriction = newSidewaysFriction; // Gán lại

            // Tương tự cho forward friction 
            WheelFrictionCurve newForwardFriction = forwardFriction;
            newForwardFriction.stiffness *= driftForwardStiffness;
            wheel.forwardFriction = newForwardFriction;
        }
    }
    
    void StopDrift()
    {
        isDrifting = false;
        Debug.Log("Stop Drifting"); // Log để debug

        for (int i = 0; i < rearWheels.Count; i++)
        {
            // Khôi phục lại ma sát gốc đã lưu
            rearWheels[i].sidewaysFriction = originalRearSidewaysFriction[i];
            rearWheels[i].forwardFriction = originalRearForwardFriction[i];
        }
    }
    void UpdateEngineSound()// method điều chỉnh độ cao âm thanh dựa trên tốc độ
    {
        // Lấy độ lớn vận tốc tổng thể (m/s)
        float currentSpeed = rb.velocity.magnitude;
        // Tính toán giá trị chuẩn hóa (0 đến 1) dựa trên tốc độ
        float normalizedSpeed = Mathf.Clamp01(currentSpeed / maxSpeedForSound);
        // Dùng giá trị chuẩn hóa (normalizedSpeed hoặc normalizedEngineValue) để nội suy giữa minPitch và maxPitch
        float targetPitch = Mathf.Lerp(minPitch, maxPitch, normalizedSpeed);
        // Làm mượt sự thay đổi Pitch 
        // Thay đổi pitch hiện tại dần dần về phía pitch mục tiêu
        currentPitch = Mathf.Lerp(currentPitch, targetPitch, Time.deltaTime * pitchChangeSpeed);
        // Giới hạn pitch trong khoảng min/max phòng ngừa lỗi
        currentPitch = Mathf.Clamp(currentPitch, minPitch, maxPitch);
        // Áp dụng pitch đã làm mượt cho AudioSource
        engineAudioSource.pitch = currentPitch;
    }
    // hàm bật tắt audio

    void OnDisable()
    {
        // Khi xe bị vô hiệu hóa, dừng âm thanh và reset 
        StopEngineSound();
    }

    public void StartEngineSound()
    {
         
        if (!engineSoundPlaying)
        {
            engineAudioSource.Play();
            engineSoundPlaying = true;
            
        }
        
    }
    public void StopEngineSound()
    {
       
        if (engineSoundPlaying)
        {
            engineAudioSource.Stop();
        }
        engineSoundPlaying = false;
    }
    // Hàm để bật/tắt khả năng điều khiển 
    public void SetControlEnabled(bool enabled)
    {
        canControl = enabled;
        Debug.Log("Player control set to: " + enabled);
        if (!enabled && rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }
    // hàm va chạm với killzone 
    private void OnTriggerEnter(Collider other)
    {
        if (!isResetting && other.CompareTag("KillZone"))
        {
            StartCoroutine(ResetCarPosition());
        }
    }
    // hàm reset position car
    IEnumerator ResetCarPosition()
    {
        isResetting = true; // Đánh dấu đang trong quá trình reset
        Debug.Log("Car is out of bounds! Resetting...");

        SetControlEnabled(false); // Tắt điều khiển


        if (resetPoint != null)
        {
            transform.position = resetPoint.position;
            transform.rotation = resetPoint.rotation;
        }

        yield return new WaitForSeconds(1);
        SetControlEnabled(true);

        isResetting = false; // Hoàn tất reset

    }



    void UpdateWheels()
    {
        // Cập nhật vị trí và xoay của các Transform bánh xe 
        // dựa trên trạng thái của Wheel Collider tương ứng
        UpdateSingleWheel(frontLeftWheelCollider, frontLeftWheelTransform);
        UpdateSingleWheel(frontRightWheelCollider, frontRightWheelTransform);
        UpdateSingleWheel(rearLeftWheelCollider, rearLeftWheelTransform);
        UpdateSingleWheel(rearRightWheelCollider, rearRightWheelTransform);
    }
    // Hàm helper để cập nhật một bánh xe
    void UpdateSingleWheel(WheelCollider wheelCollider, Transform wheelTransform)
    {
        if (wheelTransform == null) return; 

        Vector3 pos;
        Quaternion rot;
        // Lấy vị trí và góc xoay thế giới của Wheel Collider
        wheelCollider.GetWorldPose(out pos, out rot);
        // Áp dụng cho Transform của bánh xe visual
        wheelTransform.rotation = rot;
        wheelTransform.position = pos;
    }
}
