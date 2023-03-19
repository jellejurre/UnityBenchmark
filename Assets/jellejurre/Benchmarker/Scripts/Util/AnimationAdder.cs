using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

public class AnimationAdder : MonoBehaviour
{
	public AnimatorController controller;
	private int index;

	public void DoThing()
	{
		AnimatorControllerLayer[] layers = controller.layers;
		index = 0;
		for (var i = 0; i < layers.Length; i++)
		{
			AnimatorControllerLayer l = layers[i];
			ChildAnimatorState[] states = l.stateMachine.states;
			for (var i1 = 0; i1 < states.Length; i1++)
			{
				states[i1].state.motion = AssignStates(states[i1].state.motion);
			}

			l.stateMachine.states = states;
			layers[i] = l;
		}

		controller.layers = layers;
	}

	public Motion AssignStates(Motion motion)
	{
		if (motion is BlendTree tree)
		{
			var treeChildren = tree.children;
			for (var i = 0; i < treeChildren.Length; i++)
			{
				treeChildren[i].motion = AssignStates(treeChildren[i].motion);
			}

			tree.children = treeChildren;
			return tree;
		}

		index++;
		return AnimationHelper.GetOrCreateTwoStateToggle("test"+index, index)[0];
	}
}

[CustomEditor(typeof(AnimationAdder))]
public class AnimationAdderEditor : Editor
{
	public override void OnInspectorGUI()
	{
		AnimationAdder adder = (AnimationAdder) target;
		if (GUILayout.Button("Set Animations"))
		{
			adder.DoThing();
		}
		DrawDefaultInspector();
	}
}