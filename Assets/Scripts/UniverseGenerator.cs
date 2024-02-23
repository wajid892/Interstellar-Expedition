using UnityEngine;

public class UniverseGenerator : MonoBehaviour
{
    public GameObject galaxyParent;
    public GameObject starParent;
    public GameObject planetParent;
    public GameObject[] starPrefabs;
    public GameObject[] planetPrefabs;
    public int minStars = 50;
    public int maxStars = 100;
    public int minPlanets = 100;
    public int maxPlanets = 200;
    public float minStarDistance = 20f;
    public float maxStarDistance = 50f;
    public float minPlanetDistance = 20f;
    public float maxPlanetDistance = 50f;
    public float environmentRadius = 1000f;

    void Start()
    {
        // Create parent objects for stars, planets, and galaxies
        galaxyParent = new GameObject("Galaxy");
        starParent = new GameObject("Stars");
        planetParent = new GameObject("Planets");

        GenerateStars();
        GeneratePlanets();
    }

    void GenerateStars()
    {
        starParent.transform.parent = galaxyParent.transform; // Set star parent as child of galaxy parent

        int numberOfStars = Random.Range(minStars, maxStars + 1);
        for (int i = 0; i < numberOfStars; i++)
        {
            GameObject starPrefab = starPrefabs[Random.Range(0, starPrefabs.Length)];
            Vector3 position = GetRandomPosition(minStarDistance, maxStarDistance);
            GameObject star = Instantiate(starPrefab, position, Quaternion.identity);

            // Check for overlap with other stars and planets
            if (!CheckOverlap(star, starParent.transform))
            {
                star.transform.parent = starParent.transform; // Set star as child of star parent
            }
            else
            {
                Destroy(star); // Destroy star if it overlaps with other objects
            }
        }
    }

    void GeneratePlanets()
    {
        planetParent.transform.parent = galaxyParent.transform; // Set planet parent as child of galaxy parent

        int numberOfPlanets = Random.Range(minPlanets, maxPlanets + 1);
        for (int i = 0; i < numberOfPlanets; i++)
        {
            GameObject planetPrefab = planetPrefabs[Random.Range(0, planetPrefabs.Length)];
            Vector3 position = GetRandomPosition(minPlanetDistance, maxPlanetDistance);
            GameObject planet = Instantiate(planetPrefab, position, Quaternion.identity);

            // Check for overlap with other stars, planets, and galaxies
            if (!CheckOverlap(planet, starParent.transform) && !CheckOverlap(planet, planetParent.transform) && !CheckOverlap(planet, galaxyParent.transform))
            {
                planet.transform.parent = planetParent.transform; // Set planet as child of planet parent
            }
            else
            {
                Destroy(planet); // Destroy planet if it overlaps with other objects
            }
        }
    }

    Vector3 GetRandomPosition(float minDistance, float maxDistance)
    {
        Vector3 randomDirection = Random.insideUnitSphere.normalized;
        Vector3 randomPosition = transform.position + randomDirection * Random.Range(minDistance, maxDistance);
        return randomPosition;
    }

    bool CheckOverlap(GameObject obj, Transform parent)
    {
        Collider[] colliders = Physics.OverlapSphere(obj.transform.position, 1f);

        foreach (Collider collider in colliders)
        {
            if (collider.transform.parent == parent)
            {
                return true; // Overlap detected with objects in the same parent
            }
        }

        return false; // No overlap detected
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, environmentRadius);
    }
}
