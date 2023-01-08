
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    [Header("Inputs")] 
    
    [SerializeField] private Joystick inputJoystick;

    public float Horizontal { get; private set;}
    public float Vertical { get; private set;}

    void Update()
    {
        Horizontal = inputJoystick.Horizontal;
        Vertical = inputJoystick.Vertical ;
    }
}
