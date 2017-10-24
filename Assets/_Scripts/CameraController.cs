using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 cameraTransform = gameObject.transform.position;
		float multiplier = .50f * GetComponent<Camera> ().fieldOfView / 35;
		gameObject.transform.position = new Vector3 (
			cameraTransform.x + Input.GetAxis ("Vertical") * multiplier + Input.GetAxis("Horizontal") * multiplier, 
			cameraTransform.y, 
			cameraTransform.z + Input.GetAxis ("Vertical") * multiplier + Input.GetAxis("Horizontal") * -multiplier
		);
		if (GetComponent<Camera> ().fieldOfView + Input.GetAxis ("Mouse ScrollWheel") > 5 &&
		   GetComponent<Camera> ().fieldOfView + Input.GetAxis ("Mouse ScrollWheel") < 35) {
			GetComponent<Camera> ().fieldOfView = GetComponent<Camera> ().fieldOfView + Input.GetAxis ("Mouse ScrollWheel");
		}

	}
}
