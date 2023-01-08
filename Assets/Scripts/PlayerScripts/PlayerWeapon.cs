
using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerWeapon : MonoBehaviour
{

    public WeaponType m_Type;
    
    [Header("Stats")]

    public int bulletsPerShoot = 1;
    public int bulletDamage = 1;
    public float bulletSize = 1.0f;

    public float bulletSpeed = 250f;
    
    public float bulletTimeToLive = 3.0f;
    
    private int currentBullet = 0;
    private GameObject Muzzle;

    
    public float FireRate = 0.1f;
    [SerializeField] private Vector3 targetOffset;
    
    [Header("References & VFX")]
    public float shootShakeFactor = 2.0f;
    public Transform ShootFXPos;
    public GameObject BulletPrefab;
    public GameObject ShootFXFLash;
    
    private PlayerCamera playerCamera;
    
    [HideInInspector]
    public Transform ShootTarget;
//    [HideInInspector]
    public GameObject Player;
    
    [Header("Sound FX")]
    public AudioClip ShootSFX;
    
    [HideInInspector]
    public AudioSource Audio;
    
    [Header("Autoaim")]
    public float AutoAimDistance = 10.0f;
    
    private Vector3 EnemyTargetAuto = Vector3.zero;
    [HideInInspector]
    public Vector3 FinalTarget = Vector3.zero;
    
    //Object Pooling Manager
    public bool usePooling = true;
    private GameObject[] GameBullets;
    private GameObject BulletsParent;


    void Start()
    {

        
        Audio = transform.parent.GetComponent<AudioSource>();
        
        if (usePooling)
        {
            GameBullets = new GameObject[20];
            BulletsParent = new GameObject(m_Type + "_Bullets");

            for (int i = 0; i < 20; i++)
            {
                GameBullets[i] = Instantiate(BulletPrefab, ShootFXPos.position, ShootFXPos.rotation) as GameObject;
                GameBullets[i].SetActive(false);
                GameBullets[i].name = m_Type + "_Bullet_" + i.ToString();
                GameBullets[i].transform.parent = BulletsParent.transform;
                
                GameBullets[i].GetComponent<PlayerBullet>().usePooling = true;
                GameBullets[i].GetComponent<PlayerBullet>().InitializePooling();
            }
        }
        
        if (ShootFXFLash)
        {
            Muzzle = Instantiate(ShootFXFLash, ShootFXPos.position, ShootFXPos.rotation) as GameObject;
            Muzzle.transform.parent = ShootFXPos.transform;
            Muzzle.transform.localPosition = Vector3.zero;
            Muzzle.transform.localScale = Vector3.one;
            Muzzle.SetActive(false);
        }
        
        if (GameObject.Find("PlayerCamera") != null)
        {
            playerCamera = GameObject.Find("PlayerCamera").GetComponent<PlayerCamera>();

        }
    }
    
    private void OnDestroy()
    {
        if (BulletsParent)
            Destroy(BulletsParent);
    }
    
    
    public bool DetectEnemy()
    {
        GameObject[] Enemys = GameObject.FindGameObjectsWithTag("Enemy");
        if (Enemys != null)
        {
            foreach (GameObject Enemy in Enemys)
            {
                if (Vector3.Distance(Enemy.transform.position, Player.transform.position) <=  AutoAimDistance)
                {
                    if (Enemy.GetComponent<EnemyAI>().m_State != CurrentState.Dead)
                    {
//                        FinalTarget = Enemy.transform.position;
                        return true;
                    }
                }
            }
        }
        return false;
    }
    
    void AutoAim()
    {

        GameObject[] Enemys = GameObject.FindGameObjectsWithTag("Enemy");
        if (Enemys != null)
        {
            float BestDistance = 100.0f;

            foreach (GameObject Enemy in Enemys)
            {
                Vector3 EnemyPos = Enemy.transform.position;
                Vector3 EnemyDirection = EnemyPos - Player.transform.position;
                float EnemyDistance = EnemyDirection.magnitude;

                if (Vector3.Distance(Enemy.transform.position, Player.transform.position) <=  AutoAimDistance)
                {
                    
                    if (Enemy.GetComponent<EnemyAI>().m_State != CurrentState.Dead)
                    {
                        if (EnemyDistance < BestDistance)
                        {
                            BestDistance = EnemyDistance;
                            EnemyTargetAuto = EnemyPos + targetOffset;
                        }
                    }
                }
            }
        }

        if (EnemyTargetAuto != Vector3.zero)
        {
            FinalTarget = EnemyTargetAuto;
            ShootFXPos.transform.LookAt(FinalTarget);
        }

    }
    
    public void PlayShootAudio()
    {
        if (ShootSFX)
        {
            Audio.PlayOneShot(ShootSFX, 0.75f);
        }
    }
    
    public void Shoot()
	{
        AutoAim();

        Player.GetComponent<PlayerController>().aimRotation = FinalTarget;
        
        for (int i = 0; i < bulletsPerShoot; i++)
		{
            ShootFXPos.transform.LookAt(FinalTarget);
            
            GameObject Bullet = GameBullets[currentBullet];
            if (usePooling)
            {
                Bullet.gameObject.SetActive(false);
                Bullet.transform.position = ShootFXPos.position;
                Bullet.transform.rotation = ShootFXPos.rotation;
                Bullet.GetComponent<Rigidbody>().collisionDetectionMode = CollisionDetectionMode.Discrete;
                Bullet.GetComponent<Rigidbody>().isKinematic = true;
                Bullet.GetComponent<Rigidbody>().velocity = Vector3.zero;
                Bullet.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
                Bullet.GetComponent<Rigidbody>().isKinematic = false;
                Bullet.GetComponent<Rigidbody>().collisionDetectionMode = CollisionDetectionMode.Continuous;
                Bullet.GetComponent<Collider>().enabled = true;
                Bullet.GetComponent<PlayerBullet>().timeToLive = bulletTimeToLive;
                Bullet.GetComponent<PlayerBullet>().ResetPooling();
                
                currentBullet += 1;
                if (currentBullet >= GameBullets.Length)
                    currentBullet = 0;
            }
            
            //Object Pooling VFX
            Muzzle.transform.rotation = transform.rotation;
            EmitParticles(Muzzle);
            
//            Generic 
            Bullet.GetComponent<PlayerBullet>().Damage = bulletDamage;
            Bullet.GetComponent<PlayerBullet>().BulletSpeed = bulletSpeed;
            PlayShootAudio();
            Bullet.SetActive(true);
            if (usePooling)
                Bullet.transform.localScale = Bullet.GetComponent<PlayerBullet>().OriginalScale * bulletSize;

            
            if (playerCamera)
            {
                playerCamera.Shake(shootShakeFactor * 0.5f, 0.2f);
            }
            EnemyTargetAuto = Vector3.zero;
        }
	}
    
    void EmitParticles(GameObject VFXEmiiter)
    {
        VFXEmiiter.gameObject.SetActive(true);
        VFXEmiiter.transform.GetChild(0).gameObject.GetComponent<ParticleSystem>().Play();
    }
}
