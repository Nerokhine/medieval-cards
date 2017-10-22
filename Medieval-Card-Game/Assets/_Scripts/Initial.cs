using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Initial : MonoBehaviour {
	public static int dimX = 15;
	public static int dimZ = 15;
	public static GameObject[,] tiles;

	void Start () {
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
				tiles [x, z].GetComponent<TileController> ().x = x;
				tiles [x, z].GetComponent<TileController> ().z = z;
			}
		}
	}

}
