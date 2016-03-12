using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

public class Pathfinding : Singleton<Pathfinding> 
{
	//The width of the map
	private int mapWidth;
	//The depth of the map
	private int mapDepth;
	//Density of tiles
	private float tileSize;
	//2D array of nodes representing the map A* uses
	private Tile[,] theGrid;
	//Open list of tiles for A*
	private Tile[] _open;
	//Number of items on the open list used for heapSort
	private int _numberOfOpenListItems;
	//The closed list of tile for A*
	private ArrayList _closed;
	//Tile the path starts from
	private Tile startTile;
	//Tile that a path is needed to get to
	private Tile targetTile;
	//start tile for temporary use
	private Tile s;
	//target tile for temporary use
	private Tile t;
	//Sorts the open list to get best node
	private HeapSort heapSort;
	// the gameObject that called the calculatePath method
	public GameObject pathfinder;
	
	// accessors
	public int MapWidth{get {return mapWidth;}}
	public int MapDepth{get {return mapDepth;}}
	public float TileSize{get{return tileSize;}}
	public Tile[,] TheGrid{get {return theGrid;}}	
	
	void OnDrawGizmos()
	{
		if (!Constants.ENABLE_TILES) return;

		if (theGrid != null)
		{
			for (int i = 0; i < theGrid.GetLength(0); i++)
			{
				for (int j = 0; j < theGrid.GetLength(1); j++)
				{
					if (theGrid[i, j] != null) 
					{
						if (theGrid[i, j].currentColor != theGrid[i, j].originalColor)
						{
							Gizmos.color = theGrid[i, j].currentColor;
							Gizmos.DrawCube(theGrid[i, j].position, new Vector3(theGrid[i, j].size, 0.1f, theGrid[i, j].size));
						}

						if (Constants.SHOW_ENTIRE_GRID || theGrid[i, j].walkable == false)
						{							
							Gizmos.color = theGrid[i, j].originalColor;
							Gizmos.DrawCube(theGrid[i, j].position, new Vector3(theGrid[i, j].size, 0.1f, theGrid[i, j].size));
						}
					}
				}
			}
		}
	}

	void Start ()
	{
	}

	public void SetupMap()
	{
		heapSort = new HeapSort();

		// Initialize the lists
		_open = new Tile[62500];
		_closed = new ArrayList();
		_numberOfOpenListItems = 0;
		
		// Create the grid for A*
		CreateGrid();
	}

	#region Grid
	void CreateGrid()
	{
		string loadedLevel = Application.loadedLevelName;

		if (loadedLevel == "Inn")
		{
			mapWidth = 30;
			mapDepth = 40;
			tileSize = .4f;
		}
		else if (loadedLevel.Contains("House"))
		{
			mapWidth = 25;
			mapDepth = 25;
			tileSize = .4f;
		}
		else
		{
			mapWidth = 150;
			mapDepth = 120;
			tileSize = .8f;
		}

		// Loops throught the size of the grid and creates tiles at each point.
		theGrid = new Tile[(int)(mapWidth / tileSize), (int)(mapDepth / tileSize)];
		for(int i = 0; i < (int)(mapWidth / tileSize); i++)
		{
			for(int j = 0; j < (int)(mapDepth / tileSize); j++)
			{
				//Creates a new tile at a certain location.
				theGrid[i, j] = new Tile();
				theGrid[i, j].iIndex = i;
				theGrid[i, j].jIndex = j;

				theGrid[i, j].currentList = Tile.List.None;
				theGrid[i, j].walkable = true;
				
				//Position of each tile
				Vector3 tempPosition = new Vector3(i * tileSize, 0, j * tileSize); // y is arbitrary
				if (Terrain.activeTerrain != null)
				{
					tempPosition.y = Terrain.activeTerrain.SampleHeight(tempPosition);
					
					//Height restrictions high points
					if(tempPosition.y >= 10.1)
					{
						theGrid[i, j].walkable = false;
						if(Constants.ENABLE_TILES)
						{
							theGrid[i, j].SetColor(Color.black);
							theGrid[i, j].originalColor = Color.black;
						}
					}
					//Height restrictions low points
					if(tempPosition.y <= 9.9)
					{
						theGrid[i, j].walkable = false;
						if(Constants.ENABLE_TILES)
						{
							theGrid[i, j].SetColor(Color.black);
							theGrid[i, j].originalColor = Color.black;
						}
					}
				}
				
				// Move the position to the center
				theGrid[i, j].position = new Vector3(tempPosition.x + tileSize / 2, tempPosition.y, tempPosition.z + tileSize / 2);
				theGrid[i, j].size = tileSize;
			}
		}
	}
	
