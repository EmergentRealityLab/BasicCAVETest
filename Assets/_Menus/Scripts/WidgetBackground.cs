using UnityEngine;
using System.Collections.Generic;

//Represents the background of a widget using a tiling system. It dynamically resizes itself to fit containing text.
public class WidgetBackground : MonoBehaviour {
	
	public GameObject parent_menu;
	
	public float total_width;
	public float total_height;
	
	public Widget parent;
	
	public float tileW;
	public float tileH;
	
	public int num_x_tiles;
	public int num_y_tiles;
	
	public List<GameObject> tiles;
	
	//Row 1
	public GameObject tile00;
	public GameObject tile01;
	public GameObject tile02;
	
	//Row 2
	public GameObject tile10;
	public GameObject tile11;
	public GameObject tile12;
	
	//Row 3
	public GameObject tile20;
	public GameObject tile21;
	public GameObject tile22;
	
	/*
	 * Naming scheme:
	 * 
	 * (tile00)  (tile01)  (tile02)
	 * 
	 * (tile10)  (tile11)  (tile12)
	 * 
	 * (tile20)  (tile21)  (tile22)
	 * 
	 */
	
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	//Build the background pieces based on the text mesh given (involves instantiation)
	//Must take in host widget as argument. It will scale down to 1,1,1 first, then back to original when done.
	public void BuildBackground(TextMesh text, Widget widget)
	{
		widget.transform.localScale = new Vector3(1,1,1);
		
		GameObject menu = widget.menu.gameObject;
		
		Vector3 saved_scale = menu.transform.localScale;
		menu.transform.localScale = new Vector3(1,1,1);
		
		if(tiles == null)
			tiles = new List<GameObject>();
		else
		{
			foreach(GameObject tile in tiles)
			{
				Destroy(tile);
			}
			tiles.Clear();
		}
		
		tileW = tile00.transform.localScale.x * widget.transform.localScale.x;
		tileH = tile00.transform.localScale.y * widget.transform.localScale.y;
		
		float length = text.renderer.bounds.size.x * 1.1f;
		float height = text.renderer.bounds.size.y;
		
		//Debug.Log ("Length = " + length + ", height = " + height);
		//Debug.Log ("TileW = " + tileW + ", TileH = " + tileH);
		
		num_x_tiles = 1;
		num_y_tiles = 1;
		
		while((num_x_tiles+2)*tileW <= length)
		{
			num_x_tiles++;
			if(num_x_tiles > 100)
			{
				Debug.Log ("Warning: Infinite loop in tiling");
				break;
			}
		}
		
		while((num_y_tiles+2)*tileH <= height)
		{
			num_y_tiles++;
			if(num_y_tiles > 100)
			{
				Debug.Log ("Warning: Infinite loop in tiling");
				break;
			}
		}
		
		Vector3 pos = new Vector3(0,0,0);
		
		int maxI = num_y_tiles+2;
		int maxJ = num_x_tiles+2;
		
		total_width = maxJ * tileW;
		total_height = maxI * tileH;
		
		//Draw the topmost at 0,0 for now
		for(int i=0;i<maxI;i++)
		{
			for(int j=0;j<maxJ;j++)
			{
				pos = new Vector3(j * tileW,-i * tileH, 0);
				
				GameObject current_tile;
				
				if(i==0 && j==0)
					current_tile = tile00;
				else if(i==0 && j==maxJ-1)
					current_tile = tile02;
				else if(i==maxI-1 && j==0)
					current_tile = tile20;
				else if(i==maxI-1 && j==maxJ-1)
					current_tile = tile22;
				else if(i == 0)
					current_tile = tile01;
				else if(i==maxI-1)
					current_tile = tile21;
				else if(j == 0)
					current_tile = tile10;
				else if(j == maxJ-1)
					current_tile = tile12;
				else
					current_tile = tile11;
				

				GameObject piece = Instantiate(current_tile,Vector3.zero,Quaternion.identity) as GameObject;
				
				piece.transform.parent = this.transform;
				piece.transform.localPosition = new Vector3(pos.x - (total_width/2) + (tileW/2),pos.y + (total_height/2) - (tileH/2),pos.z);
				piece.transform.localRotation = Quaternion.identity;
				
				tiles.Add(piece);
			}
		}
		
		menu.transform.localScale = saved_scale;
		
	}
}
