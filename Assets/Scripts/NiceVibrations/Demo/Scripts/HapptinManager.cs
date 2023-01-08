using System.Collections;
using System.Collections.Generic;
using MoreMountains.NiceVibrations;
using UnityEngine;

public class HapptinManager : MonoBehaviour
{
    public static HapptinManager instance;
    void Awake()
    {
        if (!instance) instance = this;
        
    }

    public void LowVibrate()
    {
        MMVibrationManager.Haptic(HapticTypes.LightImpact);
    }
    public void MediumVibrate()
    {
        MMVibrationManager.Haptic(HapticTypes.MediumImpact);
    }
    public void HighVibrate()
    {
        MMVibrationManager.Haptic(HapticTypes.HeavyImpact);
    }

    public void SuccesVibrate()
    {
        MMVibrationManager.Haptic(HapticTypes.Success);
    } 
    public void FailVibrate()
    {
        MMVibrationManager.Haptic(HapticTypes.Failure);
    }
    
}
