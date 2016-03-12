using UnityEngine;
using System.Collections;

public class SpawnPoint : MonoBehaviour {

	void OnDrawGizmos()
	{
		Gizmos.color = Color.cyan;
		Gizmos.DrawCube(transform.position, new Vector3(.5f, .1f, .5f));
	}

	void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.cyan;
		Vector3 forwardPosition = transform.forward * .35f + transform.position;
		Gizmos.DrawCube(forwardPosition, new Vector3(.2f, .1f, .2f));
	}
}
