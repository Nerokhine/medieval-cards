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

	//List<Vector2> shortestPath;

	void shortestPath(){
		int[,] array;
		array = new int[Initial.dimX + 2,Initial.dimZ + 2];

		// boundaries of the game
		for(int x = 0; x < Initial.dimX; x ++){
			//horizontal edge
			array[x, 0] = 1;
			array[x,Initial.dimX - 1] = 1;

		}

		for(int x = 0; x < Initial.dimZ; x ++){
			//vertical edge
			array[0, x] = 1;
			array[Initial.dimZ - 1, x] = 1;

		}

		// obstacles in the game
		for (int x = 0; x < Initial.dimX; x++) {
			for (int z = 0; z < Initial.dimZ; z++) {
				if (getController (x, z).entity != null) {
					array [x, z] = 1;
				}
			}
		}


		//shortestPath = new List<Vector2> ();


	}

	/*void shortestPath(int x1, int y1, int x2, int y2, int [][] array){ 

		if(x1 == x2 && y1 == y2){ 
			shortestPath.Add(new Vector2(x1, y1));//;[x1][y1] = 2; 
			return array; 
		}else if ((array.second)[x1][y1] == 1 || (array.second)[x1][y1] == 2){ //cannot go outside bounds 
			array.first = 100000; 
			return array; 
		} else if ((array.first) > 15){ //to make algorithm more efficient, disregard obvious poor path choices 
			array.first = 100000; 
			return array; 
		} 

		int ** newArray; 

		newArray = new int *[dim]; 
		for(int x = 0; x < dim; x ++){ 
			newArray[x] = new int[dim]; 
		} 

		for(int x = 0; x < dim; x ++){ 
			for(int y = 0; y < dim; y ++){ 
				newArray[x][y] = (array.second)[x][y]; 
			} 
		}  
		newArray[x1][y1] = 2; 
		pair<int, int **> myPair = make_pair(array.first + 1, newArray); 

		pair<int, int **> temp1 = shortestPath(x1 + 1, y1, x2, y2, myPair); 
		pair<int, int **> temp2 = shortestPath(x1 - 1, y1, x2, y2, myPair); 
		pair<int, int **> temp3 = shortestPath(x1, y1 + 1, x2, y2, myPair); 
		pair<int, int **> temp4 = shortestPath(x1, y1 - 1, x2, y2, myPair); 

		int minimum = min(min(temp1.first, temp2.first), min(temp3.first, temp4.first)); 
		if(temp1.first == minimum){ 
			if(temp1.first != myPair.first) deleteArray(newArray); 
			return temp1; 
		}else if(temp2.first == minimum){ 
			if(temp2.first != myPair.first) deleteArray(newArray); 
			return temp2; 
		}else if(temp3.first == minimum){ 
			if(temp3.first != myPair.first) deleteArray(newArray); 
			return temp3; 
		}else{ 
			if(temp4.first != myPair.first) deleteArray(newArray); 
			return temp4; 
		} 

	}*/

	void moveTo(List<Vector2> moveList, GameObject movingEntity){
		StartCoroutine (moveToCoroutine(moveList, movingEntity));
	}

	bool fin = false;

	public void SetFinTrue(){
		fin = true;
	}
	public delegate void Fin();

	IEnumerator moveToCoroutine(List<Vector2> moveList, GameObject movingEntity){
		int x = 0;
		bool last = false;
		foreach (Vector2 vector in moveList) {
			x++;
			if (x == moveList.Count) {
				last = true;
			}
			TileController controller = movingEntity.transform.parent.GetComponentInChildren<TileController> ();
			if (vector.x == controller.x && vector.y < controller.z) {
				controller.movePlayer (Direction.Down, SetFinTrue, last);
			} else if (vector.x == controller.x && vector.y > controller.z) {
				controller.movePlayer (Direction.Up, SetFinTrue, last);
			} else if (vector.x < controller.x && vector.y == controller.z) {
				controller.movePlayer (Direction.Left, SetFinTrue, last);
			} else if (vector.x > controller.x && vector.y == controller.z) {
				controller.movePlayer (Direction.Right, SetFinTrue, last);
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


	void movePlayer(Direction direction, Fin fin, bool last){
		StartCoroutine(move(direction, fin, last));
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
	IEnumerator move(Direction direction, Fin fin, bool last){
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
		Initial.tiles [x + xAmt, z + zAmt].GetComponentInChildren<Animator> ().enabled = true;

		while (moveCondition(getController (x + xAmt, z + zAmt).entity.transform.localPosition, direction)) {
			Initial.tiles [x + xAmt, z + zAmt].GetComponentInChildren<Animator>().SetFloat("Walk",1);
			Initial.tiles [x + xAmt, z + zAmt].GetComponentInChildren<Animator> ().SetFloat ("Run", 0.2f);
			yield return new WaitForSeconds(0.01f);
		}

		if (!last) {
			Initial.tiles [x + xAmt, z + zAmt].GetComponentInChildren<Animator> ().enabled = false;
		} else {
			Initial.tiles [x + xAmt, z + zAmt].GetComponentInChildren<Animator> ().SetFloat ("Walk", 0);
			Initial.tiles [x + xAmt, z + zAmt].GetComponentInChildren<Animator> ().SetFloat ("Run", 0f);

			while (getController (x + xAmt, z + zAmt).entity.transform.localPosition.z != 0 &&
			       getController (x + xAmt, z + zAmt).entity.transform.localPosition.x != 0) {
				getController (x + xAmt, z + zAmt).entity.transform.localPosition = new Vector3 (0f, 0f, 0f);
				getController (x + xAmt, z + zAmt).entity.transform.eulerAngles = new Vector3 (0, rotationAmt);
				yield return new WaitForSeconds (0.01f);
			}
		}
	
		fin ();
		yield return null;
	}




}
