using UnityEngine;

public class SlotReel : MonoBehaviour
{
    public int symbolCount = 6;
    public float symbolSpacing = 60f; // Adjust based on your wheel design

    public void SetToRandomSymbol()
    {
        int symbolIndex = Random.Range(0, symbolCount);
        float finalXRotation = symbolIndex * symbolSpacing;

        // Set only X axis
        transform.localEulerAngles = new Vector3(finalXRotation, 0, 0);
    }
}
