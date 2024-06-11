using System.Collections;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public float speed = 5f;
    private Rigidbody2D rigidbody;
    private Animator anim;

    [Header("Attack")]
    private float attackTime;
    [SerializeField] private float timeBetweenAttack;
    private bool canMove = true;
    [SerializeField] Transform checkEnemy;
    public LayerMask whatIsEnemy;

    void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (canMove)
        {
            Move();
        }
        Attack();
    }

    void Move()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        if (Mathf.Abs(horizontal) > 0.1f || Mathf.Abs(vertical) > 0.1f)
        {
            anim.SetFloat("lastInputX", horizontal);
            anim.SetFloat("lastInputY", vertical);
        }

        rigidbody.velocity = new Vector2(horizontal * speed, vertical * speed);

        if (horizontal != 0 || vertical != 0)
        {
            anim.SetFloat("inputX", horizontal);
            anim.SetFloat("inputY", vertical);
        }
    }

    private void Attack()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (Time.time >= attackTime)
            {
                rigidbody.velocity = Vector2.zero;
                Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                mousePosition.z = 0; // Assurez-vous que la position Z est 0 pour une attaque en 2D

                Vector2 attackDirection = (mousePosition - transform.position).normalized;

                Debug.Log($"Mouse Position: {mousePosition}");
                Debug.Log($"Player Position: {transform.position}");
                Debug.Log($"Calculated Attack Direction: {attackDirection}");

                anim.SetFloat("attackDirectionX", attackDirection.x);
                anim.SetFloat("attackDirectionY", attackDirection.y);
                anim.SetTrigger("attack");

                Debug.Log($"Set Animator Parameters: attackDirectionX = {attackDirection.x}, attackDirectionY = {attackDirection.y}");

                // Update checkEnemy position based on attack direction
                checkEnemy.position = transform.position + (Vector3)attackDirection;

                StartCoroutine(Delay());

                IEnumerator Delay()
                {
                    canMove = false;
                    yield return new WaitForSeconds(0.3f);
                    canMove = true;
                }

                attackTime = Time.time + timeBetweenAttack;
            }
        }
    }


    public void OnAttack()
    {
        Collider2D[] enemy = Physics2D.OverlapCircleAll(checkEnemy.position, 0.5f, whatIsEnemy);

        foreach(var enemy_ in enemy)
        {
            //
        }
    }
}
