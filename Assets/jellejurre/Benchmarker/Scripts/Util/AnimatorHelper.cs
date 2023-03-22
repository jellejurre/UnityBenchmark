using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.Windows;
using AnimatorController = UnityEditor.Animations.AnimatorController;
using AnimatorControllerLayer = UnityEditor.Animations.AnimatorControllerLayer;

public class AnimatorHelpers
{
	public static string controllerPath = "Assets/jellejurre/Benchmarker/Assets/Generated/Controllers/";

	public static void ReadyPath(string folderPath)
	{
		folderPath = folderPath.Substring(0 , folderPath.Length - 1);
		if (Directory.Exists(folderPath)) return;
		Directory.CreateDirectory(controllerPath);
		Directory.CreateDirectory(folderPath);
		AssetDatabase.ImportAsset(folderPath);
	}
	
	#region AnyState
	public static AnimatorController SetupAnyStateToggle(int layerCount, bool canTransitionToSelf = false, bool writeDefaults = true)
	{
		string path = writeDefaults ? canTransitionToSelf ? "AnyStateSelf/" : "AnyState/" : "AnyStateWDOff/";
		ReadyPath(controllerPath + path);
		AnimatorController controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(controllerPath + path + layerCount + ".controller");

		if (controller != null)
		{
			RandomiseParameters(controller);
			return controller;
		}
		AssetDatabase.StartAssetEditing();

		controller = new AnimatorController();
		AnimatorControllerLayer[] layers = new AnimatorControllerLayer[layerCount];
		AnimatorStateTransition[] transitions = new AnimatorStateTransition[layers.Length * 2];
		AddParameters(controller, layerCount);
		for (int j = 0; j < layers.Length; j++)
		{
			string index = j.ToString();
			AnimatorControllerLayer layer = new AnimatorControllerLayer();
			layer.name = index;
			AnimatorStateMachine stateMachine = new AnimatorStateMachine();
			AnimatorState onState = new AnimatorState();
			AnimatorState offState = new AnimatorState();
			onState.writeDefaultValues = writeDefaults;
			offState.writeDefaultValues = writeDefaults;
			onState.name = index + "on";
			offState.name = index + "off";
			AnimationClip[] anims = AnimationHelper.GetOrCreateTwoStateToggle("test"+j.ToString(), j);
			onState.motion = anims[0];
			offState.motion = anims[1];
			stateMachine.AddState(onState, Vector3.one);
			stateMachine.AddState(offState, Vector3.one);
			AnimatorStateTransition onToOffTransition = stateMachine.AddAnyStateTransition(offState);
			AnimatorStateTransition offToOnTransition = stateMachine.AddAnyStateTransition(onState);
			transitions[j * 2] = onToOffTransition;
			transitions[j * 2 + 1] = offToOnTransition;
			onToOffTransition.exitTime = 1;
			offToOnTransition.exitTime = 1;
			onToOffTransition.hasExitTime = true;
			offToOnTransition.hasExitTime = true;
			onToOffTransition.destinationState = offState;
			offToOnTransition.destinationState = onState;
			onToOffTransition.canTransitionToSelf = canTransitionToSelf;
			offToOnTransition.canTransitionToSelf = canTransitionToSelf;
			onToOffTransition.AddCondition(AnimatorConditionMode.Greater, 0.5f, j.ToString());
			offToOnTransition.AddCondition(AnimatorConditionMode.Less, 0.5f, j.ToString());
			layer.stateMachine = stateMachine;
			layer.defaultWeight = 1;
			layers[j] = layer;
		}
		controller.layers = layers;
		AssetDatabase.CreateAsset(controller, controllerPath + path + layerCount + ".controller");
		SerializeController(controller);
		AssetDatabase.StopAssetEditing();
		RandomiseParameters(controller);
		return controller;
	}
	
