using UnityEngine;

public class BallSlotDetector : MonoBehaviour
{
    public static string winningNumber;

    private void OnTriggerEnter(Collider other)
    {
        // Check if it hit a SlotNumber parent
        if (other.gameObject.name == "SlotNumber")
        {
            // Loop through all the children of the SlotNumber parent
            foreach (Transform child in other.transform)
            {
                // Check if ball is physically close to the child
                float distance = Vector3.Distance(transform.position, child.position);

                // You can adjust this distance threshold if needed
                if (distance < 0.5f)
                {
                    winningNumber = child.name;
                    Debug.Log("Ball landed in: " + winningNumber);
                    break; // Found the correct slot, no need to keep checking
                }
            }
        }
    }
}
