using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed = 5;
    public float moveableDistance = 3; //포인터와 플레이어의 위치가 3이상 차이나면 움직이게끔
    public Transform mousePointer;

    Plane plane = new Plane(new Vector3(0, 1, 0), 0); //두번째 인자 0은 평면을 만드는 노멀의 방향?



    void Update()
    {
        //RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

    
        float enter = 0.0f;
        if (plane.Raycast(ray, out enter))
        {
            Vector3 hitPoint = ray.GetPoint(enter);
            mousePointer.position = hitPoint;
            float distance = Vector3.Distance(hitPoint, transform.position);
            if (distance > moveableDistance)
            {
                var dir = hitPoint - transform.position;
                dir.Normalize();
                transform.Translate(dir * speed * Time.deltaTime);
            }
        }
    }
}