	#endregion
	
	/// <summary>
	/// Overloaded function for passing in vector3
	/// instead of a tile. Allows for easier use later.
	/// </summary>
	/// <param name="startPos">
	/// A <see cref="Vector3"/>
	/// </param>
	/// <param name="targetPos">
	/// A <see cref="Vector3"/>
	/// </param>
	/// <returns>
	/// A <see cref="ArrayList"/>
	/// </returns>
	public ArrayList CalculatePath(Vector3 startPos, Vector3 targetPos)
	{
		//Turning positions into tiles
		Tile sTile = GetTileAt(Mathf.FloorToInt(startPos.x/tileSize),  Mathf.FloorToInt(startPos.z/tileSize));
		Tile tTile = GetTileAt(Mathf.FloorToInt(targetPos.x/tileSize),  Mathf.FloorToInt(targetPos.z/tileSize));		
		
		return CalculatePath(sTile, tTile);
	}
	
	/// <summary>
	/// Overloaded CalculatesPath allowing for easier use of the class
	/// </summary>
	/// <returns>
	/// The path from start to end tiles
	/// </returns>
	/// <param name='start'>
	/// Starting tile
	/// </param>
	/// <param name='target'>
	/// Target tile
	/// </param>
	public ArrayList CalculatePath(Tile start, Tile target)
	{
		s = start;
		t = target;
		return CalculatePath();
	}
	
	/// <summary>
	/// Calculates the path.
	/// </summary>
	/// <returns>
	/// The path.
	/// </returns>
	public ArrayList CalculatePath()
	{
		uint limitCount = 0;

		// Find close tiles in case positions arent walkable.
		startTile = FindNearestWalkableTile(s);
		targetTile = FindNearestWalkableTile(t);
		
		// Dont check if standing at target
		if (startTile == targetTile)
		{
			ArrayList oneTilePath = new ArrayList(); 
			oneTilePath.Add(startTile);
			return oneTilePath;
		}
		
		// Clear lists from last check.
		ResetTiles();
			
		// Add starting tile
		startTile.currentList = Tile.List.Open;
		_open[_numberOfOpenListItems] = startTile;
		_numberOfOpenListItems++;
						
		// Preparing for the loop
		Tile currentTile = startTile;
		
		// If open list is 0, no path exists
		while (_numberOfOpenListItems > 0)
		{
			limitCount++;

			// Target tile was found meaning path was found.
			if (targetTile.currentList == Tile.List.Open)
			{
				return PathReached(currentTile);
			}	
			else if (limitCount >= 10000)
			{
				// throw new Exception ("Limit reached error in CalculatePath()");
				return LimitReached(currentTile);
			}
			
			// Add 8 surrounding tiles
			if (currentTile.iIndex + 1 < mapWidth / tileSize && currentTile.jIndex + 1 < mapDepth / tileSize &&
			    currentTile.iIndex > 0 && currentTile.jIndex > 0)
			{
				CheckAdjacentSquares(currentTile, targetTile);
			}
			
			// Remove currentTile
			_open[0] = _open[_numberOfOpenListItems - 1];
			_numberOfOpenListItems--;			
			
			// Sort the list
			heapSort.heapSortMethod(_open, 0, _numberOfOpenListItems - 1);
			
			// Set the nextTile to the lowestF
			Tile nextTile = _open[0];
					
			// Setup to loop back through again.
			_closed.Add(currentTile);
			currentTile.currentList = Tile.List.Closed;
			currentTile = nextTile;
		} // End while
		
		/************************
		 * NO PATH FOUND
		 ************************/
		Debug.LogWarning ("No path found!"); // If this happens, it could be because the game object we're looking for is on a tile that is on the edge of the grid
		ArrayList noPath = new ArrayList();
		noPath.Add(startTile);
		startTile.currentList = Tile.List.None;
		
		return noPath;
	}
	
