using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformObstacle : MonoBehaviour
{
    [SerializeField] float rotationRate = 25f;

    public Rigidbody platform;
    public float delta;

    // Start is called before the first frame update
    void Start()
    {
        platform = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        delta = Time.deltaTime;
        transform.Rotate(Vector3.forward * rotationRate * delta);
    }
}
