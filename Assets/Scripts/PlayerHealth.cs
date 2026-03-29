using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int vida = 100;
    public PlayerAudio playerAudio;

    public void RecibirDanio(int dano)
    {
        vida -= dano;

        if (vida <= 0)
        {
            Morir();
        }
    }

    void Morir()
    {
        playerAudio.PlayMuerte();
        FindObjectOfType<GameManager>().GameOver();
        Destroy(gameObject, 0.5f);
    }
}