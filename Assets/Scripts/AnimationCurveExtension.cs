using UnityEngine;

public static class AnimationCurveExtension {

	//This function presupposed that the values of the animation curve are always rising
	public static float ReverseEvaluate(this AnimationCurve animCurve, float value) {

		float min = 0;
		float max = 1.0f;
		
		float currentTime = 0;
		float currentValue = 0;

		for(int i = 0; i < 100; i++) {
			currentTime = (min + max) / 2;
			currentValue = animCurve.Evaluate(currentTime);

			if (currentValue > value) {
				max = currentTime;
			}

			if (currentValue < value) {
				min = currentTime;
			}

			if (animCurve.Evaluate(currentTime) == value) {
				break;
			}
		}
		
		return currentTime;
	}
}
