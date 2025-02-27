using UnityEngine;

public class SoundEffect : MonoBehaviour
{
    public AudioClip shotgunTest;
    public AudioClip reloadTest;
    public void ShotGunSound()
    {
         GetComponent<AudioSource>().PlayOneShot(shotgunTest);
    }
    public void ReloadSound()
    {
         GetComponent<AudioSource>().PlayOneShot(reloadTest);
    }
}
