using UnityEngine;
using System.Collections;

public class DialogueOptionClicked : MonoBehaviour {

	int optionNumber;
	
	void Start () 
	{
		optionNumber = System.Convert.ToInt32(gameObject.name.Substring(gameObject.name.Length - 1));
	}
	
	void OnClick () 
	{
		// TODO This is ridiculously long and needs to be taken care of
		AIConversation aiConvo = Game.Instance.playerScript.raycastManager.objectWeAreInteractingWith.transform.parent.gameObject.GetComponent<AIConversation>();
		aiConvo.HandleOptionClicked(optionNumber);
	}
}
