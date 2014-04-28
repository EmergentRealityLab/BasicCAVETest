using UnityEngine;
using System.Collections;

public class ExtrapolationExplicitlySynched : MonoBehaviour 
{
	//Script for extrapolation that can be perfectly predicted.
	//Allows you to set up a synch state, 
	//and then each machine can run the code on the object independently,
	//and the object SHOULD be ~ perfectly synched across all the machines.
	
	//You can also use this on objects that are less predictable, 
	//but require relatively infrequent updates. (RPC overhead can be an issue if you spam them)
	
	//PERFECT SYNCH:
	//TODO: delay before (scheduling in a ping delay)
	//TODO: jump ahead by ping on receive

	//alternative: blending out the "error" caused by ping delay over a few frames.
	
	#region vars
	Vector3 velocity = new Vector3( 0.0f, 0.0f, 0.0f );     //velocity:             u/s
	Vector3 acceleration = new Vector3( 0.0f, 0.0f, 0.0f ); //acceleration:         u/s^2
	Vector3 omega = new Vector3( 0.0f, 0.0f, 0.0f );        //angular velocity:     degrees/s
	Vector3 alpha = new Vector3( 0.0f, 0.0f, 0.0f );        //angular acceleration: degrees/s^2
	#endregion
	
	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		this.gameObject.transform.position += velocity * Time.deltaTime;
		velocity += acceleration * Time.deltaTime;
		this.gameObject.transform.rotation = Quaternion.Euler( this.gameObject.transform.rotation.eulerAngles + alpha * Time.deltaTime );
		alpha += omega * Time.deltaTime;
	}
	
	[RPC]
	void MyRPC( Vector3 position_net, Vector3 velocity_net, Vector3 acceleration_net, Quaternion rotation_net, Vector3 omega_net, Vector3 alpha_net )
	{
		//synch position, velocity, acceleration, rotation, omega, alpha?
		
		//TODO: split into component RPCs as well to allow for bandwidth conservation.
		//(sending all params in one RPC call has less overhead)
		//But not sending some of the data (via multiple RPCs) can be better.
		this.gameObject.transform.position = new Vector3( position_net.x, position_net.y, position_net.z );
		velocity = velocity_net;
		acceleration = acceleration_net;
		gameObject.transform.rotation = rotation_net;
		omega = omega_net;
		alpha = alpha_net;
	}
}