	private ArrayList PathReached(Tile currentTile)
	{
		//Adds the currentTile to the closed list
		_closed.Add(currentTile);
		currentTile.currentList = Tile.List.Closed;
		
		//Setting up the path to be followed
		Tile temp = targetTile;
		ArrayList victoryPath = new ArrayList();
		
		//Loops back from the parent to create a path
		while (temp.parent != null)
		{
			victoryPath.Add(temp);
			temp = temp.parent;
		}
		
		//Flips the list for use.
		victoryPath.Reverse();
		
		return victoryPath;
	}
	
	private ArrayList LimitReached(Tile currentTile)
	{
		Tile temp = currentTile;
		ArrayList victoryPath = new ArrayList();
		
		//Loops back from the parent to create a path
		while(temp.parent != null)
		{
			victoryPath.Add(temp);
			temp = temp.parent;
		}
		
		//Flips the list for use.
		victoryPath.Reverse();
		
		return victoryPath;
	}

	#region CheckAdjecentSquares
	private void SetupAdjacentTile(Tile tile, Tile current, Tile target)
	{
		tile.parent = current;
		tile.currentList = Tile.List.Open;
		tile.H = Mathf.Abs(tile.iIndex - target.iIndex) + Mathf.Abs(tile.jIndex - target.jIndex);
		tile.F = tile.G + tile.H;
		tile.F = (float)((int)(tile.F * 10 + 0.5)) / 10; // round to nearest tenth
		_open[_numberOfOpenListItems] = tile;
		_numberOfOpenListItems++;
	}
	
	
	//private int index
	//0 , 1
	//0, -1
	//1, 0
	//-1, 0
	public void CheckAdjacentSquares(Tile currentTile, Tile target)
	{
		Tile centerTop = theGrid[currentTile.iIndex, currentTile.jIndex+1];
		if(centerTop.currentList != Tile.List.Closed && centerTop.walkable)
		{
			if (centerTop.currentList == Tile.List.Open)
			{
				if (currentTile.G + 1 < centerTop.G)
				{
					centerTop.G = currentTile.G + 1;
					SetupAdjacentTile(centerTop,currentTile,target);
				}
			}
			else
			{			
				centerTop.G = currentTile.G + 1;
				SetupAdjacentTile(centerTop,currentTile,target);
			}
		}
	
		Tile centerBottom = theGrid[currentTile.iIndex, currentTile.jIndex-1];
		if(centerBottom.currentList != Tile.List.Closed && centerBottom.walkable)
		{
			if (centerBottom.currentList == Tile.List.Open)
			{
				if (currentTile.G + 1 < centerBottom.G)
				{
					centerBottom.G = currentTile.G + 1;
					SetupAdjacentTile(centerBottom,currentTile,target);
				}
			}
			else
			{
				centerBottom.G = currentTile.G + 1;
				SetupAdjacentTile(centerBottom,currentTile,target);
			}
		}
	
		Tile centerRight = theGrid[currentTile.iIndex+1, currentTile.jIndex];
		if(centerRight.currentList != Tile.List.Closed && centerRight.walkable)
		{
			if (centerRight.currentList == Tile.List.Open)
			{
				if (currentTile.G + 1 < centerRight.G)
				{
					centerRight.G = currentTile.G + 1;
					SetupAdjacentTile(centerRight,currentTile,target);
				}
			}
			else
			{
				centerRight.G = currentTile.G + 1;
				SetupAdjacentTile(centerRight,currentTile,target);
			}
		}
	
		Tile centerLeft = theGrid[currentTile.iIndex-1, currentTile.jIndex];
		if(centerLeft.currentList != Tile.List.Closed && centerLeft.walkable)
		{
			if (centerLeft.currentList == Tile.List.Open)
			{
				if (currentTile.G + 1 < centerLeft.G)
				{
					centerLeft.G = currentTile.G + 1;
					SetupAdjacentTile(centerLeft,currentTile,target);
				}
			}
			else
			{
				centerLeft.G = currentTile.G + 1;
				SetupAdjacentTile(centerLeft,currentTile,target);
			}
		}
		
		if(centerLeft.walkable && centerTop.walkable)
		{
			Tile topLeft = theGrid[currentTile.iIndex-1, currentTile.jIndex+1];
			if(topLeft.currentList != Tile.List.Closed && topLeft.walkable)
			{
				if (topLeft.currentList == Tile.List.Open)
				{
					if (currentTile.G + 1 < topLeft.G)
					{
						topLeft.G = currentTile.G + 1;
						SetupAdjacentTile(topLeft,currentTile,target);
					}
				}
				else
				{
					topLeft.G = currentTile.G + 1.4f;
					SetupAdjacentTile(topLeft,currentTile,target);
				}
			}
		}
		
		if(centerBottom.walkable && centerLeft.walkable)
		{
			Tile bottomLeft = theGrid[currentTile.iIndex-1, currentTile.jIndex-1];
			if(bottomLeft.currentList != Tile.List.Closed && bottomLeft.walkable)
			{
				if (bottomLeft.currentList == Tile.List.Open)
				{
					if (currentTile.G + 1 < bottomLeft.G)
					{
						bottomLeft.G = currentTile.G + 1;
						SetupAdjacentTile(bottomLeft,currentTile,target);
					}
				}
				else
				{
					bottomLeft.G = currentTile.G + 1.4f;
					SetupAdjacentTile(bottomLeft,currentTile,target);
				}
			}
		}
		
		if(centerTop.walkable && centerRight.walkable)
		{
			Tile topRight = theGrid[currentTile.iIndex+1, currentTile.jIndex+1];
			if(topRight.currentList != Tile.List.Closed && topRight.walkable)
			{
				if (topRight.currentList == Tile.List.Open)
				{
					if (currentTile.G + 1 < topRight.G)
					{
						topRight.G = currentTile.G + 1;
						SetupAdjacentTile(topRight,currentTile,target);
					}
				}
				else
				{
					topRight.G = currentTile.G + 1.4f;
					SetupAdjacentTile(topRight,currentTile,target);
				}
			}
		}
		
		if(centerBottom.walkable && centerRight.walkable)
		{
			Tile bottomRight = theGrid[currentTile.iIndex+1, currentTile.jIndex-1];
			if(bottomRight.currentList != Tile.List.Closed && bottomRight.walkable)
			{
				if (bottomRight.currentList == Tile.List.Open)
				{
					if (currentTile.G + 1 < bottomRight.G)
					{
						bottomRight.G = currentTile.G + 1;
						SetupAdjacentTile(bottomRight,currentTile,target);
					}
				}
				else
				{
					bottomRight.G = currentTile.G + 1.4f;
					SetupAdjacentTile(bottomRight,currentTile,target);
				}
				
			}
		}
	}
	#endregion
	
	
	#region NearestWalkableTile
	public Tile FindNearestWalkableTile(Tile tileCurrentlyOn)
	{		
		// Clear the lists from the last check
		ArrayList openList = new ArrayList();
		ArrayList closedList = new ArrayList();
		ResetTiles();
		
		if (tileCurrentlyOn.walkable) return tileCurrentlyOn;

		Tile currentTile = tileCurrentlyOn;
		
		openList.Add(currentTile);
		currentTile.currentList = Tile.List.Open;
		
		// If open list is 0, no path exists
		while (openList.Count > 0) 
		{				
			if (currentTile.iIndex + 1 < mapWidth / tileSize && currentTile.jIndex + 1 < mapDepth / tileSize &&
		    currentTile.iIndex > 0 && currentTile.jIndex > 0)
			{
				Tile testTile = theGrid[currentTile.iIndex-1, currentTile.jIndex];
				AddTileToOpen(openList, testTile);
				
				testTile = theGrid[currentTile.iIndex-1, currentTile.jIndex+1];
				AddTileToOpen(openList, testTile);
				
				testTile = theGrid[currentTile.iIndex, currentTile.jIndex-1];
				AddTileToOpen(openList, testTile);
				
				testTile = theGrid[currentTile.iIndex-1, currentTile.jIndex-1];
				AddTileToOpen(openList, testTile);
				
				testTile = theGrid[currentTile.iIndex+1, currentTile.jIndex+1];
				AddTileToOpen(openList, testTile);
				
				testTile = theGrid[currentTile.iIndex, currentTile.jIndex+1];
				AddTileToOpen(openList, testTile);
				
				testTile = theGrid[currentTile.iIndex+1, currentTile.jIndex-1];
				AddTileToOpen(openList, testTile);
				
				testTile = theGrid[currentTile.iIndex+1, currentTile.jIndex];
				AddTileToOpen(openList, testTile);
			}
			
			Tile closestWalkableTile = (Tile)tileCurrentlyOn;
			float distanceToClosestWalkableTile = float.MaxValue;
			foreach(Tile tile in openList) // if tile is walkable and closest, return it
			{
				if (tile.walkable)
				{
					float tempDist = Vector3.Distance(tile.position, pathfinder.transform.position);
					if (tempDist < distanceToClosestWalkableTile)
					{
						closestWalkableTile = tile;
						distanceToClosestWalkableTile = tempDist;
					}
				}
			}
			
			// if there was a tile from the distance check that was walkable, return it
			if (distanceToClosestWalkableTile != float.MaxValue)
			{				
				foreach(Tile o in openList)
				{
					o.currentList = Tile.List.None;
					o.parent = null;			
				}
				
				foreach(Tile c in closedList)
				{
					c.currentList = Tile.List.None;
					c.parent = null;			
				}
				
				return closestWalkableTile;
			}
			
			// else we continue looking on the next tier of nodes
			openList.Remove(currentTile);
			closedList.Add(currentTile);
			currentTile.currentList = Tile.List.Closed;
			if (openList.Count == 0)
			{
				return tileCurrentlyOn;
			}
			currentTile = (Tile)openList[0];
		}

		return tileCurrentlyOn;
	
	}

