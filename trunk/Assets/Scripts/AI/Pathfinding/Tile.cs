using UnityEngine;
using System;
using System.Collections;

public class Tile
{	
	public int iIndex;
	public int jIndex;

	public float size;
	public float halfSize
	{
		get { return size / 2f; }
	}
	public Vector3 position;
	public Vector3 bottomLeftCorner
	{
		get { return new Vector3(position.x - halfSize, position.y, position.z - halfSize); }
	}
	public Vector3 bottomRightCorner
	{
		get { return new Vector3(position.x + halfSize, position.y, position.z - halfSize); }
	}
	public Vector3 topLeftCorner
	{
		get { return new Vector3(position.x - halfSize, position.y, position.z + halfSize); }
	}
	public Vector3 topRightCorner
	{
		get { return new Vector3(position.x + halfSize, position.y, position.z + halfSize); }
	}

	public enum List { Open, Closed, None };
	public List currentList;

	public Tile parent; // To find the path

	public float F; // F = G + H
	public float G = 0f; // Cost so far
	public int H; // Heuristic to targetPosition

	public Color originalColor;
	public Color currentColor;

	public GameObject objectOnThisTile;
	public bool walkable;

	public Tile()
	{
		this.originalColor = Color.white;
		this.currentColor = originalColor;
	}
	
	public void SetColor(Color color)
	{
		this.currentColor = color;
	}
}
