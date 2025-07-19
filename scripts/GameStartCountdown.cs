using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameStartCountdown : MonoBehaviour
{
    public float countdownDuration = 3f; // Thời gian đếm ngược 
    public TMP_Text countdownDisplayText; // TextMeshPro

    private Carcontroller carControllerScript;// tham chiếu đến script xe

    public string goText = "GO"; // text go
    public float goTextDisplayTime = 1.5f; // Thời gian hiển thị chữ "GO"

    [Header("Audio Settings")]
    public AudioClip countdownBeepSound; // Âm thanh cho mỗi lần đếm (1, 2, 3)
    public AudioClip goSound;            // Âm thanh khi chữ "GO!" xuất hiện
    private AudioSource audioSource;     // Component để phát âm thanh


    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Script khởi tạo xe sẽ gọi hàm này
    public void PrepareCountdown(Carcontroller carController)
    {
        this.carControllerScript = carController;
        // Vô hiệu hóa điều khiển xe ban đầu
        this.carControllerScript.enabled = false;
        countdownDisplayText.gameObject.SetActive(true);
        
        StartCoroutine(CountdownCoroutine());
    }
    // hàm đếm giây
    IEnumerator CountdownCoroutine()
    {
        float currentCountdown = countdownDuration;
        while (currentCountdown > 0)
        {
            countdownDisplayText.text = Mathf.CeilToInt(currentCountdown).ToString();
            // PHÁT ÂM THANH ĐẾM NGƯỢC 
            audioSource.PlayOneShot(countdownBeepSound);
            yield return new WaitForSeconds(1f); // Đợi 1 giây
            currentCountdown--;
        }
        // Đếm ngược kết thúc
        countdownDisplayText.text = goText;
        // PHÁT ÂM THANH GO!
        audioSource.PlayOneShot(goSound);
        // Kích hoạt điều khiển xe
        if (carControllerScript != null)
        {
            carControllerScript.enabled = true;
            // phát âm thanh xe
            carControllerScript.StartEngineSound();
        }
        // Đợi một chút để hiển thị chữ GO
        yield return new WaitForSeconds(goTextDisplayTime);
        // Ẩn text GO
        countdownDisplayText.gameObject.SetActive(false);
    }  
}