	void AddTileToOpen(ArrayList openList, Tile testTile)
	{
		if (testTile.currentList != Tile.List.Open && testTile.currentList != Tile.List.Closed)
		{
			openList.Add(testTile);
			testTile.currentList = Tile.List.Open;
		}
	}
	#endregion
	
	public Tile FindNearestTileUnderObject(GameObject obj)
	{
		// find all tiles under the object
		ArrayList tiles = new ArrayList();
		for (int i = 0; i < mapWidth / tileSize; i++)
		{
			for (int j = 0; j < mapDepth / tileSize; j++)
			{
				if (theGrid[i, j].objectOnThisTile == obj)
				{
					tiles.Add(theGrid[i, j]);
				}
			}
		}
		
		// find the closest tile under the object (will be unwalkable)
		Tile closestTileUnderObj = (Tile)tiles[0];
		float distanceToClosestWalkableTile = float.MaxValue;
		foreach (Tile tile in tiles)
		{
			float tempDist = Vector3.Distance(tile.position, pathfinder.transform.position);
			if (tempDist < distanceToClosestWalkableTile)
			{
				closestTileUnderObj = tile;
				distanceToClosestWalkableTile = tempDist;
			}
		}
		
		// find closest tile under the object and return it
		return closestTileUnderObj;
	}

