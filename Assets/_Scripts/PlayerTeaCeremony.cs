using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerTeaCeremony : MonoBehaviour {
	
	public int checkNumber;
	public TeaDialogTree dialogTree;
	
	public List<GameObject> teacups = new List<GameObject>();
	public GameObject teapot;
	public GameObject pitcher;	
	
	public static int teapotCapacity = 1000;
	public static int teacupCapacity = 200;
	private bool finished = false;
	private bool failed = false;

	// Use this for initialization
	void Awake () 
	{
		checkNumber = 1;
	}
	
	// Update is called once per frame
	void Update () 
	{
		CheckEndConditions();
	}
		
	public void EndCheck()
	{
		switch(checkNumber)
		{
		case 1:
			failed = false;
			checkNumber = 2;
			finished = true;
			teapot.GetComponent<TeapotScript>().waterOutside = (int)(teapotCapacity / 2.0f) + 1;
			teapot.GetComponent<TeapotScript>().waterInside = (int)(teapotCapacity / 2.0f) + 1;
			break;
		case 2:
			failed = false;
			checkNumber = 3;
			break;
		case 3:
			failed = false;
			checkNumber = 4;
			teapot.GetComponent<TeapotScript>().teaCount = 33;
			teapot.GetComponent<TeapotScript>().waterInside = 0;
			break;
		case 4:
			failed = false;
			checkNumber = 5;
			teapot.GetComponent<TeapotScript>().teaCount = 5;
			break;
		case 5:
			failed = false;
			checkNumber = 6;
			teapot.GetComponent<TeapotScript>().teaCount = 33;
			teapot.GetComponent<TeapotScript>().waterInside = 0;
			break;
		case 6:
			failed = false;
			checkNumber = 0;
			break;
		default:
			break;
		}
		dialogTree.OnPlayerActionSuccess();
	}

	private void CheckEndConditions()
	{
		if(teapot == null || pitcher == null)
		{
			return;
		}
		foreach (GameObject teacup in teacups)
		{
			if(teacup == null)
				return;
		}

		switch(checkNumber)
		{
		case 1:
			if(teapot.GetComponent<TeapotScript>().waterInside >= teapotCapacity / 2.0f && teapot.GetComponent<TeapotScript>().waterOutside >= teapotCapacity / 2.0f)
			{
				print("step 1 complete");
				EndCheck();
			}/**/
			break;
		case 2:
			if(teapot.GetComponent<TeapotScript>().teaLeafCount >= 3.0f)
			{
				print ("step 2 complete");
				EndCheck();
			}/**/
			break;
		case 3:
			if(teapot.GetComponent<TeapotScript>().waterInside >= teapotCapacity)
			{
				print("step 3 complete");
				EndCheck();
			}/**/
			break;
		case 4:
			int filled = 0;
			string template = "";
			for(int teacupCount = 0; teacupCount < teacups.Count; teacupCount++)
			{
				template += teacups[teacupCount].GetComponent<TeacupScript>().teaCount.ToString() + " ";
				if(teacups[teacupCount].GetComponent<TeacupScript>().teaCount >= teacupCapacity)
				{
					filled++;
				}
			}
			if(filled >= teacups.Count)
			{
				print("step 4 complete");
				EndCheck();
			}
			//print(template);/**/
			break;
		case 5:
			bool complete = false;
			if(teapot.GetComponent<TeapotScript>().teaCount <= 0 && teapot.GetComponent<TeapotScript>().waterInside >= teapotCapacity / 2.0f)
			{
				complete = true;
			}

			for(int teacupCount = 0; teacupCount < teacups.Count; teacupCount++)
			{
				int teacupTeaCount = teacups[teacupCount].GetComponent<TeacupScript>().teaCount;
				if(teacupTeaCount > 0)
				{
					 complete = false;
				}
			}

			if(complete)
			{
				print("step 5 complete");
				EndCheck();
			}
			break;
		case 6:
			int cupsFilled = 0;
			int maximumTea = 0;
			int minimumTea = 9999;
			for(int teacupCount = 0; teacupCount < teacups.Count; teacupCount++)
			{
				int teacupTeaCount = teacups[teacupCount].GetComponent<TeacupScript>().teaCount;
				if(teacupTeaCount >= teacupCapacity)
				{
					cupsFilled++;
				}

				if(teacupTeaCount > maximumTea)
				{
					maximumTea = teacupCount;
				}
				else if(teacupTeaCount < minimumTea)
				{
					minimumTea = teacupTeaCount;
				}
			}

			// unsure of exactly how large tea/water counts get with new particle collision
			// temporarily removing this as it is stopping progression
			/*if(maximumTea - minimumTea > 500)
			{
				failed = true;
			}*/

			if(cupsFilled >= teacups.Count && !failed)
			{
				print("step 6 complete");
				EndCheck();
			}
			break;
		default:
			break;
		}		
	}

	public bool CanPour(float objectHeight)
	{
		if((checkNumber == 3 && objectHeight < GameObject.Find("CheckHeight3").transform.position.y) ||
		   (checkNumber == 6 && objectHeight > GameObject.Find("CheckHeight6").transform.position.y))
		{
			return false;
		}

		return true;
	}

}
