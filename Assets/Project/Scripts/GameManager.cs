using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public int vidas = 3;
    int zombiesMuertos = 0;
    bool juegoTerminado = false;
    string mensajeFinal = "";

    // Referencia al jugador (se asigna automáticamente o manual)
    private Transform jugador;

    // Opción A: Tag del refugio
    private string refugioTag = "Refugio";

    // Opción B: Coordenadas de la casa refugio (si prefieres coordenadas fijas)
    public Vector3 coordenadasRefugio = new Vector3(100f, 0f, 0f); // Ajusta según tu escena
    public float radioVictoria = 2f; // Distancia a la que se activa la victoria

    void Start()
    {
        // Buscar al jugador por tag o por componente (ajusta según tu player)
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            jugador = playerObj.transform;
        else
            Debug.LogError("No se encontró el objeto con tag 'Player'");

        // Ya no calculamos zombiesTotales
        Debug.Log("Juego iniciado. Vidas: " + vidas);
    }

    void Update()
    {
        // Reiniciar con R SIEMPRE
        if (Input.GetKeyDown(KeyCode.R))
        {
            ReiniciarJuego();
        }

        // Verificar victoria por llegada al refugio
        if (!juegoTerminado && jugador != null)
        {
            if (VerificarVictoriaRefugio())
            {
                Victoria();
            }
        }
    }

    bool VerificarVictoriaRefugio()
    {
        // Opción A: Buscar objeto con tag "Refugio" y ver si el jugador está cerca
        GameObject refugio = GameObject.FindGameObjectWithTag(refugioTag);
        if (refugio != null)
        {
            float distancia = Vector2.Distance(jugador.position, refugio.transform.position);
            return distancia < radioVictoria;
        }

        // Opción B: Usar coordenadas fijas
        // float distancia = Vector2.Distance(jugador.position, coordenadasRefugio);
        // return distancia < radioVictoria;

        return false;
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
        Debug.Log("Zombies muertos: " + zombiesMuertos);
        // No hay condición de victoria aquí
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
        if (juegoTerminado) return;
        juegoTerminado = true;
        mensajeFinal = "VICTORIA - Has llegado al refugio! Presiona R para reiniciar";
        Time.timeScale = 0f;
        Debug.Log("VICTORIA");
    }

    void ReiniciarJuego()
    {
        Debug.Log("Reiniciando juego...");
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void OnGUI()
    {
        GUIStyle estilo = new GUIStyle();
        estilo.fontSize = 20;
        estilo.normal.textColor = Color.white;
        GUI.Label(new Rect(10, 10, 200, 30), "Vidas: " + vidas);
        // Mostrar solo el número de zombies muertos (sin total)
        GUI.Label(new Rect(10, 40, 200, 30), "Zombies eliminados: " + zombiesMuertos);

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