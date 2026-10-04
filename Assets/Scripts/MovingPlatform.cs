using UnityEngine;

public class MovingPlatform : BasePlatform
{
    [Header("Movement")]
    public Transform pointA;
    public Transform pointB;
    public float moveSpeed = 2f;

    private Transform targetPoint;

    void Start()
    {
        targetPoint = pointB;
    }

    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPoint.position, moveSpeed * Time.deltaTime);

        if (transform.position == targetPoint.position)
        {
            if (targetPoint == pointA)
            {
                targetPoint = pointB;
            }
            else
            {
                targetPoint = pointA;
            }
        }
    }
}