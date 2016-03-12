using UnityEngine;
using System.Collections;

public class AIAnimation : MonoBehaviour {

	Animator anim;

	Type currentAnimation;
	public enum Type
	{
		Walk,
		Idle,
		Drink,
		Sleep,
		Eat
	}

	void Start () 
	{
		anim = GetComponent<Animator>();
	}

	public void PlayMove(float speed)
	{
		if (currentAnimation != Type.Walk)
		{
			anim.SetBool(currentAnimation.ToString(), false);
		}

		anim.SetFloat("Speed", speed);
		currentAnimation = Type.Walk;
	}

	public void PlayAnimation(Type animation)
	{
		if (currentAnimation != Type.Walk)
		{
			anim.SetBool(currentAnimation.ToString(), false);
		}

		if (animation == Type.Walk) return;

		currentAnimation = animation;
		
		anim.SetFloat("Speed", 0f);
		anim.SetBool(animation.ToString(), true);
	}
}
