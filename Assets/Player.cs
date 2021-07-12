using System.Collections;
using UnityEngine;
using NaughtyAttributes;

public class Player : MonoBehaviour
{
    public float speed = 5;
    float normalSpeed;
    public float walkDistance = 12; //포인터와 플레이어의 위치가 3이상 차이나면 움직이게끔
    public float stopDistance = 7;
    public Transform mousePointer;
    public AnimationCurve jumpYac;
    Plane plane = new Plane(new Vector3(0, 1, 0), 0); //두번째 인자 0은 평면을 만드는 노멀의 방향?

    public Transform spriteTr;
    SpriteTrailRenderer.SpriteTrailRenderer spriteTrailRenderer;

    private void Start()
    {
        normalSpeed = speed;
        animator = GetComponentInChildren<Animator>();
        spriteTr = GetComponentInChildren<SpriteRenderer>().transform; //GetChild해도 자기자신이 우선
        spriteTrailRenderer = GetComponentInChildren<SpriteTrailRenderer.SpriteTrailRenderer>();
        spriteTrailRenderer.enabled = false;
    }

    void Update()
    {
        Move();
        Jump();
        Dash();
    }
    #region Dash
    [Foldout("Dash")]
    public float dashableDistance = 10;
    [Foldout("Dash")]
    public float dashableTime = 0.4f;
    float mouseDownTime; //대쉬 드래그로 구현하기 위해서 마우스를 눌렀을 때의 시간과 포지션 담을 변수
    Vector3 mouseDownPosition;
    private void Dash()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            mouseDownTime = Time.time;
            mouseDownPosition = Input.mousePosition; //월드 포지션이 아닌 화면의 포지션
        }

        if (nextDashableTime < Time.time)
        {
            if (Input.GetKeyUp(KeyCode.Mouse0))
            {
                bool isDashDrag = IsSuccessDashDrag();
                if (isDashDrag)
                {
                    nextDashableTime = Time.time + dashCoolTime;
                    StartCoroutine(DashCo());
                }
            }
        }
    }
    [Foldout("Dash")]
    public float dashCoolTime = 2;
    [Foldout("Dash")]
    public float nextDashableTime;  //다음 대쉬 가능한 시간 (대쉬 쿨타임에 사용하는 다음 대쉬타임??)
    [Foldout("Dash")]
    public float dashTime = 0.3f; //dash하는 시간? 
    [Foldout("Dash")]
    public float dashSpeedMultiply = 4f;
    Vector3 dashDirection;
    IEnumerator DashCo()
    {
        spriteTrailRenderer.enabled = true;
        dashDirection = Input.mousePosition - mouseDownPosition;

        dashDirection.y = 0;
        dashDirection.z = 0;
        dashDirection.Normalize();

        speed = normalSpeed * dashSpeedMultiply;
        State = StateType.Dash;
        yield return new WaitForSeconds(dashTime);

        speed = normalSpeed;
        State = StateType.Idle;
        spriteTrailRenderer.enabled = false;
    }

    private bool IsSuccessDashDrag()
    {
        //시간 체크
        float dragTime = Time.time - mouseDownTime;
        if (dragTime > dashableTime)
            return false;
        //거리 체크해서 
        float dragDistance = Vector3.Distance(mouseDownPosition, Input.mousePosition);
        if (dragDistance < dashableDistance)
            return false;

        return true;
    }
    #endregion Dash
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
        Fall,
        Attack,
        Dash,
    }
    public StateType state = StateType.Idle;
    StateType State
    {
        get { return state; }
        set
        {
            if (state == value)
                return;
            state = value;
            animator.Play(state.ToString());

        }
    }

    Animator animator;
    public JumpStateType jumpState;
    IEnumerator JumpCo()
    {

        State = StateType.Jump;
        jumpState = JumpStateType.Jump;
        float jumpStartTime = Time.time;
        float jumpDuration = jumpYac[jumpYac.length - 1].time; //마지막키의 시간을 구하는 것
        jumpDuration *= jumpTimeMultiply;
        float jumpEndTime = jumpStartTime + jumpDuration;
        float sumEvaluateTime = 0;
        float previousY = 0;

        while (Time.time < jumpEndTime)
        {
            float y = jumpYac.Evaluate(sumEvaluateTime / jumpTimeMultiply); //evaluate는 키와 관계 없이 그래프 전체에 관련된 함수
            y *= jumpYMultiply;
            transform.Translate(0, y, 0);
            yield return null;

            if (previousY > y)
            {
                State = StateType.Fall;
            }
            previousY = y;
            sumEvaluateTime += Time.deltaTime;
        }

        jumpState = JumpStateType.Ground;
        State = StateType.Idle;
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

            float moveableDistance = stopDistance;
            // State가 Walk일 때 7(stopDistance) 사용, 
            // Idle에서 Walk로 갈 때는 12(walkDistance) 사용
            if (State == StateType.Idle)
                moveableDistance = walkDistance;

            if (distance > moveableDistance) //moveableDistance 변경해서 idle walk 변경 반복하던 것 수정할 예쩡.
            {
                var dir = hitPoint - transform.position;
                dir.Normalize();

                if (State == StateType.Dash)
                    dir = dashDirection;

                transform.Translate(dir * speed * Time.deltaTime, Space.World);

                bool isRightSide = dir.x > 0;
                if (isRightSide)
                {
                    transform.rotation = Quaternion.Euler(Vector3.zero);
                    spriteTr.rotation = Quaternion.Euler(45, 0, 0);
                }
                else
                {
                    transform.rotation = Quaternion.Euler(0, 180, 0);
                    spriteTr.rotation = Quaternion.Euler(-45, 180, 0); //부모의 로테이션이 변경되어서 로컬 y축 값도 180으로 변경해야되더라..?
                }
                if (ChangeableState())
                    State = StateType.Walk;
            }
            else
            {
                if (ChangeableState())
                    State = StateType.Idle;
            }

            bool ChangeableState()
            {
                if (jumpState == JumpStateType.Jump)
                    return false;

                if (state == StateType.Dash)
                    return false;

                return true;
            }
        }
    }
}
