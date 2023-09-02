using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Animations;

public class FakeParentConstraint : MonoBehaviour
{
	public GameObject[] sources;
    public Vector3 defaultPosition;
    public Vector3 defaultRotation;
    public float[] sourceWeights;
    public bool enabled;
    public Vector3[] positionOffsets;
    public Vector3[] rotationOffsets;
    public bool xPos = true;
    public bool yPos = true;
    public bool zPos = true;
    public bool xRot = true;
    public bool yRot = true;
    public bool zRot = true;
}

public class FakeConstraintManager : MonoBehaviour
{
	private FakeParentConstraint[] _constraints;

	public void FixedUpdate()
	{
		if (_constraints == null)
		{
			_constraints = FindObjectsOfType<FakeParentConstraint>();
		}

		FakeParentConstraint[] enabled = _constraints.Where(x => x.enabled).ToArray();
		
		Vector3[] positions = new Vector3[enabled.Length];
		Vector3[] rotations = new Vector3[enabled.Length];
		float[] totalWeights = new float[enabled.Length];
		for (var j = 0; j < enabled.Length; j++)
		{
			positions[j] = Vector3.zero;
			rotations[j] = Vector3.zero;
			var c = enabled[j];
			if (c.sources.Length == 1)
			{
				positions[j] = c.defaultPosition * c.sourceWeights[0] +
				           ((c.sources[0].transform.position + c.positionOffsets[0]) * (1 - c.sourceWeights[0]));
				rotations[j] = c.defaultRotation * c.sourceWeights[0] +
				           ((c.sources[0].transform.rotation.eulerAngles + c.rotationOffsets[0]) * (1 - c.sourceWeights[0]));
			}
			else
			{
				totalWeights[j] = 0;
				for (var i = 0; i < c.sources.Length; i++)
				{
					totalWeights[j] += c.sourceWeights[i];
				}
				totalWeights[j] = totalWeights[j] == 0 ? 1 : totalWeights[j];
				for (int i = 0; i < c.sources.Length; i++)
				{
					Transform t = c.sources[i].transform;
					float myWeight = c.sourceWeights[i] / totalWeights[j];
					positions[j] += (t.position + c.positionOffsets[i]) * myWeight;
					rotations[j] += (t.rotation.eulerAngles + c.rotationOffsets[i]) * myWeight;
				}
			}
			rotations[j] = new Vector3(c.xRot ? rotations[j].x : 0, c.yRot ? rotations[j].y : 0, c.zRot ? rotations[j].z : 0);
			positions[j] = new Vector3(c.xPos ? positions[j].x : 0, c.yPos ? positions[j].y : 0, c.zPos ? positions[j].z : 0);
			Transform t2 = c.transform;
			t2.rotation = Quaternion.Euler(rotations[j]);
			t2.position = positions[j];
		}
	}
}