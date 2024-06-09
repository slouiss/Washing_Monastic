using UnityEditor;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public float speed = 5;
    new Rigidbody2D rigidbody;
    Animator anim;

    [Header("Attack")]
    private float attackTime;
    [SerializeField] float timeBetweenAttack;

    void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        Move();
        if (Input.GetMouseButtonDown(0))
        {
            if (Time.time >= attackTime)
            {
                anim.SetTrigger("attack");
                attackTime = Time.time + timeBetweenAttack;
            }

        }

    }


    void Move()
    {

        if (Input.GetAxisRaw("Horizontal") > 0.1 || Input.GetAxisRaw("Horizontal") < -0.1 || Input.GetAxisRaw("Vertical") > 0.1 || Input.GetAxisRaw("Vertical") > -0.1)
        {
            anim.SetFloat("lastInputX", Input.GetAxisRaw("Horizontal"));
            anim.SetFloat("lastInputY", Input.GetAxisRaw("Vertical"));
        }

        float Horizontal = Input.GetAxis("Horizontal");
        float Vertical = Input.GetAxis("Vertical");

        rigidbody.velocity = new Vector2(Horizontal * speed, Vertical * speed);

        if (Horizontal != 0 || Vertical != 0)
        {
            anim.SetFloat("inputX", Horizontal);
            anim.SetFloat("inputY", Vertical);
        }
    }
}