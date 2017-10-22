using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileController : MonoBehaviour {
	public int x;
	public int z;
	public GameObject entity;
	public GameObject leftWall;
	public GameObject rightWall;
	public GameObject topWall;
	public GameObject bottomWall;

	Color32 prevColor;

	void OnAwake(){
		entity = null;
	}

	void OnMouseEnter () {
		Renderer rend = GetComponent<Renderer>();
		prevColor = rend.material.color;
		rend.material.SetColor("_Color", new Color32(255,255,255,100));
	}

	void OnMouseExit(){
		Renderer rend = GetComponent<Renderer>();
		rend.material.SetColor("_Color", prevColor);
	}

	void OnMouseDown () {
		
	}

	void OnMouseOver () {
		if (Input.GetMouseButtonDown (0)) {
			if (entity == null)
				wallConstruction ();
		} else if (Input.GetMouseButtonDown (1)) {
			if (entity != null) {
				Destroy (entity);
				Destroy (topWall);
				Destroy (bottomWall);
				Destroy (rightWall);
				Destroy (leftWall);
			}
		}
	}
		

	void wallConstruction(){
		GameObject tileObject = (GameObject)Instantiate (Resources.Load ("buildings/tower"), transform);
		tileObject.transform.localScale =  new Vector3(10f, 10f, 100f);
		entity = tileObject;

		if (x < Initial.dimX - 1) {
			TileController rightObject = Initial.tiles [x + 1, z].GetComponent<TileController> ();
			if (rightObject.entity != null) {
				rightWall = (GameObject)Instantiate (Resources.Load ("buildings/tower"), transform);
				rightWall.transform.localScale = new Vector3 (15.5f, 10f, 100f);
				rightWall.transform.localPosition = new Vector3 (
					0.5f,
					0,
					0
				);
				rightObject.leftWall = rightWall;
			}
		}

		if (x > 0) {
			TileController leftObject = Initial.tiles [x - 1, z].GetComponent<TileController> ();
			if (leftObject.entity != null) {
				leftWall = (GameObject)Instantiate (Resources.Load ("buildings/tower"), transform);
				leftWall.transform.localScale = new Vector3 (15.5f, 10f, 100f);
				leftWall.transform.localPosition = new Vector3 (
					-0.5f,
					0,
					0
				);
				leftObject.rightWall = leftWall;
			}
		}

		if (z < Initial.dimZ - 1) {
			TileController topObject = Initial.tiles [x, z + 1].GetComponent<TileController> ();
			if (topObject.entity != null) {
				topWall = (GameObject)Instantiate (Resources.Load ("buildings/tower"), transform);
				topWall.transform.localScale = new Vector3 (10f, 15.5f, 100f);
				topWall.transform.localPosition = new Vector3 (
					0,
					0,
					0.5f
				);
				topObject.bottomWall = topWall;
			}
		}

		if (z > 0) {
			TileController bottomObject = Initial.tiles [x, z - 1].GetComponent<TileController> ();
			if (bottomObject.entity != null) {
				bottomWall = (GameObject)Instantiate (Resources.Load ("buildings/tower"), transform);
				bottomWall.transform.localScale = new Vector3 (10f, 15.5f, 100f);
				bottomWall.transform.localPosition = new Vector3 (
					0,
					0,
					-0.5f
				);
				bottomObject.topWall = bottomWall;
			}
		}
	}


}
