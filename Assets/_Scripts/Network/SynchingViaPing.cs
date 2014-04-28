using UnityEngine;
using System.Collections;

using System.Reflection; //for string -> method name + invokation resolution

public class SynchingViaPing : MonoBehaviour 
{
	//This one aims to introduce a scheduling delay to everything visible (problem?)
	//so that everything waits the highest ping in the system before proceeding.
	//This SHOULD ensure ~ perfect synch.
	
	//PROBLEMS:
	//    hitting everything with this 
	//        (probably will require a highest ping storer and separate puller + delayer)
	//    not interrupting continuous tasks
	//        (ie camera moves, rather than breaking up continuous motion, need to just tack on a front end delay.)
	//         buffering? FIFO
	
	//This one's application specific b/c of 3 networked cameras.
	
	//Immediately send, stall on every recieve on each client.
	//player.ipAddress; //we need to store ip + send ping to single clients?
	//then we just pass delay, subtract ping, if <= 0, go!
	//else, wait a frame, subtract time.deltatime until <= 0.
		
	//ALT. FORM:
	//schedule once, serverside (per event)
	//we delay sending the rpc until delay >= elapsed time + ping.
	
	//SO, that can delay RPCs and synch them.
	//What about data?
	
	//interpolation: we store a few frames in the past, then act on the data once greatest ping has been reached.
	//not quite right, account for delay too.
	//Ping changes can cause issues.
	
	//THEN, there's the possibility the connection's good enough we don't even have to worry about this,
	//and all this does is needlessly complicate a 1 frame delay?

	
	#region vars
	ArrayList RPC_data_queue = new ArrayList(); //analogue to a vector of <RPCData>.
	
	Ping[] pings = new Ping[3];
	public float[] ping_results = new float[3];
	
	float greatest_ping = 0.0f;

	NetworkPlayer[] network_players = new NetworkPlayer[3];
	int npindex = 0;
	#endregion


	// Use this for initialization
	void Start () 
	{
		#region DEBUG
		//temporary
		//DontDestroyOnLoad( this.gameObject );
		//Network.InitializeServer ( 3, 25000, false );
		#endregion


		if ( Network.isServer )
		{
			Debug.Log ("Server.");

			ping_results[0] = 0.0f;
			ping_results[1] = 0.0f;
			ping_results[2] = 0.0f;

			//Add the server to the 3 connections data
			network_players[ npindex ] = Network.player; //on the server, this gets the local player (ie, the server)
			pings[ npindex ] = new Ping( network_players[ npindex ].ipAddress );
			npindex++;

			//Add any already connected clients to the 3 connections data
			foreach ( NetworkPlayer pl in Network.connections )
			{
				network_players[ npindex ] = Network.player;
				pings[ npindex ] = new Ping( network_players[ npindex ].ipAddress );
				npindex++;
			}
			Debug.Log ( npindex );

			ScheduleRPC ( "ExampleFunction", 1.0f, 0 ); //TEST
		}
		else
		{
			//Clients don't need this.
			//They just need the delay? (Depending on implementation they may not need anything at all.)
		}
	}
	
	void OnPlayerConnected( NetworkPlayer player )
	{	
		//Store networkplayers in the 3 connections data on connect.
		network_players[ npindex ] = player;
		//ping them.
		pings[ npindex ] = new Ping( network_players[ npindex ].ipAddress );
		//increment.
		npindex++;
		
		//networkView.RPC ("functionname", NetworkPlayer <Target>, params);
	}
	
