using UnityEngine;

public class PlayerAudio : MonoBehaviour
{
    public AudioSource audioSource;

    public AudioClip disparoEscopeta;
    public AudioClip golpeBate;
    public AudioClip meleeEscopeta;
    public AudioClip meleeBate;
    public AudioClip muerte;

    public void PlayDisparo()
    {
        audioSource.PlayOneShot(disparoEscopeta);
    }

    public void PlayGolpeBate()
    {
        audioSource.PlayOneShot(golpeBate);
    }

    public void PlayMeleeEscopeta()
    {
        audioSource.PlayOneShot(meleeEscopeta);
    }

    public void PlayMeleeBate()
    {
        audioSource.PlayOneShot(meleeBate);
    }

    public void PlayMuerte()
    {
        audioSource.PlayOneShot(muerte);
    }
}