	public static AnimatorController SetupAnyStateToggle(int layerCount, int stateCount, bool canTransitionToSelf = false)
	{
		string path = canTransitionToSelf ? "AnyStateSelf/" : "AnyState/";
		ReadyPath(controllerPath + path);
		AnimatorController controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(controllerPath + $"{path}/{layerCount}_{stateCount}.controller");

		if (controller != null)
		{
			RandomiseParameters(controller);
			return controller;
		}
		AssetDatabase.StartAssetEditing();

		controller = new AnimatorController();
		AnimatorControllerLayer[] layers = new AnimatorControllerLayer[layerCount];
		AddParameters(controller, layerCount);
		for (int j = 0; j < layers.Length; j++)
		{
			string index = j.ToString();
			AnimatorControllerLayer layer = new AnimatorControllerLayer();
			layer.name = index;
			AnimatorStateMachine stateMachine = new AnimatorStateMachine();
			layer.stateMachine = stateMachine;
			AnimationClip[] anims = AnimationHelper.GetOrCreateTwoStateToggle("test"+j.ToString(), j);
			AnimatorStateTransition[] anyStates = new AnimatorStateTransition[stateCount];
			ChildAnimatorState[] states = new ChildAnimatorState[stateCount];
			for (int i = 0; i < stateCount; i++)
			{
				AnimatorState secondState = new AnimatorState();
				secondState.name = "state" + i;
				secondState.writeDefaultValues = true;
				secondState.motion = anims[i%2];
				var onTransition = new AnimatorStateTransition();
				onTransition.conditions = new[]
				{
					new AnimatorCondition()
					{
						mode = AnimatorConditionMode.Greater,
						threshold = i / ((float)stateCount),
						parameter = j.ToString()
					},
					new AnimatorCondition()
					{
						mode = AnimatorConditionMode.Less,
						threshold = i+1 / ((float)stateCount),
						parameter = j.ToString()
					}
				};
				onTransition.destinationState = secondState;
				anyStates[i] = onTransition;
				states[i] = new ChildAnimatorState(){state = secondState, position = Vector3.one};
				onTransition.canTransitionToSelf = canTransitionToSelf;
			}
			layer.stateMachine.anyStateTransitions = anyStates;
			layer.stateMachine.states = states;
			layer.defaultWeight = 1;
			layers[j] = layer;
		}
		controller.layers = layers;
		AssetDatabase.CreateAsset(controller, controllerPath + $"{path}{layerCount}_{stateCount}.controller");
		SerializeController(controller);
		AssetDatabase.StopAssetEditing();
		RandomiseParameters(controller);
		return controller;
	}
	
		public static AnimatorController SetupAnyStateEdgeCase(int stateCount)
	{
		string path = "AnyStateEC/";
		ReadyPath(controllerPath + path);
		AnimatorController controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(controllerPath + $"{path}/{stateCount}.controller");

		if (controller != null)
		{
			RandomiseParameters(controller);
			return controller;
		}
		AssetDatabase.StartAssetEditing();

		controller = new AnimatorController();
		AnimatorControllerLayer[] layers = new AnimatorControllerLayer[1];
		AddParameters(controller, stateCount, true);
		AnimatorControllerLayer layer = new AnimatorControllerLayer();
		layer.name = 1.ToString();
		AnimatorStateMachine stateMachine = new AnimatorStateMachine();
		layer.stateMachine = stateMachine;
		AnimatorStateTransition[] anyStates = new AnimatorStateTransition[stateCount];
		ChildAnimatorState[] states = new ChildAnimatorState[stateCount];
		for (int i = 0; i < stateCount; i++)
		{
			AnimationClip[] anims = AnimationHelper.GetOrCreateTwoStateToggle("test"+i.ToString(), i);
			AnimatorState secondState = new AnimatorState();
			secondState.name = "state" + i;
			secondState.writeDefaultValues = true;
			secondState.motion = anims[i%2];
			var onTransition = new AnimatorStateTransition();
			onTransition.conditions = new[]
			{
				new AnimatorCondition()
				{
					mode = AnimatorConditionMode.If,
					parameter = i.ToString()
				}
			};
			onTransition.hasExitTime = false;
			onTransition.duration = 0;
			onTransition.destinationState = secondState;
			anyStates[i] = onTransition;
			states[i] = new ChildAnimatorState(){state = secondState, position = Vector3.one};
			onTransition.canTransitionToSelf = false;
		}
		layer.stateMachine.anyStateTransitions = anyStates;
		layer.stateMachine.states = states;
		layer.defaultWeight = 1;
		layers[0] = layer;
		controller.layers = layers;
		AssetDatabase.CreateAsset(controller, controllerPath + $"{path}{stateCount}.controller");
		SerializeController(controller);
		AssetDatabase.StopAssetEditing();
		RandomiseParameters(controller);
		return controller;
	}
	
