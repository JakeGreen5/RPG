using UnityEngine;
using System.Collections;

public class Bartender : MonoBehaviour {

	Animator anim;
	
	void Start () 
	{
		anim = GetComponent<Animator>();
	}

	public void Bartend()
	{
		anim.SetTrigger("Bartend");
	}
}
