using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    [Header("Bullet Settings")]
    public GameObject bulletPrefab;
    public Transform firePoint;

    [Header("Gun Rotation")]
    [Tooltip("Drag the gun pivot Transform here (the child that should rotate toward the cursor)")]
    public Transform gunPivot;
    [Tooltip("Offset in degrees to align the barrel with the +X axis. " +
             "Barrel points UP by default → set -90. Points LEFT → set 180. Points DOWN → set 90.")]
    public float rotationOffset = 0f;

    private bool inputEnabled = true;

    /// <summary>Called by FinalLevelIntro to lock/unlock shooting.</summary>
    public void SetInputEnabled(bool enabled) { inputEnabled = enabled; }

    void Update()
    {
        if (!inputEnabled) return;

        RotateGunTowardCursor();

        if (Input.GetMouseButtonDown(0))
        {
            Shoot();
        }
    }

    void RotateGunTowardCursor()
{
    if (gunPivot == null) return;

    Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    mouseWorld.z = 0f;

    Vector2 direction = mouseWorld - gunPivot.position;
    float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

    bool aimingLeft = Mathf.Abs(angle) > 90f;

    // 1) Cancel the player's flip so the gun is never mirrored sideways
    float parentFlip = Mathf.Sign(gunPivot.parent.lossyScale.x);

    Vector3 gs = gunPivot.localScale;
    gs.x = Mathf.Abs(gs.x) * parentFlip;
    // 2) Flip Y when aiming left so the sprite isn't upside-down
    gs.y = aimingLeft ? -Mathf.Abs(gs.y) : Mathf.Abs(gs.y);
    gunPivot.localScale = gs;

    // 3) Rotate toward the cursor (set scale first, then rotation)
    gunPivot.rotation = Quaternion.Euler(0f, 0f, angle + rotationOffset);
}

    void Shoot()
    {
        if (bulletPrefab == null || firePoint == null) return;

        // Fire toward cursor so bullets match the gun's aim direction.
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = 0f;
        Vector2 dir = ((Vector2)(mouseWorld - firePoint.position)).normalized;

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        Bullet b = bullet.GetComponent<Bullet>();
        if (b != null)
        {
            b.Launch(dir);
        }
    }
}