using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(NewMovement), typeof(SpecifyAttack))]
public class Attack : MonoBehaviour
{
	//Components
	NewMovement movementComponent;
	SpecifyAttack attackData;

	//Frame data
	private float startUpFrames = 32;
	private float activeFrames = 32;
	private float recoveryFrames = 32;

	//Time holders
	private float startUpTime = 0;
	private float activeTime = 0;
	private float recoveryTime = 0;

	//Counters for frame data
	private float startUpTimer = 0;
	private float activeTimer = 0;
	private float recoveryTimer = 0;

	//Constant time between frames
	private const float FPS = 1.0f / 60.0f;

	//Controls
	public KeyCode attackKey = KeyCode.X;

	//State of the attack
	bool isAttacking = false;
	bool inStartUp = false;
	bool inActive = false;
	bool inRecovery = false;

	private void Start() {
		movementComponent = GetComponent<NewMovement>();
		attackData = GetComponent<SpecifyAttack>();

		//Pull all frame data from attack data object
		//The rest of the data will be pulled as needed instead of saving it
		PullFrameData();

		//Convert the frames into time
		startUpTime = startUpFrames * FPS;
		activeTime = activeFrames * FPS;
		recoveryTime = recoveryFrames * FPS;
	}

	private void PullFrameData() {
		startUpFrames = attackData.startUpFrames;
		activeFrames = attackData.activeFrames;
		recoveryFrames = attackData.recoveryFrames;
	}

	//TODO: Currently, we enforce the specific effects with some bad code. We can probably make this more flexible and have less code reu-se.
	//NOTE: It is important to note that when we do something for a period, it keeps being true till the end.
	//	For example, if we disable movement on frame 5, it keeps being disabled until the end.

	private void Update() {

		//Start the attack
		if (Input.GetKeyDown(attackKey) && !isAttacking) {
			isAttacking = true;
			inStartUp = true;

			//The attack has started, disable jumping
			if (attackData.preventJumpingWhileAttacking) {
				movementComponent.preventJumping = true;
			}
		}

		//In start-up frames
		if (inStartUp && isAttacking) {

			//We are in the set start-up frames, stop movement
			if(attackData.stopMovementInStartup && startUpTimer > (attackData.frameToStopGroundMovement * FPS)){
				movementComponent.stopGroundMovement = true;
			}

			startUpTimer += Time.deltaTime;
			if(startUpTimer > startUpTime) {
				inStartUp = false;
				inActive = true;
				startUpTimer = 0;

				//Start-up is over, reenable movement
				if (attackData.stopMovementInStartup) {
					movementComponent.stopGroundMovement = false;
				}
			}
		}

		//In active frames
		if(inActive && isAttacking) {

			activeTimer += Time.deltaTime;
			if(activeTimer > activeTime) {
				inActive = false;
				inRecovery = true;
				activeTimer = 0;
			}
		}

		//In recovery frames, end of attack
		if(inRecovery && isAttacking) {

			recoveryTimer += Time.deltaTime;
			if(recoveryTimer > recoveryTime) {
				inRecovery = false;
				isAttacking = false;
				recoveryTimer = 0;

				//The attack is over, reenable jumping
				if (attackData.preventJumpingWhileAttacking) {
					movementComponent.preventJumping = false;
				}
			}
		}
    }
}
