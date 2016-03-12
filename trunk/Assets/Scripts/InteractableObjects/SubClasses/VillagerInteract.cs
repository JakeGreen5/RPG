using UnityEngine;
using System.Collections;

public class VillagerInteract : InteractableObject {
	
	public override int InteractDistance 
	{ 
		get { return 12; }
	}

	public override void Interact()
	{
		AIManager manager = transform.parent.gameObject.GetComponent<AIManager>();
		manager.data.AddTask(new Converse(Application.loadedLevelName, Game.Instance.player.name));
		
		AIConversation aiConvo = manager.conversation;
		aiConvo.HandleConversationStart();

		UIManager.HUD.ShowConversationUI(true);
		
		Cursor.visible = true;
	}
}
