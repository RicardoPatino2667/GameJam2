using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadScene("What da fuq is wrong with my gun"); // Cambia por el nombre de tu nivel
    }

    public void OpenHowToPlay()
    {
        SceneManager.LoadScene("HowToPlay"); // Escena de instrucciones
    }

    public void OpenCredits()
    {
        SceneManager.LoadScene("WhoAreUs"); // Escena de "Who are us"
    }

    public void OpenMainMenu()
    {
        SceneManager.LoadScene("MainMenu"); // Escena de "Who are us"
    }

    public void QuitGame()
    {
        Debug.Log("Saliendo...");
        Application.Quit();
    }
}