	/// <summary>
	/// Gets the tile at BaseObject. Uses the ObjectNode if its an InanimateObject that has a node set
	/// </summary>
	/// <returns>The <see cref="Tile"/>.</returns>
	/// <param name="baseObject">Base object.</param>
	public Tile GetTileAt(BaseObject baseObject)
	{
		if (baseObject is InanimateObject && ((InanimateObject)baseObject).nodes.Count > 0)
		{
			return GetTileAt(baseObject.nodes[0].transform.position);
		}

		return GetTileAt(baseObject.transform.position);
	}
	
	/// <summary>
	/// Gets the tile at position.
	/// </summary>
	/// <returns>
	/// The <see cref="Tile"/>.
	/// </returns>
	/// <param name='position'>
	/// Position.
	/// </param>
	public Tile GetTileAt(Vector3 position)
	{
		return theGrid[Mathf.FloorToInt(position.x / tileSize), Mathf.FloorToInt(position.z / tileSize)];
	}

	/// <summary>
	/// Gets the tile at xValue and zValue.
	/// </summary>
	/// <returns>
	/// The <see cref="Tile"/>.
	/// </returns>
	/// <param name='xValue'>
	/// X value.
	/// </param>
	/// <param name='zValue'>
	/// Z value.
	/// </param>
	public Tile GetTileAt(int xValue, int zValue)
	{
		return theGrid[xValue, zValue];
	}	
	
