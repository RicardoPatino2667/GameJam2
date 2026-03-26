using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public int vidas = 3;
    int zombiesMuertos = 0;
    int zombiesTotales;
    bool juegoTerminado = false;
    string mensajeFinal = "";
    
    void Start()
    {
        zombiesTotales = GameObject.FindGameObjectsWithTag("Zombie").Length;
        Debug.Log("Juego iniciado. Vidas: " + vidas + " Zombies: " + zombiesTotales);
    }
    
    void Update()
    {
        // Reiniciar con R SIEMPRE (incluso cuando el juego está detenido)
        if (Input.GetKeyDown(KeyCode.R))
        {
            ReiniciarJuego();
        }
    }
    
    public void PlayerRecibeDaño(int daño)
    {
        if (juegoTerminado) return;
        
        vidas -= daño;
        Debug.Log("Vidas: " + vidas);
        
        if (vidas <= 0)
        {
            GameOver();
        }
    }
    
    public void ZombieMuere()
    {
        if (juegoTerminado) return;
        
        zombiesMuertos++;
        Debug.Log("Zombies: " + zombiesMuertos + "/" + zombiesTotales);
        
        if (zombiesMuertos >= zombiesTotales)
        {
            Victoria();
        }
    }
    
    void GameOver()
    {
        juegoTerminado = true;
        mensajeFinal = "GAME OVER - Presiona R para reiniciar";
        Time.timeScale = 0f;
        Debug.Log("GAME OVER");
    }
    
    void Victoria()
    {
        juegoTerminado = true;
        mensajeFinal = "VICTORIA - Presiona R para reiniciar";
        Time.timeScale = 0f;
        Debug.Log("VICTORIA");
    }
    
    void ReiniciarJuego()
    {
        Debug.Log("Reiniciando juego...");
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    
    // Mostrar información en pantalla
    void OnGUI()
    {
        // Mostrar vidas y zombies
        GUIStyle estilo = new GUIStyle();
        estilo.fontSize = 20;
        estilo.normal.textColor = Color.white;
        GUI.Label(new Rect(10, 10, 200, 30), "Vidas: " + vidas);
        GUI.Label(new Rect(10, 40, 200, 30), "Zombies: " + zombiesMuertos + "/" + zombiesTotales);
        
        // Mostrar mensaje si el juego terminó
        if (juegoTerminado)
        {
            GUIStyle estiloFinal = new GUIStyle();
            estiloFinal.fontSize = 30;
            estiloFinal.alignment = TextAnchor.MiddleCenter;
            estiloFinal.normal.textColor = Color.red;
            
            float ancho = 400;
            float alto = 100;
            float x = (Screen.width / 2) - (ancho / 2);
            float y = (Screen.height / 2) - (alto / 2);
            
            GUI.Label(new Rect(x, y, ancho, alto), mensajeFinal, estiloFinal);
        }
    }
}