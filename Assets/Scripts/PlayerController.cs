using System.Collections;
using UnityEngine;
using UnityEngine.UI;
public class PlayerController : MonoBehaviour
{
    public float speed = 5f;
    private Rigidbody2D rigidbody;
    private Animator anim;

    [Header("Attack")]
    private float attackTime;
    public int currentHealth;
    public int maxHealth;
    [SerializeField] private float timeBetweenAttack;
    private bool canMove = true;
    [SerializeField] Transform checkEnemy;
    public LayerMask whatIsEnemy;
    //public Image bar;

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
        OnAttack();
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

        anim.SetFloat("inputX", horizontal);
        anim.SetFloat("inputY", vertical);
        anim.SetBool("isMoving", horizontal != 0 || vertical != 0);
    }

    private void OnAttack()
    {
        if (Input.GetMouseButtonDown(0) && Time.time >= attackTime)
        {
            rigidbody.velocity = Vector2.zero;
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 0;

            Vector2 attackDirection = (mousePosition - transform.position).normalized;

            anim.SetFloat("attackDirectionX", attackDirection.x);
            anim.SetFloat("attackDirectionY", attackDirection.y);
            anim.SetTrigger("attack");

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

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Destroy(this.gameObject);
        }
    }  
}
