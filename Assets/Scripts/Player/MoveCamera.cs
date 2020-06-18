using UnityEngine;

public class MoveCamera : MonoBehaviour
{

    public Transform player;
    public float turnSpeed = 10.0f;
    private Vector3 offset;  


    public float height = 1f;
    public float distance = 5f;

    private Vector3 offsetX;


    void Start()
    {

        offsetX = new Vector3(0, height, distance);

    }

    void LateUpdate()
    {
        offsetX = Quaternion.AngleAxis(Input.GetAxis("Mouse X") * turnSpeed, Vector3.up) * offsetX;
        transform.position = player.position + offsetX;
        transform.LookAt(player.position);
    }
}