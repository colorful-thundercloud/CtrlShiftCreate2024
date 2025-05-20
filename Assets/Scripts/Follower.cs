using UnityEngine;

public class Follower : MonoBehaviour
{
    public Transform Target;

    private void Start()
    {
        
    }
    void Update()
    {
        if (Target == null) return;
        transform.position = InputManager.WorldToScreen(Target.position);
        transform.localScale = Target.localScale;
    }
}
