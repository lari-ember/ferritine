using UnityEngine;

public class VehicleController : MonoBehaviour
{
    public float speed = 5f;

    public void MoveTowards(Vector3 target)
    {
        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
    }
}

