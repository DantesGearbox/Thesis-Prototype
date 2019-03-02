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

	public float gravity = -10;
	public float acceleration = 10;
	public float jumpForce = 100;

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

		if (Input.GetKey(KeyCode.A)) {
			//rb.AddForce(new Vector2(-acceleration, 0));
			rb.velocity -= new Vector2(AnimationCurveExtension.ReverseEvaluate(curve, rb.velocity.x), 0);
		}

		if (Input.GetKey(KeyCode.D)) {
			rb.velocity += new Vector2(AnimationCurveExtension.ReverseEvaluate(curve, rb.velocity.x), 0);
		}

		if (Input.GetKeyDown(KeyCode.Space)) {
			rb.AddForce(new Vector2(0, jumpForce));
		}

		if (!onGround) {
			rb.AddForce(new Vector2(0, gravity));
		}
	}
}
