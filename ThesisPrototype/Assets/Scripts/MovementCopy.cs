using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementCopy : MonoBehaviour {

	/* Movement:
	 * Simple rules that add up to complex behavior
	 * Keep things seperate
	 * Make sure to keep things understandable
	 */

	private Rigidbody2D rb;

	public float gravity = -10;
	public float acceleration = 10;
	public float jumpForce = 100;

	public bool onGround = false;

	public AnimationCurve accelCurve;
	public AnimationCurve deccelCurve;

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

		float velocityTillZero = 0 - rb.velocity.x;

		//Add velocity according to acceleration curve
		if (Input.GetKey(KeyCode.A)) {
			rb.velocity -= new Vector2(AnimationCurveExtension.ReverseEvaluate(accelCurve, rb.velocity.x), 0);
		}
		//Remove acceleration according to decceleration curve, clamp to not get under 0
		else if (velocityTillZero < 0) {
			float curveDeccel = AnimationCurveExtension.ReverseEvaluate(deccelCurve, rb.velocity.x);
			float clampedCurveDeccel = Mathf.Clamp(curveDeccel, 0, velocityTillZero);
			//rb.velocity += new Vector2(clampedCurveDeccel, 0);
		}

		//Add velocity according to acceleration curve
		if (Input.GetKey(KeyCode.D)) {
			rb.velocity += new Vector2(AnimationCurveExtension.ReverseEvaluate(accelCurve, rb.velocity.x), 0);
		}
		//Remove acceleration according to decceleration curve, clamp to not get under 0
		else if (velocityTillZero > 0) {
			float curveDeccel = AnimationCurveExtension.ReverseEvaluate(deccelCurve, rb.velocity.x);
			float clampedCurveDeccel = Mathf.Clamp(curveDeccel, 0, velocityTillZero);
			//rb.velocity -= new Vector2(clampedCurveDeccel, 0);
		}

		if (Input.GetKeyDown(KeyCode.Space)) {
			rb.AddForce(new Vector2(0, jumpForce));
		}

		if (!onGround) {
			rb.AddForce(new Vector2(0, gravity));
		}
	}
}
