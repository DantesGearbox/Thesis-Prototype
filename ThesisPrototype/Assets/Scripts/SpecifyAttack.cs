using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecifyAttack : MonoBehaviour {

	/*  ---TYPOLOGY---
		Start-up, Active, Recovery frames - Classic fighting game style concepts
		Landing behavior - What happens when we land mid-attack
		Move cancel - What happens when we move while attacking
		Jump cancel - What happens when we jump while attacking
		Ground movement momentum - What happens when we attack while moving
		Turning behavior - What happens when you attack while/before turning
		Air behavior - What happens when you attack while in the air
		Air turning behavior - What happens when you attack in the air while/before turning
		Inherent movement - The movement that is part of the attack
		Input modifier - Can you modify the attack by doing additional input
		(Cooldown without recovery frames - Time between attacks after recovery frames, SFV?)
		(Attack buffering? SFV, Salt)
		(Attack multiple times during a jump? SFV)
		(Hitboxes, SFV, SkullGirls)
		(Attack comboing, Salt, SFV)
	 */

	[Header("Frame Data")]
	public float startUpFrames = 10;
	public float activeFrames = 10;
	public float recoveryFrames = 10;

	[Header("Landing Behavior")]
	public bool cancelAttackOnLanding = false;
	public bool stopMovement = false;

	[Header("Move To Cancel Attack")]
	public bool moveCancelDuringStartup = false;
	public bool moveCancelDuringActive = false;
	public bool moveCancelDuringRecovery = false;
	public float frameToCancelMove = 0;

	[Header("Jump To Cancel Attack")]
	public bool jumpCancelDuringStartup = false;
	public bool jumpCancelDuringActive = false;
	public bool jumpCancelDuringRecovery = false;
	public float frameToCancelJump = 0;

	[Header("Ground Movement Behavior")]
	public bool stopMovementInStartup = true;
	public bool stopMovementInActive = false;
	public bool stopMovementInRecovery = false;
	public float frameToStopGroundMovement = 5;

	[Header("Air X Movement Behavior")]
	public bool stopXMovementInStartup = false;
	public bool stopXMovementInActive = false;
	public bool stopXMovementInRecovery = false;
	public float frameToStopXAirMovement = 0;

	[Header("Air Y Movement Behavior")]
	public bool stopYMovementInStartup = false;
	public bool stopYMovementInActive = false;
	public bool stopYMovementInRecovery = false;
	public float frameToStopYAirMovement = 0;

	[Header("Ground Turning Behavior")]
	public bool turnWhileGroundAttacking = false;

	[Header("Air Turning Behavior")]
	public bool turnWhileAirAttacking = false;

	[Header("Inherent Movement")]
	public float amountOfMovement = 0;
	public AnimationCurve movementDecreaseOverTime;

	[Header("Input Modifier")]
	public bool canAttackUpwardsOnGround = false;
	public bool canAttackUpwardsInAir = false;
	public bool canAttackDownwardsOnGround = false;
	public bool canAttackDownwardsInAir = false;

	[Header("Misc")]
	public float cooldownFramesBetweenAttacks = 0;
	public bool preventJumpingWhileAttacking = true;
	public bool attackBuffering = false;
	public bool multipleAttacksInAJump = false;
}
