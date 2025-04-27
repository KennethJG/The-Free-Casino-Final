using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    public static PlayerMovement instance;

    public CharacterController controller;
    public float speed = 12f;
    public float gravity = -9.81f;

    Vector3 velocity;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ResizePlayerByScene(scene.name);
    }

    void Update()
    {
        float x = 0f;
        float z = 0f;

        if (Input.GetKey(KeyCode.A)) x = -1f;
        if (Input.GetKey(KeyCode.D)) x = 1f;
        if (Input.GetKey(KeyCode.W)) z = 1f;
        if (Input.GetKey(KeyCode.S)) z = -1f;

        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move * speed * Time.deltaTime);

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    void ResizePlayerByScene(string sceneName)
    {
        if (sceneName == "Casino_Scene") 
        {
            transform.localScale = new Vector3(1f, 1f, 1f);
            transform.localPosition = new Vector3(3.51f, 2.42f, -5.05f); 
        }
        else if (sceneName == "Baccuratte")
            transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        else if (sceneName == "BlackJackGame") 
        { 
            transform.localScale = new Vector3(1f, 1.5f, 1f);
            transform.localPosition = new Vector3(3.682f, 4.705f, -1.02f);
        }
    }
}
