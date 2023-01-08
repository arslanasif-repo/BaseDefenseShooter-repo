
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [Header("Camera Basic Settings")]
    public float FollowSpeed = 1.0f;
    public Transform TargetToFollow;
    //Offset
    [HideInInspector]
    public float TargetHeight = 0.0f;
    [HideInInspector]
    public float targetHeightOffset = -1.0f;
    [HideInInspector]
    public float ActualHeight = 0.0f;
   
    public float HeightSpeed = 1.0f;
    public Transform CameraOffset;

    [Header("Shooting Shake Settings")]
    public bool isShaking = false;
    public float shakeFactor = 3f;
    public float shakeTimer = .2f;
    public float shakeSmoothness = 5f;
    [HideInInspector]
    public float actualShakeTimer = 0.2f;
    
    private Vector3 randomShakePos = Vector3.zero;


    void Start()
    {
        actualShakeTimer = shakeTimer;
    }
    
    void LateUpdate()
    {
        if (TargetToFollow)
        {
            transform.position = Vector3.Lerp(transform.position, TargetToFollow.position, FollowSpeed * Time.deltaTime);

            ActualHeight = Mathf.Lerp(ActualHeight, TargetHeight + targetHeightOffset, Time.deltaTime * HeightSpeed);

            CameraOffset.localPosition = new Vector3(0.0f, 0.0f, ActualHeight);
        }

        if (isShaking)
        {
            if (actualShakeTimer >= 0.0f)
            {
                actualShakeTimer -= Time.deltaTime;
                Vector3 newPos = transform.localPosition + CalculateRandomShake(shakeFactor, false);
                transform.localPosition = Vector3.Lerp(transform.localPosition, newPos, shakeSmoothness * Time.deltaTime);
            }
            else
            {
                isShaking = false;
                actualShakeTimer = shakeTimer;
            }
        }

    }
    
    public void Shake(float factor, float duration)
    {
        isShaking = true;
        shakeFactor = factor;
        shakeTimer = duration;
        actualShakeTimer = shakeTimer;
    }
    
    public Vector3 CalculateRandomShake(float shakeFac, bool isExplosion)
    {
        randomShakePos = new Vector3(Random.Range(-shakeFac, shakeFac), Random.Range(-shakeFac, shakeFac), Random.Range(-shakeFac, shakeFac));
        return randomShakePos * (actualShakeTimer / shakeTimer);
    }

}
