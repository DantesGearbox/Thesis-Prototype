using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(NewMovement))]
public class Attack : MonoBehaviour
{
	//Components
	NewMovement movementComponent;
	ChangeSpriteColor spriteComponent;
	Animator animatorComponent;
	public Transform swordPlacement;

	//Counters for frame data
	float startUpTimer = 0;
	float activeTimer = 0;
	float recoveryTimer = 0;

	//Constant time between frames
	const float FPS = 1.0f / 60.0f;

	//Controls
	[Header("Controls")]
	public KeyCode attackKey = KeyCode.X;
	public KeyCode upInput = KeyCode.UpArrow;
	public KeyCode downInput = KeyCode.DownArrow;

	//State of the attack
	bool isAttacking = false;
	bool inStartUp = false;
	bool inActive = false;
	bool inRecovery = false;

	//Weapon placements during attacks
	Vector2 weaponPlacement = new Vector2(1, 0.25f);
	Vector2 rightPlacement = new Vector2(1, 0.25f);
	Vector2 upPlacement = new Vector2(0, 1);
	Vector2 downPlacement = new Vector2(0, -1);
	Vector2 leftPlacement = new Vector2(-1, 0.25f);

	//Facing direction
	enum Directions { left, right, up, down };
	Directions facingDirection = Directions.right;
	Directions attackingDirection = Directions.right;

	//Weapon rotation during attack
	float weaponRotation = 0;
	float rightRotation = 0;
	float upRotation = 90;

	//Enums
	[System.Serializable] public enum Phases { startUp, active, recovery, startUpAndActive, activeAndRecovery, wholeAttack };
	[System.Serializable] public enum Effects { moveToCancelAttack, jumpToCancelAttack, stopGroundMovement, stopHorizontalAirMovement,
												stopUpwardsAirMovement, stopDownwardsAirMovement, preventJumping, attackBuffering };
	
	[Header("Frame Data")]
	public float startUpFrames = 32;
	public float activeFrames = 32;
	public float recoveryFrames = 32;

	[Header("Frame Timed Events")]
	public FrameTime[] frameTimes;

	[Header("Landing Behavior")]
	public bool cancelAttackOnLanding = false;
	public bool stopMovement = false; //TODO: DO I EVEN NEED THIS??? It might be a product of other rules

	[Header("Ground Turning Behavior")]
	public bool turnWhileGroundAttacking = false;

	[Header("Air Turning Behavior")]
	public bool turnWhileAirAttacking = false;

	[Header("Inherent Movement")]
	public float amountOfMovement = 0;
	public AnimationCurve movementDecreaseOverTime;
	public AnimationCurve animCuve = AnimationCurve.Linear(0, 1, 1, 0);

	[Header("Input Modifier")]
	public bool canAttackUpwardsOnGround = false;
	public bool canAttackUpwardsInAir = false;
	public bool canAttackDownwardsOnGround = false;
	public bool canAttackDownwardsInAir = false;

	[Header("Misc")]
	public float cooldownFramesBetweenAttacks = 0;
	float cooldownBetweenAttacksCounter = 0;
	bool attackOnCooldown = false;
	
	bool AttackBufferingActive = false; 
	bool attackBuffered = false;

	public bool lockedAmountOfAttacksInTheAir = false;
	public int attackChargesInAir = 1;
	int attacksInAir = 0;

	private void Start() {
		movementComponent = GetComponent<NewMovement>();
		spriteComponent = GetComponentInChildren<ChangeSpriteColor>();
		animatorComponent = GetComponentInChildren<Animator>();
	}

	//NOTE: It is important to note that when we do something for a phase, it keeps being true till the end.
	//	For example, if we disable movement on frame 5, it keeps being disabled until the end.

	private void DisplayFrameTimes() {
		for (int i = 0; i < frameTimes.Length; i++) {
			ref FrameTime frameTime = ref frameTimes[i];
			Debug.Log("FrameTime " + i + ": " + frameTime.activated + ", " + frameTime.phase + ", " + frameTime.effect);
		}
	}

