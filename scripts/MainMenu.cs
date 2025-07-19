using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    // Tên của scene chứa màn hình cài đặt 
    public string settingsSceneName = "Setting";

    public string selectLevel = "SelectLevel";

    private void Start()
    {
        // Đảm bảo DifficultyManager tồn tại khi MainMenu bắt đầu
        if (DifficultyManager.Instance == null)
        {
            Debug.LogError("MenuManager: DifficultyManager.Instance is NULL .");
        }
    }
    public void OnPlayButtonClicked()
    {
        if (DifficultyManager.Instance != null)
        {
            //DifficultyManager sẽ tải scene game chính
            //  sử dụng currentDifficulty đã được load từ PlayerPrefs hoặc giá trị mặc định.
            DifficultyManager.Instance.LoadGameScene();
        }
        else
        {
            Debug.LogError("MenuManager: DifficultyManager.Instance is NULL");
        }
    }
    public void OnLevelButtonClicked()
    {
        // Tải scene chọn độ khó
        // DifficultyManager.Instance sẽ vẫn tồn tại khi chuyển qua scene này
        SceneManager.LoadScene(selectLevel);
    }
    public void SelectLevel()
    {
        SceneManager.LoadScene(selectLevel);
    }

    // Hàm nút "Cài đặt"
    public void OpenSettings()
    {
        //  Chuyển sang scene Cài đặt 
        Debug.Log("Loading Settings Scene: " + settingsSceneName);
        SceneManager.LoadScene(settingsSceneName);
    }
    public void QuitGame()
    {
        Debug.Log("Quitting Game..."); // Log hiển thị trong Console 
        Application.Quit();
    }
}
