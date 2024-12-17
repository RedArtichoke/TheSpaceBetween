using UnityEngine;

public class LockParticleRotation : MonoBehaviour
{
    void LateUpdate()
    {
        // Reset the rotation to keep it fixed in world space
        transform.rotation = Quaternion.Euler(-90, 0, 0);
    }
}
