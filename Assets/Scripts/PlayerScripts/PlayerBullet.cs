using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    
    [Header("General Stats")]
    public bool UsePhysicsToTranslate = false;
    public bool UsePhysicsCollisions = false;
    
    public int Damage = 0;

    public float BulletSpeed = 1.0f;
    
    
    [Header("VFX")]
    public bool generatesDamage = true;
    public GameObject DefaultImpactFX;
    private bool UseDefaultImpactFX = true;

    //Object Pooling
    [Header("Object Pooling")]
    [HideInInspector]
    public bool usePooling = true;
    public float timeToLive = 3.0f;
    public Vector3 OriginalScale;

    private bool alreadyDestroyed = false;

    private PlayerCamera playerCamera;
    

    private void OnEnable()
    {
        if (UsePhysicsToTranslate)
        {
            GetComponent<Rigidbody>().AddForce(transform.forward * BulletSpeed * 10);
        }

        if (GameObject.Find("PlayerCamera"))
            playerCamera = GameObject.Find("PlayerCamera").GetComponent<PlayerCamera>();
    }


    public void InitializePooling()
    {
        OriginalScale = this.transform.localScale;
    }
    
    public void ResetPooling()
    {
        alreadyDestroyed = false;
        UseDefaultImpactFX = true;

        if (GetComponentInChildren<TrailRenderer>())
        {
            GetComponentInChildren<TrailRenderer>().Clear();
        }
    }

    void Update()
    {
        timeToLive -= Time.deltaTime;
        if (timeToLive <= 0.0f)
        { 
            if (usePooling)
                this.gameObject.SetActive(false);
            else
                Destroy(this.gameObject);
        }
    }
    
    void DestroyBullet(Vector3 HitNormal, Vector3 HitPos, GameObject Target, string HitTag)
	{
        alreadyDestroyed = true;

        if (HitTag.Equals("Enemy"))
        {

            if (generatesDamage)
            {
                Target.GetComponent<iHealth>().ModifyHealth(Damage);
            }

            else
            {
                //if you don't want to send damage
            }
        }

        GetComponent<Rigidbody>().collisionDetectionMode = CollisionDetectionMode.Discrete;
        GetComponent<Rigidbody>().isKinematic = true;
        GetComponent<Collider>().enabled = false;
		

		if (DefaultImpactFX && UseDefaultImpactFX)
			Instantiate(DefaultImpactFX, HitPos  , Quaternion.LookRotation( HitNormal) );
        

        if (usePooling)
            //Object Pooling Mode
            this.gameObject.SetActive(false);
    }
    
	
	void OnCollisionEnter(Collision collision)
	{
        if (UsePhysicsCollisions)
        {
            if (!alreadyDestroyed)
                DestroyBullet(collision.contacts[0].normal, collision.contacts[0].point, collision.gameObject, collision.transform.tag);
        }
			
 
    }
}
