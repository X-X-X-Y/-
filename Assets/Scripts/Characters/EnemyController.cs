using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyStates { GUARD,PATROL,CHASE,DEAD }

[RequireComponent(typeof(NavMeshAgent))]                    //Լ��-��ҪXXX���-�Զ������Ӧ���
[RequireComponent (typeof(CharacterStats))]
public class EnemyController : MonoBehaviour ,IEndGmaeObserver
{
    private EnemyStates enemyStates;

    private NavMeshAgent agent;

    private Animator anim;
    private Collider coll;

    protected CharacterStats characterStats;

    [Header("Basic Settings")]              //���ִ������

    public float sightRadius;               //���ӷ�Χ

    public bool isGuard;                    //��ѡ�Ļ�������Ϊվ׮ģʽ

    private float speed;

    protected GameObject attackTarget;

    public float LookAtTime;
    private float remainLookAtTime;

    private float LastAttackTime;

    private Quaternion guardRotation;       //��¼��ת�Ƕ�

    [Header("Patrol State")]
    public float patrolRange;

    private Vector3 wayPoint;
    private Vector3 guardPos;


    //Bool��϶���
    bool isWalk;
    bool isChase;
    bool isFollow;
    bool isDead;
    bool playDead;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        characterStats = GetComponent<CharacterStats>();
        coll = GetComponent<Collider>();

