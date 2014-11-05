using UnityEngine;
using System.Collections;

/* 
Class to control the camera within the game world.
Camera will move up, down, left and right when the users mouse hits the side of the screen in 2D space.
Camera will check desired location, will stop if over limits.
Camera can also be controlled by W,A,S,D keys, will call the same movement as the mouse events.
*/


public class WorldCamera : MonoBehaviour {
	
	#region structs
	
	//box limits Struct
	public struct BoxLimit {
		public float LeftLimit;
		public float RightLimit;
		public float TopLimit;
		public float BottomLimit;
		
	}
	
	#endregion
	
	
	#region class variables
	
	public static BoxLimit cameraLimits       = new BoxLimit();
	public static BoxLimit mouseScrollLimits  = new BoxLimit();
	public static WorldCamera Instance;
	public Material mat;
	
	private float cameraMoveSpeed = 60f;
	private float shiftBonus      = 45f;
	private float mouseBoundary   = 25f;
	
	#endregion
	
	
	void Awake()
	{
		Instance = this;
	}
	

	void Start () {
		
		//Declare camera limits
		cameraLimits.LeftLimit   = 10.0f;
		cameraLimits.RightLimit  = 200.0f;
		cameraLimits.TopLimit    = 160.0f;
		cameraLimits.BottomLimit = -20.0f;
		
		//Declare Mouse Scroll Limits
		mouseScrollLimits.LeftLimit   = mouseBoundary;
		mouseScrollLimits.RightLimit  = mouseBoundary;
		mouseScrollLimits.TopLimit    = mouseBoundary;
		mouseScrollLimits.BottomLimit = mouseBoundary;
			
	}
	
	
	
	
	void Update () {
						
		if(CheckIfUserCameraInput())
		{
			Vector3 cameraDesiredMove = GetDesiredTranslation();
			
			if(!isDesiredPositionOverBoundaries(cameraDesiredMove))
			{
				this.transform.Translate(cameraDesiredMove);
			}
		}
	}
	
	
	
	
	//Check if the user is inputting commands for the camera to move
	public bool CheckIfUserCameraInput()
	{
		bool keyboardMove;
		bool mouseMove;
		bool canMove;
		
		//check keyboard		
		if(WorldCamera.AreCameraKeyboardButtonsPressed()){
			keyboardMove = true;			
		} else {
			keyboardMove = false;
		}
		
		//check mouse position
		if(WorldCamera.IsMousePositionWithinBoundaries())
			mouseMove = true; else mouseMove = false;
		
		
		if(keyboardMove || mouseMove)
			canMove = true; else canMove = false;
		
		return canMove;
	}
	
	
	
	
	//Works out the cameras desired location depending on the players input
	public Vector3 GetDesiredTranslation()
	{
		float moveSpeed = 0f;
		float desiredX = 0f;
		float desiredZ = 0f;
		
		if(Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
			moveSpeed = (cameraMoveSpeed + shiftBonus) * Time.deltaTime;
		else
			moveSpeed = cameraMoveSpeed * Time.deltaTime;
		
		
		//move via keyboard
		if(Input.GetKey(KeyCode.W))
			desiredZ = moveSpeed;
		
				
		if(Input.GetKey(KeyCode.S))
			desiredZ = moveSpeed * -1;
		
		
		if(Input.GetKey(KeyCode.A))
			desiredX = moveSpeed * -1;
		
		
		if(Input.GetKey(KeyCode.D))
			desiredX = moveSpeed;
		
		
		
		
		//move via mouse
		if(Input.mousePosition.x < mouseScrollLimits.LeftLimit){
			desiredX = moveSpeed * -1;
		}
		
		if(Input.mousePosition.x > (Screen.width - mouseScrollLimits.RightLimit)){
			desiredX = moveSpeed;
		}
		
		if(Input.mousePosition.y < mouseScrollLimits.BottomLimit){
			desiredZ = moveSpeed * -1;
		}
		
		if(Input.mousePosition.y > (Screen.height - mouseScrollLimits.TopLimit)){
			desiredZ = moveSpeed;
		}
			
		return new Vector3(desiredX, desiredZ, 0);
	}
	
	
	
	
	
	//checks if the desired position crosses boundaries
	public bool isDesiredPositionOverBoundaries(Vector3 desiredPosition)
	{
		bool overBoundaries = false;
		//check boundaries
		if((this.transform.position.x + desiredPosition.x) < cameraLimits.LeftLimit)
			overBoundaries = true;
		
		if((this.transform.position.x + desiredPosition.x) > cameraLimits.RightLimit)
			overBoundaries = true;
			
		if((this.transform.position.z + desiredPosition.z) > cameraLimits.TopLimit)
			overBoundaries = true;
			
		if((this.transform.position.z + desiredPosition.z) < cameraLimits.BottomLimit)
			overBoundaries = true;
		
		return overBoundaries;
	}

	#region Draw Map


	void OnPostRender()
	{
		if (!mat) {
			Debug.LogError("Please Assign a material on the inspector");
			return;
		}

		mat.SetPass (0);

		GL.Begin( GL.LINES );
		
		GL.Color(Color.white);
		

			GL.Vertex3 (0,0,0);
			GL.Vertex3 (0,0,BattleManager.instance.worldSize.y);
			GL.Vertex3 (0,0,BattleManager.instance.worldSize.y);
			GL.Vertex3 (BattleManager.instance.worldSize.x,0,BattleManager.instance.worldSize.y);
			GL.Vertex3 (BattleManager.instance.worldSize.x,0,BattleManager.instance.worldSize.y);
			GL.Vertex3 (BattleManager.instance.worldSize.x,0,0);
			GL.Vertex3 (BattleManager.instance.worldSize.x,0,0);
			GL.Vertex3 (0,0,0);
	
		GL.End();
	}





	#endregion
	
	
	
	
	
	#region Helper functions
		
	public static bool AreCameraKeyboardButtonsPressed()
	{
		if(Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
			return true; else return false;
	}
	
	public static bool IsMousePositionWithinBoundaries()
	{
		if(
			(Input.mousePosition.x < mouseScrollLimits.LeftLimit && Input.mousePosition.x > -5) ||
			(Input.mousePosition.x > (Screen.width - mouseScrollLimits.RightLimit) && Input.mousePosition.x < (Screen.width + 5)) ||
			(Input.mousePosition.y < mouseScrollLimits.BottomLimit && Input.mousePosition.y > -5) ||
			(Input.mousePosition.y > (Screen.height - mouseScrollLimits.TopLimit) && Input.mousePosition.y < (Screen.height + 5))
			)
				return true; else return false;
	}
	#endregion
}
