using UnityEditor;
using UnityEngine;

public class AnimationHelper
{
	private static string animationPath = "Assets/jellejurre/Benchmarker/Assets/Generated/Animations/";
	public static AnimationClip[] GetOrCreateTwoStateToggle(string path, int i)
	{
		AnimationClip clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(animationPath + "GameObjectOn" + i + ".anim");
		AnimationClip clip2 = AssetDatabase.LoadAssetAtPath<AnimationClip>(animationPath+ "GameObjectOff" + i + ".anim");
		if (clip != null && clip2 != null)
		{
			return new[] { clip, clip2 };
		}

		AnimationClip onClip = new AnimationClip();
		AnimationClip offClip = new AnimationClip();
		onClip.name = "GameObjectOn" + i;
		offClip.name = "GameObjectOff" + i;
		EditorCurveBinding binding = new EditorCurveBinding();
		binding.path = path;
		binding.type = typeof(GameObject);
		binding.propertyName = "m_IsActive";
		AnimationCurve curveOn = AnimationCurve.Linear(0, 1, 1/60f, 1);
		AnimationCurve curveOff = AnimationCurve.Linear(0, 0, 1/60f, 0);
		AnimationUtility.SetEditorCurve(onClip, binding, curveOn);
		AnimationUtility.SetEditorCurve(offClip, binding, curveOff);
		AssetDatabase.CreateAsset(onClip, animationPath + "GameObjectOn" + i + ".anim");
		AssetDatabase.CreateAsset(offClip, animationPath + "GameObjectOff" + i + ".anim");
		return new[] { onClip, offClip };
	}
	
	public static AnimationClip GetOrCreateBigOnToggle(string path, int i)
	{
		AnimationClip clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(animationPath + "GameObjectsOn" + i + ".anim");
		if (clip != null)
		{
			return clip;
		}

		AnimationClip onClip = new AnimationClip();
		onClip.name = "GameObjectOn" + i;
		for (int j = 0; j < i; j++)
		{
			EditorCurveBinding binding = new EditorCurveBinding();
			binding.path = path + j;
			binding.type = typeof(GameObject);
			binding.propertyName = "m_IsActive";
			AnimationCurve curveOn = AnimationCurve.Linear(0, 1, 1/60f, 1);
			AnimationUtility.SetEditorCurve(onClip, binding, curveOn);
		}

		AssetDatabase.CreateAsset(onClip, animationPath + "GameObjectsOn" + i + ".anim");
		return onClip;
	}
	
	public static AnimationClip GetOrCreateOneStateToggle(string path, int i)
	{
		AnimationClip clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(animationPath+ "GameObjectOff" + i + ".anim");
		if (clip != null)
		{
			return clip;
		}

		AnimationClip offClip = new AnimationClip();
		offClip.name = "GameObjectOff" + i;
		EditorCurveBinding binding = new EditorCurveBinding();
		binding.path = path;
		binding.type = typeof(GameObject);
		binding.propertyName = "m_IsActive";
		AnimationCurve curveOff = AnimationCurve.Linear(0, 0, 1/60f, 0);
		AnimationUtility.SetEditorCurve(offClip, binding, curveOff);
		AssetDatabase.CreateAsset(offClip, animationPath + "GameObjectOff" + i + ".anim");
		return offClip;
	}
	
	
}