	// Update is called once per frame
	void Update ()
	{	
		//Constantly update pings.
		for ( int i = 0; i < npindex; i++ )
		{
			if ( pings[i].isDone )
			{
				ping_results[i] = pings[i].time;
				
				//Check if the greatest ping has changed
				CheckGreatestPing();
				
				//Ping it again! (ping can change)
				string temp_ip = pings[i].ip;
				pings[i].DestroyPing(); //good memory management?
				
				pings[i] = new Ping( temp_ip );
			}
		}
		
		for (int i = 0; i < RPC_data_queue.Count; i++ )
		{
			RPCData RPC_data = (RPCData)RPC_data_queue[i];
			((RPCData)RPC_data_queue[i]).delay -= Time.deltaTime;
			//abstraction inefficiency
			float delay = RPC_data.delay;
			float ping = ping_results[ RPC_data.id ];
			
			if ( ping <= delay )
			{
				//SEND THE MESSAGE! NAO!
				//get message data (abstraction inefficiency)
				string function_name = RPC_data.function_name;
				NetworkPlayer target = network_players[ RPC_data.id ];
				object[] args = RPC_data.args;
				
				//Send the message
				if ( RPC_data.id != 0 ) //not self-referential
				{
					networkView.RPC ( function_name, target, args );
				}
				else
				{
					//call it locally from string.

					#region reflection
					//APPROACH 1: (using reflection)
					/*
					Type a_type = Type.GetType("SynchingViaPing");
					System.Type a_type = System.Type.GetType("SynchingViaPing");
					MethodInfo a_method = a_type.GetMethod( function_name );
					a_method.Invoke( this, args );
					*/
					#endregion

					#region invokation
					//APPROACH 2: Using invokation. No parameters can be passed, though
					//Invoke ( function_name, 0.0f ); //call function now
					#endregion

					#region coroutine
					//APPROACH 3: using coroutine. Parameters can be passed.
					StartCoroutine ( function_name, args );
					#endregion

					//NOTE: to use this, THIS SCRIPT must contain the function to call...
					//Consider abstracting this away to be more general...?

					#region terribad hax
					//APPROACH 4:
					//The hacky way of doing reflection: (this does give you power + control for weird cases)
					/*
					if ( function_name == "ExampleFunction" )
					{
						ExampleFunction ( args );
					}
					else
					{
						Debug.LogError ( "Bad RPC function name!" );
					}
					*/
					#endregion
				}
				
				//Mark request as processed.
				((RPCData) RPC_data_queue[i]).tagged_for_deletion = true;
			}
		}
		
		//Delete items tagged for deletion (processed requests)
		for ( int i = RPC_data_queue.Count - 1; i >= 0; i-- )
		{
			if ( ((RPCData) RPC_data_queue[i]).tagged_for_deletion )
			{
				RPC_data_queue.RemoveAt(i);
			}
		}
	}
	
	void CheckGreatestPing()
	{
		//Finds the greatest ping in the system.
		float temp_greatest = ping_results[0];
		
		for ( int i = 0; i < npindex; i++ )
		{
			if ( ping_results[ i ] > temp_greatest )
			{
				temp_greatest = ping_results[ i ];
			}
		}
		
		greatest_ping = temp_greatest;
	}

	public void ScheduleRPC( string function_name, float delay, params object[] args )
	{
		//Function that will call function_name with args on all screens, after delay.
		//Use this to schedule functions.
		for ( int i = 0; i < npindex; i++ ) //for each connection,
		{
			RPCData temp_RPC = new RPCData( function_name, delay, i );
			temp_RPC.args = args;

			RPC_data_queue.Add ( temp_RPC );
		}
	}

	[RPC]
	IEnumerator ExampleFunction( params object[] args )
	{
		//int i = (int)args[0];
		//Debug.Log ( "Success" );
		//Debug.Log ( args );
		yield return null;
	}
	/*public void ExampleFunction( /*params object[] args*//* ) //can an RPC take this style of args?
	{
		//We probably should improve the 0 param thing.
		Debug.Log( "Success." );
	}*/

	/*
	void ExampleDelayedRPC( string function_name, NetworkPlayer player, params object[] args )
	{
		//I should have a vector of classes with function names, players, params arrays, and delays
		//then we iterate through it on update, and boom! :D
		//PROBLEM: dispatching that to the appropriate script...
		
		//A) include a copy of this code + update handler in all scripts needing networking?
		//B) ALT. solution...
		
		//So we add it to the vector, for each player (x3), with proper delay.
		//Optimization: message queue it, sort by out time.
		
		//this won't exactly work b/c of the way netviews and RPCs must correspond.
		networkView.RPC ( function_name, player, args ); 
	}
	*/
	
	public class RPCData
	{
		#region vars
		public string function_name;
		//public NetworkPlayer player; //might want to sort this out, 3 players. (check if server, else dispatch)
		public object[] args;
		public int id; //the index to use with ping_results and network_players arrays to get the ping data + where to send it.
		
		public float delay; //might want to sort this out, 3 pings
		//public float ping;  //this has dynamism.
		
		public bool tagged_for_deletion;
		#endregion

		public RPCData( string function_name, float delay, int id )
		{
			this.function_name = function_name;
			this.delay = delay;
			this.id = id;

			tagged_for_deletion = false;
		}
	}
}
