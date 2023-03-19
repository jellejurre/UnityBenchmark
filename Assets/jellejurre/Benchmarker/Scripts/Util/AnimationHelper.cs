using UnityEditor;
using UnityEngine;

public class AnimationHelper
{
	public static AnimationClip[] CreateTwoStateToggle(Transform root, Transform target)
	{
		AnimationClip onClip = new AnimationClip();
		AnimationClip offClip = new AnimationClip();
		onClip.name = target.name + "on";
		offClip.name = target.name + "off";
		EditorCurveBinding binding = new EditorCurveBinding();
		binding.path = AnimationUtility.CalculateTransformPath(target, root);
		binding.type = typeof(GameObject);
		binding.propertyName = "m_IsActive";
		AnimationCurve curveOn = AnimationCurve.Linear(0, 1, 1/60f, 1);
		AnimationCurve curveOff = AnimationCurve.Linear(0, 0, 1/60f, 0);
		AnimationUtility.SetEditorCurve(onClip, binding, curveOn);
		AnimationUtility.SetEditorCurve(offClip, binding, curveOff);
		return new[] { onClip, offClip };
	}
}