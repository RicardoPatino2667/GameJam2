using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject zombiePrefab;

    [Header("Puntos de Spawn")]
    public Transform[] puntosSpawn; // Arrastra aquí los puntos en el Inspector

    [Header("Configuración")]
    public float tiempoEntreSpawns = 3f;  // Segundos entre cada spawn
    public int maxZombiesEnEscena = 10;   // Límite de zombies simultáneos
    public float tiempoEntreSpawnsMin = 2f; // Tiempo mínimo aleatorio
    public float tiempoEntreSpawnsMax = 5f; // Tiempo máximo aleatorio

    private float temporizador;
    private int zombiesActuales;

    void Start()
    {
        if (zombiePrefab == null)
            Debug.LogError("Falta asignar el prefab de Zombie en SpawnManager");

        if (puntosSpawn.Length == 0)
            Debug.LogError("No hay puntos de spawn asignados en SpawnManager");

        // Primer spawn aleatorio
        temporizador = Random.Range(tiempoEntreSpawnsMin, tiempoEntreSpawnsMax);
    }

    void Update()
    {
        // Contar zombies actuales en escena
        zombiesActuales = FindObjectsByType<Zombie>(FindObjectsSortMode.None).Length;

        if (zombiesActuales >= maxZombiesEnEscena) return;

        temporizador -= Time.deltaTime;

        if (temporizador <= 0f)
        {
            SpawnZombie();
            // Tiempo aleatorio para el siguiente spawn
            temporizador = Random.Range(tiempoEntreSpawnsMin, tiempoEntreSpawnsMax);
        }
    }

    void SpawnZombie()
    {
        if (zombiePrefab == null || puntosSpawn.Length == 0) return;

        // Elegir punto de spawn aleatorio
        int indice = Random.Range(0, puntosSpawn.Length);
        Transform puntoElegido = puntosSpawn[indice];

        // Instanciar zombie
        Instantiate(zombiePrefab, puntoElegido.position, Quaternion.identity);
        Debug.Log("Zombie spawneado en: " + puntoElegido.name);
    }

    // Llamar esto desde GameManager cuando muere un zombie si quieres
    public void ZombieMurio()
    {
        zombiesActuales--;
    }
}