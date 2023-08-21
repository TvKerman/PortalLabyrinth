using UnityEngine;

public class Teleporter : MonoBehaviour
{
    public Teleporter Other;
    private void OnTriggerStay(Collider other)
    {
        // convert the player's global coordinates to local relative to the portal
        float zPos = transform.worldToLocalMatrix.MultiplyPoint3x4(other.transform.position).z;

        // teleport if most of the player went behind the portal
        if (zPos > 0) Teleport(other.transform);
    }

    private void Teleport(Transform obj)
    {
        // Player position after teleporting
        
        Vector3 localPos = transform.worldToLocalMatrix.MultiplyPoint3x4(obj.position);
        
        // invert and set the new position of the player
        localPos = new Vector3(-localPos.x, localPos.y, -localPos.z);
        obj.position = Other.transform.localToWorldMatrix.MultiplyPoint3x4(localPos);

        // Player rotation after teleporting
        Quaternion difference = Other.transform.rotation * Quaternion.Inverse(transform.rotation * Quaternion.Euler(0, 180, 0));
        obj.rotation = difference * obj.rotation;
    }
}
