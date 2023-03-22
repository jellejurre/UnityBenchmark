
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
	public bool senders;
	public bool receivers;
	public AnimationClip rotate;

	public void SetupProximity()
	{
		GameObject gameObject = Instantiate(prefab);
		Animator animator = gameObject.GetOrAddComponent<Animator>();
		if (receivers)
		{
			for (int i = 0; i < amount; i++)
			{
				int width = (int)Math.Sqrt(amount);
				int x = i % width;
				int y = (int)(i - x) / width;
				GameObject toggleObject = new GameObject("test" + i.ToString());
				toggleObject.transform.parent = gameObject.transform;
				VRCContactReceiver constraint = toggleObject.AddComponent<VRCContactReceiver>();
				constraint.position = new Vector3((x - width / 2) / 10f, (y - width / 2) / 10f, 0);
				constraint.radius = 0.1f;
				constraint.shapeType = ContactBase.ShapeType.Capsule;
				constraint.parameter = i.ToString();
				constraint.collisionTags = new List<string>() { "Finger" };
			}
		}

		if (senders)
		{
			for (int i = 0; i < amount; i++)
			{
				GameObject toggleObject = new GameObject("tast" + i.ToString());
				toggleObject.transform.parent = gameObject.transform;
				ContactSender sender = toggleObject.AddComponent<VRCContactSender>();
				sender.position = new Vector3(0, 0, i + 10);
				sender.collisionTags = new List<string>() { "Finger" };
			}
		}

		AnimatorController controller = new AnimatorController();
		AnimatorHelpers.AddParameters(controller, amount);
		AnimatorControllerLayer layer = new AnimatorControllerLayer();
		layer.name = "rotate";
		layer.defaultWeight = 1;
		AnimatorStateMachine stateMachine = new AnimatorStateMachine();
		AnimatorState state = new AnimatorState();
		AnimationClip offAnim = rotate;
		state.motion = offAnim;
		stateMachine.AddState(state, Vector3.one);
		layer.stateMachine = stateMachine;
		AnimatorControllerLayer[] layers = new []{layer};
		controller.layers = layers;
		AssetDatabase.CreateAsset(controller, $"Assets/{amount}.controller");
		AnimatorHelpers.SerializeController(controller);
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
