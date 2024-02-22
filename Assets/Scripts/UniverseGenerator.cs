using UnityEngine;
using System.Collections.Generic;

public class UniverseGenerator : MonoBehaviour
{
    public GameObject starPrefab;
    public GameObject planetPrefab;
    public int initialStarPoolSize = 50;
    public int initialPlanetPoolSize = 100;
    public float spawnDistance = 200f;
    public float despawnDistance = 250f;

    private Transform player;
    private List<GameObject> starPool = new List<GameObject>();
    private List<GameObject> planetPool = new List<GameObject>();

    void Start()
    {
        player = Camera.main.transform;
        InitializePool();
    }

    void Update()
    {
        RecycleObjects();
        SpawnObjects();
    }

    void InitializePool()
    {
        for (int i = 0; i < initialStarPoolSize; i++)
        {
            GameObject star = Instantiate(starPrefab, RandomPosition(), Quaternion.identity);
            star.SetActive(false);
            starPool.Add(star);
        }

        for (int i = 0; i < initialPlanetPoolSize; i++)
        {
            GameObject planet = Instantiate(planetPrefab, RandomPosition(), Quaternion.identity);
            planet.SetActive(false);
            planetPool.Add(planet);
        }
    }

    void RecycleObjects()
    {
        foreach (GameObject star in starPool)
        {
            if (Vector3.Distance(star.transform.position, player.position) > despawnDistance)
            {
                star.SetActive(false);
                star.transform.position = RandomPosition();
            }
        }

        foreach (GameObject planet in planetPool)
        {
            if (Vector3.Distance(planet.transform.position, player.position) > despawnDistance)
            {
                planet.SetActive(false);
                planet.transform.position = RandomPosition();
            }
        }
    }

    void SpawnObjects()
    {
        foreach (GameObject star in starPool)
        {
            if (!star.activeSelf && Vector3.Distance(star.transform.position, player.position) < spawnDistance)
            {
                star.SetActive(true);
            }
        }

        foreach (GameObject planet in planetPool)
        {
            if (!planet.activeSelf && Vector3.Distance(planet.transform.position, player.position) < spawnDistance)
            {
                planet.SetActive(true);
            }
        }
    }

    Vector3 RandomPosition()
    {
        return player.position + Random.onUnitSphere * Random.Range(spawnDistance, despawnDistance);
    }
}