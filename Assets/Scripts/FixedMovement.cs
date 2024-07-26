using UnityEngine;
using System.Collections;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;


public class FixedMovement : MonoBehaviour
{
	public float moveDistance = 1.0f; // Distance to move per key press
	public float moveDuration = 0.2f; // Duration of the movement
	
	public float jumpHeight = 2.0f; // The height the player should jump
	public float inputBufferTime = 0.2f; // Time window to detect diagonal input
	
	//public float jumpDuration = 0.5f; // The time the jump should take to reach the peak

	private bool isMoving = false;
	
	private KeyCode lastKeyPressed;
	private float lastKeyPressTime;

	
	
	[SerializeField] private Rigidbody2D rb;
	[SerializeField] private Transform groundCheck;
	[SerializeField] private LayerMask groundLayer;
	[SerializeField] private Animator animator;
	
	

	private float horizontalInput;
	private bool jumpInput;
	private bool diagonalJumpInput;
	private float jumpBufferTimer;
	private float horizontalBufferTimer;
	
/* 	void Update()
	{
		if (isGrounded() && !isMoving)
		{
			
			if (Input.GetKeyDown(KeyCode.LeftArrow))
			{
				HandleKeyPress(KeyCode.LeftArrow);
			}
			else if (Input.GetKeyDown(KeyCode.RightArrow))
			{
				HandleKeyPress(KeyCode.RightArrow);
			}
			else if (Input.GetKeyDown(KeyCode.UpArrow))
			{
				HandleKeyPress(KeyCode.UpArrow);
			}
		}
	}
	
	
	



	private void HandleKeyPress(KeyCode key)
	{
		float currentTime = Time.time;
		Vector2 direction = Vector2.zero;

		if (lastKeyPressed != KeyCode.None && (currentTime - lastKeyPressTime) <= inputBufferTime)
		{
			// A second key is pressed within the buffer time window
			if ((key == KeyCode.UpArrow && (lastKeyPressed == KeyCode.LeftArrow || lastKeyPressed == KeyCode.RightArrow)) ||
				((key == KeyCode.LeftArrow || key == KeyCode.RightArrow) && lastKeyPressed == KeyCode.UpArrow))
			
			
			

				if (lastKeyPressed == KeyCode.LeftArrow || key == KeyCode.LeftArrow)
				{
					direction = Vector2.up + Vector2.left;
				}
				else if (lastKeyPressed == KeyCode.RightArrow || key == KeyCode.RightArrow)
				{
					direction = Vector2.up + Vector2.right;
				}

				StartCoroutine(Jump(direction));
			

		}
		else
		{
			// Only one key pressed, jump in the respective direction
			if (key == KeyCode.UpArrow)
			{
				StartCoroutine(Jump(Vector2.up));
			}
			else if (key == KeyCode.LeftArrow)
			{
				StartCoroutine(Move(Vector2.left * moveDistance));
			}
			else if (key == KeyCode.RightArrow)
			{
				StartCoroutine(Move(Vector2.right * moveDistance));
			}
		}

		lastKeyPressed = key;
		lastKeyPressTime = currentTime;
	} */

	void Update()
	{
		if (!isMoving)
		{
			
			Vector2 direction = Vector2.zero;
			
			if (Input.GetKeyDown(KeyCode.UpArrow) && isGrounded() && !isMoving)
			{
				
				
				//Vector2 direction = Vector2.zero;
				
				if (Input.GetKey(KeyCode.LeftArrow))
				{
					direction = Vector2.up + Vector2.left;
				}
				else if (Input.GetKey(KeyCode.RightArrow))
				{
					direction = Vector2.up + Vector2.right;
				}
				else
				{
					direction = Vector2.up;
				}

				if (direction != Vector2.zero)
				{
					StartCoroutine(Jump(direction));
				}
			}

			else if (Input.GetKeyDown(KeyCode.LeftArrow))
			{
				if (Input.GetKeyDown(KeyCode.UpArrow) && isGrounded() && !isMoving)
				{
					direction = Vector2.up + Vector2.left;
					StartCoroutine(Jump(direction));
				}
				
				else
			
				StartCoroutine(Move(Vector2.left));
			}
			
			else if (Input.GetKeyDown(KeyCode.RightArrow))
			{
				if (Input.GetKeyDown(KeyCode.UpArrow) && isGrounded() && !isMoving)
				{
					direction = Vector2.up + Vector2.right;
					StartCoroutine(Jump(direction));
				}
				
				else
				
				StartCoroutine(Move(Vector2.right));
			} 
		}
	}

