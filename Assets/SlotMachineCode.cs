using System.Collections;
using UnityEngine;

public class SlotMachine : MonoBehaviour
{
    public Animation anim;
    public SlotReel[] reels;


    public void Spin()
    {
        anim.Play("Take 001");
        StartCoroutine(FinishSpin());
    }

    IEnumerator FinishSpin()
    {
        yield return new WaitForSeconds(anim["Take 001"].length); // Wait for animation
        yield return new WaitForEndOfFrame(); // Wait for Unity to finish applying animation

        foreach (var reel in reels)
        {
            reel.SetToRandomSymbol(); // Snap wheel to result
        }

        Debug.Log("Final positions applied.");
    }

}
