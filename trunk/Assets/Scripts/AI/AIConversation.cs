using UnityEngine;
using System.Collections;

public class AIConversation : MonoBehaviour {

	private AIManager manager;
	
	private string reply;
	private string choice1;
	private string choice2;
	private string choice3;
	private string choice4;
	private int phase = 0;
	public int personalityNumber = 0;
	
	void Start ()
	{
		manager = GetComponent<AIManager>();
	}
	
	public void HandleConversationStart()
	{
		UpdateWords("0");		
		UpdateHUD();
	}
	
	public void HandleOptionClicked(int option)
	{
		if (option == 1)
		{
			UpdateWords("1");
		}
		else if (option == 2)
		{
			UpdateWords("2");
		}
		else if (option == 3)
		{
			UpdateWords("3");
		}
		else if (option == 4)
		{
			phase = 0;
			manager.data.CurrentTask.ProceedToNextSubtask();
			manager.stateMachine.StateIdle();
			
			Cursor.visible = false;
			
			NGUITools.SetActive(UIManager.HUD.conversationPanel, false);

			Game.Instance.playerScript.raycastManager.objectWeAreInteractingWith = null;
		}
		
		UpdateHUD();
	}
	
	private string GetTaskObjectName(int number)
	{
		if (number < manager.data.TaskCount)
		{
			return manager.data.tasks[number].CurrentSubtask.gameObjectName;
		}

		return "Nowhere";
	}
	
	private void UpdateHUD()
	{
		UIManager.HUD.wordsOfOtherLabel.text = reply;
		UIManager.HUD.option1Label.text = choice1;
		UIManager.HUD.option2Label.text = choice2;
		UIManager.HUD.option3Label.text = choice3;
		UIManager.HUD.option4Label.text = choice4;
	}
	
	private void UpdateWords(string choice)
	{
		string temp = phase.ToString();
		temp += choice;
		phase = int.Parse(temp);
		
		if (personalityNumber == 0)
		{
			if (phase == 0)
			{
				reply = "Hello, I'm " + manager.data.name + ". Nice to meet you.";
				choice1 = "What are you doing?";
				choice2 = "Who are you?";
				choice3 = "Well met.";
				choice4 = "I have to go.";
			}
			else if (phase == 1)
			{
				reply = "I am headed to the " + GetTaskObjectName(1) + ".";
				choice1 = "Why?";
				choice2 = "What are you doing next?";
				choice3 = "Don't do that.";
			}
			else if (phase == 11)
			{
				if (GetTaskObjectName(1) == "Eatery")
				{
					reply = "Because I like to eat.";
					choice1 = "We need to finish building the village. You should head over to the workplace.";
					choice2 = "You look cold. You should go to the fireplace.";
					choice3 = "You did a good work today. Go to sleep."; 
					// need to update priority list because of convo
				}
				else if (GetTaskObjectName(1) == "Work Bench")
				{
					reply = "Because I need to do my job.";
					choice1 = "You did good work today. Go to sleep."; 
					choice2 = "You look cold. You should go to the fireplace.";
					choice3 = "There are free donuts at the eatery!";
					// need to update priority list because of convo
				}
				else if (GetTaskObjectName(1) == "Bed")
				{
					reply = "Because I am tired.";
					choice1 = "We need to finish building the village. You should head over to the workplace."; 
					choice2 = "You look cold. You should go to the fireplace.";
					choice3 = "There are free donuts at the eatery!";
					// need to update priority list because of convo
				}
				else if (GetTaskObjectName(1) == "Fireplace")
				{
					reply = "Because it's too damn cold!";
					choice1 = "We need to finish building the village. You should head over to the workplace."; 
					choice2 = "You did a good work today. Go to sleep.";
					choice3 = "There are free donuts at the eatery!";
					// need to update priority list because of convo
				}
				choice4 = "I shouldn't keep you busy.";
			}
			else if (phase == 12)
			{
				reply = "I am going to the " + GetTaskObjectName(2) + ".";
				
				choice1 = "Why don't you go to " + GetTaskObjectName(3) + " next?";
				choice2 = "Why don't you go to " + GetTaskObjectName(4) + " next?";
				choice3 = "Why don't you stay at " + GetTaskObjectName(1) + "?";
			}
			else if (phase == 121)
			{
				reply = "Ok, I'll go to " + GetTaskObjectName(3) + " next."; //update priority list due to convo
				//mind.priorityList.Insert(0, getPriority(3));
				choice1 = "";
				choice2 = "";
				choice3 = "";
				choice4 = "Bye";
			}
			else if (phase == 122)
			{
				reply = "Ok, I'll go to " + GetTaskObjectName(4) + "."; //update priority list due to convo
				choice1 = "";
				choice2 = "";
				choice3 = "";
				choice4 = "Bye";
			}
			else if (phase == 123)
			{
				reply = "Ok, I'll go to " + GetTaskObjectName(4) + "."; //update priority list due to convo
				choice1 = "";
				choice2 = "";
				choice3 = "";
				choice4 = "Bye";
			}
			else if (phase == 13)
			{
				reply = "Why are you telling me why I can't do something?";
				choice1  = "I am not telling you to do anything! I was just going to suggest something.";
				choice2 = "You shouldn't go to the " + GetTaskObjectName(1) + ". The community needs you to do somehting else.";
				choice3 = "If you keep on working in the future you should listen to me.";
			}
			else if (phase == 131)
			{
				reply = "Well suggest to someone else! Now if you'll excuse me I have work to do.";
				choice1 = "";
				choice2 = "";
				choice3 = "";
				choice4 = "Exit conversation";
			}
			else if (phase == 132)
			{
				reply = "What would the community want me to do?";
				choice1 = "We need more workers at the workplace. You should head over there.";
				choice2 = "We are having a meeting in the inn near the fireplace.";
				choice3 = "";
			}
		}
	}
}
