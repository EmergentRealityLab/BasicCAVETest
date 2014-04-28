private var motor : CharacterMotor;

private var pointer : Transform;

var turnSpeed : float = 60;

// Use this for initialization
function Awake () {
	motor = GetComponent(CharacterMotor);
	
	pointer = GameObject.Find("PointerRoot").transform;
}

// Update is called once per frame
function Update () {
	// Get the input vector from kayboard or analog stick
	var turn: float = Input.GetAxis("Horizontal");
	
	transform.Rotate(0, turn * turnSpeed * Time.deltaTime, 0);
	 
	var directionVector = new Vector3(0, 0, Input.GetAxis("Vertical"));
	
	if (directionVector != Vector3.zero) {
		// Get the length of the directon vector and then normalize it
		// Dividing by the length is cheaper than normalizing when we already have the length anyway
		var directionLength = directionVector.magnitude;
		directionVector = directionVector / directionLength;
		
		// Make sure the length is no bigger than 1
		directionLength = Mathf.Min(1, directionLength);
		
		// Make the input vector more sensitive towards the extremes and less sensitive in the middle
		// This makes it easier to control slow speeds when using analog sticks
		directionLength = directionLength * directionLength;
		
		// Multiply the normalized direction vector by the modified length
		directionVector = directionVector * directionLength;
	}
	
	/* transform using the wand direction */
	
	var matrix = Matrix4x4();
	matrix.SetColumn(0,pointer.right);
	matrix.SetColumn(1,pointer.up);
	matrix.SetColumn(2,pointer.forward);
	
	directionVector = matrix.MultiplyVector(directionVector);
	
	// get pointer forward, up, and right vectors
	// construct matrix
	// transform directionVector using the matrix
	
	// Apply the direction to the CharacterMotor
	motor.inputMoveDirection = directionVector;
	motor.inputJump = Input.GetButton("Jump");
}

// Require a character controller to be attached to the same game object
@script RequireComponent (CharacterMotor)
@script AddComponentMenu ("Character/FPS Input Controller")
