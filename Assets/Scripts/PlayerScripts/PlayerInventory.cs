using System;
using UnityEngine;


public class PlayerInventory : MonoBehaviour
{
    
    [Header("Stats")]
    [HideInInspector] public bool isDead = false;

    
    [Header("Weapons")]


    [HideInInspector]
    public bool Armed = true;
    private PlayerWeapon currentWeapon;
    public GameObject[] Weapons;
    
    [HideInInspector] public int ActiveWeapon = 0;
    
    private bool CanShoot = true;
    
    [HideInInspector] public bool Aiming = false;

    private float FireRateTimer = 0.0f;
    private float LastFireTimer = 0.0f;
    
    
    [HideInInspector] public bool UsingObject = false;
    
   
    
    [HideInInspector]
    public PlayerController charController;
    [HideInInspector]
    public Animator charAnimator;

    
    void Start()
    {
        charAnimator = GetComponent<Animator>();

        if (GetComponent<PlayerController>())
            charController = GetComponent<PlayerController>();



        if (Weapons.Length > 0)
        {
            InitializeWeapon(ActiveWeapon);
            Armed = true;
        }
        else
        {
            Armed = false;
        }

    }
    
    
    
    void InitializeWeapon(int currentWeaponIndex)
    {
        ActiveWeapon = currentWeaponIndex;
        currentWeapon = Weapons[ActiveWeapon].GetComponent<PlayerWeapon>();
        currentWeapon.Player = this.gameObject;
        FireRateTimer = currentWeapon.FireRate;
        currentWeapon.Audio = currentWeapon.GetComponent<AudioSource>();

        int WeaponLayer = charAnimator.GetLayerIndex(currentWeapon.m_Type.ToString());
        charAnimator.SetLayerWeight(WeaponLayer, 1.0f);
        
        currentWeapon.gameObject.SetActive(true);
    }

    void Update()
    {
        if (!isDead)
		{
            if (currentWeapon.DetectEnemy())
            {
//                charController.aimRotation = currentWeapon.FinalTarget;
                Aiming = true;
                charAnimator.SetBool("Aiming", true);
                
                if (CanShoot && currentWeapon != null && Time.time >= (LastFireTimer + FireRateTimer))
                {
                    if (Aiming)
                    {
                        LastFireTimer = Time.time;
                        currentWeapon.Shoot();
                    }
                }
            }
            else
            {
                Aiming = false;
                charAnimator.SetBool("Aiming", false);
            }
        }
    }
    
    
    void EquipWeapon(int weapon)
    {
        currentWeapon.gameObject.SetActive(false);
        int WeaponLayer = charAnimator.GetLayerIndex(currentWeapon.m_Type.ToString());
        
        charAnimator.SetLayerWeight(WeaponLayer, 0.0f);
        InitializeWeapon(weapon);
    }

    
    public void SetNewSpeed(float speedFactor)
    {
        charController.m_MoveSpeedSpecialModifier = speedFactor;
    }
    
    
    
    void OnTriggerEnter(Collider other) { 
        if (other.CompareTag("Pickup"))
        {
            if (other.gameObject.GetComponent<PickableObject>())
            {
                EquipWeapon((int)other.gameObject.GetComponent<PickableObject>().m_type);
                other.gameObject.SetActive(false);
            }
            else
            {
                other.gameObject.SetActive(false);
            }


        }
    }
}

public enum WeaponType
{
    Pistol,
    MachineGun,
    Shotgun,
    Launcher
}
