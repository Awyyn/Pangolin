using UnityEngine;

public class AutoDestroy : MonoBehaviour
{
    public float lifetime = 1f; // match animation length
    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void OnDisable()
    {
        // stop any running Animator graphs cleanly
        var anim = GetComponent<Animator>();
        if (anim != null) anim.enabled = false;
    }
}