	/// <summary>
	/// Resets the tiles.
	/// </summary>
	public void ResetTiles()
	{		
		for(int i = 0; i < _numberOfOpenListItems;i++)
		{
			_open[i].currentList = Tile.List.None;
			_open[i].parent = null;
			//_open[i].ColorSphere(_open[i].originalColor);
		}
		
		foreach(Tile tile in _closed)
		{
			tile.currentList = Tile.List.None;
			tile.parent = null;			
			//tile.ColorSphere(tile.originalColor);
		}
		
		_closed.Clear();
		_numberOfOpenListItems = 0;
	}
		
	// Sets the walkability of tiles under the object passed in
	public List<Tile> SetWalkabilityOfTilesUnderObject(GameObject obs, bool walkability)
	{
		List<Tile> tilesUnderObject = new List<Tile>();

		Collider[] colliders = obs.GetComponentsInChildren<Collider>();
		foreach (Collider c in colliders)
		{
			Bounds bounds = c.bounds;
			Vector3 center = bounds.center;
			Vector3 extents = bounds.extents;
			Vector3 topRight = new Vector3(center.x + extents.x, 0, center.z + extents.z);
			//Vector3 topLeft = new Vector3(center.x - extents.x, 0, center.z + extents.z);
			//Vector3 bottomRight = new Vector3(center.x + extents.x, 0, center.z - extents.z);
			Vector3 bottomLeft = new Vector3(center.x - extents.x, 0, center.z - extents.z);
			
			// Mark the tiles the obstacle sits on as not walkable
			tilesUnderObject.AddRange(ChangeNav(bottomLeft.x,
								                   topRight.x,
								                   bottomLeft.z,
								                   topRight.z,
								                   walkability, obs));
		}

		return tilesUnderObject;
	}
		
	/* Changes the navigability between the positions passed in
	 * on the grid to either walkable or un-walkable
	 */
	List<Tile> ChangeNav(float _startX, float _endX, float _startZ, float _endZ, bool navigable, GameObject obs)
	{
		List<Tile> tiles = new List<Tile>();

		int startX  = (int)(_startX / tileSize);
		int endX  = (int)(_endX / tileSize);
		int startZ = (int)(_startZ / tileSize);
		int endZ = (int)(_endZ / tileSize);

		for(int i = startX; i <= endX; i++)
		{
			for(int j = startZ; j <= endZ; j++)
			{
				theGrid[i, j].walkable = navigable;
				
				if (navigable)
				{
					theGrid[i, j].objectOnThisTile = null;
					
					theGrid[i, j].SetColor(Color.white);
					theGrid[i, j].originalColor = Color.white;
				}
				else
				{
					theGrid[i, j].objectOnThisTile = obs;
					
					theGrid[i, j].SetColor(Color.black);
					theGrid[i, j].originalColor = Color.black;
				}
				
				tiles.Add (theGrid[i, j]);
			}
		}

		return tiles;
	}
	
	public bool TestNavigability(Vector3 topLeftCorner, Vector3 bottomRightCorner)
	{
		//Positions to stay between.
		int startX  = (int)(bottomRightCorner.x / tileSize);
		int endX  = (int)(topLeftCorner.x / tileSize);
		int startZ = (int)(bottomRightCorner.z / tileSize);
		int endZ = (int)(topLeftCorner.z / tileSize);
		for(int i = startX; i <= endX; i++)
		{
			for(int j = startZ; j <= endZ; j++)
			{
				if(!theGrid[i, j].walkable)
				{
					return false;
				}
				
			}
		}
		return true;
	}	
}
	
