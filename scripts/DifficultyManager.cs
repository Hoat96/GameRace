using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; 

public class DifficultyManager : MonoBehaviour
{
    public static DifficultyManager Instance { get; private set; } // Singleton pattern
    // cài đặt độ khó ban đầu
    [Header("Difficulty Settings")]
    public DifficultyLevel currentDifficulty = DifficultyLevel.Normal;
    public LevelTimeLimits levelTimeLimits;// biến giới hạn thời gian cấp độ
    
   
    [Header("Scene Navigation")]
    public string gameSceneName = "scene1"; // scene game chính
    void Awake()
    {

        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            // ko bị xoá khi chuyển scene 
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }


        // Tải độ khó đã lưu từ lần chơi trước 
        if (PlayerPrefs.HasKey("SelectedDifficulty"))
        {
            currentDifficulty = (DifficultyLevel)PlayerPrefs.GetInt("SelectedDifficulty");
            
        }
        else
        {
            // Nếu không có gì được lưu, lưu độ khó mặc định hiện tại
            PlayerPrefs.SetInt("SelectedDifficulty", (int)currentDifficulty);
            PlayerPrefs.Save();
        }
    }
    
    // hàm lưu độ khó khi được người chơi chọn
    public void SetDifficulty(DifficultyLevel difficulty)
    {
        currentDifficulty = difficulty;
        PlayerPrefs.SetInt("SelectedDifficulty", (int)currentDifficulty);
        PlayerPrefs.Save(); // Lưu ngay khi độ khó được set
    }
    // hàm load scene 
    public void LoadGameScene()
    {
        if (string.IsNullOrEmpty(gameSceneName))
        {
            return;
        }
        SceneManager.LoadScene(gameSceneName);
    }

}
[System.Serializable] // Để có thể thấy trong Inspector
public class LevelTimeLimits
{

    public float timeLimitEasy = 120f;
    public float timeLimitNormal = 90f;
    public float timeLimitHard = 60f;
}
