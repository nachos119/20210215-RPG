using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private PlayerMove playerMove;
    //[SerializeField]
    private Camera mainCamera;
    private Vector3 targetPos;
    public bool col;
    public bool monster;
    public int HP;
    float EX;
    int gold;
    int offense;
    int LV;
    int defense;
    private Vector3 monsterPos;

    // Start is called before the first frame update
    void Start()
    {
        playerMove = GetComponent<PlayerMove>();
        mainCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
        targetPos = transform.position;
        playerMove.ctrlStart();
        col = false;
        monster = false;
        HP = 100;
        EX = 0;
        gold = 0;
        offense = 10;
        LV = 1;
        defense = 5;
    }



    // Update is called once per frame
    void Update()
    {
        // 마우스 입력을 받았 을 때
        if (Input.GetMouseButton(0))
        {
            // 마우스로 찍은 위치의 좌표 값을 가져온다
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 10000f, 5))
            {
                Debug.Log(hit.transform.tag);
                // 몬스터인지 검사
                if (hit.transform.tag.Equals("Enemy"))
                {
                    //hit.transform.gameObject.
                    // 거리 검사
                    targetPos = hit.transform.position;
                    //float dis = Vector3.Distance(transform.position, targetPos);
                    //Debug.Log(dis);

                    monster = true;

                    if (col) // 몬스터의 크기 넣기
                    {
                        // 공격 실행
                        Debug.Log("공격실행");
                        targetPos = transform.position;
                        playerMove.ChangeState(Player_State.Attack, PlayerAni.ANI_ATTACK);
                    }
                    else
                    {
                        col = false;
                        playerMove.ChangeState(Player_State.Walk, PlayerAni.ANI_WALK);
                    }
                }
                else
                {
                    targetPos = hit.point;
                    monster = false;
                    col = false;
                    playerMove.ChangeState(Player_State.Walk, PlayerAni.ANI_WALK);
                }
                //Debug.Log(targetPos);
            }
        }

        // 공격
        if (!col)
        {
            // 캐릭터가 움직이고 있다면
            if (playerMove.Run(targetPos))
            {
                // 회전도 같이 시켜준다
                // playerMove.Turn(targetPos);
            }
            else
            {
                // 캐릭터 애니메이션(정지 상태)
                //GetComponent<Animator>().SetFloat("Blend", 0);
                Debug.Log("애니메이션 : 정지");
                playerMove.ChangeState(Player_State.Idle, PlayerAni.ANI_IDLE);
            }
        }
        else if(col)
        {
            if (!monster)
                monster = true;
            // 공격 실행
            if (playerMove.state != Player_State.Attack)
            {
                targetPos = transform.position;
                playerMove.ChangeState(Player_State.Attack, PlayerAni.ANI_ATTACK);
            }

            playerMove.Attack(targetPos);
        }

        playerMove.UpdateState();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "Enemy")
        {
            col = true;
            // 여기에 적의 위치를 넣어주고 
            monsterPos = other.transform.position;
            transform.LookAt(monsterPos);
        }
    }
    void OnTriggerStay(Collider other)
    {

    }
    void OnTriggerExit(Collider other)
    {
        //if (other.transform.tag == "Enemy")
        //{
        //    col = false;
        //}
    }
}
