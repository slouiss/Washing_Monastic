using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using UnityEngine.UI;

public class Monster : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] float speed;
    public int Health;
    public int HealthMax;


    private float playerDetectTime;
    public float playerDetectRate;
    public float chaseRange;

    [Header("Attack")]
    [SerializeField] float attackRange;
    [SerializeField] int damage;
    [SerializeField] float attackRate;
    private float lastAttackTime;
    public Transform AttackPoint;
    public LayerMask playerLayerMask;

    [Header("Component")]
    Rigidbody2D rb;
    private PlayerController targetPlayer;
    Animator anim;

    [Header("Pathfinding")]
    public float nextWaypointDistance = 2f;
    Path path;
    int currentWaypoint = 0;
    bool reachEndPath = false;
    Seeker seeker;

    public Image bar;




    private void Awake()
    {
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    private void Start()
    {
        RoomManager roomManager = FindObjectOfType<RoomManager>();
        if (roomManager != null)
        {
            roomManager.OnGenerationComplete.AddListener(StartPathfinding);
        }
    }

    private void StartPathfinding()
    {
        InvokeRepeating("UpdatePath", 0f, 0.5f);
    }

    void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWaypoint = 0;
        }
    }

    void UpdatePath()
    {
        if (rb != null && seeker.IsDone() && targetPlayer != null)
        {
            seeker.StartPath(rb.position, targetPlayer.transform.position, OnPathComplete);
        }
    }

    private void FixedUpdate()
    {
        if (targetPlayer != null)
        {
            float dist = Vector2.Distance(transform.position, targetPlayer.transform.position);
            if (dist < attackRange && Time.time - lastAttackTime >= attackRate)
            {
                rb.velocity = Vector2.zero;
                lastAttackTime = Time.time;
                anim.SetTrigger("attack");
            }

            else if (dist > attackRange)
            {
                if (path == null)
                    return;

                if (currentWaypoint >= path.vectorPath.Count)
                {
                    reachEndPath = true;
                    return;
                }
                else
                {
                    reachEndPath = false;
                }

                Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;
                Vector2 force = direction * speed * Time.fixedDeltaTime;
                rb.velocity = force;

                float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);
                if (distance < nextWaypointDistance)
                {
                    currentWaypoint++;
                }
            }
            else
            {
                rb.velocity = Vector2.zero;
            }
        }

        DetectPlayer();
    }

    void DetectPlayer()
    {
        if (Time.time - playerDetectTime > playerDetectRate)
        {
            playerDetectTime = Time.time;
            foreach (PlayerController player in FindObjectsOfType<PlayerController>())
            {
                if (player != null)
                {
                    Vector2 directionToPlayer = player.transform.position - transform.position;
                    RaycastHit2D hit = Physics2D.Raycast(transform.position, directionToPlayer, Mathf.Infinity, LayerMask.GetMask("wall"));

                    if (hit.collider == null)
                    {
                        float dist = directionToPlayer.magnitude;
                        if (player == targetPlayer)
                        {
                            if (dist > chaseRange)
                            {
                                targetPlayer = null;
                                rb.velocity = Vector2.zero;
                                anim.SetBool("OnMove", false);
                            }
                        }
                        else if (dist < chaseRange)
                        {
                            if (targetPlayer == null)
                            {
                                targetPlayer = player;
                            }
                            anim.SetBool("OnMove", true);

                        }
                    }
                }
            }
        }
    }

    void Attack()
    {
        Collider2D player = Physics2D.OverlapCircle(AttackPoint.transform.position, 0.5f, playerLayerMask);

        if (player != null && player.tag == "Player")
        {
            player.GetComponent<PlayerController>().TakeDamage(damage);
        }
    }

    public void TakeDamage(int damage)
    {
        Health -= damage;
        UpdateHealthBar(Health);

        if (Health <= 0)
        {
            Destroy(this.gameObject);
        }
    }


    public void UpdateHealthBar(int value)
    {
        bar.fillAmount = (float)value / HealthMax;
    }
}
