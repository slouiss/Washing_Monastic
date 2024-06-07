using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Entities")]
public class ennemiesData : ScriptableObject
{
    public float hp;
    public float damage;
    public float attackSpeed;
    public float movementSpeed;
    public float jumpHeight;
    public float attackRange;
    public float detectRange;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}