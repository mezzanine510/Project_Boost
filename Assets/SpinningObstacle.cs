using UnityEngine;

[DisallowMultipleComponent]

public class SpinningObstacle : MonoBehaviour
{
    [SerializeField] float rotationRate = 25f;
    public float delta;
    
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        delta = Time.deltaTime;
        transform.Rotate(Vector3.forward * rotationRate * delta);
    }
}
