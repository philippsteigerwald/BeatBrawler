using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
	private float timeBtwAttack;
	public float startTimeBtwAttack;
	
	public Transform attackPos;
	public float attackRange;
	
	public LayerMask whatisEnemies;
	[SerializeField] private Animator attackAnimator;

	public int health;
	public int damage = 1;
	

	void Update()
	{
		if(timeBtwAttack <= 0)
		{
			if(Input.GetKeyDown(KeyCode.Space))
			{
				attackAnimator.SetTrigger("Swing");
			}
			timeBtwAttack = startTimeBtwAttack;
			
/* 			foreach(Collider2D enemy in enemiesToDamage)
			{
				
			} */
			
		}
		else
		{
			timeBtwAttack -= Time.deltaTime;
		}
	}
	
	void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(attackPos.position, attackRange);
	}
	
	void InflictDamage()
	{
		
				
		Collider2D[] enemiesToDamage = Physics2D.OverlapCircleAll(attackPos.position, attackRange, whatisEnemies);
				for (int i = 0; i < enemiesToDamage.Length; i++)
				{
					enemiesToDamage[i].GetComponent<EnemyMovement>().TakeDamage(damage);
					
				}
	}
}
