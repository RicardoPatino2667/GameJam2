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
        
        GameObject refugio = GameObject.FindGameObjectWithTag(refugioTag);
        if (refugio != null)
        {
            float distancia = Vector2.Distance(jugador.position, refugio.transform.position);
            return distancia < radioVictoria;
        }

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
        mensajeFinal = "GAME OVER\nPresiona R para reiniciar";
        Time.timeScale = 0f;
        Debug.Log("GAME OVER");
    }

    void Victoria()
    {
        if (juegoTerminado) return;
        juegoTerminado = true;
        mensajeFinal = "<color=yellow>VICTORIA</color>\nHas llegado al refugio!\nPresiona R para reiniciar";
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
        // Estilo para los textos de información (vidas y zombies)
        GUIStyle infoStyle = new GUIStyle();
        infoStyle.fontSize = 40;               // Tamaño grande
        infoStyle.alignment = TextAnchor.MiddleCenter;
        infoStyle.normal.textColor = Color.red; // Color rojo
        infoStyle.fontStyle = FontStyle.Bold;   // Negrita (opcional)
        infoStyle.richText = true;

        // Mostrar vidas
        GUI.Label(new Rect(20, 20, 300, 80), "Vidas: " + vidas, infoStyle);

        // Mostrar zombies eliminados
        GUI.Label(new Rect(20, 100, 500, 80), "Zombies eliminados: " + zombiesMuertos, infoStyle);

        // Mensaje final (sin cambios, pero también puedes agrandarlo)
        if (juegoTerminado)
        {
            GUIStyle estiloFinal = new GUIStyle();
            estiloFinal.fontSize = 50;
            estiloFinal.alignment = TextAnchor.MiddleCenter;
            estiloFinal.normal.textColor = Color.red;
            estiloFinal.fontStyle = FontStyle.Bold;

            float ancho = 600;
            float alto = 120;
            float x = (Screen.width / 2) - (ancho / 2);
            float y = (Screen.height / 2) - (alto / 2);

            GUI.Label(new Rect(x, y, ancho, alto), mensajeFinal, estiloFinal);
        }
    }
}