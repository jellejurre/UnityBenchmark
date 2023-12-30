using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

public class ManyStatesDisconnectedBenchmark : BenchmarkTask1d
{
	public override GameObject PrepareIteration1d(GameObject prefab, int stateCount)
	{
		GameObject gameObject = Instantiate(prefab);
		Animator animator = gameObject.GetOrAddComponent<Animator>();
		GameObject toggleObject = new GameObject("test0");
		toggleObject.transform.parent = gameObject.transform;
		AnimatorController controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(AnimatorHelpers.controllerPath + $"ManyStatesDisconnected/{stateCount}.controller");

		if (controller != null)
		{
			AnimatorHelpers.RandomiseParameters(controller);
			animator.runtimeAnimatorController = controller;
			animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
			AssetDatabase.SaveAssets();
			return gameObject;
		}
		AssetDatabase.StartAssetEditing();

		controller = new AnimatorController();
		AnimatorControllerLayer[] layers = new AnimatorControllerLayer[1];
		AnimatorHelpers.AddParameters(controller, 1);
		string index = 0.ToString();
		AnimatorControllerLayer layer = new AnimatorControllerLayer();
		layer.name = index;
		AnimatorStateMachine stateMachine = new AnimatorStateMachine();
		layer.stateMachine = stateMachine;
		AnimationClip[] anims = AnimationHelper.GetOrCreateTwoStateToggle("test"+index.ToString(), 0);
		AnimatorState firstState = new AnimatorState();
		firstState.name = "state1";
		firstState.writeDefaultValues = false;
		firstState.motion = anims[0];
		ChildAnimatorState[] states = new ChildAnimatorState[stateCount + 1];
		states[0] = new ChildAnimatorState() { state = firstState, position = Vector3.one };
		for (int i = 0; i < stateCount; i++)
		{
			AnimatorState secondState = new AnimatorState();
			secondState.name = "state" + i;
			secondState.writeDefaultValues = false;
			secondState.motion = anims[1];
			states[i+1] = new ChildAnimatorState() { state = secondState, position = Vector3.one };
		}
		layer.stateMachine.states = states;
		layer.defaultWeight = 1;
		layers[0] = layer;
		controller.layers = layers;
		AssetDatabase.CreateAsset(controller, AnimatorHelpers.controllerPath + $"ManyStatesDisconnected/{stateCount}.controller");
		foreach (var animatorControllerLayer in controller.layers)
		{
			animatorControllerLayer.stateMachine.hideFlags = HideFlags.HideInHierarchy;
			AssetDatabase.AddObjectToAsset(animatorControllerLayer.stateMachine, controller);
			foreach (var childAnimatorState in animatorControllerLayer.stateMachine.states)
			{
				childAnimatorState.state.hideFlags = HideFlags.HideInHierarchy;
				AssetDatabase.AddObjectToAsset(childAnimatorState.state, controller);
			}
		}
		AssetDatabase.StopAssetEditing();
		AnimatorHelpers.RandomiseParameters(controller);
		animator.runtimeAnimatorController = controller;
		animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
		AssetDatabase.SaveAssets();
		return gameObject;
	}
		
	public override string GetName()
	{
		return "StatesNoTransitions";
	}

	public override string GetDescription()
	{
		return "Benchmarks many states with no transitions";
	}

	public override string GetParameterName()
	{
		return "States";
	}
}