using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour {

	/* Movement:
	 * Simple rules that add up to complex behavior
	 * Keep things seperate
	 * Make sure to keep things understandable
	 */

	private Rigidbody2D rb;

	private float gravity = -10;
	private float acceleration = 10;
	private float maxGroundVelocity = 5;
	private float jumpForce = 100;

	public KeyCode leftButton = KeyCode.A;
	public KeyCode rightButton = KeyCode.D;
	public KeyCode jumpButton = KeyCode.Space;
	private bool inputLeft;
	private bool inputRight;
	private bool inputJump;
	public bool onGround = false;

	public AnimationCurve curve;

	private void OnCollisionEnter2D(Collision2D collision) {
		onGround = true;
	}

	private void OnCollisionExit2D(Collision2D collision) {
		onGround = false;
	}

	private void Start() {
		rb = GetComponent<Rigidbody2D>();
	}

	// Update is called once per frame
	private void Update() {

		inputLeft = Input.GetKey(leftButton);
		inputRight = Input.GetKey(rightButton);
		inputJump = Input.GetKeyDown(jumpButton);

	}


	//Bug: When at full speed it is impossible to turn around
	//It happens because the deltaVelocity becomes 0 since no more velocity has to be added

	//The fix shouldn't just turn the velocity around, but should also make sure that the proper acceleration is being applied

	private void FixedUpdate() {

		if(inputLeft || inputRight) {
			float value = Mathf.Abs(rb.velocity.x / maxGroundVelocity);

			float currentTime = AnimationCurveExtension.ReverseEvaluate(curve, value);
			float newTime = currentTime + Time.deltaTime;
			float newVelocity = curve.Evaluate(newTime) * maxGroundVelocity;
			float velocityDelta = newVelocity - Mathf.Abs(rb.velocity.x);
			Debug.Log("ReverseGetTime: " + currentTime + ", currentVelocity: " + rb.velocity.x + ", newVelocity: " + newVelocity + ", velocity delta: " + velocityDelta);

			Vector2 addVector = new Vector2(velocityDelta, 0);

			if (inputLeft) {
				rb.velocity -= addVector;
			}

			if (inputRight) {
				rb.velocity += addVector;
			}
		}

		//Clamp horizontal speed
		float clampedXSpeed = Mathf.Clamp(rb.velocity.x, -maxGroundVelocity, maxGroundVelocity);
		rb.velocity = new Vector2(clampedXSpeed, rb.velocity.y);

		if (inputJump && onGround) {
			rb.AddForce(new Vector2(0, jumpForce));	
		}

		if (!onGround) {
			rb.AddForce(new Vector2(0, gravity));
		}
	}
}
