using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKDecoration : IKBehavior
{
	[Header("IK Decoration")]

	[Range(0,16)]
	public int passes = 1;

	public float lerpFactor = 0.1f;

	public bool debug;


	private Vector3 IKTarget;

	void Start()
	{
		CreateSpine();

		if (spineTarget != null)
		{
			IKTarget = spineTarget.position;
		}
		else
		{
			Debug.LogError("IKDecoration has no target.");
			this.enabled = false;
			return;
		}

	}

	void LateUpdate()
	{
		if ( Vector3.Distance(IKTarget, spine[0].transform.position) > (spineLength) )
		{
			IKTarget = spine[0].transform.position + Vector3.ClampMagnitude(IKTarget - spine[0].transform.position, spineLength);
			//Debug.Log("[IKDecoration] IKTarget is too far away, clamping magnitude.");
		}

		IKTarget = Vector3.Lerp( IKTarget, spineTarget.position, lerpFactor );



		for (int p = 0; p < passes; p++)
		{
			IKPass(IKTarget);
		}


		//	Debug
		if (debug)
		{
			Debug.DrawLine(spine[0].transform.position, IKTarget, Color.magenta);

			for (int b = 0; b < spineCount; b++)
			{
				Debug.DrawLine(spine[b].transform.position, spine[b].transform.position + spine[b].transform.forward / 4, Color.blue);
				Debug.DrawLine(spine[b].transform.position, spine[b].transform.position + spine[b].transform.up / 4, Color.green);
				Debug.DrawLine(spine[b].transform.position, spine[b].transform.position + spine[b].transform.right / 4, Color.red);
			}
		}

	}
	
}
