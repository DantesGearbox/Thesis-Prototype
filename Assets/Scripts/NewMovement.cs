using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
public class NewMovement : MonoBehaviour {

	//Controls
	public KeyCode leftKey = KeyCode.LeftArrow;
	public KeyCode rightKey = KeyCode.RightArrow;
	public KeyCode jumpKey = KeyCode.Z;

	//Components
	Rigidbody2D rb;
	BoxCollider2D boxColl;
	Attack attackComponent;

	//Internal use
	float xVelocity = 0;
	float yVelocity = 0;

	//State for outer classes to read
	[HideInInspector] public bool onGround = false;

	//State for outer classes to set
	[HideInInspector] public bool stopGroundMovement = false;
	[HideInInspector] public bool stopHorizontalAirMovement = false;
	[HideInInspector] public bool stopDownwardsAirMovement = false;
	[HideInInspector] public bool stopUpwardsAirMovement = false;
	[HideInInspector] public bool preventJumping = false;
	[HideInInspector] public bool cancelAttackOnMovement = false;
	[HideInInspector] public bool cancelAttackOnJump = false;

	//Physics calculated variables 
	float groundAcceleration = 0;
	float groundDecceleration = 0;
	float airAcceleration = 0;
	float airDecceleration = 0;
	float jumpMaxVelocity = 0;
	float jumpMinVelocity = 0;
	float gravity = 0;

	//Physics user-set variables
	float maxGroundSpeed = 10;
	float groundAccelerationTime = 0.1f;
	float groundDeccelerationTime = 0.1f;
	float maxAirSpeed = 10;
	float airAccelerationTime = 0.2f;
	float airDeccelerationTime = 0.2f;

	float maxJumpHeight = 6;
	float minJumpHeight = 2;
	float timeToJumpApex = 0.3f;

    void Start() {
		rb = GetComponent<Rigidbody2D>();
		boxColl = GetComponent<BoxCollider2D>();
		attackComponent = GetComponent<Attack>();

		SetupPhysics();
    }

    void Update() {

		//Start off by saving the current velocity to local variables
		xVelocity = rb.velocity.x;
		yVelocity = rb.velocity.y;

		//Apply all movement rules
		//It might be neccesarry to have a priority system that determines what order the rules execute in
		//It is important that all rules only work with information that is not set by the other rules.
		//For example, when checking velocity, check rb.velocity and not xVelcoty/yVelocity
		HorizontalGroundAcceleration();
		HorizontalGroundDecceleration();
		HorizontalAirAcceleration();
		HorizontalAirDecceleration();
		Jump();
		Gravity();
		CancelAttackOnMoveInput();

		//Rules used by the Attack class to enforce the effects
		StopGroundMovement();
		StopUpwardsAirMovement();
		StopDownwardsAirMovement();
		StopHorizontalAirMovement();
	}

	#region Rules

	void CancelAttackOnMoveInput() {
		if (cancelAttackOnMovement) {
			if (Input.GetKey(leftKey) || Input.GetKey(rightKey)) {
				attackComponent.CancelCurrentAttack();
			}
		}
	}

	void StopHorizontalAirMovement() {
		if(stopHorizontalAirMovement && !onGround) {
			xVelocity = 0;		
		}
	}

	void StopDownwardsAirMovement() {
		if (stopDownwardsAirMovement && !onGround) {
			if(yVelocity < 0) {
				yVelocity = 0;
			}
		}
	}

	void StopUpwardsAirMovement() {
		if (stopUpwardsAirMovement && !onGround) {
			if (yVelocity > 0) {
				yVelocity = 0;
			}
		}
	}

	void StopGroundMovement() {
		if (stopGroundMovement && onGround) {
			xVelocity = 0;
		}
	}

	void Jump() {
		if (Input.GetKeyDown(jumpKey) && onGround && !preventJumping) {
			yVelocity += jumpMaxVelocity;

			if (cancelAttackOnJump) {
				attackComponent.CancelCurrentAttack();
			}
		}

		if (Input.GetKeyUp(jumpKey) && rb.velocity.y > jumpMinVelocity && !onGround) {
			yVelocity = jumpMinVelocity;
		}
	}

