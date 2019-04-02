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
	private BoxCollider2D boxCol;

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

	private void CheckGroundCollision() {
		//Get the 10 first contacts of the box collider
		ContactPoint2D[] contacts = new ContactPoint2D[10];
		int count = boxCol.GetContacts(contacts);

		//If we find any horizontal surfaces, we are on the ground
		onGround = false;
		for (int i = 0; i < count; i++) {

			//If the angle between the normal and up is less than 5, we are on the ground
			if (Vector2.Angle(contacts[i].normal, Vector2.up) < 5.0f) {
				onGround = true;
				//rb.velocity = new Vector2(rb.velocity.x, 0);
			}
		}
	}

	//Whenever we collide, check if we are touching the ground
	private void OnCollisionEnter2D(Collision2D collision) {
		CheckGroundCollision();
	}

	private void OnCollisionExit2D(Collision2D collision) {
		CheckGroundCollision();
	}

	private void Start() {
		rb = GetComponent<Rigidbody2D>();
		boxCol = GetComponent<BoxCollider2D>();
	}

	// Update is called once per frame
	private void Update() {

		inputLeft = Input.GetKey(leftButton);
		inputRight = Input.GetKey(rightButton);
		inputJump = Input.GetKeyDown(jumpButton);

	}

	//BUG: We are currently missing inputs. Fix it by looking at this: https://answers.unity.com/questions/409507/eliminating-input-loss.html

	//Bug: When at full speed it is impossible to turn around
	//It happens because the velocity delta becomes 0 since no more velocity has to be added

	//The fix shouldn't just turn the velocity around, but should also make sure that the proper acceleration is being applied

	private void FixedUpdate() {

		if(inputLeft || inputRight) {
			float value = Mathf.Abs(rb.velocity.x / maxGroundVelocity);

			float currentTime = AnimationCurveExtension.ReverseEvaluate(curve, value);
			float newTime = currentTime + Time.deltaTime;
			float newVelocity = curve.Evaluate(newTime) * maxGroundVelocity;
			float velocityDelta = newVelocity - Mathf.Abs(rb.velocity.x);

			//This is a nasty solution to the above issue, but I guess it works
			if (velocityDelta == 0) {
				velocityDelta = 0.01f;
			}

			//Debug.Log("ReverseGetTime: " + currentTime + ", currentVelocity: " + rb.velocity.x + ", newVelocity: " + newVelocity + ", velocity delta: " + velocityDelta);

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

		if (inputJump) {
			Debug.Log("Got input");
		}

		if (inputJump && onGround) {
			rb.AddForce(new Vector2(0, jumpForce));
		}

		if (!onGround) {
			rb.AddForce(new Vector2(0, gravity));
		}
	}
}
