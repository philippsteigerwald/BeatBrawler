using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

	private float horizontal;
	private float speed = 8f;
	private float jumpingPower = 16f;
	private bool isFacingRight = true; 

	[SerializeField] private Rigidbody2D rb;
	[SerializeField] private Transform groundCheck;
	[SerializeField] private LayerMask groundLayer;
	[SerializeField] private Animator animator;



	void Start()
	{
		//animator = GetComponent<Animator>();
	}    
	void Update()

	{
		horizontal = Input.GetAxisRaw("Horizontal");
		
		animator.SetFloat("xVelocity", Mathf.Abs(rb.velocity.x));
			
		Flip(); // check if we have to flip. // maybe always face enemy and ony flip once they cross
		
		if (Input.GetKeyDown(KeyCode.UpArrow) && isGrounded()) // jumping
		{
			rb.velocity = new Vector2(rb.velocity.x, jumpingPower);
		}
	}

	private void FixedUpdate()  // left right movement
	{
		rb.velocity = new Vector2(horizontal * speed, rb.velocity.y);
	
	}
	
	private bool isGrounded()
	{
		return	Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
	}
	
	private void Flip()
	{
		if (isFacingRight && horizontal < 0f || !isFacingRight && horizontal > 0f)
		{
			isFacingRight = !isFacingRight;
			Vector3 localScale = transform.localScale;
			localScale.x *= -1f;
			transform.localScale = localScale;
		}
	}
	
	
}
