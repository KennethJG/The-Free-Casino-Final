using UnityEngine;
using UnityEngine.UI; // For accessing UI components

public class CubeInteraction : MonoBehaviour
{
    public Button sceneChangeButton;

    private void OnMouseDown()
    {
        // When the cube is clicked, the button will be triggered
        sceneChangeButton.onClick.Invoke();
    }
}
