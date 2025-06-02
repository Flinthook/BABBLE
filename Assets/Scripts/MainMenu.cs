using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("SCENE 1 CHURCH");
    }

    public void OpenOptions()
    {
        // Show your options menu (enable a panel, etc.)
        Debug.Log("Options menu opened.");
    }

    public void ExitGame()
    {
        Application.Quit();
        Debug.Log("Game exited.");
    }
}