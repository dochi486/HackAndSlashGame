using UnityEngine;

public class FollowTargetCamera : MonoBehaviour
{
    public Transform target;
    public Vector3 offset;

    public BoxCollider moveableArea;

    public float minX, maxX, minZ, maxZ;


    void Start()
    {
        var camera = GetComponent<Camera>();

        float height = 2f * camera.orthographicSize;
        float width = height * camera.aspect;

        offset = target.position - transform.position;
        offset.x = 0; //플레이어가 가장자리에 있더라도 카메라 위치는 중앙에서 시작?
        minX = width / 2 + moveableArea.transform.position.x + moveableArea.center.x - moveableArea.size.x / 2;
        maxX = -width / 2 + moveableArea.transform.position.x + moveableArea.center.x + moveableArea.size.x / 2;

        minZ = height / 2 + moveableArea.transform.position.z + moveableArea.center.z - moveableArea.size.z / 2;
        maxZ = -height / 2 + moveableArea.transform.position.z + moveableArea.center.z + moveableArea.size.z / 2;
        //센터에서 x값의 반을 뺀 것이 최소랑.. 더한 게 최대..!에서 시작
    }

    public float lerp = 0.05f;
    void LateUpdate()
    {

        var newPos = target.position - offset;

        newPos.x = Mathf.Min(newPos.x, maxX);
        newPos.x = Mathf.Max(newPos.x, minX);

        newPos.z = Mathf.Min(newPos.z, maxZ);
        newPos.z = Mathf.Max(newPos.z, minZ);
        transform.position = Vector3.Lerp(transform.position, newPos, lerp);

    }
}
