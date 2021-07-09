using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed = 5;
    public float moveableDistance = 3; //포인터와 플레이어의 위치가 3이상 차이나면 움직이게끔
    public Transform mousePointer;
    public AnimationCurve jumpYac;
    Plane plane = new Plane(new Vector3(0, 1, 0), 0); //두번째 인자 0은 평면을 만드는 노멀의 방향?

    void Update()
    {
        //RaycastHit hit;
        Move();
        Jump();
    }

    private void Jump()
    {

        if (jumpState == JumpStateType.Jump)
            return;
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            StartCoroutine(JumpCo());
        }
    }
    public float jumpYMultiply = 20;
    public float jumpTimeMultiply = 1;


    public enum JumpStateType
    {
        Ground,
        Jump,
    }

    public enum StateType
    {
        Idle,
        Walk,
        Jump,
        Attack,
    }

    StateType state = StateType.Idle;

    JumpStateType jumpState;
    IEnumerator JumpCo()
    {
        jumpState = JumpStateType.Jump;
        state = StateType.Jump;
        float jumpStartTime = Time.time;
        float jumpDuration = jumpYac[jumpYac.length - 1].time;
        jumpDuration *= jumpTimeMultiply;
        float jumpEndTime = jumpStartTime + jumpDuration;
        float sumEvaluateTime = 0;

        while (Time.time < jumpEndTime)
        {
            float y = jumpYac.Evaluate(sumEvaluateTime / jumpTimeMultiply);
            y *= jumpYMultiply;
            transform.Translate(0, y, 0);
            yield return null;
            sumEvaluateTime += Time.deltaTime;
        }

        jumpState = JumpStateType.Ground;
        state = StateType.Idle;
    }

    private void Move()
    {
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
                state = StateType.Walk;
            }
            else
            {
                state = StateType.Idle;
            }
        }
    }
}