        speed = agent.speed;                                        
        guardPos = transform.position;
        guardRotation = transform.rotation;
        remainLookAtTime = LookAtTime;

    }

    private void Start()
    {
        if (isGuard)
        {
            enemyStates = EnemyStates.GUARD;
        }
        else 
        {
            enemyStates = EnemyStates.PATROL;   //��ʼ��Ҫ����һ��Ŀ��λ��
            GetNewWayPoint();
        }
        //FIXME:����������Ҫ�޸�
        GameManager.Instance.AddObsrever(this);
    }
    //�л���������
    //void OnEnable()
    //{
    //    GameManager.Instance.AddObsrever(this);
    //}

    void OnDisable()
    {
        //���GameManagerû�б�����-����༭������
        if (!GameManager.IsInitialized) return;
        GameManager.Instance.RemoveObserver(this);


        if (GetComponent<LootSpawner>() && isDead)  //����������ʱ��
        {
            GetComponent<LootSpawner>().SpawnLoot();
        }

        if (QuestManager.IsInitialized && isDead)   //QuestManager�Ѿ�ʵ����
        {
            QuestManager.Instance.UpdateQuestProgress(this.name,1);
        }
    }

    private void Update()
    {
        if (characterStats.CurrentHealth == 0)
            isDead = true;

        if (!playDead)
        {
            SwitchStates();
            SwitchAnimation();
            LastAttackTime -= Time.deltaTime;
        }
        
    }

    void SwitchAnimation()
    {   //����������Boolֵ
        anim.SetBool("Walk",isWalk);
        anim.SetBool("Chase",isChase);
        anim.SetBool("Follow", isFollow);
        anim.SetBool("Critical",characterStats.isCritical);
        anim.SetBool("Death",isDead);
    }

    void SwitchStates()
    {
        if (isDead)
            enemyStates = EnemyStates.DEAD;

        //���������ң��л���CHASE
        else if (FoundPlayer())
        {
            enemyStates = EnemyStates.CHASE;
            //Debug.Log("�ҵ����");
        }

        switch (enemyStates)
        {
            case EnemyStates.GUARD:
                isChase = false;

                if (transform.position != guardPos)
                {
                    isWalk = true;
                    agent.isStopped = false;
                    agent.destination = guardPos;
                    //SqrMagnitude-������ά����Ŀ���ֵ-���ܿ�������
                    if (Vector3.SqrMagnitude(guardPos - transform.position) <= agent.stoppingDistance)
                    {
                        isWalk = false;
                        transform.rotation = Quaternion.Lerp(transform.rotation, guardRotation, 0.01f);     //ƽ��ת����Ӧ�Ƕ�
                    }
                       
                }
                break;


            case EnemyStates.PATROL:
                isChase = false;
                agent.speed = speed * 0.5f;             //�˷�ϵͳ������С

                //�ж��Ƿ������Ѳ�ߵ�-stoppingDistance��NavĬ�ϵ�ֵ
                if (Vector3.Distance(wayPoint, transform.position) <= agent.stoppingDistance)
                {
                    isWalk = false;
                    if (remainLookAtTime > 0)   //Ѳ�ߵ�һ����ͣ��һ���
                        remainLookAtTime -= Time.deltaTime;
                    GetNewWayPoint();
                }
                else {
                    isWalk = true;
                    agent.destination = wayPoint;
                }

                break;


            case EnemyStates.CHASE:
                isWalk = false;
                isChase = true;

                agent.speed = speed;        //����״̬ʱ���ٶ�
                if (!FoundPlayer())
                {
                    //��ս�󷵻���һ��״̬
                    isFollow = false;
                    if (remainLookAtTime > 0)
                    {
                        agent.destination = transform.position;
                        remainLookAtTime -= Time.deltaTime;
                    }
                    else if (isGuard)
                        enemyStates = EnemyStates.GUARD;
                    else
                        enemyStates = EnemyStates.PATROL;
                }
                else
                {//׷�����
                    isFollow = true;
                    agent.isStopped = false;
                    agent.destination = attackTarget.transform.position;
                }
                //�ڹ�����Χ�ڹ���
                if (TargetInAttackRange() || TargetInSkillRange())
                {
                    isFollow = false;
                    agent.isStopped = true;

                    if (LastAttackTime < 0)
                    {
                        LastAttackTime = characterStats.attackData.coolDown;

                        //�����ж�-�������ֵ�Ƿ�С�ڱ���
                        characterStats.isCritical = Random.value < characterStats.attackData.criticalChance;
                        //ִ�й���
                        Attack();
                    }

                }
                break;


            case EnemyStates.DEAD:
                coll.enabled = false;
                //agent.enabled = false;  -��ֹ������ס
                agent.radius = 0;
                Destroy(gameObject, 2f);
                break;
        }
    }

    void Attack()
    {
        transform.LookAt(attackTarget.transform);
        if (TargetInAttackRange()) 
        {
            //ִ�н���������
            anim.SetTrigger("Attack");
        }
        if (TargetInSkillRange())
        {
            //ִ�м��ܹ�������
            anim.SetTrigger("Skill");
        }
    }

    bool FoundPlayer()
    {
        var colliders = Physics.OverlapSphere(transform.position, sightRadius);     //�ҵ�һ����ײ��

        foreach (var target in colliders)                                           //����������ײ��
        {
            if (target.CompareTag("Player")) 
            {
                attackTarget = target.gameObject;
                return true;
            }
        }
        attackTarget = null;
        return false;
    }

    bool TargetInAttackRange()  //ƽA����-
    {
        if (attackTarget != null)
            return Vector3.Distance(attackTarget.transform.position, transform.position) <= characterStats.attackData.attackRange;
        else
            return false;
    }

    bool TargetInSkillRange()   //���ܷ�Χ
    {
        if (attackTarget != null)
            return Vector3.Distance(attackTarget.transform.position, transform.position) <= characterStats.attackData.skillRange;
        else
            return false;
    }


    void GetNewWayPoint()
    {
        remainLookAtTime = LookAtTime;

        float randomX = Random.Range(-patrolRange, patrolRange);
        float randomZ = Random.Range(-patrolRange, patrolRange);
        //���ܳ��ֵ�����-Ѳ�߷�Χ��Ҫ�Գ�ʼ����λ��
        Vector3 randomPoint = new Vector3(guardPos.x + randomX, transform.position.y, guardPos.z+ randomZ);

        //SamplePosition-��ֹѲ�ߵ����޷��ƶ����򣬵���AI��ס-����һ��boolֵ--����1����Nav�е�Areas
        NavMeshHit hit;
        wayPoint = NavMesh.SamplePosition(randomPoint, out hit, patrolRange, 1) ? hit.position : transform.position;        //����Ƿ���Hitλ�ã����򷵻��Լ��ĵ�
    }

    //����ָʾ��-ѡ��ʱ����ָʾ��
    private void OnDrawGizmosSelected()             
    {
        Gizmos.color = Color.blue;
        //���߷�Χ-��ʵ��
        Gizmos.DrawWireSphere(transform.position, sightRadius);
    }

    //Animation Event
    void Hit()
    {
        //�����жϹ���ʱĿ���Ƿ񻹴���-��չ����
        if (attackTarget != null && transform .IsFacingTarget(attackTarget.transform))
        {
            var targetStats = attackTarget.GetComponent<CharacterStats>();
            targetStats.TakeDamage(characterStats,targetStats);
        }
    }

    public void EndNotify()
    {
        //���˻�ʤ-����������������
        // ֹͣ�����ƶ�agent

        anim.SetBool("Win",true);
        playDead = true;
        isChase = false;
        isWalk = false;
        attackTarget = null;
    }
}
