using UnityEngine;
using System.Collections;

public class SlotMachineController : MonoBehaviour
{
    public WheelRotate[] wheels;              // Your wheel scripts
    public Animation legacyAnim;              // Drag the Animation component here
    public string animationName = "HopefullyTake001"; // The name of the legacy .anim clip

    private bool isSpinning = false;

    void OnMouseDown()
    {
        if (!isSpinning)
        {
            // 🔁 Play legacy animation
            if (legacyAnim != null && !legacyAnim.isPlaying)
            {
                legacyAnim.Play(animationName);
            }

            StartCoroutine(SpinAll());
        }
    }

    IEnumerator SpinAll()
    {
        isSpinning = true;

        foreach (WheelRotate wheel in wheels)
        {
            StartCoroutine(wheel.SpinAndStop());
        }

        yield return new WaitForSeconds(2.5f);
        isSpinning = false;
    }
}
