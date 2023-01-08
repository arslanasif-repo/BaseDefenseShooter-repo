
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : DeathEvent, iHealth
{

    [Header("Player Movement")]
    [SerializeField] float m_MoveSpeedMultiplier = 1f;
    [HideInInspector]
    public float m_MoveSpeedSpecialModifier = 1f;
    [SerializeField] float m_AnimSpeedMultiplier = 1f;
    

    public float playerRunSpeed = 1f;
    public float playerAimSpeed = 1f;
    
    
    public float runRotationSpeed = 100f;
    public float aimingRotationSpeed = 25f;

    public float animatorRunDampValue = 0.2f;
    public float animatorAimingDampValue = 0.1f;

    Rigidbody m_Rigidbody;
    Animator charAnimator;
    
    float m_TurnAmount;
    float m_ForwardAmount;
    
    [HideInInspector] public bool m_isDead;
    private bool b_CanRotate = true;
    
    
    [Header("Aiming")]
    private GameObject playerLookRotation;
    [HideInInspector] public Vector3 aimRotation;
    public PlayerCamera cameraHandler;

    private Transform m_Cam;                  // Main camera reference
    [HideInInspector]
    public Vector3 m_Move;					  // the world-relative desired move direction
    private Vector3 smoothMove;


    private PlayerInput m_input;
    private PlayerInventory Inventory;
    
    
    
    [Header("Stats")]
    public int playerHealth = 100;
    public bool oneShotHealth = false;
    public GameObject HealthBar;
    


    void Start()
    {
        
        base.Start();
        
        Health = playerHealth;
        
        if (oneShotHealth)
            ApplyOneShotHealth();
        
        m_input = GetComponent<PlayerInput>();
        Inventory = GetComponent<PlayerInventory>();
        
        playerLookRotation = new GameObject();
        playerLookRotation.name = "PlayerLookRotation";
        playerLookRotation.transform.position = transform.position;
        playerLookRotation.transform.parent = transform;
        

        // get the transform of the main camera
        if (Camera.main != null)
        {
            m_Cam = cameraHandler.transform.GetComponentInChildren<Camera>().transform;
        }
        else
        {
            Debug.LogError(
                "There should be a Camera with a tag \"MainCamera\", for camera movement.");
        }

        charAnimator = GetComponent<Animator>();
        m_Rigidbody = GetComponent<Rigidbody>();

        m_Rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
        
      
    }
    

    void ApplyOneShotHealth()
    {
        Health = 1;
    }
    
    
    public void CantRotate()
    {
        b_CanRotate = false;
    }

    // Update is called once per frame
    void Update()
    {

//        float h = Input.GetAxis("Horizontal");
//        float v = Input.GetAxis("Vertical");
        float h = m_input.Horizontal;
        float v = m_input.Vertical;
        
        
        
        if (!m_isDead)
		{
            
            if (b_CanRotate)
            {
                if (Inventory.Aiming)
                {
                    UpdateAim(aimRotation);
                }
                else
                {
                    RunningLook(new Vector3(h, 0, v));
                }
            }

            m_Move = new Vector3(h, 0, v);
            

            m_Move = m_Move.normalized * m_MoveSpeedSpecialModifier;
            //Rotate move in camera space
            m_Move = Quaternion.Euler(0, 0 - transform.eulerAngles.y + m_Cam.transform.parent.transform.eulerAngles.y, 0) * m_Move;

            //Move Player
            Move(m_Move);
            
            
        }
        else
        {
            m_ForwardAmount = 0.0f;
            m_TurnAmount = 0.0f;
            Inventory.Aiming = false;
//            UpdateAnimator(Vector3.zero);
        }
        
        
        
    }
    
    public void Move(Vector3 move)
    {
        if (!Inventory.UsingObject)
        {

            m_TurnAmount = move.x;
            m_ForwardAmount = move.z;

            
            if (charAnimator.GetCurrentAnimatorStateInfo(0).IsName("Aiming"))
            {
                m_Rigidbody.velocity = new Vector3(m_Rigidbody.velocity.x, 0, m_Rigidbody.velocity.z);
            }

            // send input and other state parameters to the animator
            UpdateAnimator(move);
        }
		
    }

    void UpdateAnimator(Vector3 move)
	{
         // update the animator parameters
        charAnimator.SetFloat("Y", m_ForwardAmount, animatorAimingDampValue, Time.deltaTime);
		charAnimator.SetFloat("X", m_TurnAmount, animatorAimingDampValue, Time.deltaTime);
        
        charAnimator.SetFloat("Speed", move.magnitude, animatorRunDampValue, Time.deltaTime);
        

		if (move.magnitude > 0)
		{
            
            if (Inventory.Aiming)
            {
                move *= playerAimSpeed;
                transform.Translate(move * Time.deltaTime);
            }
            else if (Inventory.UsingObject)
            {
                move = move * 0.0f;
                transform.Translate(Vector3.zero);
            }
            else
            {
                move *= playerRunSpeed;
                transform.Translate(move * Time.deltaTime );
            }

            charAnimator.speed = m_AnimSpeedMultiplier ;
		}
		else
		{
			charAnimator.speed = 1;
		}
	}
    
    private void UpdateAim(Vector3 FinalPos)
    {
        playerLookRotation.transform.LookAt(FinalPos);
        transform.rotation = Quaternion.Lerp(transform.rotation, playerLookRotation.transform.rotation, Time.deltaTime * aimingRotationSpeed);
        transform.localEulerAngles = new Vector3(0, transform.localEulerAngles.y, 0);
    }
    
    private void RunningLook(Vector3 Direction)
    {
        if (Direction.magnitude >= 0.25f)
        {
            Direction = Quaternion.Euler(0, 0 + m_Cam.transform.parent.transform.eulerAngles.y, 0) * Direction;

            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(Direction), Time.deltaTime * (runRotationSpeed * 0.1f));

            transform.localEulerAngles = new Vector3(0, transform.localEulerAngles.y, 0);
        }
    }

    public float Health { get; set;}
    public int HealthMax { get; }
    public void ModifyHealth(int amount)
    {
        if (Health > 0)
        {
            //Here you can put some Damage Behaviour if you want
            Health -= amount;

            float healthPercentage = Health / playerHealth;
            
            HealthBar.GetComponent<Image>().fillAmount = healthPercentage;
                
            if (Health <= 0)
            {
                Die();
            }
        }
    }
    
    public void Die()
    {
        m_isDead = true;
        Inventory.isDead = true;
        m_Rigidbody.isKinematic = true;
        charAnimator.enabled = false;
        
        this.tag = "Untagged";
       
        base.OnDeath();
        
    }
    
    protected override void DeathDelayTasks(float initialDelay, float finalDelay)
    {
        StartCoroutine(Disappear(initialDelay, finalDelay));
    }
    
    IEnumerator Disappear(float initialDelay ,float otherDelay)
    {
        yield return new WaitForSeconds(initialDelay);
        GameManager.instance.LevelFail();
        yield return new WaitForSeconds(otherDelay);
        this.gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Fail"))
        {
            GameManager.instance.LevelFail();
        }
        else if (other.CompareTag("FinishArea"))
        {
            GameManager.instance.LevelComplete();
        }
    }
}
