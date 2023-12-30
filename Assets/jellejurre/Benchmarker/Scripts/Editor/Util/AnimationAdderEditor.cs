using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

[CustomEditor(typeof(AnimationAdder))]
public class AnimationAdderEditor : Editor
{
	public override void OnInspectorGUI()
	{
		AnimationAdder adder = (AnimationAdder) target;
		if (GUILayout.Button("Set Animations"))
		{
			DoThing(adder);
		}
		DrawDefaultInspector();
	}
	
	public void DoThing(AnimationAdder adder)
	{
		AnimatorControllerLayer[] layers = adder.controller.layers;
		adder.index = 0;
		for (var i = 0; i < layers.Length; i++)
		{
			AnimatorControllerLayer l = layers[i];
			ChildAnimatorState[] states = l.stateMachine.states;
			for (var i1 = 0; i1 < states.Length; i1++)
			{
				states[i1].state.motion = AssignStates(states[i1].state.motion, adder);
			}

			l.stateMachine.states = states;
			layers[i] = l;
		}

		adder.controller.layers = layers;
	}

	public Motion AssignStates(Motion motion, AnimationAdder adder)
	{
		if (motion is BlendTree tree)
		{
			var treeChildren = tree.children;
			for (var i = 0; i < treeChildren.Length; i++)
			{
				treeChildren[i].motion = AssignStates(treeChildren[i].motion, adder);
			}

			tree.children = treeChildren;
			return tree;
		}

		adder.index++;
		return AnimationHelper.GetOrCreateTwoStateToggle("test"+adder.index, adder.index)[0];
	}
}