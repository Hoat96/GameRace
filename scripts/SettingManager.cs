using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingManager : MonoBehaviour
{
    [Header("Car Setup")]
    public List<GameObject> carPrefabs; // Prefab Inspector
    public Transform carSpawnPoint;     // Vị trí mà xe sẽ được hiển thị 

    [Header("Rotate Car")]
    public GameObject toRotate;// xoay bàn 
    public float rotateSpeed;// tốc độ xoay
   
    private int currentCarIndex = 0;// biến phần tử xe hiện tại
    private GameObject currentCarInstance;// biến khởi tạo xe

   

    void Start()
    {
        // Hiển thị chiếc xe đầu tiên trong danh sách
        SelectCar(currentCarIndex);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        
        toRotate.transform.Rotate(Vector3.up * rotateSpeed * Time.deltaTime);
      
    }
    
    void SelectCar(int index)
    {
        currentCarIndex = index;
        // Hủy instance xe cũ 
        if (currentCarInstance != null)
        {
            Destroy(currentCarInstance);
        }
        // Instantiate xe mới tại vị trí spawnPoint
        // Xe được instantiate nên có rotation và scale phù hợp với spawnPoint
        currentCarInstance = Instantiate(carPrefabs[currentCarIndex], carSpawnPoint.position, carSpawnPoint.rotation);
        // Đặt xe mới làm con của spawnPoint để dễ quản lý transform
        currentCarInstance.transform.SetParent(carSpawnPoint, true); // true để giữ world position sau khi set parent
    }
    // Hàm Next Xe
    public void NextCar()
    {
        int nextIndex = currentCarIndex + 1;
        if (nextIndex >= carPrefabs.Count)
        {
            nextIndex = 0; // Quay lại xe đầu tiên nếu hết danh sách
        }
        SelectCar(nextIndex);
    }
    //hàm xe trước đó
    public void PreviousCar()
    {
       
        int prevIndex = currentCarIndex - 1;
        if (prevIndex < 0)
        {
            prevIndex = carPrefabs.Count - 1; // Đi đến xe cuối cùng nếu đang ở xe đầu tiên
        }
        SelectCar(prevIndex);
    }
    // hàm lưu và chọn xe
    public void ConfirmSelection()
    {
        if (currentCarInstance != null)
        {
            PlayerPrefs.SetInt("SelectedCarIndex", currentCarIndex); // Lưu index của xe đã chọn
            PlayerPrefs.Save(); // lưu xe
            Debug.Log("Đã xác nhận chọn xe: " + carPrefabs[currentCarIndex].name + " (Index: " + currentCarIndex + ")");

            // Chuyển sang scene game chính 
             UnityEngine.SceneManagement.SceneManager.LoadScene("Scene1");
        }
        else
        {
            Debug.LogWarning("Chưa có xe nào được hiển thị để chọn!");
        }
    }
}
