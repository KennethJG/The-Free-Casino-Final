using UnityEngine;
using System.Collections;

public class WheelRotate : MonoBehaviour
{
    public float spinDuration = 2.0f;      // How long the wheel spins before stopping
    public float spinSpeed = 720f;         // Degrees per second
    public int symbolCount = 6;            // Number of symbols on the reel
    public float symbolSpacing = 60f;      // Degrees between symbols

    public SlotManager slotManager;        // Reference to SlotManager (assign in Inspector)

    private bool spinning = false;

    void Start()
    {
        StartCoroutine(SpinAndStop());
    }


    public IEnumerator SpinAndStop()
    {
        spinning = true;
        float elapsed = 0f;

        // Step 1: Spin for a fixed duration
        while (elapsed < spinDuration)
        {
            transform.Rotate(spinSpeed * Time.deltaTime, 0f, 0f, Space.World);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Step 2: Pick a random final angle based on symbol count
        int randomSymbol = Random.Range(0, symbolCount);
        float targetAngle = randomSymbol * symbolSpacing;

        // Step 3: Snap to stopping point
        float currentX = transform.eulerAngles.x;
        float snapAngle = Mathf.Floor(currentX / 360f) * 360f + targetAngle;

        // Smooth rotation to final position
        float duration = 0.5f;
        float t = 0f;
        float startAngle = currentX;

        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            float angle = Mathf.Lerp(startAngle, snapAngle, t);
            transform.eulerAngles = new Vector3(angle, transform.eulerAngles.y, transform.eulerAngles.z);
            yield return null;
        }

        transform.eulerAngles = new Vector3(snapAngle, transform.eulerAngles.y, transform.eulerAngles.z);
        spinning = false;

        Debug.Log($"{gameObject.name} stopped on symbol: {randomSymbol}");

        // 🔁 Report to SlotManager
        if (slotManager != null)
        {
            slotManager.ReportResult(this, randomSymbol);
        }
    }
}
