using NaughtyAttributes;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Player : MonoBehaviour
{
    public static Player instance;

    private void Awake()
    {
        instance = this;
    }

    [BoxGroup("Move")] public float speed = 5;
    float normalSpeed;
    [BoxGroup("Move")] public float walkDistance = 12; //포인터와 플레이어의 위치가 3이상 차이나면 움직이게끔
    [BoxGroup("Move")] public float stopDistance = 7;
    [BoxGroup("Move")] public Transform mousePointer;
    [BoxGroup("Jump")] public AnimationCurve jumpYac;
    Plane plane = new Plane(new Vector3(0, 1, 0), 0); //두번째 인자 0은 평면을 만드는 노멀라이즈된 방향??
    //가상의 무한한 평면을 만들어서 그 위에 Ray를 쏘고 감지하기 위한 용도
    NavMeshAgent agent;
    public Transform spriteTr;
    SpriteTrailRenderer.SpriteTrailRenderer spriteTrailRenderer;
    //네임스페이스와 클래스의 이름이 같아서 네임스페이스.클래스 이렇게 선언해줬다.
    //using문으로는 네임스페이스와 클래스의 구분이 모호해서
    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        normalSpeed = speed;
        animator = GetComponentInChildren<Animator>();
        spriteTr = GetComponentInChildren<SpriteRenderer>().transform; //GetChild해도 자기자신이 우선
        spriteTrailRenderer = GetComponentInChildren<SpriteTrailRenderer.SpriteTrailRenderer>();
        spriteTrailRenderer.enabled = false;
    }

    void Update()
    {
        if (IsMoveableState())
        {
            Move();
            Jump();
        }
        bool isSucceedDash = Dash();
        Attack(isSucceedDash);
    }

    private bool IsMoveableState()
    {
        if (State == StateType.Attack)
            return false;
        if (State == StateType.TakeHit)
            return false;
        if (State == StateType.Death)
            return false;

        return true;
    }
    #region AttackAndDamage
    private void Attack(bool isSucceedDash)
    {
        if (isSucceedDash)
            return;
        //마우스 왼쪽버튼을 뗏을 때 (키업), 다운은 대쉬로 해놨으니까 대쉬할 때와 상태가 겹치지 않도록?
        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            StartCoroutine(AttackCo());
        }
    }
    public float attackTime = 1;
    public float attackApplyTime = 0.2f;
    public LayerMask enemyLayer;
    public SphereCollider attackCollider;
    public float power = 10;
    IEnumerator AttackCo()
    {
        State = StateType.Attack;
        yield return new WaitForSeconds(attackApplyTime);

        var enemyColliders = Physics.OverlapSphere(attackCollider.transform.position, attackCollider.radius, enemyLayer);
            foreach (var item in enemyColliders)
        {
            item.GetComponent<Monster>().TakeHit(power);
        } //별로 많이 호출 안하기 때문에 오버랩 스피어를 사용해도 괜찮다. 

        //Collider[] colliders = new Collider[10]; //nonAlloc으로 하면 메모리 비용은 싸지만 정해져있는 카운트만큼만 사용할 수 있어서 어렵다
        //int collideCount = Physics.OverlapCapsuleNonAlloc(attackCollider.transform.position, attackCollider.radius,)

        yield return new WaitForSeconds(attackTime);
        State = StateType.Idle;
    }
    public float hp = 100;
    internal void TakeHit(int damage)
    {
        if (State == StateType.Death)
            return;

        hp -= damage;
        StartCoroutine(TakeHitCo());
    }
    public float deathTime = 0.3f;
    IEnumerator DeathCo()
    {
        State = StateType.Death;
        yield return new WaitForSeconds(deathTime);
        Debug.LogWarning("게임 종료");
    }

    public float takeHitTime = 0.3f;
    IEnumerator TakeHitCo()
    {
        State = StateType.TakeHit;
        yield return new WaitForSeconds(takeHitTime);

        if (hp > 0)
            State = StateType.Idle;
        else
            StartCoroutine(DeathCo()); //죽을 때 피격모션 1회 플레이 후 죽도록
    }
    #endregion AttackAndDamage

    #region Dash
    [Foldout("Dash")]
    public float dashableDistance = 10;
    [Foldout("Dash")]
    public float dashableTime = 0.4f; //0.4초 안에 드래그해서 KeyUp이 발생해야 dash가 되는 시간을 의미함
    float mouseDownTime; //대쉬 드래그로 구현하기 위해서 마우스를 눌렀을 때의 시간과 포지션 담을 변수
    Vector3 mouseDownPosition;
    private bool Dash()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            mouseDownTime = Time.time; //현재시간
            mouseDownPosition = Input.mousePosition; //월드 포지션이 아닌 화면(해상도)의 포지션
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
                    return true;
                }
            }
        }
        return false;
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

    #region Jump
    private void Jump()
    {

        if (jumpState == JumpStateType.Jump)
            return;
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            StartCoroutine(JumpCo());
        }
    }
    [BoxGroup("Jump")] public float jumpYMultiply = 20;
    [BoxGroup("Jump")] public float jumpTimeMultiply = 1;

    public enum JumpStateType
    {
        Ground,
        Jump,
    }

    [BoxGroup("Jump")] public JumpStateType jumpState;
    IEnumerator JumpCo()
    {

        State = StateType.Jump;
        jumpState = JumpStateType.Jump;
        float jumpStartTime = Time.time;
        float jumpDuration = jumpYac[jumpYac.length - 1].time; //마지막키의 시간을 구하는 것
        jumpDuration *= jumpTimeMultiply;
        float jumpEndTime = jumpStartTime + jumpDuration;
        float sumEvaluateTime = 0;
        float previousY = float.MinValue;
        agent.enabled = false;
        while (Time.time < jumpEndTime)
        {
            float y = jumpYac.Evaluate(sumEvaluateTime / jumpTimeMultiply); //evaluate는 키와 관계 없이 그래프 전체에 관련된 함수
            y *= jumpYMultiply * Time.deltaTime;
            transform.Translate(0, y, 0);
            yield return null;

            if (previousY > transform.position.y)
            {
                State = StateType.Fall;
            }

            if (transform.position.y < 0)
                break;
            previousY = transform.position.y;
            sumEvaluateTime += Time.deltaTime;
        }
        agent.enabled = true;
        jumpState = JumpStateType.Ground;
        State = StateType.Idle;
    }

    #endregion Jump

    #region State
    public enum StateType
    {
        Idle,
        Walk,
        Jump,
        Fall,
        Attack,
        Dash,
        TakeHit,
        Death,
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
    #endregion State
    Animator animator;
    private void Move()
    {
        if (Time.timeScale == 0)
            return;

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

            Vector3 dir = Vector3.zero;
            dir = hitPoint - transform.position;

            if (State == StateType.Dash)
                dir = dashDirection;

            dir.Normalize();

            if (distance > moveableDistance || State == StateType.Dash ) //moveableDistance 변경해서 idle walk 변경 반복하던 것 수정할 예쩡.
            {
                transform.Translate(dir * speed * Time.deltaTime, Space.World);

                if (ChangeableState())
                    State = StateType.Walk;
            }
            else
            {
                if (ChangeableState())
                    State = StateType.Idle;
            }

            bool isRightSide = dir.x > 0;
            if (isRightSide)
            {
                transform.rotation = Quaternion.Euler(Vector3.zero);
                //spriteTr.rotation = Quaternion.Euler(45, 0, 0); 이제 부모가 회전한대로 그대로 따라가며 ㄴ돼서 필요없다!
            }
            else
            {
                transform.rotation = Quaternion.Euler(0, 180, 0);
                //spriteTr.rotation = Quaternion.Euler(-45, 180, 0); //부모의 로테이션이 변경되어서 로컬 y축 값도 180으로 변경해야되더라..?
            }

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

