using UnityEngine;
using System.Collections;

public class HeadsUpDisplay : MonoBehaviour
{	
	[HideInInspector] public GameObject hudCamera;
	
	GameObject centerAnchor;
	GameObject centerPanel;
	
	GameObject bottomAnchor;
	[HideInInspector] public GameObject conversationPanel;
	
	[HideInInspector] public GameObject crosshair;	
	[HideInInspector] public GameObject pressToInteract;	
		
	[HideInInspector] public GameObject wordsOfOther;
	[HideInInspector] public UILabel wordsOfOtherLabel;
	
	[HideInInspector] public GameObject option1;
	[HideInInspector] public GameObject option2;
	[HideInInspector] public GameObject option3;
	[HideInInspector] public GameObject option4;
	[HideInInspector] public UILabel option1Label;
	[HideInInspector] public UILabel option2Label;
	[HideInInspector] public UILabel option3Label;
	[HideInInspector] public UILabel option4Label;
	
	public void Awake()
	{
		Initialize();

		transform.position = new Vector3(0f, -100f, 0f);
	}

	public void Initialize()
	{		
		hudCamera = transform.Find("Camera").gameObject;
		
		centerAnchor = hudCamera.transform.Find("Anchor - Center").gameObject;
		bottomAnchor = hudCamera.transform.Find("ConversationUI").gameObject;
		
		crosshair = centerAnchor.transform.Find("Panel 1").Find("Crosshair").gameObject;
		pressToInteract = centerAnchor.transform.Find("Panel 1").Find("PressToInteract").gameObject;
		
		#region conversation interface
		conversationPanel = bottomAnchor.transform.Find("Conversation Panel").gameObject;
		
		wordsOfOther = conversationPanel.transform.Find("Words of Other").gameObject;		
		wordsOfOtherLabel = wordsOfOther.transform.Find("Label").GetComponent<UILabel>();
		
		option1 = conversationPanel.transform.Find("Option 1").gameObject;
		option2 = conversationPanel.transform.Find("Option 2").gameObject;
		option3 = conversationPanel.transform.Find("Option 3").gameObject;
		option4 = conversationPanel.transform.Find("Option 4").gameObject;
		
		option1Label = option1.transform.Find("Label").GetComponent<UILabel>();
		option2Label = option2.transform.Find("Label").GetComponent<UILabel>();
		option3Label = option3.transform.Find("Label").GetComponent<UILabel>();
		option4Label = option4.transform.Find("Label").GetComponent<UILabel>();
		
		NGUITools.SetActive(conversationPanel, false);
		NGUITools.SetActive(pressToInteract, false);
		#endregion
	}

	public void ShowInteractionUI(bool show)
	{		
		NGUITools.SetActive(pressToInteract, show);
	}

	public void ShowConversationUI(bool show)
	{
		NGUITools.SetActive(conversationPanel, show);
	}
}
