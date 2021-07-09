using UnityEngine;

public class FollowTargetCamera : MonoBehaviour
{
    public Transform target;
    public Vector3 offset;

    public BoxCollider moveableArea;

    public float minX, maxX, minZ, maxZ;


    void Start()
    {
        offset = target.position - transform.position;
        minX = moveableArea.transform.position.x +  moveableArea.center.x - moveableArea.size.x/2;
        maxX = moveableArea.transform.position.x +  moveableArea.center.x + moveableArea.size.x/2;

        minZ = moveableArea.transform.position.z +moveableArea.center.z - moveableArea.size.z/2;
        maxZ = moveableArea.transform.position.z + moveableArea.center.z + moveableArea.size.z/2;

    }

    // Update is called once per frame
    void LateUpdate()
    {

        var newPos = target.position - offset;

        newPos.x = Mathf.Min(newPos.x, maxX);
        newPos.x = Mathf.Max(newPos.x, minX);

        newPos.z = Mathf.Min(newPos.z, maxZ);
        newPos.z = Mathf.Max(newPos.z, minZ);
        transform.position = newPos;

    }
}
