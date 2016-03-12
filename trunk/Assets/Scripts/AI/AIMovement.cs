using UnityEngine;
using System.Collections;

public class AIMovement : MonoBehaviour {

	private AIManager manager;
	
	// Movement variables
	private Vector3 moveDirection;
	private float speed = 2f;
	private float gravity = 10f;
	
	private Vector3 seekPosition;
	private float arrivalRadius = 0.2f;
	private Vector3 steeringForce;

	// The game object A* is trying to move to
	[HideInInspector] public BaseObject destination;
	// Used to keep track of the current node to move to in A*
	[HideInInspector] public int destinationCount;
	// The A* path to follow
	public ArrayList yellowBrickRoad;
	
	// true if another villager and you are going to collide, but they told you to keep going with your path (they'll move around you)
	// prevents a 'dance off' where villagers would side step each other perfectly in sync
	private bool givenTheHeadNod = false;
	
	void Start () 
	{
		manager = GetComponent<AIManager>();
	}
	
	void OnGUI()
	{
		//GUI.Box(Rect(0, 0, 80, 20), (1.0f / Time.deltaTime).ToString());
	}

	/// <summary>
	/// Assigns the yellowBrickRoad and the seekPosition
	/// </summary>
	/// <param name="obj">Object.</param>
	public void SetPath(BaseObject obj)
	{
		destination = obj;

		if (destination == null)
		{
			throw new UnityException("You tried to SetPath to a null destination");
		}

		// Reset the color of the old yellowBrickRoad
		if (yellowBrickRoad != null)
		{
			for (int i = 0; i < yellowBrickRoad.Count; i++)
			{
				(yellowBrickRoad[i] as Tile).SetColor((yellowBrickRoad[i] as Tile).originalColor);	
			}
		}
		
		// Set this mind's gameObject as the pathfinder
		Pathfinding.Instance.pathfinder = gameObject;
		
		// Find the starting tile
		Tile startTile = Pathfinding.Instance.GetTileAt(transform.position);

		// Find the end tile
		Tile endTile = Pathfinding.Instance.GetTileAt(obj);

		// Calculate a path to the end of the yellowBrickRoad
		yellowBrickRoad = Pathfinding.Instance.CalculatePath(startTile, endTile);
		
		if (yellowBrickRoad.Count != 0)
		{
			// Start from beginning
			destinationCount = 0;
			// Set next target to seek
			seekPosition = (yellowBrickRoad[0] as Tile).position;
		
			for (int i = 0; i < yellowBrickRoad.Count; i++)
			{
				(yellowBrickRoad[i] as Tile).SetColor(Color.yellow);
			}
		}
		else
		{
			Debug.LogError ("YellowBrickRoad has a count of 0 in SetPath()");
		}
	}

	/// <summary>
	/// Updates the seek position each time one is reached. Handles obstacles blocking the way (and recalculates a new path)
	/// </summary>
	public void FollowPath()
	{		
		// When you reach the next tile in the list do some checks and continue to the next one		
		Tile currentTile = (Tile)yellowBrickRoad[destinationCount];		
		
		if (Vector2.Distance(new Vector2(transform.position.x, transform.position.z), new Vector2(currentTile.position.x, currentTile.position.z)) <= arrivalRadius)
		{	
			if (destinationCount + 1 < yellowBrickRoad.Count)
			{
				// Dynamic A* - when an obstacle is encountered recalculate a new path around
				Tile nextTile = (Tile)yellowBrickRoad[destinationCount + 1];
				bool walkabilityChanged = false;
				
				if (!givenTheHeadNod)
				{
					// If a person is in your way, set their tile to not walkable temporarily
					for (int i = 0; i < Game.Instance.allPeople.Count; i++)
					{
						if (Pathfinding.Instance.GetTileAt(Game.Instance.allPeople[i].transform.position) == nextTile)
						{
							// Temporarily change the walkability of the nextTile so that we can find a new path around the person blocking us
							nextTile.walkable = false;
							walkabilityChanged = true;
							
							AIMovement othersMovement  = Game.Instance.allPeople[i].GetComponent<AIMovement>();
							if (othersMovement != null)
							{
								othersMovement.givenTheHeadNod = true;
							}
							break;
						}
					}
				}
				
				if (nextTile.walkable == false)
				{
					// Set a new path
					SetPath(manager.data.CurrentTask.CurrentDestination);
					
					// Reset the walkability if it was temporarily changed above
					if (walkabilityChanged)
					{
						nextTile.walkable = true;
					}
				}
				else
				{
					// color the current tile green
					nextTile.SetColor(Color.green);
					// color the last tile yellow
					currentTile.SetColor(Color.yellow);
					
					// Increment the count and continue to the next spot
					destinationCount++;
					seekPosition = nextTile.position;
					givenTheHeadNod = false;
				}
			}
			else
			{
				destinationCount++;
			}
		}
	}
	
	// Moves the character towards seekPosition
	public void MoveToSeekPosition()
	{
		moveDirection = (seekPosition - transform.position).normalized;
		moveDirection.y = 0;
		transform.forward = moveDirection;

		// Apply gravity
		moveDirection.y -= gravity;

		manager.animation.PlayMove(moveDirection.magnitude * Time.deltaTime * speed);
	}

	/// <summary>
	/// When two villagers from walking into each other, possibly (based off a random chance) redirect your path
	/// The random prevents both villagers from redirecting simultaneously
	/// </summary>
	/// <param name="hit">Hit.</param>
	void OnControllerColliderHit (ControllerColliderHit hit)
	{
		if (destination == null) return;

	 	if (hit.gameObject.tag == "Villager" || hit.gameObject.tag == "Player")
	 	{
	 		int rand = Random.Range(0, 30);
	 		
	 		if (rand == 0)
			{
		 		// Set the tile we are standing on to unwalkable
		 		Pathfinding.Instance.GetTileAt(transform.position).walkable = false;
		 		
				// Set a new path
				SetPath(destination);
				
				// Reset the tile we are standing on to walkable
		 		Pathfinding.Instance.GetTileAt(transform.position).walkable = true;
	 		}
	 	}
}
}
