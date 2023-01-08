
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("UI Panels")] 
    public GameObject Gameplay;
    public GameObject CompletePanel, FailPanel, PlayButton;
    
    [Header("UI Bars")] 
    public Image progressBar;

    [Header("UI Texts")] 
    public Text currentLevel;
    public Text nextLevel;
    public Text CompleteText;
}
