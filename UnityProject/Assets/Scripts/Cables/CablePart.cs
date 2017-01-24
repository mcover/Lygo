using UnityEngine;

public class CablePart : MonoBehaviour
{
    public float cablePartLength { private set; get; }

    private void Awake()
    {
        CircleCollider2D col = GetComponent<CircleCollider2D>();

        if (col == null)
        {
            Debug.LogError("Cable Part collider is not Circle Collider! Can't get length");
        }
        else
        {
            cablePartLength = col.radius * 2;
        }
    }
}
