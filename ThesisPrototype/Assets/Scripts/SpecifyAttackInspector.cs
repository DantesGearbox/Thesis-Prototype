using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

//TODO: UNCOMMENT THE LINE BELOW TO ACTIVATE THE INSPECTOR
//[CustomEditor(typeof(SpecifyAttack))]
public class SpecifyAttackInspector : Editor {

	//Frame Data
	bool frameDataGroup = true;
	SerializedProperty startUpFramesProp;
	SerializedProperty activeFramesProp;
	SerializedProperty recoveryFramesProp;

	//Landing Behavior
	bool landingBehaviorGroup = true;
	SerializedProperty cancelAttackOnLandingProp;
	SerializedProperty stopMovementProp;

	//Move Cancel
	bool moveToCancelGroup = true;

	//Jump Cancel
	bool jumpToCancelGroup = true;

	//Ground Movemenet Behavior
	bool groundMovementBehaviorGroup = true;

	//Air X Movement Behavior
	bool airXMovementBehaviorGroup = true;

	//Air Y Movement Behavior
	bool airYMovementBehaviorGroup = true;

	//Inherent Movement
	bool inherentMovement = true;

	//Input Modifier
	bool inputModifier = true;

	//Misc
	bool miscGroup = true;

	void OnEnable() {
		startUpFramesProp = serializedObject.FindProperty("startUpFrames");
		activeFramesProp = serializedObject.FindProperty("activeFrames");
		recoveryFramesProp = serializedObject.FindProperty("recoveryFrames");
		cancelAttackOnLandingProp = serializedObject.FindProperty("cancelAttackOnLanding");
		stopMovementProp = serializedObject.FindProperty("stopMovement");
	}

	public override void OnInspectorGUI() {
		//base.OnInspectorGUI();
		serializedObject.Update();

		//Making the fouldout titles bold
		EditorStyles.foldout.fontStyle = FontStyle.Bold;

		//Frame data group
		frameDataGroup = EditorGUILayout.Foldout(frameDataGroup, "Frame Data");
		if(frameDataGroup) {
			EditorGUILayout.PropertyField(startUpFramesProp, new GUIContent("Start-up Frames"));
			EditorGUILayout.PropertyField(activeFramesProp, new GUIContent("Active Frames"));
			EditorGUILayout.PropertyField(recoveryFramesProp, new GUIContent("Recovery Frames"));
		}

		//Seperator between groups
		EditorGUILayout.Separator();

		//Landing behavior group
		landingBehaviorGroup = EditorGUILayout.Foldout(landingBehaviorGroup, "Landing Behavior");
		if (landingBehaviorGroup) {
			EditorGUILayout.PropertyField(cancelAttackOnLandingProp, new GUIContent("Cancel Attack On Landing"));
			EditorGUILayout.PropertyField(stopMovementProp, new GUIContent("Stop Movement On Landing"));
		}


		serializedObject.ApplyModifiedProperties();
	}
}
