using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScenes : MonoBehaviour
{

    public void GoToSceneTwo() {
        SceneManager.LoadScene("BlackJackGame");    
    }
    
}
