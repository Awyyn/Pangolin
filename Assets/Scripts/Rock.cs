using UnityEngine;

public class Rock : MonoBehaviour
{
    private Vector3 initialPosition;

    private void Start()
    {
        initialPosition = transform.position;
    }

    public void ResetRock()
    {
        transform.position = initialPosition;
    }
}
