using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; 
using UnityEngine.UI;

public class SelectLevelUI : MonoBehaviour
{
    // Tham chiếu đến các nút 
    [Header("Button")]
    public Button easyButton;
    public Button normalButton;
    public Button hardButton;
    public Color selectedColor = Color.green; // Màu khi nút được chọn
    public Color defaultColor = Color.white;  // Màu mặc định của nút


    void Start()
    {
        if (DifficultyManager.Instance == null)
        {
            // vô hiệu hóa các nút nếu DifficultyManager không tồn tại
            if (easyButton) easyButton.interactable = false;
            if (normalButton) normalButton.interactable = false;
            if (hardButton) hardButton.interactable = false;
            return;
        }
        // Cập nhật trạng thái trực quan của các nút dựa trên độ khó hiện tại
        UpdateButtonVisuals();

    }
    
    // Hàm nút Dễ
    public void OnEasyButtonClicked()
    {
        if (DifficultyManager.Instance != null)
        {
            DifficultyManager.Instance.SetDifficulty(DifficultyLevel.Easy);
            UpdateButtonVisuals();
        }   
    }
    // Hàm nút Vừa
    public void OnNormalButtonClicked()
    {
        if (DifficultyManager.Instance != null)
        {
            DifficultyManager.Instance.SetDifficulty(DifficultyLevel.Normal);
            UpdateButtonVisuals();

        }
    }
    // Hàm nút Khó
    public void OnHardButtonClicked()
    {
        if (DifficultyManager.Instance != null)
        {
            DifficultyManager.Instance.SetDifficulty(DifficultyLevel.Hard);
            UpdateButtonVisuals();
        }
    }
    // Hàm nút "Lưu" 
    public void OnConfirmAndStartGameClicked()
    {
        if(DifficultyManager.Instance != null )
        {
            // Yêu cầu DifficultyManager tải Scene game chính
            DifficultyManager.Instance.LoadGameScene();
        }
        else
        {
            Debug.LogError("SelectLevelUI: Cannot start game, DifficultyManager.Instance is null.");
        }
    }
    // Cập nhật màu sắc của các nút để hiển thị lựa chọn hiện tại
    void UpdateButtonVisuals()
    {
        if (DifficultyManager.Instance == null || easyButton == null || normalButton == null || hardButton == null)
        {
            // Không làm gì nếu thiếu tham chiếu
            return;
        }
        // các nút có component Image để đổi màu
        easyButton.GetComponent<Image>().color = (DifficultyManager.Instance.currentDifficulty == DifficultyLevel.Easy) ? selectedColor : defaultColor;
        normalButton.GetComponent<Image>().color = (DifficultyManager.Instance.currentDifficulty == DifficultyLevel.Normal) ? selectedColor : defaultColor;
        hardButton.GetComponent<Image>().color = (DifficultyManager.Instance.currentDifficulty == DifficultyLevel.Hard) ? selectedColor : defaultColor;
    }
}
