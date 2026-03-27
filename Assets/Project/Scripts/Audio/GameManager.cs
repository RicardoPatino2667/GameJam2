using UnityEngine;

public class GameManager : MonoBehaviour
{
    public AudioSource musicSource;
    public AudioClip gameOverMusic;

    public void GameOver()
    {
        musicSource.Stop();

        musicSource.clip = gameOverMusic;
        musicSource.loop = false;
        musicSource.Play();

        Debug.Log("GAME OVER");
    }
}