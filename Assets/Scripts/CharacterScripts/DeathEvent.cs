using UnityEngine;

public abstract class DeathEvent : MonoBehaviour
{
    
    private CapsuleCollider mainCollider;
    public bool useRagdollDeath = true;
    public float ragdollForceFactor = 50f;
    public GameObject targetBone;


    protected void Start()
    {
        mainCollider = GetComponent<CapsuleCollider>();
        gameObject.AddComponent<RagdollHandler>();
    }

    protected void OnDeath()
    {
        if (useRagdollDeath)
        {
            mainCollider.enabled = false;
            GetComponent<RagdollHandler>().ThrowRagdoll( ragdollForceFactor, targetBone.transform);
        }
        else
        {
            //Death animation
        }
        DeathDelayTasks(0.5f, 1.5f);
    }

    protected abstract void DeathDelayTasks(float initialDelay, float finalDelay);
}