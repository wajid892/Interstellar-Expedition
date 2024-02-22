using UnityEngine;
using System.Collections;

public class PlanetRotation : MonoBehaviour
{
    public GameObject itemToRotate;
    int Speed;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        // Speed = Random.Range(150, 250);
        Speed = Random.Range(1, 2);
        // itemToRotate.transform.Rotate(Vector3.forward * Time.deltaTime * Speed);
        itemToRotate.transform.Rotate(Vector3.down * Time.deltaTime * Speed);
    }
}
