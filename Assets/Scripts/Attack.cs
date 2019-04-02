using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
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
	public KeyCode atkKey = KeyCode.K;

	//For keeping track of the state of the attack
	public bool isAttacking = false;
	public bool inStartUp = false;
	public bool inActive = false;
	public bool inRecovery = false;


	private void Start() {
		//Convert the frames into time
		startUpTime = startUpFrames * FPS;
		activeTime = activeFrames * FPS;
		recoveryTime = recoveryFrames * FPS;
	}

	private void Update() {

		//Start the attack
		if (Input.GetKeyDown(atkKey) && !isAttacking) {

			isAttacking = true;
			inStartUp = true;
		}

		//In start-up frames
		if (inStartUp && isAttacking) {

			startUpTimer += Time.deltaTime;
			if(startUpTimer > startUpTime) {
				inStartUp = false;
				inActive = true;
				startUpTimer = 0;
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
			}
		}
    }
}
