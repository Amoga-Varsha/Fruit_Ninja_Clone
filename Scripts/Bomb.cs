using UnityEngine;

public class Bomb : MonoBehaviour
{
    [SerializeField] private AudioClip bombsound; // Sound when fruit is sliced
    private AudioSource audioSource; // AudioSource to play the slice sound

    
    private void OnTriggerEnter(Collider other)
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        if (other.CompareTag("Player"))
        {
            GetComponent<Collider>().enabled = false;
            GameManager.Instance.Explode();
            audioSource.PlayOneShot(bombsound);
        }
    }

}
