using UnityEngine;

public class SoundEffect : MonoBehaviour
{
    public AudioClip shotgunTest;
    public void ShotGunSound()
    {
         GetComponent<AudioSource>().PlayOneShot(shotgunTest);
    }
}
