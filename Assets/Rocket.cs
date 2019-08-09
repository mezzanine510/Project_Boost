using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour
{
    [SerializeField] float mainThrust = 1500f;
    [SerializeField] float rcsThrust = 150f;
    [SerializeField] float thrustLightRate = 30f;
    float thrustLightMin = 0;
    float thrustLightMax = 5;
    float levelLoadDelay = 3f;
    bool deathOnCollision = true;

    [SerializeField] AudioClip mainEngineSound;
    [SerializeField] AudioClip successSound;
    [SerializeField] AudioClip crashSound;

    [SerializeField] ParticleSystem mainEngineParticles = default;
    [SerializeField] ParticleSystem successParticles = default;
    [SerializeField] ParticleSystem crashParticles = default;

    [SerializeField] Light thrustLight;

    public Rigidbody rocketRigidbody = default;
    AudioSource audioSource;
    int numberOfLevels;
    bool thrustSoundIsPlaying;
    float delta;

    bool isTransitioning = false;
    bool collisionDeathDisabled = false;

    // keep this method for future reference
    // enum State { Alive, Dying, Transcending };
    // State currentState = State.Alive;


    // Start is called before the first frame update
    void Start()
    {
        rocketRigidbody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
        numberOfLevels = SceneManager.sceneCountInBuildSettings;
        print(numberOfLevels);
    }

    // Update is called once per frame
    void Update()
    {
        if (!isTransitioning) {
            delta = Time.deltaTime;
            Thrust();
            Rotate();
        }

        if (Debug.isDebugBuild)
        {
            RespondToDebugKeys();
        }
    }

    void RespondToDebugKeys()
    {
        if (Input.GetKeyDown(KeyCode.L)) // debugging
        {
            LoadNextLevel();
        }
        else if (Input.GetKeyDown(KeyCode.C)) // debugging
        {
            collisionDeathDisabled = !collisionDeathDisabled;

            // GameObject[] collisionObjects = GameObject.FindGameObjectsWithTag("Obstacle");
            // foreach (GameObject gameObject in collisionObjects)
            // {
            //     gameObject.tag = "Friendly";
            // }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (isTransitioning || collisionDeathDisabled) { return; }

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
        isTransitioning = true;
        successParticles.Play();
        mainEngineParticles.Stop();
        StopThrustSound();
        PlaySuccessSound();
        Invoke("LoadNextLevel", levelLoadDelay);
    }

    void StartCrashSequence() {
        isTransitioning = true;
        crashParticles.Play();
        mainEngineParticles.Stop();
        StopThrustSound();
        PlayCrashSound();
        Invoke("LoadFirstLevel", levelLoadDelay);
    }

    void LoadNextLevel() {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;
        if (nextSceneIndex == numberOfLevels) 
        {
            LoadFirstLevel();
        }
        else {
            SceneManager.LoadScene(nextSceneIndex);
        }
    }

    void LoadFirstLevel() {
        SceneManager.LoadScene(0);
    }

    void Thrust() {
        if (Input.GetKey(KeyCode.Space))
        {
            ApplyThrust();
        }
        else
        {
            StopApplyingThrust();
        }
    }

    void ApplyThrust()
    {
        rocketRigidbody.AddRelativeForce(Vector3.up * mainThrust * delta);

        if (!thrustSoundIsPlaying)
        {
            PlayThrustSound();
        }

        mainEngineParticles.Play();
        if (thrustLight.intensity < thrustLightMax) 
        {
            thrustLight.intensity += thrustLightRate * delta;
        }
    }

    void StopApplyingThrust()
    {
        StopThrustSound();
        mainEngineParticles.Stop();
        if (thrustLight.intensity > thrustLightMin) 
        {
            thrustLight.intensity -= thrustLightRate * delta;
        }
    }

    void PlayThrustSound() {
        audioSource.PlayOneShot(mainEngineSound);
        thrustSoundIsPlaying = true;
    }

    void StopThrustSound() {
        audioSource.Stop();
        thrustSoundIsPlaying = false;
    }

    private void PlaySuccessSound() {
        audioSource.PlayOneShot(successSound);
    }

    void PlayCrashSound() {
        audioSource.PlayOneShot(crashSound);
    }

    void Rotate()
    {
        if (Input.GetKey(KeyCode.A))
        {
            ManualRotation(rcsThrust * delta);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            ManualRotation(-rcsThrust * delta);
        }
    }

    void ManualRotation(float rotationThisFrame) {
        rocketRigidbody.freezeRotation = true; // take manual control of rotation
        transform.Rotate(Vector3.forward * rotationThisFrame);
        rocketRigidbody.freezeRotation = false; // resume physics control of rotation;
    }
}
