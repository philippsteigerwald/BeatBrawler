using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using DG.Tweening;



public class BufferedMovement : MonoBehaviour
{
	public float moveDistance = 1.0f; // Distance to move per key press
	
	
	public float jumpHeight = 2.0f; // The height the player should jump
	public float inputBufferTime = 0.2f; // Time window to detect diagonal input
	
	public float movementDurationDecrease = 0.3f;
	
	//public float jumpDuration = 0.5f; // The time the jump should take to reach the peak

	[HideInInspector] public static bool isMoving = false;
	
	private KeyCode lastKeyPressed;
	private float lastKeyPressTime;
	
	[HideInInspector] public float moveDuration; // Duration of the movement
	
	
	[SerializeField] private Rigidbody2D rb;
	[SerializeField] private Transform groundCheck;
	[SerializeField] private LayerMask groundLayer;
	[SerializeField] private Animator animator;
	
	[SerializeField] private WwiseClockSync wwiseClockSync;
	
	public InputEvaluation inputEvaluation;
	
	

	private float horizontalInput;
	private bool jumpInput;
	private bool diagonalJumpInput;
	private float jumpBufferTimer;
	private float horizontalBufferTimer;
	private float startTime;
	private int arrowKeyCount;
	
	void Start()
	{
		moveDuration = WwiseClockSync.secondsPerBeat - movementDurationDecrease;
	}

	void Update()
	{

		if (!isMoving && (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow)) && arrowKeyCount == 0)
		{	

				StartCoroutine(Buffer());

			
		}
		
		arrowKeyCount = (Input.GetKey(KeyCode.UpArrow) ? 1 : 0) +
					(Input.GetKey(KeyCode.DownArrow) ? 1 : 0) +
					(Input.GetKey(KeyCode.LeftArrow) ? 1 : 0) +
					(Input.GetKey(KeyCode.RightArrow) ? 1 : 0);

	}		
	
	IEnumerator Buffer()
	{
		yield return new WaitForSeconds(inputBufferTime);
		MovementPicker();
	}	
	
	
	private void MovementPicker()
	{				
		
					Vector2 direction = Vector2.zero;
	
					if (Input.GetKey(KeyCode.UpArrow) && Input.GetKey(KeyCode.LeftArrow) && arrowKeyCount == 2 && isGrounded() && inputEvaluation.WindowChecker())
					
					{ 

						direction = Vector2.up + Vector2.left;
						StartCoroutine(Jump(direction));
						inputEvaluation.UpdateAccordingToWindow();

						

					}
					else if (Input.GetKey(KeyCode.UpArrow) && Input.GetKey(KeyCode.RightArrow) &&  arrowKeyCount == 2 && isGrounded() && inputEvaluation.WindowChecker())
					{

						direction = Vector2.up + Vector2.right;
						StartCoroutine(Jump(direction));
						inputEvaluation.UpdateAccordingToWindow();

					}
					else if (Input.GetKey(KeyCode.UpArrow) && arrowKeyCount == 1 && isGrounded() && inputEvaluation.WindowChecker())
					{


						direction = Vector2.up;
						StartCoroutine(Jump(direction));
						inputEvaluation.UpdateAccordingToWindow();
						
					}
					
					else if (Input.GetKey(KeyCode.LeftArrow) && arrowKeyCount == 1 && isGrounded() && inputEvaluation.WindowChecker())
					{


						direction = Vector2.left;
						StartCoroutine(Move(direction));
						inputEvaluation.UpdateAccordingToWindow();

					}
					
					else if (Input.GetKey(KeyCode.RightArrow) && arrowKeyCount == 1 && isGrounded() && inputEvaluation.WindowChecker())
					{

						direction = Vector2.right;
						StartCoroutine(Move(direction));
						inputEvaluation.UpdateAccordingToWindow();

					}	
					
					//One of the Keys was pressed outside of the timing Window / we can do an else here maybe a confusion animation  
					else 
					{

						inputEvaluation.WindowChecker();
						inputEvaluation.UpdateAccordingToWindow();
						return;
					}	

		}
		
	

	private IEnumerator Move(Vector2 direction)
	{
		isMoving = true;
		float currentTime = Time.time;

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
		float currentTime = Time.time;
		
		Vector2 startPosition = transform.position;
		Vector2 peakPosition = startPosition + (Vector2.up * jumpHeight) + (direction.normalized * moveDistance / 2);
		Vector2 endPosition = startPosition + direction.normalized * moveDistance;

		float elapsedTime = 0;

		// Jump up to the peak
		while (elapsedTime < moveDuration / 2)
		{
			transform.position = Vector2.Lerp(startPosition, peakPosition, elapsedTime / (moveDuration / 2));
				
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