	#endregion

	#region nonAnyState

	public static AnimatorController SetupManyStateToggle(int layerCount, int stateCount)
	{
		ReadyPath(controllerPath + "ManyState/");
		AnimatorController controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(controllerPath + $"ManyState/{layerCount}_{stateCount}.controller");

		if (controller != null)
		{
			RandomiseParameters(controller);
			return controller;
		}
		AssetDatabase.StartAssetEditing();

		controller = new AnimatorController();
		AnimatorControllerLayer[] layers = new AnimatorControllerLayer[layerCount];
		AnimatorStateTransition[] transitions = new AnimatorStateTransition[layers.Length * stateCount * 3];
		AddParameters(controller, layerCount);
		for (int j = 0; j < layers.Length; j++)
		{
			string index = j.ToString();
			AnimatorControllerLayer layer = new AnimatorControllerLayer();
			layer.name = index;
			AnimatorStateMachine stateMachine = new AnimatorStateMachine();
			layer.stateMachine = stateMachine;
			AnimationClip[] anims = AnimationHelper.GetOrCreateTwoStateToggle("test"+j.ToString(), j);
			AnimatorState firstState = new AnimatorState();
			firstState.name = "state1";
			firstState.writeDefaultValues = false;
			firstState.motion = anims[0];
			ChildAnimatorState[] states = new ChildAnimatorState[stateCount + 1];
			for (int i = 0; i < stateCount; i++)
			{
				AnimatorState secondState = new AnimatorState();
				secondState.name = "state" + i;
				secondState.writeDefaultValues = false;
				secondState.motion = anims[1];
				AnimatorStateTransition onTransition = new AnimatorStateTransition();
				onTransition.destinationState = secondState;
				firstState.transitions = new[] { onTransition };
				AnimatorStateTransition offTransition = new AnimatorStateTransition();
				AnimatorStateTransition off2Transition = new AnimatorStateTransition();
				offTransition.destinationState = firstState;
				off2Transition.destinationState = firstState;
				secondState.transitions = new[] { off2Transition, offTransition };
				transitions[3 * (j * stateCount + i)] = onTransition;
				transitions[3 * (j * stateCount + i) + 1] = offTransition;
				transitions[3 * (j * stateCount + i) + 2] = off2Transition;
				onTransition.conditions = new[]
				{
					new AnimatorCondition()
					{
						mode = AnimatorConditionMode.Greater, threshold = i / ((float)stateCount),
						parameter = j.ToString()
					},
					new AnimatorCondition()
					{
						mode = AnimatorConditionMode.Less, threshold = (1+i) / ((float)stateCount),
						parameter = j.ToString()
					}
				};
				offTransition.conditions = new[]
				{
					new AnimatorCondition()
					{
						mode = AnimatorConditionMode.Less, threshold = i / ((float)stateCount), parameter = j.ToString()
					}
				};
				off2Transition.conditions = new[]
				{
					new AnimatorCondition()
					{
						mode = AnimatorConditionMode.Greater, threshold = (i + 1) / (float)stateCount,
						parameter = j.ToString()
					}
				};
				states[i] = new ChildAnimatorState(){state = secondState, position = Vector3.one};
			}

			states[stateCount] =  new ChildAnimatorState(){state = firstState, position = Vector3.one};
			layer.stateMachine.states = states;
			layer.defaultWeight = 1;
			layers[j] = layer;
		}
		controller.layers = layers;
		AssetDatabase.CreateAsset(controller,  controllerPath + $"ManyState/{layerCount}_{stateCount}.controller");
		SerializeController(controller);
		AssetDatabase.StopAssetEditing();
		RandomiseParameters(controller);
		return controller;
	}

