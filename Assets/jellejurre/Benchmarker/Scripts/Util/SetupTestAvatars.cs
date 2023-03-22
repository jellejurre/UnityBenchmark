
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using VRC.Dynamics;
using VRC.SDK3.Dynamics.Contact.Components;

public class SetupTestAvatars : MonoBehaviour
{
	public GameObject prefab;
	public int amount;

	public void SetupProximity()
	{
		GameObject gameObject = Instantiate(prefab);
		Animator animator = gameObject.GetOrAddComponent<Animator>();
		for (int i = 0; i < amount; i++)
		{
			int width = (int)Math.Sqrt(amount);
			int x = i % width;
			int y = (int)(i - x) / width;
			GameObject toggleObject = new GameObject("test"+ i.ToString());
			toggleObject.transform.parent = gameObject.transform;
			ContactReceiver constraint = toggleObject.AddComponent<VRCContactReceiver>();
			constraint.position = new Vector3((x - width/2)/10f, (y-width/2)/10f, 0);
			constraint.radius = 0.1f;
			constraint.parameter = i.ToString();
			constraint.collisionTags = new List<string>(){"Finger"};
		}
		AnimatorController controller = AnimatorHelpers.SetupTwoToggles(amount);
		animator.runtimeAnimatorController = controller;
		animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
	}
}
[CustomEditor(typeof(SetupTestAvatars))]
public class SetupTestAvatarsEditor : Editor
{

	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();
		if (target == null)
		{
			return;
		}

		SetupTestAvatars setup = (SetupTestAvatars) target;
		if (GUILayout.Button("Setup Proximity Controller"))
		{
			setup.SetupProximity();
		}
	}

}
