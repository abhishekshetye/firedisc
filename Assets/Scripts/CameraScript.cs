using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour {


	public GameObject disc;

	private Vector3 offset;

	// Use this for initialization
	void Start () 
	{
		float pos_x = PlayerPrefs.GetFloat ("Camera_pX");
		float pos_y = PlayerPrefs.GetFloat ("Camera_pY");
		float pos_z = PlayerPrefs.GetFloat ("Camera_pZ");


		float rot_x = PlayerPrefs.GetFloat ("Camera_rX");
		float rot_y = PlayerPrefs.GetFloat ("Camera_rY");
		float rot_z = PlayerPrefs.GetFloat ("Camera_rZ");

		transform.position = new Vector3 (pos_x, pos_y, pos_z);
		Quaternion target = Quaternion.Euler(rot_x, rot_y, rot_z);
		transform.rotation = target;
		
		//Calculate and store the offset value by getting the distance between the player's position and camera's position.
		offset = transform.position - disc.transform.position;
	}

	// LateUpdate is called after Update each frame
	void LateUpdate () 
	{
		// Set the position of the camera's transform to be the same as the player's, but offset by the calculated offset distance.
		transform.position = disc.transform.position + offset;
	}
}