	private IEnumerator Move(Vector2 direction)
	{
		isMoving = true;

		Vector2 startPosition = transform.position;
		Vector2 endPosition = startPosition + direction * moveDistance;
		float elapsedTime = 0;

		while (elapsedTime < moveDuration)
		{

			
			transform.position = Vector2.Lerp(startPosition, endPosition, elapsedTime / moveDuration);
			
			if (!IsPathClear(transform.position, endPosition))
			{
				break;
			}
			
			
			elapsedTime += Time.deltaTime;
			yield return null;
		}

		transform.position = endPosition;
		isMoving = false;
	}
	
	
	private IEnumerator Jump(Vector2 direction)
	{

		isMoving = true;
		
		Vector2 startPosition = transform.position;
		Vector2 peakPosition = startPosition + (Vector2.up * jumpHeight) + (direction.normalized * moveDistance / 2);
		Vector2 endPosition = startPosition + direction.normalized * moveDistance;

		float elapsedTime = 0;

		// Jump up to the peak
		while (elapsedTime < moveDuration / 2)
		{
			transform.position = Vector2.Lerp(startPosition, peakPosition, elapsedTime / (moveDuration / 2));
			
			//Vector2 newPosition = Vector2.Lerp(startPosition, peakPosition, elapsedTime / (moveDuration / 2));
			
			// Perform collision detection
			if (!IsPathClear(transform.position, peakPosition))
			{
				break;
			}
			
			elapsedTime += Time.deltaTime;
			yield return null;
		}

		transform.position = peakPosition;

		elapsedTime = 0;

		// Fall back down to the end position
		while (elapsedTime < moveDuration / 2)
		{
			transform.position = Vector2.Lerp(peakPosition, endPosition, elapsedTime / (moveDuration / 2));
			
						// Perform collision detection
			if (!IsPathClear(transform.position, peakPosition))
			{
				break;
			}
			
			elapsedTime += Time.deltaTime;
			yield return null;
		}
		
		transform.position = endPosition;

		isMoving = false;
	}

		private bool isGrounded()
	{
		return	Physics2D.OverlapCircle(groundCheck.position, 0.1f, groundLayer);
	}
	
	
	private bool IsPathClear(Vector2 start, Vector2 end)
	{
		// Perform a raycast from start to end
		RaycastHit2D hit = Physics2D.Raycast(start, end - start, Vector2.Distance(start, end), groundLayer);
		return hit.collider == null;
	}
	
	/*  private bool IsPathClear(Vector2 start, Vector2 end)
	{
		float distance = Vector2.Distance(start, end);
		Vector2 direction = (end - start).normalized;
		int steps = Mathf.CeilToInt(distance / collisionRadius);

		for (int i = 0; i <= steps; i++)
		{
			Vector2 point = start + direction * (i * collisionRadius);
			Collider2D hit = Physics2D.OverlapCircle(point, collisionRadius, collisionLayer);
			if (hit != null)
			{
				return false; // Collision detected
			}
		}

		return true; // No collision detected
	}

	void OnDrawGizmosSelected()
	{
		// Draw a sphere at the transform's position to visualize the collision check radius
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(transform.position, collisionRadius);
	} */

}