	public static AnimatorController SetupTwoToggles(int layerCount, bool writeDefaults = true, bool b = false)
	{
		string path = b ? "TwoToggleBool/" : writeDefaults ? "TwoToggle/" : "TwoToggleWDOff/";
		ReadyPath(controllerPath + path);
		AnimatorController controller = 
			AssetDatabase.LoadAssetAtPath<AnimatorController>(controllerPath + path + layerCount + ".controller");

		if (controller != null)
		{
			RandomiseParameters(controller);
			return controller;
		}
		
		AssetDatabase.StartAssetEditing();
		controller = new AnimatorController();
		AnimatorControllerLayer[] layers = new AnimatorControllerLayer[layerCount];
		AddParameters(controller, layerCount, b);
		AnimatorStateTransition[] transitions = new AnimatorStateTransition[layers.Length * 2];
		for (int j = 0; j < layers.Length; j++)
		{
			string index = j.ToString();
			AnimatorControllerLayer layer = new AnimatorControllerLayer();
			layer.name = index;
			AnimatorStateMachine stateMachine = new AnimatorStateMachine();
			AnimatorState onState = new AnimatorState();
			AnimatorState offState = new AnimatorState();
			onState.name = index + "on";
			offState.name = index + "off";
			AnimationClip[] anims = AnimationHelper.GetOrCreateTwoStateToggle("test"+j.ToString(), j);
			onState.motion = anims[0];
			offState.motion = anims[1];
			onState.writeDefaultValues = writeDefaults;
			offState.writeDefaultValues = writeDefaults;
			stateMachine.AddState(onState, Vector3.one);
			stateMachine.AddState(offState, Vector3.one);
			AnimatorStateTransition onToOffTransition = new AnimatorStateTransition();
			AnimatorStateTransition offToOnTransition = new AnimatorStateTransition();
			transitions[j * 2] = onToOffTransition;
			transitions[j * 2 + 1] = offToOnTransition;
			onToOffTransition.exitTime = 1;
			offToOnTransition.exitTime = 1;
			onToOffTransition.hasExitTime = true;
			offToOnTransition.hasExitTime = true;
			onToOffTransition.destinationState = offState;
			offToOnTransition.destinationState = onState;
			if (b)
			{
				onToOffTransition.AddCondition(AnimatorConditionMode.IfNot, 0.5f, j.ToString());
				offToOnTransition.AddCondition(AnimatorConditionMode.If, 0.5f, j.ToString());
			}
			else
			{
				onToOffTransition.AddCondition(AnimatorConditionMode.Greater, 0.5f, j.ToString());
				offToOnTransition.AddCondition(AnimatorConditionMode.Less, 0.5f, j.ToString());	
			}
			onState.AddTransition(onToOffTransition);
			offState.AddTransition(offToOnTransition);
			layer.stateMachine = stateMachine;
			layer.defaultWeight = 1;
			layers[j] = layer;
		}
		controller.layers = layers;
		AssetDatabase.CreateAsset(controller, controllerPath + path + layerCount + ".controller");
		SerializeController(controller);
		AssetDatabase.StopAssetEditing();
		RandomiseParameters(controller);
		return controller;
	}
	
