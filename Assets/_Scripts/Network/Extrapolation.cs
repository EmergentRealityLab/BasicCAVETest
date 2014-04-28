using UnityEngine;
using System.Collections;

public class Extrapolation : MonoBehaviour 
{
	//Dead reckoning extrapolation script for objects that can be predicted, but not very well.
	//Uses continuous network updates to correct data,
	//but extrapolates between the network updates, using prediction data.
	//PLANNED FEATURE: blends out the error instead of snapping to it.
	
	//Not especially useful with 60 network ticks per second 
	//    (except it can help with smoothness when packet loss occurs)
	//More helpful at a lower frequency of updates. ( bandwidth constrained? )
	
	#region instructions
	//TO USE:
	//    Put a networkview on the object.
	//    Set the observed property of the network view to this script.
	//
	//    +Make sure is_enabled is checked in the editor

	//If you're using this on a prefab, be sure to network.instantiate it,
	//OR set up the networkview correspondence manually. 
	//(just network.instantiate it.)
	#endregion
	
	#region vars
	public bool is_enabled = true; //disable sending data
	
	//physics vars
	public Vector3 velocity = new Vector3( 0.0f, 0.0f, 0.0f );
	public Vector3 acceleration = new Vector3( 0.0f, 0.0f, 0.0f );
	#endregion
	
	
	// Use this for initialization
	void Start () 
	{
	
	}
	
	
	// Update is called once per frame
	void Update () 
	{
		//TODO: call physics / update in another script using this script's data.
		this.gameObject.transform.position += velocity * Time.deltaTime;
		velocity += acceleration * Time.deltaTime;
	}
	
	
	#region OnNetworkUpdate
	void OnSerializeNetworkView( BitStream stream, NetworkMessageInfo info )
	{
		//This gets called on every network update tick, 
		//and allows you to send custom data (in primitive form)
		
		//this is a one-to-all communication
		
		//To prevent this from consuming bandwidth, you can disable it
		if ( is_enabled == false )
		{
			return;
		}
		
		//NOTES:
		//  Potential bandwidth optimizations do exist. 
		//  (ie if this does not use angular acceleration, only moves in two dimensions, you need less data)
		
		if ( stream.isWriting )
		{
			//SENDER 
			//(you own the data) (client running the script)
			//Send your data over the network
			
			//set up values to send
			Vector3 pos = this.gameObject.transform.position;
			Vector3 vel = velocity;
			Vector3 acc = acceleration;
			
			Quaternion rotation = this.gameObject.transform.rotation;
			
			#region NetworkWrite
			//PRO TIP: Make sure to read and write in the same order
			stream.Serialize ( ref pos ); //sends pos to other client(s) + server
			stream.Serialize ( ref vel );
			stream.Serialize ( ref acc );
			
			stream.Serialize ( ref rotation );
			#endregion
		}
		else if ( stream.isReading )
		{
			//RECIEVER 
			//(another client (or server) wanting to know the data)
			//update your data based on the incoming "network update" data
			
			//Set up holding variables
			Vector3 pos = new Vector3( 0.0f, 0.0f, 0.0f );
			Vector3 vel = new Vector3( 0.0f, 0.0f, 0.0f );
			Vector3 acc = new Vector3( 0.0f, 0.0f, 0.0f );
			
			Quaternion rotation = new Quaternion( 0.0f, 0.0f, 0.0f, 0.0f );
			
			#region NetworkRead
			//PRO TIP: Make sure to read and write in the same order
			stream.Serialize ( ref pos ); //sets pos to network data
			stream.Serialize ( ref vel );
			stream.Serialize ( ref acc );
			
			stream.Serialize ( ref rotation );
			#endregion
			
			//Update actual values based on what you read
			this.gameObject.transform.position = new Vector3( pos.x, pos.y, pos.z ); 
			//we can do fancy error blending here based on how far off the pos is from the recieved one
			velocity = vel;
			acceleration = acc;
			
			this.gameObject.transform.rotation = new Quaternion ( rotation.x, rotation.y, rotation.z, rotation.w );
		}
	}
	#endregion
}
