//Nicholas Cesare

using UnityEngine;
using System.Collections;
using System.Xml;
using System.IO;

public class AnimationIndices : MonoBehaviour {

	public static Hashtable animIndices = new Hashtable();
	TextAsset textAsset;
	XmlReader reader;

	// Use this for initialization
	void Start () {
		string last_element = "";
		int counter = 1;
	

		textAsset = (TextAsset) Resources.Load("XMLs/tea_dialog");
		reader = XmlReader.Create(new StringReader(textAsset.text));

		//pull in the animation names from the xml file
		while (reader.Read ()) {

			if(reader.NodeType == XmlNodeType.Element){

				while(reader.MoveToNextAttribute())
				{
					if(reader.Name == "id")
					{
						//print (counter + " : " + reader.Value);
						animIndices.Add (counter, reader.Value);
						counter += 1;
					}
				}

			}
		}

		//print out hash table, for testing purposes
		//printHashTable ();

	}

	//prints out the hash table, for testing purposes
	void printHashTable(){

		for (int i = 1; i < animIndices.Count + 1; i++) {
			print (i + " : " + animIndices[i]);
		}
	}
}