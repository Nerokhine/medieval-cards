using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileController : MonoBehaviour {
	public static GameObject selectedTile = null;
	public static Color32 originalColor = new Color32(255,255,255,20);
	public static Color32 highlightColor = new Color32(255,255,255,100);
	public static Color32 selectedColor = new Color32(255,255,255,150);

	public int x;
	public int z;
	public GameObject tileSpawner;
	public GameObject entity;
	public GameObject leftWall;
	public GameObject rightWall;
	public GameObject topWall;
	public GameObject bottomWall;

	public enum Direction
	{
		Up,
		Down,
		Left,
		Right
	}




	void Start(){
		entity = null;
		tileSpawner = transform.parent.gameObject;
	}

	void OnMouseEnter () {
		Renderer rend = GetComponent<Renderer>();
		rend.material.SetColor("_Color", highlightColor);
	}

	void OnMouseExit(){
		if (selectedTile != gameObject) {
			Renderer rend = GetComponent<Renderer> ();
			rend.material.SetColor ("_Color", originalColor);
		} else {
			Renderer rend = GetComponent<Renderer> ();
			rend.material.SetColor ("_Color", selectedColor);
		}
	}

	void OnMouseOver () {
		if (Input.GetMouseButtonDown (0)) { // left click
			if (selectedTile != null) {
				selectedTile.GetComponent<TileController>().Select (false);
			}
			selectedTile = gameObject;
			Select (true);
			if (entity == null) {
				//wallConstruction ();
				playerConstruction ();
			}
		} else if (Input.GetMouseButtonDown (1)) { // right click
			if (entity != null) {
				Destroy (entity);
				Destroy (topWall);
				Destroy (bottomWall);
				Destroy (rightWall);
				Destroy (leftWall);
			}
		}
	}

	void Select(bool select){
		if (select) {
			Renderer rend = GetComponent<Renderer> ();
			rend.material.SetColor ("_Color", selectedColor);
		} else {
			Renderer rend = GetComponent<Renderer>();
			rend.material.SetColor("_Color", originalColor);
		}
	}
		

	void wallConstruction(){
		GameObject tileObject = (GameObject)Instantiate (Resources.Load ("buildings/tower"), tileSpawner.transform);
		tileObject.transform.localScale =  new Vector3(10f, 10f, 10f);
		entity = tileObject;

		if (x < Initial.dimX - 1) {
			TileController rightObject = Initial.tiles [x + 1, z].GetComponentInChildren<TileController> ();
			if (rightObject.entity != null) {
				rightWall = (GameObject)Instantiate (Resources.Load ("buildings/tower"), tileSpawner.transform);
				rightWall.transform.localScale = new Vector3 (15.5f, 10f, 10f);
				rightWall.transform.localPosition = new Vector3 (
					0.5f,
					0,
					0
				);
				rightObject.leftWall = rightWall;
			}
		}

		if (x > 0) {
			TileController leftObject = Initial.tiles [x - 1, z].GetComponentInChildren<TileController> ();
			if (leftObject.entity != null) {
				leftWall = (GameObject)Instantiate (Resources.Load ("buildings/tower"), tileSpawner.transform);
				leftWall.transform.localScale = new Vector3 (15.5f, 10f, 10f);
				leftWall.transform.localPosition = new Vector3 (
					-0.5f,
					0,
					0
				);
				leftObject.rightWall = leftWall;
			}
		}

		if (z < Initial.dimZ - 1) {
			TileController topObject = Initial.tiles [x, z + 1].GetComponentInChildren<TileController> ();
			if (topObject.entity != null) {
				topWall = (GameObject)Instantiate (Resources.Load ("buildings/tower"), tileSpawner.transform);
				topWall.transform.localScale = new Vector3 (10f, 15.5f, 10f);
				topWall.transform.localPosition = new Vector3 (
					0,
					0,
					0.5f
				);
				topObject.bottomWall = topWall;
			}
		}

		if (z > 0) {
			TileController bottomObject = Initial.tiles [x, z - 1].GetComponentInChildren<TileController> ();
			if (bottomObject.entity != null) {
				bottomWall = (GameObject)Instantiate (Resources.Load ("buildings/tower"), tileSpawner.transform);
				bottomWall.transform.localScale = new Vector3 (10f, 15.5f, 10f);
				bottomWall.transform.localPosition = new Vector3 (
					0,
					0,
					-0.5f
				);
				bottomObject.topWall = bottomWall;
			}
		}
	}

	void playerConstruction(){
		GameObject tileObject = (GameObject)Instantiate (Resources.Load ("characters/Prefabs/Char_2"), tileSpawner.transform);
		tileObject.transform.localScale =  new Vector3(0.2f, 0.2f, 0.2f);
		entity = tileObject;
		List<Vector2> moveList = new List<Vector2>();
		moveList.Add(new Vector2(x + 1, z));
		moveList.Add(new Vector2(x + 2, z));
		moveList.Add(new Vector2(x + 2, z - 1));
		moveList.Add(new Vector2(x + 1, z - 1));
		moveList.Add(new Vector2(x + 1, z));
		moveList.Add(new Vector2(x + 1, z + 1));
		//moveList.Add(new Vector2(x + 2, z - 2));
		GameObject movingEntity = entity;
		moveTo (moveList, movingEntity);
	}

	void moveTo(List<Vector2> moveList, GameObject movingEntity){
		StartCoroutine (moveToCoroutine(moveList, movingEntity));
	}

	bool fin = false;

	public void SetFinTrue(){
		fin = true;
	}
	public delegate void Fin();

	IEnumerator moveToCoroutine(List<Vector2> moveList, GameObject movingEntity){
		foreach (Vector2 vector in moveList) {
			TileController controller = movingEntity.transform.parent.GetComponentInChildren<TileController> ();
			if (vector.x == controller.x && vector.y < controller.z) {
				controller.movePlayer (Direction.Down, SetFinTrue);
			} else if (vector.x == controller.x && vector.y > controller.z) {
				controller.movePlayer (Direction.Up, SetFinTrue);
			} else if (vector.x < controller.x && vector.y == controller.z) {
				controller.movePlayer (Direction.Left, SetFinTrue);
			} else if (vector.x > controller.x && vector.y == controller.z) {
				controller.movePlayer (Direction.Right, SetFinTrue);
			} else {
				Debug.Log ("Bad vector input");
			}
			while (!fin) {
				yield return new WaitForSeconds (0.01f);
			}
			fin = false;
		}
		yield return null;
	}


	void movePlayer(Direction direction, Fin fin){
		StartCoroutine(move(direction, fin));
	}

	TileController getController(int x1, int z1){
		return Initial.tiles [x1, z1].GetComponentInChildren<TileController> ();
	}

	bool moveCondition(Vector3 position, Direction direction){
		if (direction == Direction.Up) {
			if (position.z < 0)
				return true;
		} else if (direction == Direction.Down) {
			if (position.z > 0)
				return true;
		} else if (direction == Direction.Left) {
			if (position.x > 0)
				return true;
		} else if (direction == Direction.Right) {
			if (position.x < 0)
				return true;
		}
		return false;
	}

	//public static bool fin = false;
	IEnumerator move(Direction direction, Fin fin){
		int xAmt = 0;
		int zAmt = 0;
		float rotationAmt = 0;

		if (direction == Direction.Up) {
			zAmt = 1;
		} else if (direction == Direction.Down) {
			zAmt = -1;
			rotationAmt = 180f;
		} else if (direction == Direction.Left) {
			xAmt = -1;
			rotationAmt = 270f;
		} else if (direction == Direction.Right) {
			xAmt = 1;
			rotationAmt = 90f;
		}

		getController (x + xAmt, z + zAmt).entity = entity;
		getController (x + xAmt, z + zAmt).entity.transform.parent = Initial.tiles [x + xAmt, z + zAmt].transform;
		entity = null;
		getController (x + xAmt, z + zAmt).entity.transform.eulerAngles = new Vector3 (0, rotationAmt);

		while (moveCondition(getController (x + xAmt, z + zAmt).entity.transform.localPosition, direction)) {
			Initial.tiles [x + xAmt, z + zAmt].GetComponentInChildren<Animator>().SetFloat("Walk",1);
			Initial.tiles [x + xAmt, z + zAmt].GetComponentInChildren<Animator> ().SetFloat ("Run", 0.2f);
			yield return new WaitForSeconds(0.01f);
		}

		Initial.tiles [x + xAmt, z + zAmt].GetComponentInChildren<Animator>().SetFloat("Walk",0);
		Initial.tiles [x + xAmt, z + zAmt].GetComponentInChildren<Animator> ().SetFloat ("Run", 0f);

		while (getController (x + xAmt, z + zAmt).entity.transform.localPosition.z != 0 && 
			getController (x + xAmt, z + zAmt).entity.transform.localPosition.x != 0) {
			getController(x + xAmt,z + zAmt).entity.transform.localPosition = new Vector3 (0f, 0f, 0f);
			yield return new WaitForSeconds(0.01f);
		}
		//Initial.tiles [x + xAmt, z + zAmt].GetComponentInChildren<Animator> ().enabled = true;
	
		fin ();
		yield return null;
	}




}
