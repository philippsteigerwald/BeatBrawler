using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public int health;
    public float speed;

    void Start() { }

    void Update()
    {
        transform.Translate(Vector2.left * speed * Time.deltaTime);
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        Debug.Log("damage taken is: " + damage);

        if (health <= 0)
        {
            Destroy(this.gameObject);
        }
    }
}
