using UnityEngine;
public AudioClip zombieSonido;
public AudioSource audioSource;
public class ZombieSonido : MonoBehaviour

{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
   void Start()
{
    InvokeRepeating("SonidoZombie", 2f, 5f);
}

void SonidoZombie()
{
    audioSource.PlayOneShot(zombieSonido);
}
    void Update()
    {
        
    }
}
