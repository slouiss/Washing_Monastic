using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;


public class Monster : MonoBehaviour
{

    [Header("Stats")]
    [SerializeField] float speed;
    private float playerDetectTime;
    public float playerDetectRate;
    public float chaseRange;

    [Header("Attack")]
    [SerializeField] float attackRange;
    [SerializeField] int damage;
    [SerializeField] float attackRate;
    private float lastAttackTime;

    [Header("Component")]
    Rigidbody2D rb;
    private PlayerController targetPlayer;

    [Header("Pathfinding")]
    public float nextWaypointDistance = 2f;
    Path path;
    int currentWaypoint = 0;
    bool reachEndPath = false;
    Seeker seeker;


    void Start()
    {
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();
        InvokeRepeating("UpdatePath", 0f,.5f);
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
                // Calcule la direction vers le joueur
                Vector2 directionToPlayer = player.transform.position - transform.position;
                
                // Lance un rayon depuis le monstre vers le joueur
                RaycastHit2D hit = Physics2D.Raycast(transform.position, directionToPlayer, Mathf.Infinity, LayerMask.GetMask("Walls"));
                
                // Si le rayon ne frappe pas un mur, le joueur est visible
                if (hit.collider == null)
                {
                    float dist = directionToPlayer.magnitude;
                    if (player == targetPlayer)
                    {
                        if (dist > chaseRange)
                        {
                            targetPlayer = null;
                        }
                    }
                    else if (dist < chaseRange)
                    {
                        if (targetPlayer == null)
                        {
                            targetPlayer = player;
                        }
                    }
                }
            }
        }
    }
}
}
