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
	}


}