	void Gravity() {
		if (!onGround) {
			yVelocity -= gravity * Time.deltaTime;
		}
	}

	void HorizontalGroundAcceleration() {

		if (Input.GetKey(leftKey) && onGround) {
			xVelocity -= groundAcceleration * Time.deltaTime;
		}

		if (Input.GetKey(rightKey) && onGround) {
			xVelocity += groundAcceleration * Time.deltaTime;
		}

		xVelocity = Mathf.Clamp(xVelocity, -maxGroundSpeed, maxGroundSpeed);
	}

	void HorizontalGroundDecceleration() {

		if(!Input.GetKey(leftKey) && rb.velocity.x < 0 && onGround) {
			xVelocity += groundDecceleration * Time.deltaTime;
			xVelocity = Mathf.Clamp(xVelocity, -maxGroundSpeed, 0);
		}

		if (!Input.GetKey(rightKey) && rb.velocity.x > 0 && onGround) {
			xVelocity -= groundDecceleration * Time.deltaTime;
			xVelocity = Mathf.Clamp(xVelocity, 0, maxGroundSpeed);
		}
	}

	void HorizontalAirAcceleration() {

		if (Input.GetKey(leftKey) && !onGround) {
			xVelocity -= airAcceleration * Time.deltaTime;
		}

		if (Input.GetKey(rightKey) && !onGround) {
			xVelocity += airAcceleration * Time.deltaTime;
		}

		xVelocity = Mathf.Clamp(xVelocity, -maxAirSpeed, maxAirSpeed);
	}

	void HorizontalAirDecceleration() {

		if (!Input.GetKey(leftKey) && rb.velocity.x < 0 && !onGround) {
			xVelocity += airDecceleration * Time.deltaTime;
			xVelocity = Mathf.Clamp(xVelocity, -maxAirSpeed, 0);
		}

		if (!Input.GetKey(rightKey) && rb.velocity.x > 0 && !onGround) {
			xVelocity -= airDecceleration * Time.deltaTime;
			xVelocity = Mathf.Clamp(xVelocity, 0, maxAirSpeed);
		}
	}

	#endregion

	private void LateUpdate() {
		rb.velocity = new Vector2(xVelocity, yVelocity);
	}
	
	//Calculate acceleration and jump heights
	void SetupPhysics() {
		groundAcceleration = maxGroundSpeed / groundAccelerationTime;
		groundDecceleration = maxGroundSpeed / groundDeccelerationTime;

		airAcceleration = maxAirSpeed / airAccelerationTime;
		airDecceleration = maxAirSpeed / airDeccelerationTime;

		gravity = (2 * maxJumpHeight) / Mathf.Pow(timeToJumpApex, 2);
		jumpMaxVelocity = gravity * timeToJumpApex;
		jumpMinVelocity = Mathf.Sqrt(2 * gravity * minJumpHeight);
	}

	//Whenever we collide, check if we are touching the ground
	private void OnCollisionEnter2D(Collision2D collision) {
		CheckGroundCollision();
	}
	private void OnCollisionExit2D(Collision2D collision) {
		CheckGroundCollision();
	}

	private void CheckGroundCollision() {
		//Get the 10 first contacts of the box collider
		ContactPoint2D[] contacts = new ContactPoint2D[10];
		int count = boxColl.GetContacts(contacts);

		//If we find any horizontal surfaces, we are on the ground
		onGround = false;
		for (int i = 0; i < count; i++) {

			//If the angle between the normal and up is less than 5, we are on the ground
			if (Vector2.Angle(contacts[i].normal, Vector2.up) < 5.0f) {
				onGround = true;

				//Cancel attack on landing
				if (attackComponent.cancelAttackOnLanding) {
					attackComponent.CancelCurrentAttack();
				}

				//Restore air attack charges
				if (attackComponent.lockedAmountOfAttacksInTheAir) {
					attackComponent.RestoreAirAttackCharges();
				}

				//Since this is out of the update loop, we might have to modify rb.velocity directly 
				rb.velocity = new Vector2(rb.velocity.x, 0);
			}
		}
	}
}
