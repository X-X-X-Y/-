using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayController : MonoBehaviour
{
    private NavMeshAgent agent;

    private Animator anim;

    private CharacterStats characterStats;

    private GameObject attackTarget;
    private float LastAttackTime;               //攻击间隔CD

    private bool isDead;

    private float stopDistance;
    private void Awake()        //自身变量的获取放在Awake中
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        characterStats = GetComponent<CharacterStats>();

        stopDistance = agent.stoppingDistance;
    }

    private void OnEnable()
    {
        MouseManager.Instance.OnMouseClicked += MoveToTarget;                   //订阅MouseManager中的事件
        MouseManager.Instance.OnEnmyClicked += EventAttack;
        GameManager.Instance.RigisterPlayer(characterStats);
    }
    private void Start()
    {       
        SaveManager.Instance.LoadPlayerData();
    }

    private void OnDisable()
    {
        if (!MouseManager.IsInitialized) return;
        MouseManager.Instance.OnMouseClicked -= MoveToTarget;               
        MouseManager.Instance.OnEnmyClicked -= EventAttack;
    }


    private void Update()
    {
        isDead = characterStats.CurrentHealth == 0;

        if (isDead)
            GameManager.Instance.NotifyObserver();

        SwitchAnimation();

        LastAttackTime -= Time.deltaTime;
    }

    private void SwitchAnimation()
    {
        anim.SetFloat("Speed", agent.velocity.sqrMagnitude);            //sqrMagnitude转换浮点方法
        anim.SetBool("Death",isDead);
    }

    
    public void MoveToTarget(Vector3 target)                          //订阅事件后必须包含该事件的数据类型
    {
        StopAllCoroutines();                                        //停止所有协程，可以随时通过移动取消协程
        if (isDead) return;                                         //判断人物当前是否死亡，否则人物死亡后也可以移动

        agent.stoppingDistance = stopDistance;
        agent.isStopped = false;
        agent.stoppingDistance = characterStats.attackData.attackRange;

        agent.destination = target;
    }
    private void EventAttack(GameObject target)                     //ctrl+r 更改所有相关函数名
    {
        if (isDead) return;

        if (target != null)
        {
            attackTarget = target;
            characterStats.isCritical = UnityEngine.Random.value < characterStats.attackData.criticalChance;
            StartCoroutine(MoveToAttackTarget());
        }
    }

    IEnumerator MoveToAttackTarget()
    {
        agent.isStopped = false;
        agent.stoppingDistance = characterStats.attackData.attackRange;

        transform.LookAt(attackTarget.transform);

        //判断不同武器间的攻击距离、如果距离不够则继续移动
        while (Vector3.Distance(attackTarget.transform.position, transform.position) > characterStats.attackData.attackRange)
        {
            agent.destination = attackTarget.transform.position;
            yield return null;
        }
        agent.isStopped = true;

        //Attack
        if (LastAttackTime < 0)
        {
            anim.SetBool("Critical",characterStats.isCritical);
            anim.SetTrigger("Attack");
            //重置冷却时间
            LastAttackTime = characterStats.attackData.coolDown; 
        }

    }


    //Animation Event
    void Hit()
    {
        if (attackTarget.CompareTag("Attackable"))
        {
            if (attackTarget.GetComponent<Rock>() && attackTarget.GetComponent<Rock>().rockStats == Rock.RockStats.HitNothing)

                attackTarget.GetComponent<Rock>().rockStats = Rock.RockStats.HitEnemy;

            attackTarget.GetComponent<Rigidbody>().velocity = Vector3.one;

            attackTarget.GetComponent<Rigidbody>().AddForce(transform.forward * 20, ForceMode.Impulse);

        }
        else {
            //点击攻击时目标
            var targetStats = attackTarget.GetComponent<CharacterStats>();

            targetStats.TakeDamage(characterStats, targetStats);
        }
        
    }
        
}