	private void Update() {

		// What I want to do with a sword sprite:
		// We need a facing direction of either left or right
		// At the same time we need to detect whether or not extra directional input was given
		// Lastly we need 

		//Turning behavior feels super wierd! It would be nice to know how other games do it.
		//There might be a delay of when you press the movement button and when the attack direction actually changes.


		//If we're not attacking or if we can turn while attacking, change the current facing direction
		if (!isAttacking || (turnWhileAirAttacking && !movementComponent.onGround) || (turnWhileGroundAttacking && movementComponent.onGround)) {

			if (Input.GetKey(movementComponent.rightKey)) {
				swordPlacement.localPosition = rightPlacement;
				swordPlacement.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));
			}
			if (Input.GetKey(movementComponent.leftKey)) {
				swordPlacement.localPosition = leftPlacement;
				swordPlacement.localRotation = Quaternion.Euler(new Vector3(180, 0, 180));
			}
			if (Input.GetKey(upInput)) {
				if((movementComponent.onGround && canAttackUpwardsOnGround) || (!movementComponent.onGround && canAttackUpwardsInAir)) {
					swordPlacement.localPosition = upPlacement;
					swordPlacement.localRotation = Quaternion.Euler(new Vector3(0, 0, 90));
				}
			}
			if (Input.GetKey(downInput)) {
				if ((movementComponent.onGround && canAttackDownwardsOnGround) || (!movementComponent.onGround && canAttackDownwardsInAir)) {
					swordPlacement.localPosition = downPlacement;
					swordPlacement.localRotation = Quaternion.Euler(new Vector3(180, 0, 90));
				}
			}
		}

		if(isAttacking && movementComponent.onGround && !canAttackUpwardsOnGround && swordPlacement.localPosition.Equals(upPlacement)) {
			swordPlacement.localPosition = rightPlacement;
			swordPlacement.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));
		}
		if (isAttacking && movementComponent.onGround && !canAttackDownwardsOnGround && swordPlacement.localPosition.Equals(downPlacement)) {
			swordPlacement.localPosition = rightPlacement;
			swordPlacement.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));
		}
		if (isAttacking && !movementComponent.onGround && !canAttackUpwardsInAir && swordPlacement.localPosition.Equals(upPlacement)) {
			swordPlacement.localPosition = rightPlacement;
			swordPlacement.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));
		}
		if (isAttacking && !movementComponent.onGround && !canAttackDownwardsInAir && swordPlacement.localPosition.Equals(downPlacement)) {
			swordPlacement.localPosition = rightPlacement;
			swordPlacement.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));
		}

		//This is idiot code, but it fixes a bug
		//if (isAttacking && (swordPlacement.localPosition.Equals(upPlacement) || swordPlacement.localPosition.Equals(downPlacement))) {
		//	if (!canAttackDownwardsOnGround && movementComponent.onGround) {
		//		swordPlacement.localPosition = rightPlacement;
		//		swordPlacement.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));
		//	}

		//	if (!canAttackUpwardsOnGround && movementComponent.onGround) {
		//		swordPlacement.localPosition = rightPlacement;
		//		swordPlacement.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));
		//	}
		//}

		//Check if we have used all our air attack charges
		bool outOfAirAttackCharges = false;
		if (lockedAmountOfAttacksInTheAir && attacksInAir >= attackChargesInAir && !movementComponent.onGround) {
			outOfAirAttackCharges = true;
		}

		//Start the attack if the attack button is hit or if an attack is buffered
		if ((attackBuffered || Input.GetKeyDown(attackKey)) && !isAttacking && !attackOnCooldown && !outOfAirAttackCharges) {

			isAttacking = true;
			inStartUp = true;
			attackBuffered = false;

			if (!movementComponent.onGround) {
				attacksInAir++;
			}

			spriteComponent.ChangeToStartUpColor();
			animatorComponent.SetTrigger("StartUp");

			//Start of the attack. All "wholeAttack" effects should be started here.
			StartWholeAttackEffects();
		}

		//In start-up frames
		if (inStartUp && isAttacking) {

			//We are in the startup frames, check all "startUp" and "startUpAndActive" effects and enable those relevant.
			StartStartUpEffects(startUpTimer);

			startUpTimer += Time.deltaTime;
			if(startUpTimer > (startUpFrames * FPS)) {
				inStartUp = false;
				inActive = true;
				startUpTimer = 0;

				spriteComponent.ChangeToActiveColor();
				animatorComponent.SetTrigger("Active");

				//Start Up is over, end all "startUp" effects
				EndStartUpEffects();
			}
		}

		//In active frames
		if(inActive && isAttacking) {

			//We are in the active frames, check all "active" and "activeAndRecovery" effects and enable those relevant.
			StartActiveEffects(activeTimer);

			activeTimer += Time.deltaTime;
			if(activeTimer > (activeFrames * FPS)) {
				inActive = false;
				inRecovery = true;
				activeTimer = 0;

				spriteComponent.ChangeToRecoveryColor();
				animatorComponent.SetTrigger("Recovery");

				//Active is over, end all "active" and "startUpAndActive" effects
				EndActiveEffects();
			}
		}

		//In recovery frames, end of attack
		if(inRecovery && isAttacking) {

			//We are in the recovery frames, check all "recovery" effects and enable those relevant.
			StartRecoveryEffects(recoveryTimer);

			recoveryTimer += Time.deltaTime;
			if(recoveryTimer > (recoveryFrames * FPS)) {
				inRecovery = false;
				isAttacking = false;
				recoveryTimer = 0;
				attackOnCooldown = true;

				spriteComponent.ChangeToCooldownColor();
				animatorComponent.SetTrigger("Idle");

				//Recovery is over, end all "recovery", "activeAndRecovery" and "wholeAttack" effects
				EndRecoveryEffects();
			}
		}

		//Cooldown between attacks
		if(!isAttacking && attackOnCooldown) {

			cooldownBetweenAttacksCounter += Time.deltaTime;
			if(cooldownBetweenAttacksCounter > (cooldownFramesBetweenAttacks * FPS)) {
				attackOnCooldown = false;
				cooldownBetweenAttacksCounter = 0;
				spriteComponent.ChangeToDefaultColor();
			}
		}

		//Attack buffering
		if (isAttacking && AttackBufferingActive) {
			if (Input.GetKeyDown(attackKey)) {
				attackBuffered = true;
			}
		}

		EnforceEffects();
    }

	public void RestoreAirAttackCharges() {
		attacksInAir = 0;
	}

	public void CancelCurrentAttack() {
		if (isAttacking) {

			animatorComponent.SetTrigger("Reset");

			inStartUp = false;
			startUpTimer = 0;
			EndStartUpEffects();

			inActive = false;
			activeTimer = 0;
			EndActiveEffects();

			inRecovery = false;
			recoveryTimer = 0;
			EndRecoveryEffects();

			isAttacking = false;

			attackOnCooldown = true;
			spriteComponent.ChangeToCooldownColor();
		}
	}

	private void EnforceEffects() {
		
		for(int i = 0; i < frameTimes.Length; i++) {
			ref FrameTime frameTime = ref frameTimes[i];

			switch (frameTime.effect) {
				case Effects.moveToCancelAttack:
					if (frameTime.activated) {
						movementComponent.cancelAttackOnMovement = true;
					} else {
						movementComponent.cancelAttackOnMovement = false;
					}
					break;
				case Effects.jumpToCancelAttack:
					if (frameTime.activated) {
						movementComponent.cancelAttackOnJump = true;
					} else {
						movementComponent.cancelAttackOnJump = false;
					}
					break;
				case Effects.stopGroundMovement:
					if (frameTime.activated) {
						movementComponent.stopGroundMovement = true;
					} else {
						movementComponent.stopGroundMovement = false;
					}
					break;
				case Effects.stopHorizontalAirMovement:
					if (frameTime.activated) {
						movementComponent.stopHorizontalAirMovement = true;
					} else {
						movementComponent.stopHorizontalAirMovement = false;
					}
					break;
				case Effects.stopUpwardsAirMovement:
					if (frameTime.activated) {
						movementComponent.stopUpwardsAirMovement = true;
					} else {
						movementComponent.stopUpwardsAirMovement = false;
					}
					break;
				case Effects.stopDownwardsAirMovement:
					if (frameTime.activated) {
						movementComponent.stopDownwardsAirMovement = true;
					} else {
						movementComponent.stopDownwardsAirMovement = false;
					}
					break;
				case Effects.preventJumping:
					if (frameTimes[i].activated) {
						movementComponent.preventJumping = true;
					} else {
						movementComponent.preventJumping = false;
					}
					break;
				case Effects.attackBuffering:
					if (frameTimes[i].activated) {
						AttackBufferingActive = true;
					} else {
						AttackBufferingActive = false;
					}
					break;
				default:
					Debug.LogError("Could not recognize the effect type!");
					break;
			}
		}
	}

	#region StartAndEndEffects

	private void EndRecoveryEffects() {
		//Recovery is over, end all "recovery", "activeAndRecovery" and "wholeAttack" effects

		for (int i = 0; i < frameTimes.Length; i++) {
			ref FrameTime frameTime = ref frameTimes[i];
			if (frameTime.phase == Phases.recovery || frameTime.phase == Phases.activeAndRecovery || frameTime.phase == Phases.wholeAttack) {
				frameTime.activated = false;
			}
		}
	}

	private void StartRecoveryEffects(float timer) {
		//We are in the recovery frames, check all "recovery" effects and enable those relevant.

		for (int i = 0; i < frameTimes.Length; i++) {
			ref FrameTime frameTime = ref frameTimes[i];
			if (frameTime.phase == Phases.recovery) {
				if (timer > (frameTime.frame * FPS)) {
					frameTime.activated = true;
				}
			}
		}
	}

	private void EndActiveEffects() {
		//Active is over, end all "active" and "startUpAndActive" effects

		for (int i = 0; i < frameTimes.Length; i++) {
			ref FrameTime frameTime = ref frameTimes[i];
			if (frameTime.phase == Phases.active || frameTime.phase == Phases.startUpAndActive) {
				frameTime.activated = false;
			}
		}
	}

	private void StartActiveEffects(float timer) {
		//We are in the active frames, check all "active" and "activeAndRecovery" effects and enable those relevant.

		for (int i = 0; i < frameTimes.Length; i++) {
			ref FrameTime frameTime = ref frameTimes[i];
			if (frameTime.phase == Phases.active || frameTime.phase == Phases.activeAndRecovery) {
				if (timer > (frameTime.frame * FPS)) {
					frameTime.activated = true;
				}
			}
		}
	}

	private void EndStartUpEffects() {
		//Start Up is over, end all "startUp" effects

		for (int i = 0; i < frameTimes.Length; i++) {
			ref FrameTime frameTime = ref frameTimes[i];
			if (frameTime.phase == Phases.startUp) {
				frameTime.activated = false;
			}
		}
	}

	private void StartStartUpEffects(float timer) {
		//We are in the startup frames, check all "startUp" and "startUpAndActive" effects and enable those relevant.

		for (int i = 0; i < frameTimes.Length; i++) {
			ref FrameTime frameTime = ref frameTimes[i];
			if (frameTime.phase == Phases.startUp || frameTime.phase == Phases.startUpAndActive) {
				if (timer > (frameTime.frame * FPS)) {
					frameTime.activated = true;
				}
			}
		}
	}

	private void StartWholeAttackEffects() {
		//Start of the attack. All "wholeAttack" effects should be started here.

		for (int i = 0; i < frameTimes.Length; i++) {
			ref FrameTime frameTime = ref frameTimes[i];
			if (frameTime.phase == Phases.wholeAttack) {
				frameTime.activated = true;
			}
		}
	}

	#endregion

	[System.Serializable]
	public struct FrameTime {
		public Phases phase;
		public Effects effect;
		public float frame;

		public bool activated;
	}
}
