using UnityEngine;
using System.Collections;

public class StringUtils
{
	public static string RemoveNewlineAndIndents(string s)
	{
		bool removed_nl = false;
		
		int loop = 0;
		
		for(int i=0;i<s.Length;loop++)
		{
			if(s[i] == '\n')
			{
				s = s.Remove(i,1);
				removed_nl = true;
				continue;
			}
			if(removed_nl)
			{
				if(s[i] == ' ')
				{
					s = s.Remove(i,1);
					continue;
				}
				else
				{
					removed_nl = false;
				}
			}
			i++;
			
			if(loop > 10000){ Debug.Log ("Infinite loop"); break; }
			
		}
		return s;
	}

	//Check if the last line of string is one word.
	//i.e.
	//	"a
	//	 b
	//	 cookie"
	//returns true
	//
	//	"a
	//	 b
	//	 eat cookie"
	//returns false
	public static bool IsLastLineOneWord(string s)
	{
		if(s == "")
			return false;
		
		for(int i=s.Length-1;i>=0;i--)
		{
			if(i == s.Length-1 && s[i] == '\n')
				return false;
			if(s[i] == '\n')
				return true;
			if(s[i] == ' ')
				return false;
		}
		return true;
	}
	
	public static bool IsLastLineOneLetter(string s)
	{
		if(s.Length <= 1 || s[s.Length-2] == '\n')
			return true;
		return false;
	}


}
