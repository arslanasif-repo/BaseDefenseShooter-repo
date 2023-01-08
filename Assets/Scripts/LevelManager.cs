
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [Header("Levels")]
    public LevelDescription[] Levels;
    
    public void LoadLevel(int level)
    {
        GameObject tempLevel = Instantiate(Levels[level].level, Levels[level].spawn, Quaternion.identity, this.transform);
        GameManager.instance.InitializePlayer();
    }
}
