﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Initial : MonoBehaviour {
	public static int dimX = 15;
	public static int dimZ = 15;
	public static GameObject[,] tiles;
	GameObject panel;

	void Start () {
		// test
		panel = GameObject.Find("Panel");
		// Create a grid
		tiles = new GameObject[dimX,dimZ];

		for (int x = 0; x < dimX; x++) {
			for (int z = 0; z < dimZ; z++) {
				GameObject tileObject = (GameObject)Instantiate (Resources.Load ("_Prefabs/tile"), transform);

				tileObject.transform.position = new Vector3 (
					(tileObject.transform.localScale.x + 0.01f) * x, 
					0,
					(tileObject.transform.localScale.z + 0.01f) * z
				);
					
				tiles [x, z] = tileObject;
				tiles [x, z].GetComponentInChildren<TileController> ().x = x;
				tiles [x, z].GetComponentInChildren<TileController> ().z = z;
			}
		}
	}

	void Update(){
		RectTransform rect = panel.GetComponent<RectTransform> ();
		if (rect.anchoredPosition.x > 0) {
			if (rect.anchoredPosition.x - 40 < 0) {
				rect.anchoredPosition = new Vector2 (
					0,
					rect.anchoredPosition.y
				);
			} else {
				rect.anchoredPosition = new Vector2 (
					rect.anchoredPosition.x - 40,
					rect.anchoredPosition.y
				);
			}
		}
	}

}
