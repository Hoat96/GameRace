using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine; //  sử dụng Cinemachine

public class FollowPlayer : MonoBehaviour
{
    public List<GameObject> allPossibleCarPrefabs;// ds xe 
    public Transform playerSpawnPoint;// địa điểm xe khởi tạo

    [Header("Dependencies")]
    public SpeedometerNeedleController speedometer; // scripts Speedometer

    public GameStartCountdown countdownManager; // scripts GameStartCountdown

    public MinimapFollow minimapCameraScript; //sctipts MinimapCamera 

    private GameManagerLevel gameManagerlevel; // script GameManagerLevel


   
    void Start()
    {   // tìm script gamemanager level
        gameManagerlevel = FindObjectOfType<GameManagerLevel>();
        
        // tim scrip gamestart
        countdownManager = FindObjectOfType<GameStartCountdown>();

         // khởi tạo xe dựa trên lựa chọn trước của người chơi 
        int selectedCarIndex = PlayerPrefs.GetInt("SelectedCarIndex", 0);
        GameObject playerCarInstance = null;
        if (selectedCarIndex >= 0 && selectedCarIndex < allPossibleCarPrefabs.Count)
        {
            playerCarInstance = Instantiate(allPossibleCarPrefabs[selectedCarIndex], playerSpawnPoint.position, playerSpawnPoint.rotation);
            Debug.Log("Đã tải xe: " + allPossibleCarPrefabs[selectedCarIndex].name);
        }
        

        // Lấy CarController script từ xe vừa tạo
        Carcontroller carController = playerCarInstance.GetComponent<Carcontroller>(); // Dùng kiểu chung
        if (carController != null && countdownManager != null)
        {
            countdownManager.PrepareCountdown(carController);
        }
        // lấy script carcontroller vừa khởi tạo đăng kí với gameManagerLevel
        if(carController != null && gameManagerlevel != null)
        {
            gameManagerlevel.RegisterPlayer(carController);
            carController.SetControlEnabled(true);
        }

        // Lấy vị trí xe vừa tạo cho minimap 
        Transform playerTransform = playerCarInstance.transform;
        // Gán target cho MinimapFollow
        if (minimapCameraScript != null)
        {
            minimapCameraScript.SetTarget(playerTransform);
        }
        
        // Gán Target cho Cinemachine Virtual Camera 
        if (playerCarInstance != null)
        {
            //Tìm VCam trong scene 
            CinemachineVirtualCamera vcam = FindObjectOfType<CinemachineVirtualCamera>();
            if (vcam != null)
            {
                vcam.Follow = playerCarInstance.transform;
                
                vcam.LookAt = playerCarInstance.transform; // Mặc định nhìn vào gốc xe
            }
        }
        
        // hàm gọi speedometer

        if (speedometer != null && playerCarInstance != null)
        {
            Rigidbody playerRb = playerCarInstance.GetComponent<Rigidbody>();
            if (playerRb != null)
            {
                speedometer.SetTargetRigidbody(playerRb); // Gọi hàm trong Speedometer
            }
        }    
    }
}
