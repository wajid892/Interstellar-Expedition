using UnityEngine;

public class UniverseGenerator : MonoBehaviour
{
    [Header("Parent Objects")]
    [HideInInspector][SerializeField] private GameObject galaxyParent;
    [HideInInspector][SerializeField] private GameObject starParent;
    [HideInInspector][SerializeField] private GameObject planetParent;

    [Header("Prefabs")]
    public GameObject[] starPrefabs;
    public GameObject[] planetPrefabs;

    [Header("Star Settings")]
    [SerializeField] private int minStars = 50;
    [SerializeField] private int maxStars = 100;
    [SerializeField] private float maxStarDistance = 50f;
    [SerializeField] private float minStarDistance = 20f;

    [Header("Planet Settings")]
    [SerializeField] private int minPlanets = 100;
    [SerializeField] private int maxPlanets = 200;
    [SerializeField] private float minPlanetDistance = 20f;
    [SerializeField] private float maxPlanetDistance = 50f;
    [SerializeField] private float environmentRadius = 1000f;

    void Start()
    {
        // Create parent objects for stars, planets, and galaxies
        galaxyParent = new GameObject("Galaxy");
        starParent = new GameObject("Stars");
        planetParent = new GameObject("Planets");

        GenerateStars();
        GeneratePlanets();
    }

    // Generate stars
    void GenerateStars()
    {
        starParent.transform.parent = galaxyParent.transform;

        int numberOfStars = Random.Range(minStars, maxStars + 1);
        for (int i = 0; i < numberOfStars; i++)
        {
            GameObject starPrefab = starPrefabs[Random.Range(0, starPrefabs.Length)];
            Vector3 position = GetRandomPosition(minStarDistance, maxStarDistance);
            GameObject star = Instantiate(starPrefab, position, Quaternion.identity);

            if (!CheckOverlap(star, starParent.transform))
            {
                star.transform.parent = starParent.transform;
            }
            else
            {
                Destroy(star);
            }
        }
    }

    // Generate planets
    void GeneratePlanets()
    {
        planetParent.transform.parent = galaxyParent.transform;

        int numberOfPlanets = Random.Range(minPlanets, maxPlanets + 1);
        for (int i = 0; i < numberOfPlanets; i++)
        {
            GameObject planetPrefab = planetPrefabs[Random.Range(0, planetPrefabs.Length)];
            Vector3 position = GetRandomPosition(minPlanetDistance, maxPlanetDistance);
            GameObject planet = Instantiate(planetPrefab, position, Quaternion.identity);

            if (!CheckOverlap(planet, starParent.transform) && !CheckOverlap(planet, planetParent.transform) && !CheckOverlap(planet, galaxyParent.transform))
            {
                planet.transform.parent = planetParent.transform;
            }
            else
            {
                Destroy(planet);
            }
        }
    }

    // Get random position
    Vector3 GetRandomPosition(float minDistance, float maxDistance)
    {
        Vector3 randomDirection = Random.insideUnitSphere.normalized;
        Vector3 randomPosition = transform.position + randomDirection * Random.Range(minDistance, maxDistance);
        return randomPosition;
    }

    // Check overlap
    bool CheckOverlap(GameObject obj, Transform parent)
    {
        Collider[] colliders = Physics.OverlapSphere(obj.transform.position, 1f);

        foreach (Collider collider in colliders)
        {
            if (collider.transform.parent == parent)
            {
                return true;
            }
        }

        return false;
    }

    // Draw environment radius in the editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, environmentRadius);
    }
}
