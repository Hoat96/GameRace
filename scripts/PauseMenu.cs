using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject pausemenu;
    public string sceneName;
    public bool toggle;
    private Carcontroller playerController;
    void Start()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player"); 
        if (playerObject != null)
        {
            playerController = playerObject.GetComponent<Carcontroller>();
        }
        if (playerController == null)
        {
            Debug.LogError("PauseMenu: Carcontroller not found on a GameObject with tag 'Player'.");
            // Nếu không tìm thấy, các hàm SetControlEnabled sẽ không hoạt động
        }

    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape)) 
        {
            
            toggle = !toggle;
            if (toggle == false)
            {
                
                pausemenu.SetActive(false);
                AudioListener.pause = false;
                Time.timeScale = 1;
                playerController.SetControlEnabled(true);
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
            if(toggle == true)
            {
                
                pausemenu.SetActive(true);
                AudioListener.pause = true;
                Time.timeScale = 0;
                playerController.SetControlEnabled(false);
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }
            
        }
    }
    public void ResumeGame()
    {
        pausemenu.SetActive(false);
        AudioListener.pause = false;
        Time.timeScale = 1;
        playerController.SetControlEnabled(true);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
    public void QuitMenu()
    {
        Time.timeScale = 1;
        AudioListener.pause = false;
        SceneManager.LoadScene(sceneName);
    }
    public void QuitDeskTop()
    {
        Time.timeScale = 1;
        AudioListener.pause = false;
        Debug.Log("the game will quit");
        Application.Quit();

    }
}
