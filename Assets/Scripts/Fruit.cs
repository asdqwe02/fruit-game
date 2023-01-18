using System.Collections;
using UnityEngine;

public class Fruit : MonoBehaviour
{
    public GameObject whole;
    public GameObject sliced;

    private Rigidbody fruitRigidbody;
    private Collider fruitCollider;
    [SerializeField] private ParticleSystem juiceEffect;
    [SerializeField] private Color fruitColor;
    [SerializeField] private SplashEffect splashPrefabs;
    public int points = 1;
    public bool SliceToStart = false;

    // public  
    private void Awake()
    {
        fruitRigidbody = GetComponent<Rigidbody>();
        fruitCollider = GetComponent<Collider>();
        if (juiceEffect == null)
            juiceEffect = GetComponentInChildren<ParticleSystem>();
    }

    public void Slice(Vector3 direction, Vector3 position, float force)
    {
        AudioManager.Instance.PlaySound(AudioManager.Sound.FruitSlice, transform.position);
        if (SliceToStart)
        {
            StartCoroutine(Reset(2f));
            GameManager.Instance.StartFruitCount++;
        }

        else
        {
            GameManager.Instance.IncreaseScore(points);
        }


        // Disable the whole fruit
        fruitCollider.enabled = false;
        whole.SetActive(false);
        // Enable the sliced fruit
        sliced.SetActive(true);
        juiceEffect.Play();
        SplashEffect splashEffect = Instantiate(splashPrefabs, transform.position, Quaternion.identity);
        splashEffect.SetColor(fruitColor);
        // Rotate based on the slice angle
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        sliced.transform.rotation = Quaternion.Euler(0f, 0f, angle);

        Rigidbody[] slices = sliced.GetComponentsInChildren<Rigidbody>();
        // Add a force to each slice based on the blade direction
        foreach (Rigidbody slice in slices)
        {
            slice.AddForceAtPosition(direction * force, position, ForceMode.Impulse);
            slice.velocity = fruitRigidbody.velocity + new Vector3(0, -25f, 0);
        }
        // fruitRigidbody.mass = 3;
        // fruitRigidbody.velocity = new Vector3(0, -10, 0);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Blade blade = other.GetComponent<Blade>();
            Slice(blade.direction, blade.transform.position, blade.sliceForce);
        }
    }

    private void OnEnable()
    {
        if (SliceToStart)
            ResetFruitPosition();
    }

    private void ResetFruitPosition()
    {
        sliced.SetActive(false);
        sliced.transform.localPosition = Vector3.zero;
        foreach (Transform child in sliced.transform)
        {
            child.localPosition = Vector3.zero;
            fruitCollider.enabled = true;
        }

        fruitRigidbody.angularVelocity = new Vector3(Random.Range(-3f, 3f), Random.Range(-3f, 3f), 0);
        whole.SetActive(true);
    }

    IEnumerator Reset(float duration)
    {
        // float elapsedTime = 0;
        // while (elapsedTime < duration)
        // {
        //     elapsedTime += Time.unscaledDeltaTime;
        //     yield return new WaitForEndOfFrame();
        // }
        yield return new WaitForSecondsRealtime(duration);
        ResetFruitPosition();
        GameManager.Instance.StartFruitCount--;
    }
}