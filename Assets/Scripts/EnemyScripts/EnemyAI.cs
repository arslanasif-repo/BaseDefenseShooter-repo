
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class EnemyAI : DeathEvent, iHealth
{
    [Header("AI")]
    public CurrentState m_State;
    private Transform targetTransform;
    [SerializeField] private float updateTime;
    [SerializeField] private float detectionRadius;
    
    [Header("Stats")]
    public int enemyHealth = 100;
    public GameObject HealthBar;
    
    private NavMeshAgent m_Agent;
    private Animator m_Animator;
    private SphereCollider m_Collider;
    

    [Header("Death Bonus")]
    [SerializeField] private GameObject coinPrefab;

    private void Start()
    {
        base.Start();
        m_Agent = GetComponent<NavMeshAgent>();
        m_Animator = GetComponent<Animator>();
        m_Collider = GetComponent<SphereCollider>();
        
        if (detectionRadius > 0)
        {
            m_Collider.radius = detectionRadius;
        }
        
        Health = enemyHealth;
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (m_State != CurrentState.Dead)
            {
                targetTransform = other.transform;
                StartAttack();
            }
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (m_State != CurrentState.Dead)
            {
                if (!other.gameObject.GetComponent<PlayerController>().m_isDead)
                {
                    other.gameObject.GetComponent<iHealth>().ModifyHealth(1);
                    StopAttack();
                }
            }
        }
    }

    void StartAttack()
    {
        m_State = CurrentState.Attacking;
        m_Agent.SetDestination(targetTransform.position);
        m_Animator.SetBool("Attack", true);
        StartCoroutine(UpdateTarget());
    }

    void StopAttack()
    {
        m_State = CurrentState.Idle;
        m_Agent.isStopped = true;
        m_Animator.SetBool("Attack", false);
    }

    IEnumerator UpdateTarget()
    {
        while (!targetTransform.GetComponent<PlayerController>().m_isDead && m_State != CurrentState.Dead)
        {
            yield return new WaitForSeconds(updateTime);
            if( m_State != CurrentState.Dead)
                m_Agent.SetDestination(targetTransform.position);
        }
       
    }
    
    void Die()
    {
        m_State = CurrentState.Dead;
        m_Agent.enabled = false;
        m_Animator.enabled = false;
        
        base.OnDeath();
    }

    protected override void DeathDelayTasks(float initialDelay, float finalDelay)
    {
        m_Collider.enabled = false;
        StartCoroutine(Disappear(initialDelay, finalDelay));
        if(GameManager.instance.CheckCompletion())
            GameManager.instance.MissionComplete();
    }

    IEnumerator Disappear(float initialDelay ,float otherDelay)
    {
        yield return new WaitForSeconds(initialDelay);
        Instantiate(coinPrefab, new Vector3(targetBone.transform.position.x, 1, targetBone.transform.position.z), transform.rotation);
        yield return new WaitForSeconds(otherDelay);
        this.gameObject.SetActive(false);
    }



    public float Health { get; set; }
    public int HealthMax { get; }
    public void ModifyHealth(int amount)
    {
        if (Health > 0)
        {
            //Here you can put some Damage Behaviour if you want
            Health -= amount;

            float healthPercentage = Health / enemyHealth;
            
            HealthBar.GetComponent<Image>().fillAmount = healthPercentage;
                
            if (Health <= 0)
            {
                Die();
            }
        }
    }
}


public enum CurrentState
{
    Idle,
    Attacking,
    Dead
}
