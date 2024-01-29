using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AI_State
{
    Idle, Patrol, Trace, Attack
}

public class EnemyAI : MonoBehaviour
{
    // 공통
    AI_State state = AI_State.Idle;
    AI_State nextState = AI_State.Idle;
    Animator animator;

    // Patrol 관련 필드
    Vector3 targetPos;
    [SerializeField]
    float patrolSpeed = 5f;
    [SerializeField]
    float patrolRotation = 50f;
    [SerializeField]
    GameObject prefTarget;

    // Trace 관련 필드
    [SerializeField]
    float traceSpeed = 8f;

    Vector3 dogPos;
    [SerializeField]
    GameObject prefDog;

    // 
    Camera eye;
    Plane[] planes;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        eye = transform.GetComponentInChildren<Camera>();       // 안에있응 카메라찾기
        planes = GeometryUtility.CalculateFrustumPlanes(eye);
        ChangeState(AI_State.Idle);
        //StartCoroutine(Coroutine_Idle());
        //StopCoroutine(Coroutine_Idle());
    }
    void ChangeState(AI_State nextState)
    {
        state = nextState;

        animator.SetBool("isIdle", false);
        animator.SetBool("isPatrol", false);
        animator.SetBool("isTrace", false);
        animator.SetBool("isAttack", false);

        StopAllCoroutines();

        //prefTarget.SetActive(false);


        switch (state)
        {
            case AI_State.Idle: StartCoroutine(Coroutine_Idle()); break;
            case AI_State.Patrol: StartCoroutine(Coroutine_Patrol()); break;
            case AI_State.Trace: StartCoroutine(Coroutine_Trace()); break;
            case AI_State.Attack: StartCoroutine(Coroutine_Attack()); break;
        }
    }

    #region Update
    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case AI_State.Idle: Update_Idle(); break;
            case AI_State.Patrol: Update_Patrol(); break;
            case AI_State.Trace: Update_Trace(); break;
            case AI_State.Attack: Update_Attack(); break;
        }
    }

    void Update_Idle()
    {
        // 매 프레임 실행되는 실행문

    }

    void Update_Patrol()
    {
        if (IsFindEnemy())
        {
            ChangeState(AI_State.Trace);
            return;
        }

        // 이동
        // targetPos;

        Vector3 dir = (targetPos - transform.position);
        float dist = dir.magnitude;

        if (dist <= 0.3f)
        {
            ChangeState(AI_State.Idle);
            return;
        }

        //회전                        - 뒤에가 앞을 바라보는 방향의 회전값을 구해준다.
        var targetRotation =
            Quaternion.LookRotation(targetPos - transform.position, Vector3.up);
        transform.rotation =
            Quaternion.Slerp(transform.rotation, targetRotation, patrolRotation * Time.deltaTime); // 회전관련 보간 

        // 이동
        // transform.position += dir.normalized * patrolSpeed * Time.deltaTime; 그냥이동
        transform.position += transform.forward * patrolSpeed * Time.deltaTime;        // 회전된 방향으로 이동

    }

    void Update_Trace()
    {
        dogPos = prefDog.transform.position;

        // 적 위치까지 이동
        Vector3 dir = (dogPos - transform.position);
        float dist = dir.magnitude;

        if (dist <= 1f)
        {
            ChangeState(AI_State.Attack);
            return;
        }
        else if (dist >= 5f)
        {
            ChangeState(AI_State.Idle);
            return;
        }

        //회전                        - 뒤에가 앞을 바라보는 방향의 회전값을 구해준다.
        var dogRotation =
            Quaternion.LookRotation(dogPos - transform.position, Vector3.up);
        transform.rotation =
            Quaternion.Slerp(transform.rotation, dogRotation, patrolRotation * Time.deltaTime); // 회전관련 보간 

        // 이동
        transform.position += transform.forward * patrolSpeed * Time.deltaTime;        // 회전된 방향으로 이동
    }

    void Update_Attack()
    {
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.Attack")
            && animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
        {
            ChangeState(AI_State.Idle);
            return;
        }
    }

    #endregion

    #region Coroutine

    IEnumerator Coroutine_Idle()
    {
        // 1. 상태가 바뀌고 최초에 한번만 하는 실행문
        animator.SetBool("isIdle", true);

        //nextState = (AI_State)Random.Range((int)AI_State.Idle, (int)AI_State.Patrol);
        nextState = AI_State.Patrol;


        // 2. 일정 시간(조건)마다 동작하는 실행문
        while (true)
        {
            //Debug.Log("Coroutine_Idle 1");
            //yield return new WaitForSeconds(3.0f);  // return 후 3초 뒤에 다음 코드로 들어온다 
            //
            ////3초뒤 에  Debug.Log("Coroutine_Idle 2"); < 가 실행됨
            //Debug.Log("Coroutine_Idle 2");
            //
            //yield break;

            yield return new WaitForSeconds(2.0f);
            //yield return new WaitForSeconds(5.0f);

            ChangeState(nextState);
        }


        //return null;
    }

    IEnumerator Coroutine_Patrol()
    {
        // 1.상태가 바뀌고 최초에 한번만 하는 실행문
        animator.SetBool("isPatrol", true);

        nextState = AI_State.Idle;

        // 목표지점 설정(소환)
        targetPos = transform.position
            + new Vector3(Random.Range(-3.0f, 3.0f), 0.0f, Random.Range(-3f, 3f));

        prefTarget.SetActive(true);
        prefTarget.transform.position = targetPos;

        // 2. 일정 시간(조건)마다 동작하는 실행문
        while (true)
        {
            yield return new WaitForSeconds(5.0f);

            ChangeState(nextState);
        }

        //return null;
    }

    IEnumerator Coroutine_Trace()
    {
        // 1. 상태가 바뀌고 최초에 한번만 하는 실행문
        animator.SetBool("isTrace", true);
        nextState = AI_State.Attack;

        dogPos = prefDog.transform.position;

        // 2. 일정 시간(조건)마다 동작하는 실행문
        while (true)
        {
            yield return new WaitForSeconds(5.0f);

            //ChangeState(nextState);
        }

        //return null;
    }

    IEnumerator Coroutine_Attack()
    {
        // 1. 상태가 바뀌고 최초에 한번만 하는 실행문
        animator.SetBool("isAttack", true);

        //nextState = (AI_State)Random.Range((int)AI_State.Idle, (int)AI_State.Patrol);
        nextState = AI_State.Idle;


        // 2. 일정 시간(조건)마다 동작하는 실행문
        while (true)
        {
            yield return new WaitForSeconds(2.0f);

            ChangeState(nextState);
        }


        //return null;
    }

    #endregion

    bool IsFindEnemy()
    {
        bool isFind = false;
        planes = GeometryUtility.CalculateFrustumPlanes(eye);
        Bounds bounds = prefDog.GetComponentInChildren<Collider>().bounds;
        isFind = GeometryUtility.TestPlanesAABB(planes, bounds);     //플렌이 배열이랑 

        return isFind;
    }
}
