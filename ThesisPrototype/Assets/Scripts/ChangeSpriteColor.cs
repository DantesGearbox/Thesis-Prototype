using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeSpriteColor : MonoBehaviour {

	SpriteRenderer sr;
	Color defaultColor;
	[SerializeField] Color startUpColor;
	[SerializeField] Color activeColor;
	[SerializeField] Color recoveryColor;
	[SerializeField] Color cooldownColor;

	void Start() {
		sr = GetComponent<SpriteRenderer>();
		defaultColor = sr.color;
    }

	public void ChangeToStartUpColor() {
		sr.color = startUpColor;
	}

	public void ChangeToActiveColor() {
		sr.color = activeColor;
	}

	public void ChangeToRecoveryColor() {
		sr.color = recoveryColor;
	}

	public void ChangeToCooldownColor() {
		sr.color = cooldownColor;
	}

	public void ChangeToDefaultColor() {
		sr.color = defaultColor;
	}
}
