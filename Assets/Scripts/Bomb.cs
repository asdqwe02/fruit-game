using UnityEngine;

public class Bomb : MonoBehaviour
{
    public ParticleSystem particleSystem;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // GetComponent<Collider>().enabled = false;
            // GameManager.Instance.Explode(transform);
            // particleSystem.Play();
            Explode();
        }
    }

    public void Explode()
    {
        GetComponent<Collider>().enabled = false;
        GameManager.Instance.Explode(transform);
        particleSystem.Play();
        particleSystem.GetComponent<LockChildRotation>().enabled = true;
    }
}