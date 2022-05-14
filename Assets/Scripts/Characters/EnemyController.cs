using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyStates { GUARD,PATROL,CHASE,DEAD }

[RequireComponent(typeof(NavMeshAgent))]                    //约束-需要XXX组件-自动添加相应组件
[RequireComponent (typeof(CharacterStats))]
public class EnemyController : MonoBehaviour ,IEndGmaeObserver
{
    private EnemyStates enemyStates;

    private NavMeshAgent agent;

    private Animator anim;
    private Collider coll;

    protected CharacterStats characterStats;

    [Header("Basic Settings")]              //区分代码分区

    public float sightRadius;               //可视范围

    public bool isGuard;                    //勾选的话，敌人为站桩模式

    private float speed;

    protected GameObject attackTarget;

    public float LookAtTime;
    private float remainLookAtTime;

    private float LastAttackTime;

    private Quaternion guardRotation;       //记录旋转角度

    [Header("Patrol State")]
    public float patrolRange;

    private Vector3 wayPoint;
    private Vector3 guardPos;


    //Bool配合动画
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
            enemyStates = EnemyStates.PATROL;   //初始需要给定一个目标位置
            GetNewWayPoint();
        }
        //FIXME:场景加载需要修改
        GameManager.Instance.AddObsrever(this);
    }
    //切换场景启用
    //void OnEnable()
    //{
    //    GameManager.Instance.AddObsrever(this);
    //}

    void OnDisable()
    {
        //如果GameManager没有被生成-避免编辑器报错
        if (!GameManager.IsInitialized) return;
        GameManager.Instance.RemoveObserver(this);


        if (GetComponent<LootSpawner>() && isDead)  //怪物死亡的时候
        {
            GetComponent<LootSpawner>().SpawnLoot();
        }

        if (QuestManager.IsInitialized && isDead)   //QuestManager已经实例化
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
    {   //关联动画与Bool值
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

        //如果发现玩家，切换到CHASE
        else if (FoundPlayer())
        {
            enemyStates = EnemyStates.CHASE;
            //Debug.Log("找到玩家");
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
                    //SqrMagnitude-计算三维向量目标差值-性能开销较少
                    if (Vector3.SqrMagnitude(guardPos - transform.position) <= agent.stoppingDistance)
                    {
                        isWalk = false;
                        transform.rotation = Quaternion.Lerp(transform.rotation, guardRotation, 0.01f);     //平滑转到对应角度
                    }
                       
                }
                break;


            case EnemyStates.PATROL:
                isChase = false;
                agent.speed = speed * 0.5f;             //乘法系统开销较小

                //判断是否到了随机巡逻点-stoppingDistance是Nav默认的值
                if (Vector3.Distance(wayPoint, transform.position) <= agent.stoppingDistance)
                {
                    isWalk = false;
                    if (remainLookAtTime > 0)   //巡逻到一个点停顿一会儿
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

                agent.speed = speed;        //正常状态时的速度
                if (!FoundPlayer())
                {
                    //脱战后返回上一个状态
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
                {//追击玩家
                    isFollow = true;
                    agent.isStopped = false;
                    agent.destination = attackTarget.transform.position;
                }
                //在攻击范围内攻击
                if (TargetInAttackRange() || TargetInSkillRange())
                {
                    isFollow = false;
                    agent.isStopped = true;

                    if (LastAttackTime < 0)
                    {
                        LastAttackTime = characterStats.attackData.coolDown;

                        //暴击判断-随机的数值是否小于爆率
                        characterStats.isCritical = Random.value < characterStats.attackData.criticalChance;
                        //执行攻击
                        Attack();
                    }

                }
                break;


            case EnemyStates.DEAD:
                coll.enabled = false;
                //agent.enabled = false;  -防止动画卡住
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
            //执行近身攻击动画
            anim.SetTrigger("Attack");
        }
        if (TargetInSkillRange())
        {
            //执行技能攻击动画
            anim.SetTrigger("Skill");
        }
    }

    bool FoundPlayer()
    {
        var colliders = Physics.OverlapSphere(transform.position, sightRadius);     //找到一组碰撞体

        foreach (var target in colliders)                                           //遍历所有碰撞体
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

    bool TargetInAttackRange()  //平A距离-
    {
        if (attackTarget != null)
            return Vector3.Distance(attackTarget.transform.position, transform.position) <= characterStats.attackData.attackRange;
        else
            return false;
    }

    bool TargetInSkillRange()   //技能范围
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
        //可能出现的问题-巡逻范围需要以初始坐标位置
        Vector3 randomPoint = new Vector3(guardPos.x + randomX, transform.position.y, guardPos.z+ randomZ);

        //SamplePosition-防止巡逻点在无法移动区域，导致AI卡住-返回一个bool值--下面1代表Nav中的Areas
        NavMeshHit hit;
        wayPoint = NavMesh.SamplePosition(randomPoint, out hit, patrolRange, 1) ? hit.position : transform.position;        //如果是返回Hit位置，否则返回自己的点
    }

    //画出指示线-选中时画出指示线
    private void OnDrawGizmosSelected()             
    {
        Gizmos.color = Color.blue;
        //射线范围-非实心
        Gizmos.DrawWireSphere(transform.position, sightRadius);
    }

    //Animation Event
    void Hit()
    {
        //敌人判断攻击时目标是否还存在-扩展方法
        if (attackTarget != null && transform .IsFacingTarget(attackTarget.transform))
        {
            var targetStats = attackTarget.GetComponent<CharacterStats>();
            targetStats.TakeDamage(characterStats,targetStats);
        }
    }

    public void EndNotify()
    {
        //敌人获胜-动画………………
        // 停止所有移动agent

        anim.SetBool("Win",true);
        playDead = true;
        isChase = false;
        isWalk = false;
        attackTarget = null;
    }
}