	public static AnimatorController SetupTwoTogglesSubStateMachine(int layerCount)
	{
		ReadyPath(controllerPath + "TwoToggleSub/");
		AnimatorController controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(controllerPath + "TwoToggleSub/" + layerCount + ".controller");

		if (controller != null)
		{
			RandomiseParameters(controller);
			return controller;
		}
		
		AssetDatabase.StartAssetEditing();
		controller = new AnimatorController();
		AnimatorControllerLayer[] layers = new AnimatorControllerLayer[layerCount];
		AddParameters(controller, layerCount);
		for (int j = 0; j < layers.Length; j++)
		{
			string index = j.ToString();
			AnimatorControllerLayer layer = new AnimatorControllerLayer();
			layer.name = index;
			AnimatorStateMachine stateMachine = new AnimatorStateMachine();
			AnimatorStateMachine subStateMachine = new AnimatorStateMachine();
			stateMachine.AddStateMachine(subStateMachine, new Vector3(1, 1, 1));
			AnimatorState onState = new AnimatorState();
			AnimatorState offState = new AnimatorState();
			onState.name = index + "on";
			offState.name = index + "off";
			AnimationClip[] anims = AnimationHelper.GetOrCreateTwoStateToggle("test"+j.ToString(), j);
			onState.motion = anims[0];
			offState.motion = anims[1];
			subStateMachine.AddState(onState, Vector3.one);
			subStateMachine.AddState(offState, Vector3.one);
			AnimatorStateTransition onToOffTransition = new AnimatorStateTransition();
			AnimatorStateTransition offToOnTransition = new AnimatorStateTransition();
			onToOffTransition.exitTime = 1;
			offToOnTransition.exitTime = 1;
			onToOffTransition.hasExitTime = true;
			offToOnTransition.hasExitTime = true;
			onToOffTransition.destinationState = offState;
			offToOnTransition.destinationState = onState;
			onToOffTransition.AddCondition(AnimatorConditionMode.Greater, 0.5f, j.ToString());
			offToOnTransition.AddCondition(AnimatorConditionMode.Less, 0.5f, j.ToString());
			onState.AddTransition(onToOffTransition);
			offState.AddTransition(offToOnTransition);
			layer.stateMachine = stateMachine;
			layer.defaultWeight = 1;
			layers[j] = layer;
		}
		controller.layers = layers;
		AssetDatabase.CreateAsset(controller, controllerPath + "TwoToggleSub/" + layerCount + ".controller");
		SerializeController(controller);
		AssetDatabase.StopAssetEditing();
		RandomiseParameters(controller);
		return controller;
	}
	
	#endregion

	#region DirectBlendTrees

