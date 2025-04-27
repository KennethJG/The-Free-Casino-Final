using UnityEngine;
using UnityEngine.SceneManagement;

public class GoToSlotMachine : MonoBehaviour
{
    public void GoToSceneTwo()
    {
        SceneManager.LoadScene("SlotMachine");
    }

}
