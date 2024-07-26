using UnityEngine;
using System.Collections;

public class DiagonalJump : MonoBehaviour
{
	public float jumpHeight = 2.0f; // The height the player should jump
	public float jumpDuration = 1.0f; // The total duration of the jump
	public float horizontalDistance = 1.0f; // The horizontal distance the player should move

   // private bool isGrounded = true;
	private bool isJumping = false;
	private bool isGrounded = true;

	void Update()
	{
		if (isGrounded && !isJumping)
		{
			Vector2 direction = Vector2.zero;

			if (Input.GetKey(KeyCode.UpArrow))
			{
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
		}
	}

	private IEnumerator Jump(Vector2 direction)
	{
		isJumping = true;
		isGrounded = false;

		Vector2 startPosition = transform.position;
		Vector2 peakPosition = startPosition + (Vector2.up * jumpHeight) + (direction.normalized * horizontalDistance / 2);
		Vector2 endPosition = startPosition + direction.normalized * horizontalDistance;

		float elapsedTime = 0;

		// Jump up to the peak
		while (elapsedTime < jumpDuration / 2)
		{
			transform.position = Vector2.Lerp(startPosition, peakPosition, elapsedTime / (jumpDuration / 2));
			elapsedTime += Time.deltaTime;
			yield return null;
		}

		// Ensure the position is set exactly to the peak to avoid floating point errors
	   // transform.position = peakPosition;

		elapsedTime = 0;

		// Fall back down to the end position
		while (elapsedTime < jumpDuration / 2)
		{
			transform.position = Vector2.Lerp(peakPosition, endPosition, elapsedTime / (jumpDuration / 2));
			elapsedTime += Time.deltaTime;
			yield return null;
		}

		// Ensure the position is set exactly to the end position to avoid floating point errors
		//transform.position = endPosition;

		isGrounded = true;
		isJumping = false;
	}
	
	

}
