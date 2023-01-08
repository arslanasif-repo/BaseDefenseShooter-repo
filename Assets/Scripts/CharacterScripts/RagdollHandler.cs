
using UnityEngine;

public class RagdollHandler : MonoBehaviour
{
    
    [Header("Ragdoll handling")]
    public GameObject[] ragdollBones;
    private bool active;
    void Start()
    {
        InitializeRagdoll();
    }
    
    public void InitializeRagdoll()
    {
        Rigidbody[] temp = transform.GetComponentsInChildren<Rigidbody>();
        ragdollBones = new GameObject[temp.Length];
        int check = 0;
        foreach (Rigidbody bone in temp)
        {
            if (bone.gameObject.name != gameObject.name)
            {
                bone.gameObject.layer = LayerMask.NameToLayer("Ragdoll");
                ragdollBones.SetValue(bone.gameObject, check);
                check += 1;
            }
        }
    }
    
    public void ActivateRagdoll()
    {
        active = true;
        if (GetComponent<Animator>())
            GetComponent<Animator>().enabled = false;
        foreach (GameObject bone in ragdollBones)
        {
            if (bone != null)
            {
                bone.GetComponent<Collider>().enabled = true;
                bone.GetComponent<Rigidbody>().isKinematic = false;
                
            }
        }
    }
    
    public void ThrowRagdoll(float force, Transform target)
    {
        if (!active)
            ActivateRagdoll();
        
        GameObject targetBone = target.gameObject;
        targetBone.GetComponent<Rigidbody>().AddForce(-targetBone.transform.up * force, ForceMode.Impulse);
    }
}
