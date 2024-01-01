using System.Linq;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

[CustomEditor(typeof(ControllerValidator))]
public class ControllerValidatorEditor : Editor
{

	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();
		if (target == null)
		{
			return;
		}

		ControllerValidator validator = (ControllerValidator) target;
		if (GUILayout.Button("Validate Controllers"))
		{
			Validate(validator.controllerPath);
		}
	}
	
	public void Validate(string controllerPath)
	{
		string[] paths = AssetDatabase.FindAssets("*", new[] { controllerPath });
		AssetDatabase.StartAssetEditing();
		(string, AnimatorController)[] controllerpairs = paths
			.Select(AssetDatabase.GUIDToAssetPath)
			.Select(x =>(x, (AssetDatabase.LoadAssetAtPath<AnimatorController>(x))))
			.Where(x => (x.Item2 != null)).ToArray();
		foreach (var pair in controllerpairs)
		{
			foreach (var animatorControllerLayer in pair.Item2.layers)
			{
				if (animatorControllerLayer.stateMachine == null)
				{
					Debug.LogError("Deleting controller at " +pair.Item1); 
					AssetDatabase.DeleteAsset(pair.Item1);
					break;
				}
			}
		}
		AssetDatabase.StopAssetEditing();
		Debug.Log(controllerpairs.Length + " controllers validated.");
	}

}