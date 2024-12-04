using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Spawner : MonoBehaviour
{
    private Collider spawnArea;

    public GameObject[] fruitPrefabs;
    public GameObject bombPrefab;
    [Range(0f, 1f)]
    public float bombChance = 0.05f;

    public float minSpawnDelay = 0.25f;
    public float maxSpawnDelay = 1f;

    public float minAngle = -15f;
    public float maxAngle = 15f;

    public float minForce = 18f;
    public float maxForce = 22f;

    public float maxLifetime = 5f;

    public float gameDuration = 60f; 

    [SerializeField] private AudioClip spawnSound; // Sound when fruit is spawned
    private AudioSource audioSource; // AudioSource to play the spawn sound

    // Total game time in seconds
    private float elapsedTime = 0f;   // Track elapsed time
    private float spawnIncreaseInterval = 5f; // Time interval to decrease spawn delay
    private float spawnDelayReduction = 0.1f; // Amount to reduce spawn delay

    private void Awake()
    {
        spawnArea = GetComponent<Collider>();
        audioSource = gameObject.AddComponent<AudioSource>();
    }

    private void OnEnable()
    {
        StartCoroutine(Spawn());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    private IEnumerator Spawn()
    {
        yield return new WaitForSeconds(2f);

        while (enabled)
        {
            // Increase the elapsed time
            elapsedTime += Time.deltaTime;

            // Reduce the spawn delay every spawnIncreaseInterval seconds
            if (elapsedTime >= spawnIncreaseInterval)
            {
                minSpawnDelay = Mathf.Max(0.1f, minSpawnDelay - spawnDelayReduction); // Prevent negative delays
                maxSpawnDelay = Mathf.Max(0.1f, maxSpawnDelay - spawnDelayReduction);
                elapsedTime = 0f; // Reset the elapsed time
            }

            GameObject prefab = fruitPrefabs[Random.Range(0, fruitPrefabs.Length)];

            if (Random.value < bombChance)
            {
                prefab = bombPrefab;
            }

            Vector3 position = new Vector3
            {
                x = Random.Range(spawnArea.bounds.min.x, spawnArea.bounds.max.x),
                y = Random.Range(spawnArea.bounds.min.y, spawnArea.bounds.max.y),
                z = Random.Range(spawnArea.bounds.min.z, spawnArea.bounds.max.z)
            };

            Quaternion rotation = Quaternion.Euler(0f, 0f, Random.Range(minAngle, maxAngle));

            GameObject fruit = Instantiate(prefab, position, rotation);
            audioSource.PlayOneShot(spawnSound);
            Destroy(fruit, maxLifetime);

            float force = Random.Range(minForce, maxForce);
            fruit.GetComponent<Rigidbody>().AddForce(fruit.transform.up * force, ForceMode.Impulse);

            yield return new WaitForSeconds(Random.Range(minSpawnDelay, maxSpawnDelay));
        }
    }
}
