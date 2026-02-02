using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private GameObject loading;
    public void OnStartPress()
    {
        loading.SetActive(true);
        SceneManager.LoadScene("GameScene");
    }

    public void OnTrainingPress()
    {
        loading.SetActive(true);
        SceneManager.LoadScene("ShootingScene");
    }
    public void OnExitPress()
    {
        Application.Quit();
    }
}
