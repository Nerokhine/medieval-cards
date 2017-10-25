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

	public bool characterPath;
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
		characterPath = false;
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
			if (characterPath) {
				// dcharacter will be passing over these tiles
			}else if (entity == null && selectedTile == null) {
				if(Input.GetKey(KeyCode.Space)){
					playerConstruction ();
				}else{
					wallConstruction ();
				}
				if (selectedTile != null) {
					selectedTile.GetComponent<TileController> ().Select (false);
					selectedTile = null;
				}
			} else {
				if(selectedTile == gameObject){ // deselect self
					if (selectedTile != null) {
						selectedTile.GetComponent<TileController> ().Select (false);
						selectedTile = null;
					}
				}else if (selectedTile != null && selectedTile.GetComponent<TileController> ().entity != null && 
					selectedTile.GetComponent<TileController> ().entity.tag == "Unit") {
					if (entity == null) { // cant move to a location with an entity
						selectedTile.GetComponent<TileController> ().moveToLocation (new Vector2 (x, z));
					}
					selectedTile.GetComponent<TileController> ().Select (false);
					selectedTile = null;
				} else{
					if (selectedTile != null) {
						selectedTile.GetComponent<TileController> ().Select (false);
					}
					if (entity != null) { // don't select nothing
						selectedTile = gameObject;
						Select (true);
					} else {
						selectedTile.GetComponent<TileController> ().Select (false);
						selectedTile = null;
					}
				}

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
		GameObject tileObject = (GameObject)Instantiate (Resources.Load ("_Prefabs/Tower"), tileSpawner.transform);
		tileObject.transform.localScale =  new Vector3(10f, 10f, 10f);
		entity = tileObject;

		if (x < Initial.dimX - 1) {
			TileController rightObject = Initial.tiles [x + 1, z].GetComponentInChildren<TileController> ();
			if (rightObject.entity != null && rightObject.entity.tag == "Wall") {
				rightWall = (GameObject)Instantiate (Resources.Load ("_Prefabs/Tower"), tileSpawner.transform);
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
			if (leftObject.entity != null && leftObject.entity.tag == "Wall") {
				leftWall = (GameObject)Instantiate (Resources.Load ("_Prefabs/Tower"), tileSpawner.transform);
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
			if (topObject.entity != null && topObject.entity.tag == "Wall") {
				topWall = (GameObject)Instantiate (Resources.Load ("_Prefabs/Tower"), tileSpawner.transform);
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
			if (bottomObject.entity != null && bottomObject.entity.tag == "Wall") {
				bottomWall = (GameObject)Instantiate (Resources.Load ("_Prefabs/Tower"), tileSpawner.transform);
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
		GameObject tileObject = (GameObject)Instantiate (Resources.Load ("_Prefabs/Villager"), tileSpawner.transform);
		tileObject.transform.localScale =  new Vector3(0.2f, 0.2f, 0.2f);
		entity = tileObject;

	}

	public void moveToLocation(Vector2 location){
		List<Vector2> moveList = shortestPath (new Vector2(x, z), location);
		GameObject movingEntity = entity;
		moveTo (moveList, movingEntity);
	}


	List<Vector2> shortestPath(Vector2 start, Vector2 end){
		int[,] array;
		array = new int[Initial.dimX,Initial.dimZ];

		// obstacles in the game
		for (int x = 0; x < Initial.dimX; x++) {
			for (int z = 0; z < Initial.dimZ; z++) {
				// can't go on characterPath blocks or entity blocks
				if (getController (x, z).entity != null || getController (x, z).characterPath) {
					array [x, z] = 1;
				} else {
					array [x, z] = 0;
				}
			}
		}

		List<Vector2> path = new List<Vector2> ();

		shortPath ((int)start.x, (int)start.y, (int)end.x, (int)end.y, array, path);
		path.Reverse(); // list is backwards
		path.RemoveAt (0); // remove first element as it is current position

		string debug = "";
		for(int y = Initial.dimZ - 1; y >= 0; y --){
			for(int x = 0; x < Initial.dimX; x ++){
				foreach (Vector2 vector in path) {
					if (vector.x == x && vector.y == y) {
						array [x, y] = 2;
						getController (x, y).characterPath = true;
					}
				}
			}
		}

		for(int y = Initial.dimZ - 1; y >= 0; y --){
			for(int x = 0; x < Initial.dimX; x ++){
				debug += array[x, y] + " ";
			}
			debug += "\n";
		}

		//Debug.Log (debug);

		return path;


	}

	void shortPath(int x1, int y1, int x2, int y2, int[,] masterArray, List<Vector2> path){
		int [,] array;
		int [,] array2;
		array = new int[Initial.dimX,Initial.dimZ];
		array2 = new int[Initial.dimX,Initial.dimZ];


		for(int x = 0; x < Initial.dimX; x ++){
			for(int y = 0; y < Initial.dimZ; y ++){
				array[x, y] = 666;
				array2[x, y] = masterArray[x, y];
			}
		}

		array[x1, y1] = 0;
		shortestPathHelper(x1, y1, x2, y2, array, array2);

		findRoute(x2, y2, array, path);
	}


	void shortestPathHelper(int x1, int y1, int x2, int y2, int [,]array, int [,] array2){
		if (x1 < 0 || x1 >= Initial.dimX || y1 < 0 || y1 >= Initial.dimZ ||
			x2 < 0 || x2 >= Initial.dimX || y2 < 0 || y2 >= Initial.dimZ) {
			return;
		}
		if(x1 < Initial.dimX - 1 && array2[x1 + 1, y1] == 0 && array[x1 + 1, y1] > array[x1, y1] + 1) array[x1 + 1, y1] = array[x1, y1] + 1;
		if(x1 > 0 && array2[x1 - 1, y1] == 0 && array[x1 - 1, y1] > array[x1, y1] + 1) array[x1 - 1, y1] = array[x1, y1] + 1;
		if(y1 <  Initial.dimZ - 1 && array2[x1, y1 + 1] == 0 && array[x1, y1 + 1] > array[x1, y1] + 1) array[x1, y1 + 1] = array[x1, y1] + 1;
		if(y1 > 0 && array2[x1, y1 - 1] == 0 && array[x1, y1 - 1] > array[x1, y1] + 1) array[x1, y1 - 1] = array[x1, y1] + 1;

		array2[x1, y1] = 1;

		if(x1 < Initial.dimX - 1 && array2[x1 + 1, y1] == 0) shortestPathHelper(x1 + 1, y1, x2, y2, array, array2);
		if(x1 > 0 && array2[x1 - 1, y1] == 0) shortestPathHelper(x1 - 1, y1, x2, y2, array, array2);
		if(y1 <  Initial.dimZ - 1 && array2[x1, y1 + 1] == 0) shortestPathHelper(x1, y1 + 1, x2, y2, array, array2);
		if(y1 > 0 && array2[x1, y1 - 1] == 0) shortestPathHelper(x1, y1 - 1, x2, y2, array, array2);
	}

	void findRoute(int x1, int y1, int [,] array, List<Vector2> path){
		if (x1 < 0 || x1 >= Initial.dimX || y1 < 0 || y1 >= Initial.dimZ) {
			return;
		}
		path.Add(new Vector2(x1, y1));
		if(x1 < Initial.dimX - 1 && array[x1 + 1, y1] < array[x1, y1]) findRoute(x1 + 1, y1, array, path);
		else if(x1 > 0 && array[x1 - 1, y1] < array[x1, y1]) findRoute(x1 - 1, y1, array, path);
		else if(y1 <  Initial.dimZ - 1 && array[x1, y1 + 1] < array[x1, y1]) findRoute(x1, y1 + 1, array, path);
		else if(y1 > 0 && array[x1, y1 - 1] < array[x1, y1]) findRoute(x1, y1 - 1, array, path);
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
		// pass over control of the object to the next tile
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
		} else { // last
			getController(x + xAmt, z + zAmt).characterPath = false;
			Initial.tiles [x + xAmt, z + zAmt].GetComponentInChildren<Animator> ().SetFloat ("Walk", 0);
			Initial.tiles [x + xAmt, z + zAmt].GetComponentInChildren<Animator> ().SetFloat ("Run", 0f);

			while (getController (x + xAmt, z + zAmt).entity.transform.localPosition.z != 0 &&
			       getController (x + xAmt, z + zAmt).entity.transform.localPosition.x != 0) {
				getController (x + xAmt, z + zAmt).entity.transform.localPosition = new Vector3 (0f, 0f, 0f);
				getController (x + xAmt, z + zAmt).entity.transform.eulerAngles = new Vector3 (0, rotationAmt);
				yield return new WaitForSeconds (0.01f);
			}
		}

		characterPath = false;
	
		fin ();
		yield return null;
	}




}
