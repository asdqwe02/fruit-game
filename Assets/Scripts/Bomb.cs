using System;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    public ParticleSystem particleSystem;
    private AudioSource bombSizzle;

    private void Start()
    {
        bombSizzle = AudioManager.Instance.PlaySound(AudioManager.Sound.BombSizzle, transform.position);
    }

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
        bombSizzle.Stop();
        GetComponent<Collider>().enabled = false;
        GameManager.Instance.Explode(transform);
        particleSystem.Play();
        particleSystem.GetComponent<LockChildRotation>().enabled = true;
    }

    private void OnDestroy()
    {
        bombSizzle.Stop();
    }
}