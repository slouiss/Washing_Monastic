using UnityEditor;
using UnityEngine;

public class DemonMovements : MonoBehaviour
{
    public float speed;
    public Transform[] Waypoints;

    private Transform target;
    private int destPoint = 0;



    void Start()
    {
        target = Waypoints[0];
    }

    void Update()
    {
        Vector3 dir = target.position - transform.position;
        transform.Translate(dir.normalized * speed * Time.deltaTime, Space.World);

        if (Vector3.Distance(transform.position, target.position) < 0.3f)
            destPoint = (destPoint + 1) % Waypoints.Length;
            target = Waypoints[destPoint];
    }
}