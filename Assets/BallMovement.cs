using UnityEngine;

public class BallLauncher : MonoBehaviour
{
    public Rigidbody ballRb;
    public float launchForce = 5f;
    public float spinTorque = 10f;

    void Start()
    {
        Vector3 forceDirection = (transform.up + transform.right).normalized;
        ballRb.AddForce(forceDirection * launchForce, ForceMode.VelocityChange);
        ballRb.AddTorque(Vector3.up * spinTorque, ForceMode.VelocityChange);
    }
}
