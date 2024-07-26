using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
	
	public int health;
	public float speed;
	
	
	
	// Start is called before the first frame update
	void Start()
	{
		
	}

	// Update is called once per frame
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
