using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : MonoBehaviour
{
    [SerializeField] float mainThrust = 15f;
    [SerializeField] float rcsThrust = 100f;

    public Rigidbody rocketRigidbody;
    AudioSource thrustSound;
    public float delta;
    bool thrustSoundIsPlaying;

    // Start is called before the first frame update
    void Start()
    {
        rocketRigidbody = GetComponent<Rigidbody>();
        thrustSound = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        delta = Time.deltaTime;
        Thrust();
        Rotate();
    }

    void OnCollisionEnter(Collision collision)
    {
        string tag = collision.gameObject.tag;
        switch (tag)
        {
            case "Friendly":
                print("RAWR");
                break;
            
            case "Fuel":
                print("You gawt FUEL!");
                break;

            default:
                print("Game Over MAAAAAAAAAAN");
                break;
        }
    }

    private void Thrust() {
        if (Input.GetKey(KeyCode.Space))
        {
            // float thrustThisFrame = mainThrust * delta;
            rocketRigidbody.AddRelativeForce(Vector3.up * mainThrust);
            if (!thrustSoundIsPlaying)
            {
                thrustSound.Play();
                thrustSoundIsPlaying = true;
            }
        }
        else
        {
            thrustSoundIsPlaying = false;
            thrustSound.Stop();
        }
    }

    private void Rotate()
    {
        rocketRigidbody.freezeRotation = true; // take manual control of rotation
        float rotationThisFrame = rcsThrust * delta;
        
        if (Input.GetKey(KeyCode.A)) {
            transform.Rotate(Vector3.forward * rotationThisFrame);
        }
        else if (Input.GetKey(KeyCode.D)) {
            transform.Rotate(Vector3.back * rotationThisFrame);
        }

        rocketRigidbody.freezeRotation = false; // resume physics control of rotation;
    }
}
