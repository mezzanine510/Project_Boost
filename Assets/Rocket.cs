using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour
{
    [SerializeField] float mainThrust = 1500f;
    [SerializeField] float rcsThrust = 150f;

    [SerializeField] AudioClip mainEngineSound;
    [SerializeField] AudioClip successSound;
    [SerializeField] AudioClip crashSound;

    [SerializeField] ParticleSystem mainEngineParticles = default;
    [SerializeField] ParticleSystem successParticles = default;
    [SerializeField] ParticleSystem crashParticles = default;

    public Rigidbody rocketRigidbody;
    public Light thrustLight;
    AudioSource audioSource;
    bool thrustSoundIsPlaying;
    public float delta;

    enum State { Alive, Dying, Transcending };
    State currentState = State.Alive;

    // Start is called before the first frame update
    void Start()
    {
        rocketRigidbody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
        thrustLight = GetComponent<Light>();
    }

    // Update is called once per frame
    void Update()
    {
        if (currentState == State.Alive) {
            delta = Time.deltaTime;
            Thrust();
            Rotate();
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (currentState != State.Alive) { return; }

        string tag = collision.gameObject.tag;
        switch (tag)
        {
            case "Friendly":
                // do nothing
                break;

            case "Finish":
                StartSuccessSequence();
                break;

            default:
                StartCrashSequence();
                break;
        }
    }

    void StartSuccessSequence() {
        currentState = State.Transcending;
        successParticles.Play();
        mainEngineParticles.Stop();
        StopThrustSound();
        PlaySuccessSound();
        Invoke("LoadNextLevel", 2f);
    }

    void StartCrashSequence() {
        currentState = State.Dying;
        crashParticles.Play();
        mainEngineParticles.Stop();
        StopThrustSound();
        PlayCrashSound();
        Invoke("LoadFirstLevel", 2f);
    }

    void LoadNextLevel() {
        SceneManager.LoadScene(1);
    }

    void LoadFirstLevel() {
        SceneManager.LoadScene(0);
    }

    private void Thrust() {
        if (Input.GetKey(KeyCode.Space))
        {
            ApplyThrust();
        }
        else
        {
            StopThrustSound();
            mainEngineParticles.Stop();
        }
    }

    private void ApplyThrust()
    {
        rocketRigidbody.AddRelativeForce(Vector3.up * mainThrust * delta);

        if (!thrustSoundIsPlaying)
        {
            PlayThrustSound();
        }

        mainEngineParticles.Play();
        thrustLight.intensity = 4.5f;
    }

    private void PlayThrustSound() {
        audioSource.PlayOneShot(mainEngineSound);
        thrustSoundIsPlaying = true;
    }

    private void StopThrustSound() {
        audioSource.Stop();
        thrustSoundIsPlaying = false;
    }

    private void PlaySuccessSound() {
        audioSource.PlayOneShot(successSound);
    }

    private void PlayCrashSound() {
        audioSource.PlayOneShot(crashSound);
    }

    private void Rotate()
    {
        // rocketRigidbody.angularVelocity = Vector3.zero;
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
