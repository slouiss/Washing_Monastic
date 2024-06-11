using UnityEditor;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public float speed = 5;
    new Rigidbody2D rigidbody;


    void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        float Horizontal = Input.GetAxis ("Horizontal");
        float Vertical = Input.GetAxis ("Vertical");

        rigidbody.velocity = new Vector2 (Horizontal * speed, Vertical * speed);
    }
}