	public static AnimatorController SetupDirectBlendTree(int layerCount)
	{
		string path = "DBT/";
		ReadyPath(controllerPath + "TwoToggleSub/");
		AnimatorController controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(controllerPath + path + layerCount + ".controller");

		if (controller != null)
		{
			RandomiseParametersDBT(controller);
			return controller;
		}
		AssetDatabase.StartAssetEditing();

		controller = new AnimatorController();
		AddParameters(controller, layerCount);
		AnimatorControllerParameter oneParameter = new AnimatorControllerParameter();
		oneParameter.name = "one";
		oneParameter.defaultFloat = 1;
		oneParameter.type = AnimatorControllerParameterType.Float;
		controller.parameters = (new[] { oneParameter }).Concat(controller.parameters).ToArray();
		AnimatorControllerLayer layer = new AnimatorControllerLayer();
        layer.defaultWeight = 1;
        layer.name = "ToggleTree";
        layer.stateMachine = new AnimatorStateMachine();
        AnimatorState treeState = new AnimatorState();
        treeState.name = "ToggleTree";
        BlendTree bigTree = new BlendTree();
        bigTree.name = "ToggleTree";
        bigTree.blendType = BlendTreeType.Direct;
        for (int i = 0; i < layerCount; i++)
        {
            BlendTree child = new BlendTree();
            child.name = i.ToString();
            child.blendType = BlendTreeType.Simple1D;
            child.blendParameter = i.ToString();
            AnimationClip[] anims = AnimationHelper.GetOrCreateTwoStateToggle("test"+i.ToString(), i);
            child.AddChild(anims[1]);
            child.AddChild(anims[0]);
            child.hideFlags = HideFlags.HideInHierarchy;
            bigTree.AddChild(child);
        }
        ChildMotion[] childMotions = bigTree.children;
        for (var i = 0; i < childMotions.Length; i++)
        {
            childMotions[i].directBlendParameter = "one";
        }
        bigTree.children = childMotions;
        treeState.motion = bigTree;
        layer.stateMachine.AddState(treeState, Vector3.one);
		controller.layers = new []{layer};
		AssetDatabase.CreateAsset(controller, controllerPath + path + layerCount + ".controller");
		SerializeController(controller);
		AssetDatabase.StopAssetEditing();
		RandomiseParametersDBT(controller);
		return controller;
	}
	
	
	public static AnimatorController SetupSingleDirectBlendTree(int layerCount, bool defaultsLayer = false, bool singleAnim = false)
	{
		string path = singleAnim ? "DBT-Single-Anim/" : defaultsLayer ? "DBT-Single-Defaults/" : "DBT-Single/";
		ReadyPath(controllerPath + "TwoToggleSub/");
		AnimatorController controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(controllerPath + path + layerCount + ".controller");

		if (controller != null)
		{
			RandomiseParametersDBT(controller);
			return controller;
		}
		AssetDatabase.StartAssetEditing();

		controller = new AnimatorController();
		AddParameters(controller, layerCount);
		AnimatorControllerParameter oneParameter = new AnimatorControllerParameter();
		oneParameter.name = "one";
		oneParameter.defaultFloat = 1;
		oneParameter.type = AnimatorControllerParameterType.Float;
		controller.parameters = (new[] { oneParameter }).Concat(controller.parameters).ToArray();
		// DefaultsLayer
		AnimatorControllerLayer defLayer = new AnimatorControllerLayer();
		if (defaultsLayer)
		{
			defLayer.defaultWeight = 1;
			defLayer.name = "ToggleTree";
			defLayer.stateMachine = new AnimatorStateMachine();
			if (singleAnim)
			{
				AnimatorState defState = new AnimatorState();
				defState.motion = AnimationHelper.GetOrCreateBigOnToggle("test", layerCount);
				defLayer.stateMachine.AddState(defState, Vector3.one);
			}
			else
			{
				AnimatorState defTreeState = new AnimatorState();
				defTreeState.name = "ToggleTree";
				BlendTree defTree = new BlendTree();
				defTree.name = "ToggleTree";
				defTree.blendType = BlendTreeType.Direct;
				for (int i = 0; i < layerCount; i++)
				{
					AnimationClip[] anims = AnimationHelper.GetOrCreateTwoStateToggle("test" + i.ToString(), i);
					defTree.AddChild(anims[0]);
				}

				ChildMotion[] defChildMotions = defTree.children;
				for (var i = 0; i < defChildMotions.Length; i++)
				{
					defChildMotions[i].directBlendParameter = "one";
				}

				defTree.children = defChildMotions;
				defTreeState.motion = defTree;
				defLayer.stateMachine.AddState(defTreeState, Vector3.one);
			}
		}
		// TreeLayer
		AnimatorControllerLayer layer = new AnimatorControllerLayer();
        layer.defaultWeight = 1;
        layer.name = "ToggleTree";
        layer.stateMachine = new AnimatorStateMachine();
        AnimatorState treeState = new AnimatorState();
        treeState.name = "ToggleTree";
        BlendTree bigTree = new BlendTree();
        bigTree.name = "ToggleTree";
        bigTree.blendType = BlendTreeType.Direct;
        for (int i = 0; i < layerCount; i++)
        {
	        AnimationClip[] anims = AnimationHelper.GetOrCreateTwoStateToggle("test"+i.ToString(), i);
	        bigTree.AddChild(anims[1]);
        }
        ChildMotion[] childMotions = bigTree.children;
        for (var i = 0; i < childMotions.Length; i++)
        {
            childMotions[i].directBlendParameter = i.ToString();
        }
        bigTree.children = childMotions;
        treeState.motion = bigTree;
        layer.stateMachine.AddState(treeState, Vector3.one);
        if (defaultsLayer)
        {
	        controller.layers = new[] { defLayer, layer };
        }
        else
        {
	        controller.layers = new []{layer};
        }

        AssetDatabase.CreateAsset(controller, controllerPath + path + layerCount + ".controller");
		SerializeController(controller);
		AssetDatabase.StopAssetEditing();
		RandomiseParametersDBT(controller);
		return controller;
	}

