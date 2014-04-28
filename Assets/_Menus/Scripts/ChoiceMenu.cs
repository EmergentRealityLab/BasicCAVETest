using UnityEngine;
using System.Collections.Generic;

/*
* This is a menu that supports a multiple-choice selection interface.
* Good for: storyteller interfaces.
* You choose one or more options, then the menu's behavior can be set once you submit your choice(s).
*/
public class ChoiceMenu : Menu
{
	public bool use_submit;        //Should a submit button be used?
	
	//An array of index numbers of accepted answers.
	//(i.e. question is "What is a bird?" and answers are (1)Cat, (2)Dog, (3)Robin, and (4)Blackbird, then
	//you would set answers to an int array containing { 2, 3 } to signify that you are looking for two answer and it's the
	//second and third indices.
	public int[] answers;        
	
	public Queue<Widget> selection_queue;
	
	//Set the answers.
	public void SetAnswers(int[] ans)
	{
		answers = ans;
	}
	
	new void Awake()
	{
		base.Awake();
	}
	
	// Use this for initialization
	new void Start ()
	{
		//base.Start ();
		
		selection_queue = new Queue<Widget>();

	}
	
	// Update is called once per frame
	void Update ()
	{
		if(use_submit)
		{
			//Don't display the submit button if there are no orders.
			if(answers.Length > 0 && GetNumSelectedWidgets() >= answers.Length && use_submit == true)
				Submit.Enable();
			else
				Submit.Disable();
		}
	}
	
	bool IsCorrect()
	{
		for(int i=0;i<answers.Length;i++)
		{
			bool answer_selected = false;
			for(int j=0;j<widgets.Count;j++)
			{
				if(widgets[j].IsSelected && answers[i] == j)
				{
					answer_selected = true;
				}
			}
			if(!answer_selected)
				return false;
		}
		return true;
	}
	
	//Event that's fired when submit button is clicked.
	public override void OnSubmit(SubmitButton submitButton)
	{
		for(int i=0;i<widgets.Count;i++)
		{
			//Gather some data from the widgets...
		}
		//Then use data from the widgets for something...
		Debug.Log ("Your answer is " + IsCorrect());
		
		//Then maybe destroy the menu...
		Destroy (gameObject);
	}
	
	//Event that's fired when a widget is selected
	public override void OnWidgetClick(Widget widget)
	{
		//Debug.Log (GetNumSelectedWidgets().ToString());
		
		if(answers.Length == 0)
			return;
		
		selection_queue.Enqueue(widget);
		
		//If submit button is used:
		if(use_submit == true)
		{
			//Debug.Log ("Widget count: " + widgets.Count + " , " + "Number of selected widgets: " + GetNumSelectedWidgets());
			
			//While the number of selected widgets is greater than the number of answers, Go thru all other widgets and unselect them.
			for(int i=0;i<widgets.Count && GetNumSelectedWidgets() > answers.Length;i++)
			{
				if(selection_queue.Count > 0)
				{
					Widget unselected = selection_queue.Dequeue();
					unselected.Unselect();
				}
			}
		}
		//Else
		else
		{
			//Gather this widget's data, and possibly destroy this menu, moving on to a new menu (a la storyteller interface...)
			if(GetNumSelectedWidgets() >= answers.Length)
			{
				Debug.Log ("Your answer is " + IsCorrect());
				//Move on to the next question... (i.e. new menu needs to be created)
				Destroy (gameObject);
			}
		}
		
	}
	
}