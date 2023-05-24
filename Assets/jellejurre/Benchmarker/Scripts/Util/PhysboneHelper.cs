using UnityEngine;
using VRC.Dynamics;
using VRC.SDK3.Dynamics.PhysBone.Components;

public class PhysboneHelper
{
	public static GameObject AddPhysbone(GameObject physboneObject, int depth = 4, int amount = 64)
	{
		GameObject physBoneRoot = new GameObject();
		physBoneRoot.transform.parent = physboneObject.transform;
		VRCPhysBone physbone = physBoneRoot.AddComponent<VRCPhysBone>();
		for (int i = 0; i < amount/depth; i++)
		{
			GameObject current = new GameObject();
			current.name = "root" + i;
			current.transform.parent = physBoneRoot.transform;
			current.transform.localPosition = new Vector3(0, i, 0);
			for (int j = 0; j < depth; j++)
			{
				GameObject currentNew = new GameObject();
				currentNew.transform.parent = current.transform;
				currentNew.transform.localPosition = new Vector3(1, 0, 0);
				current = currentNew;
			}
			GameObject boneEnd = new GameObject();
			boneEnd.transform.parent = current.transform;
			boneEnd.transform.localPosition = new Vector3(1, 0, 0);
		}

		return physBoneRoot;
	}

	public static GameObject AddForkedPhysbone(GameObject physboneObject, int depth = 4, bool AddBone = true)
	{
		if (depth == 0)
		{
			return physboneObject;
		}
		GameObject current = physboneObject;
		if (AddBone)
		{
			GameObject physBoneRoot = new GameObject();
			physBoneRoot.transform.localPosition = new Vector3(1, 0, 0);
			physBoneRoot.transform.parent = physboneObject.transform;
			VRCPhysBone physbone = physBoneRoot.AddComponent<VRCPhysBone>();
			current = physBoneRoot;
		}
		GameObject physBone1 = new GameObject();
		physBone1.transform.localPosition = new Vector3(1, 0, 0);
		physBone1.transform.parent = current.transform;
		GameObject physBone2 = new GameObject();
		physBone2.transform.localPosition = new Vector3(1, 0, 0);
		physBone2.transform.parent = current.transform;
		AddForkedPhysbone(physBone1, depth - 1, false);
		AddForkedPhysbone(physBone2, depth - 1, false);
		return physboneObject;
	}
}