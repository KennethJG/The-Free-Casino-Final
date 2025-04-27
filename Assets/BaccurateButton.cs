using UnityEngine;
using UnityEngine.UI;

public class BaccurateButton : MonoBehaviour
{
    public Button sceneChangeButton;

    private void OnMouseDown()
    {
        // When the cube is clicked, the button will be triggered
        sceneChangeButton.onClick.Invoke();
    }
}
