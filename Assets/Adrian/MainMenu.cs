using UnityEngine;
using UnityEngine.SceneManagement;
public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadScene("Behlul Final");
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene("StartScene");
    }

    public void QuitGame()
    {
        Debug.Log("Game closed!");
        Application.Quit();
    }
}
