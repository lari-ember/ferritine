using UnityEngine;

[DisallowMultipleComponent]
public class VehicleMover : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 6f;          // unidades por segundo
    public float rotateSpeed = 720f;      // graus por segundo
    public Vector3 targetPosition;
    public bool preserveY = true;         // se true não altera o y atual

    void Awake()
    {
        targetPosition = transform.position;
    }

    void Update()
    {
        Vector3 desired = targetPosition;
        if (preserveY) desired.y = transform.position.y;

        // mover suavemente
        transform.position = Vector3.MoveTowards(transform.position, desired, moveSpeed * Time.deltaTime);

        // girar para a direção do movimento (suavemente)
        Vector3 dir = desired - transform.position;
        dir.y = 0;
        if (dir.sqrMagnitude > 0.0001f)
        {
            Quaternion toRot = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRot, rotateSpeed * Time.deltaTime);
        }
    }
}

