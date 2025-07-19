using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManagerLevel : MonoBehaviour
{   // trạng thái game 
    [Header("Game State")]
    private float currentTimeRemaining;// thời gian hiện tại còn lại
    private bool isTimerRunning = false;
    private DifficultyLevel activeDifficulty;// kích hoạt difficulty level
    private LevelTimeLimits timeLimitsForLevel;// time cho lvel 

    private Carcontroller playerController; // Tham chiếu đến script điều khiển của người chơi
    
    private bool gameHasEnded = false; // biến để biết game đã kết thúc chưa

    [Header("UI Elements")]
    public TMP_Text timerText; // UI Text để hiển thị thời gian
    public GameObject winTextObject;//text Win
    public GameObject gameOverText;// text gameover
    public GameObject endGamePanel; // Tham chiếu đến Panel chứa nút Chơi Lại, Thoát

    [Header("Audio UI")]
    public AudioClip audioWin;
    public AudioClip audioGameOver;
    private AudioSource audioSource;

    void Start()
    {
        gameHasEnded = false; // Reset trạng thái game
        
        audioSource = GetComponent<AudioSource>();
        if (DifficultyManager.Instance != null)
        {
            activeDifficulty = DifficultyManager.Instance.currentDifficulty;// lấy độ khó hiện tại từ Diff Manager
            timeLimitsForLevel = DifficultyManager.Instance.levelTimeLimits; // Lấy giới hạn thời gian
        }
        else
        {
            activeDifficulty = DifficultyLevel.Normal; // Mặc định 
            timeLimitsForLevel = new LevelTimeLimits(); // Sử dụng giá trị mặc định của class
            timeLimitsForLevel.timeLimitEasy = 180; 
            timeLimitsForLevel.timeLimitNormal = 120;
            timeLimitsForLevel.timeLimitHard = 60;
        }
        StartLevelTimer();
    }
    // hàm lấy độ khó từ Diff Manager
    public void StartLevelTimer()
    {
        isTimerRunning = true;

        switch (activeDifficulty)
        {
            case DifficultyLevel.Easy:
                currentTimeRemaining = timeLimitsForLevel.timeLimitEasy;
                break;
            case DifficultyLevel.Normal:
                currentTimeRemaining = timeLimitsForLevel.timeLimitNormal;
                break;
            case DifficultyLevel.Hard:
                currentTimeRemaining = timeLimitsForLevel.timeLimitHard;
                break;
            default:
                currentTimeRemaining = timeLimitsForLevel.timeLimitNormal;
                break;
        }
        
        UpdateTimerUI();
    }
    // hàm trừ thời gian
    void Update()
    {
        if (isTimerRunning)
        {
            currentTimeRemaining -= Time.deltaTime;
            UpdateTimerUI();

            if (currentTimeRemaining <= 0)
            {
                currentTimeRemaining = 0;
                isTimerRunning = false;
                HandleTimeUp();
            }
        }
    }
    // hàm tính toán thời gian 
    void UpdateTimerUI()
    {
        if (timerText != null)
        {
            if (currentTimeRemaining < 0) currentTimeRemaining = 0; // Đảm bảo không hiển thị số âm
            // Tính toán phút và giây
            int minutes = Mathf.FloorToInt(currentTimeRemaining / 60);
            int seconds = Mathf.FloorToInt(currentTimeRemaining % 60);
            
            timerText.text = string.Format("{0:00}:{1:00}",minutes, seconds); 
        }
    }
    // hàm tính toán người chơi đã về đích
    public void PlayerReachedFinishLine()
    {
        if (!isTimerRunning) return; // Nếu thời gian đã hết hoặc game chưa bắt đầu/đã thắng
        isTimerRunning = false; // Dừng timer
        HandleGameWin();
    }
    // hàm hiện game over
    void HandleTimeUp()
    {
        if (gameHasEnded) return; // Nếu game đã kết thúc thì không làm gì nữa

        if (gameOverText!= null)
        {
            gameOverText.SetActive(true);
            audioSource.PlayOneShot(audioGameOver);
        }
        EndGameSequence();
       
    }
    // hàm hiện game win
    void HandleGameWin()
    {
        if (gameHasEnded) return; // Nếu game đã kết thúc thì không làm gì nữa

        // Hiển thị chữ Win
        if (winTextObject != null)
        {
            winTextObject.SetActive(true);
            audioSource.PlayOneShot(audioWin);
            
        }
        EndGameSequence();
    }
    // hàm vô hiệu hoá script xe
    void EndGameSequence()
    {
        gameHasEnded = true; // Đánh dấu game đã kết thúc
        isTimerRunning = false; // Đảm bảo timer dừng

        // Vô hiệu hóa điều khiển người chơi
        if (playerController != null)
        {
            playerController.SetControlEnabled(false);
        }
        ShowEndGamePanel(); // Hoặc hiện ngay
    }
    // hàm hiện panel 
    void ShowEndGamePanel()
    {
        if (endGamePanel != null)
        {
            endGamePanel.SetActive(true);
        }
    }

    // hàm tải lại game
    public void RetryLevel()
    {
        Debug.Log("Retrying level...");
        // Tải lại scene hiện tại
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    // hàm quit game
    public void QuitToMainMenu()
    {
        Debug.Log("Quitting to Main Menu...");

        SceneManager.LoadScene(0);
    }

    // Hàm để PlayerController gọi để đăng ký chính nó
    public void RegisterPlayer(Carcontroller controller)
    {
        playerController = controller;
        Debug.Log("GameTimer: PlayerController registered successfully!");
    }
}
    

