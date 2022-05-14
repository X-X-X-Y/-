using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Rock : MonoBehaviour
{
    public enum RockStats { HitPlayer,HitEnemy,HitNothing }

    private Rigidbody rb;

    public  RockStats rockStats;

    [Header("Basic Settings")]

    public float force;
    public int damage;
    public GameObject target;
    private Vector3 direction;
    public GameObject breakEffect;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.velocity = Vector3.one;              //石头刚出现时速度为1，防止特殊情况下状态直接变成HitNothing

        rockStats = RockStats.HitPlayer;
        FlyToTarget();

    }

    private void FixedUpdate()
    {
        if (rb.velocity.sqrMagnitude < 1f)      //返回向量长度的平方长度，算术运算较长、但是计算速度比magnitude
        {
            rockStats = RockStats.HitNothing;
        }


    }

    public void FlyToTarget()
    {
        if (target == null)
            target = FindObjectOfType<PlayController>().gameObject;

        direction = (target.transform.position - transform.position + Vector3.up).normalized;
        rb.AddForce(direction * force, ForceMode.Impulse);          //冲击力
    }

    private void OnCollisionEnter(Collision other)
    {
        switch (rockStats)
        {
            case RockStats.HitPlayer:
                if (other.gameObject.CompareTag("Player"))
                {
                    other.gameObject.GetComponent<NavMeshAgent>().isStopped = true;
                    other.gameObject.GetComponent<NavMeshAgent>().velocity = direction * force;

                    other.gameObject.GetComponent<Animator>().SetTrigger("Dizzy");
                    other.gameObject.GetComponent<CharacterStats>().TakeDamage(damage, other.gameObject.GetComponent<CharacterStats>());

                    rockStats = RockStats.HitNothing;
                }

                break;
            case RockStats.HitEnemy:
                if (other.gameObject.GetComponent<Golem>())
                {
                    var otherStats = other.gameObject.GetComponent<CharacterStats>();       //Var 变量设置
                    otherStats.TakeDamage(damage, otherStats);

                    Instantiate(breakEffect,transform .position ,Quaternion.identity);
                    Destroy(gameObject);
                }

                break;
        }
    }

}
