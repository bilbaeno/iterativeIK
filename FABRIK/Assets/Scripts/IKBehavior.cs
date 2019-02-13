//	IKBehaviour
//		IK Position Calculator for a given set of spineCount, using the Forward-backward Iterative IK Algorithm
//
//	Author: Adam Bilbaeno
//	Date:	11/26/2018

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKBehavior : MonoBehaviour 
{
	/*	Bone Class
	 * 
	 */
	public class IKBone
	{
		public Transform transform;
		public float length;

		public IKBone child;

		public IKBone(Transform t)
		{
			transform = t;
		}
	}


	[Header("IK Behavior")]

	public Transform[] boneTransforms;
	public Transform spineTarget;


	public IKBone[] spine;
	protected int spineCount;
	protected float spineLength;

	public float rollAngle = 90;
	public bool rollCorrection = false;

	public void IKPass(Vector3 spineTarget)
	{

		Vector3[] forward = new Vector3[spineCount + 1];
		Vector3[] backward = new Vector3[spineCount + 1];


		//		"Forward" (Tail-to-Root)		//

		//	Set end of last bone to the target position
		forward[spineCount] = spineTarget;

		//	Iterate through bones, starting with the last
		for (int j = spineCount; j > 0; j--)
		{
			//	Calculate Vector from current bone to previous bone
			forward[j-1] = spine[j-1].transform.position - forward[j];
			//	Stretch Vector to correct length 
			forward[j-1] = forward[j-1].normalized * spine[j-1].length;
			//	Position Vector relative to current bone
			forward[j-1] = forward[j] + forward[j-1];
		}

		//		"Backward" (Root-to-Tail)		//

		//	Set start of first bone to root position
		backward[0] = spine[0].transform.position;

		//	Iterate through bones, starting with first
		for (int i = 0; i < spineCount; i++)
		{
			//	Calculate vector from current bone to next bone
			backward[i+1] = forward[i+1] - backward[i];
			//	Stretch Vector to correct length
			backward[i+1] =  backward[i+1].normalized * spine[i].length;
			//	Position Vector relative to current bone
			backward[i+1] = backward[i] + backward[i+1];
		}


		//		Transform Update		//

		for (int b = 0; b < spineCount; b++)
		{
			//	Update position
			spine[b].transform.position = backward[b];


			//	Update rotation
			Vector3 previousRight = spine[b].transform.right;
			spine[b].transform.LookAt(backward[b+1]);
			spine[b].transform.rotation = Quaternion.AngleAxis(rollAngle, spine[b].transform.right) * spine[b].transform.rotation;

			//	Roll correction
			if (rollCorrection)
			{
				previousRight = Vector3.ProjectOnPlane(previousRight, spine[b].transform.up);
				float roll = Vector3.Angle(previousRight, spine[b].transform.right);

				spine[b].transform.rotation = Quaternion.AngleAxis(roll, spine[b].transform.up) * spine[b].transform.rotation;
			}

		}

	}


	//	Bone Length Calculation
	public void CalculateBoneLengths()
	{
		spineLength = 0f;

		for (int b = 0; b < spineCount - 1; b++)
		{
			spine[b].length = Vector3.Distance(spine[b+1].transform.position, spine[b].transform.position);
			spineLength += spine[b].length;
		}

		spine[spineCount-1].length = Vector3.Distance(spine[spineCount-1].transform.position, spineTarget.position);
		spineLength += spine[spineCount-1].length;
	}


	//	Spine Initialization
	public void CreateSpine()
	{
		spineCount = boneTransforms.Length;

		if (spineCount == 0)
		{
			Debug.LogError("IKBehavior has no bones. Coward.");
			this.enabled = false;
			return;
		}



		spine = new IKBone[spineCount];

		for (int i = 0; i < spineCount; i++)
		{
			spine[i] = new IKBone(boneTransforms[i]);
		}

		for (int j = 0; j < spineCount - 1; j++)
		{
			spine[j].child = spine[j+1];
		}



		CalculateBoneLengths();
	}


}