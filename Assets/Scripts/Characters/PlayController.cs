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
    private float LastAttackTime;               //�������CD

    private bool isDead;

    private float stopDistance;
    private void Awake()        //��������Ļ�ȡ����Awake��
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        characterStats = GetComponent<CharacterStats>();

        stopDistance = agent.stoppingDistance;
    }

    private void OnEnable()
    {
        MouseManager.Instance.OnMouseClicked += MoveToTarget;                   //����MouseManager�е��¼�
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
        anim.SetFloat("Speed", agent.velocity.sqrMagnitude);            //sqrMagnitudeת�����㷽��
        anim.SetBool("Death",isDead);
    }

    
    public void MoveToTarget(Vector3 target)                          //�����¼������������¼�����������
    {
        StopAllCoroutines();                                        //ֹͣ����Э�̣�������ʱͨ���ƶ�ȡ��Э��
        if (isDead) return;                                         //�ж����ﵱǰ�Ƿ���������������������Ҳ�����ƶ�

        agent.stoppingDistance = stopDistance;
        agent.isStopped = false;
        agent.stoppingDistance = characterStats.attackData.attackRange;

        agent.destination = target;
    }
    private void EventAttack(GameObject target)                     //ctrl+r ����������غ�����
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

        //�жϲ�ͬ������Ĺ������롢������벻��������ƶ�
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
            //������ȴʱ��
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
            //�������ʱĿ��
            var targetStats = attackTarget.GetComponent<CharacterStats>();

            targetStats.TakeDamage(characterStats, targetStats);
        }
        
    }
        
}