	#endregion
	
	public static void SerializeController(AnimatorController controller)
	{
		foreach (var animatorControllerLayer in controller.layers)
		{
			SerializeStateMachine(controller, animatorControllerLayer.stateMachine);
		}
	}

	public static void SerializeStateMachine(AnimatorController controller, AnimatorStateMachine stateMachine)
	{
		stateMachine.hideFlags = HideFlags.HideInHierarchy;
		AssetDatabase.AddObjectToAsset(stateMachine, controller);
		foreach (var childAnimatorState in stateMachine.states)
		{
			childAnimatorState.state.hideFlags = HideFlags.HideInHierarchy;
			AssetDatabase.AddObjectToAsset(childAnimatorState.state, controller);
			foreach (var animatorStateTransition in childAnimatorState.state.transitions)
			{
				animatorStateTransition.hideFlags = HideFlags.HideInHierarchy;
				AssetDatabase.AddObjectToAsset(animatorStateTransition, controller);
			}
			if (childAnimatorState.state.motion is BlendTree tree)
			{
				Queue<BlendTree> trees = new Queue<BlendTree>();
				trees.Enqueue(tree);
				while (trees.Count>0)
				{
					tree = trees.Dequeue();
					AssetDatabase.RemoveObjectFromAsset(tree);
					AssetDatabase.AddObjectToAsset(tree, controller);
					tree.hideFlags = HideFlags.HideInHierarchy;

					foreach (var childMotion in tree.children)
					{
						if (childMotion.motion is BlendTree childTree)
						{
							trees.Enqueue(childTree);
						}
					}
				}
			}
		}
		foreach (var stateMachineAnyStateTransition in stateMachine.anyStateTransitions)
		{
			stateMachineAnyStateTransition.hideFlags = HideFlags.HideInHierarchy;
			AssetDatabase.AddObjectToAsset(stateMachineAnyStateTransition, controller);
		}
		foreach (var childAnimatorStateMachine in stateMachine.stateMachines)
		{
			SerializeStateMachine(controller, childAnimatorStateMachine.stateMachine);
		}
	}

	public static void RandomiseParameters(AnimatorController controller)
	{
		AnimatorControllerParameter[] parameters = controller.parameters;
		foreach (var animatorControllerParameter in parameters)
		{
			animatorControllerParameter.defaultFloat = Random.value;
		}
		controller.parameters = parameters;
	}
	
	public static void RandomiseParametersDBT(AnimatorController controller)
	{
		AnimatorControllerParameter[] parameters = controller.parameters;
		foreach (var animatorControllerParameter in parameters)
		{
			animatorControllerParameter.defaultFloat = (Random.value > 0.5) ? 0 : 1;
			if (animatorControllerParameter.name == "one")
			{
				animatorControllerParameter.defaultFloat = 1;
			}
		}
		controller.parameters = parameters;
	}

	public static void AddParameters(AnimatorController controller, int count, bool b = false)
	{
		AnimatorControllerParameter[] parameters = new AnimatorControllerParameter[count];
		for (int j = 0; j < count; j++)
		{
			string index = j.ToString();
			AnimatorControllerParameter parameter = new AnimatorControllerParameter();
			parameter.type = b ? AnimatorControllerParameterType.Bool : AnimatorControllerParameterType.Float;
			parameter.name = index;
			parameters[j] = parameter;
		}

		controller.parameters = parameters;
	}
}