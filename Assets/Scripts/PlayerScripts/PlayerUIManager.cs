using DG.Tweening;
using StarterAssets;
using TMPro;
using UnityEngine;

using UnityEngine.SceneManagement;

public class PlayerUIManager : MonoBehaviour
{

    private GameObject player;
    
    private Rifle rifle;
    [SerializeField] private GameObject pauseMenuUI;
    private StarterAssetsInputs input;

    private bool isPaused = false;

    [Header("PlayerData")]
    [SerializeField] private TextMeshProUGUI playerHP;
    [SerializeField] private TextMeshProUGUI playerMoney;
    [SerializeField] private TextMeshProUGUI ammoCount;

    [Header("ScriptableObject")]
    [SerializeField] private PlayerInventoryData playerInventoryData;

    private void Awake()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            FindPlayerComponent();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
        if (input.tab || input.esc )
        {
            if (isPaused)  OnResumePress();
            else OnPausePress();
            input.tab = false;
        }
        
    }

    public void FixedUpdate()
    {
        if (player!=null) 
        {
            if (ammoCount != null) ammoCount.text = rifle.GetBulletCount() + "/" + playerInventoryData.totalBullets;

            if (playerHP != null) playerHP.text = playerInventoryData.health.ToString();
            if (playerMoney != null) playerMoney.text = playerInventoryData.money.ToString();
        }
        

    }
    private void FindPlayerComponent()
    {
        rifle = player.GetComponentInChildren<Rifle>();
        input = player.GetComponentInChildren<StarterAssetsInputs>();
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
