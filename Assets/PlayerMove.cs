using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum Player_State
{
    Idle, Walk, Attack, Damage, Die
}

public class PlayerMove : MonoBehaviour
{
    public Player_State state;
    Animator animator;
    float play;

    PlayerAni myAni;
    public Player_State currentState = Player_State.Idle;

    public float speed = 2.0f;
    private Rigidbody rigid;

    // 네비
    private NavMeshAgent pathFinder;

    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        play = 0f;
        pathFinder = GetComponent<NavMeshAgent>();

        pathFinder.speed = speed;
    }

    public bool Run(Vector3 targetPos)
    {
        // 이동하고자하는 좌표 값과 현재 내 위치의 차이를 구한다.
        // y 제거
        Vector3 playerPos = new Vector3(transform.position.x, 0.0f, transform.position.z);
        Vector3 targetPos2 = new Vector3(targetPos.x, 0.0f, targetPos.z);

        float dis = Vector3.Distance(playerPos, targetPos2);
        if (dis >= 0.1f) // 차이가 아직 있다면
        {
            // 네비
            pathFinder.isStopped = false;
            // 캐릭터를 이동시킨다.
            pathFinder.SetDestination(targetPos2);

            // transform.position = Vector3.MoveTowards(transform.position, targetPos2, speed * Time.deltaTime);
            if (play < 1f)
                play += 0.005f;
            if (speed < 4f)
            {
                speed += 0.1f;
                pathFinder.speed = speed;
            }

            animator.SetFloat("Blend", play);
            //gameObject.Getcomponent<Animator>().SetFloat("파라미터명", 파라미터값)

            //Debug.Log(targetPos);
            //SetAnim(PlayerAnim.ANIM_WALK); // 걷기 애니메이션 처리
            return true;
        }
        //Debug.Log("도착");
        //Debug.Log(transform.position);
        pathFinder.isStopped = true;
        if (speed != 2f)
        {
            speed = 2f;
            pathFinder.speed = speed;
        }
        return false;

    }

    public void Turn(Vector3 targetPos)
    {
        //pathFinder.
        // 캐릭터를 이동하고자 하는 좌표값 방향으로 회전시킨다
        Vector3 dir = targetPos - transform.position;
        Vector3 dirXZ = new Vector3(dir.x, 0f, dir.z);
        // 목표방향을 바라보는 값을 반환
        Quaternion targetRot = Quaternion.LookRotation(dirXZ);
        // 회전할 목표방향 : 목쵸방향은 목적지 위치에서 자신의 위치를 빼면 구함
        // Quaternion targetRot = Quaternion.LookRotation(targetPos - transform.position);
        // 보간회전
        // 현재의 로테값 목표 로테값 최대 회전각
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, 360.0f * Time.deltaTime);
        // transform.LookAt(targetPos);
    }

    public void Attack(Vector3 targetPos)
    {
        if (!pathFinder.isStopped)
            pathFinder.isStopped = true;

        //animator.SetBool("isAttack", true);
        //if ()

        //return false;
    }

    public void ChangeState(Player_State newState, int aniNumber)
    {
        if (currentState == newState)
        {
            return;
        }

        animator.SetBool("isIdle", false);
        animator.SetBool("isWalk", false);
        animator.SetBool("isAttack", false);
        animator.SetBool("isDie", false);
        animator.SetBool("isDamage", false);
        animator.SetBool("isH", false);
        animator.SetBool("isB", false);
        animator.SetBool("thatLive", false);
        animator.SetBool("isAlive", false);

        play = 0;
        animator.SetFloat("Blend", play);

        state = newState;
        myAni.ChangeAni(aniNumber);
        currentState = newState;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ctrlStart()
    {
        // idle 기본지정
        state = Player_State.Idle;
        animator = GetComponent<Animator>();
        myAni = GetComponent<PlayerAni>();
        // ChangeState(Player_State.Idle, PlayerAni.ANI_WALK);
        ChangeState(Player_State.Idle, PlayerAni.ANI_IDLE);
    }

    public void UpdateState()
    {
        switch (currentState)
        {
            case Player_State.Idle:
                animator.SetBool("isIdle", true);
                break;
            case Player_State.Walk:
                animator.SetBool("isWalk", true);
                break;
            case Player_State.Attack:
                animator.SetBool("isAttack", true);
                // animator.SetBool("thatLive", false);
                break;
            case Player_State.Damage:
                animator.SetBool("isDamage", true);
                // animator.SetBool("isH", false);
                // animator.SetBool("isB", false);
                break;
            case Player_State.Die:
                animator.SetBool("isDie", true);
                // animator.SetBool("isAlive", false);
                break;

        }
    }
}
