using DG.Tweening;
using StarterAssets;
using TMPro;
using UnityEngine;

using UnityEngine.SceneManagement;

public class PlayerUIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI ammoCount;
    [SerializeField] private Rifle rifle;
    [SerializeField] private GameObject pauseMenuUI;
    [SerializeField] private StarterAssetsInputs input;

    private bool isPaused = false;

    [Header("ScriptableObject")]
    [SerializeField] private PlayerInventoryData playerInventoryData;

    private void Awake()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        ammoCount.text = rifle.GetBulletCount() + "/" + playerInventoryData.totalBullets;
        if (input.tab || input.esc)
        {
            if (isPaused)  OnResumePress();
            else OnPausePress();
            input.tab = false;
        }
        
    }

    public void OnRestartPress()
    {
        DOTween.KillAll();
        if (Time.timeScale != 1f)
        {
            Time.timeScale = 1f;
        }
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    
    public void OnExitPress()
    {
        DOTween.KillAll();
        if (Time.timeScale != 1f)
        {
            Time.timeScale = 1f;
        }

        SceneManager.LoadScene("Menu");
    }

    public void OnPausePress()
    {
        isPaused = true;
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f; // Freezes the game world

        //lock cursor
        input.look = Vector2.zero;

        // 1. Tell StarterAssets to stop locking the cursor
        input.cursorLocked = false;
        input.cursorInputForLook = false;

        // 2. Make the cursor visible and free
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void OnResumePress()
    {
        isPaused = false;
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f; // Unfreezes the game

        // 1. Tell StarterAssets to lock the cursor again
        input.cursorLocked = true;
        input.cursorInputForLook = true;

        // 2. Hide the cursor
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
}
