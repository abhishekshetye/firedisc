using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GameManager : MonoBehaviour {


	public Slider curveSlider;
	public Rigidbody discRigidBody;
	public Button fireButton;
	public Slider targetSlider;
	public GameObject direction;

	public GameObject nextLevelPanel;
	public GameObject retryPanel;
	private bool panelSet = false;


	private float curvedAmount = 0f;
	private bool didTouchTheGround = false;
	private bool isFired = false;
	private float power = 50f;
	private bool continueToRotate = false;
	private LineRenderer targetLineRenderer ;
	private Animator anim;
	private float massAfterGround = 10f;

	private Color startCol = new Color (237f, 89f, 78f);


	// Use this for initialization
	void Start () {

		SetUpDiscPosition ();

		//initial setup
		curveSlider.minValue = -50;
		curveSlider.maxValue = 50;

		//animator init
		anim = gameObject.GetComponent<Animator>();
		anim.SetBool("unstability", true);


		//hide the panels if not hidden
		nextLevelPanel.SetActive(false);
		retryPanel.SetActive(false);

		//set target line renderer
		targetLineRenderer = direction.GetComponent<LineRenderer>();
		decorateTargetLineRenderer ();
		renderTargetLine ();

		//slow down speed
		Time.timeScale = 0.45f;

		//setting fire listener
		Button btn = fireButton.GetComponent<Button>();
		btn.onClick.AddListener(Fire);
	}


	private void SetUpDiscPosition(){
	
		float pos_x = PlayerPrefs.GetFloat ("Disc_pX");
		float pos_y = PlayerPrefs.GetFloat ("Disc_pY");
		float pos_z = PlayerPrefs.GetFloat ("Disc_pZ");


		float rot_x = PlayerPrefs.GetFloat ("Disc_rX");
		float rot_y = PlayerPrefs.GetFloat ("Disc_rY");
		float rot_z = PlayerPrefs.GetFloat ("Disc_rZ");

		transform.position = new Vector3 (pos_x, pos_y, pos_z);
		Quaternion target = Quaternion.Euler(rot_x, rot_y, rot_z);
		transform.rotation = target;

		//set direction
		float dir_rot_x = PlayerPrefs.GetFloat ("Dir_rX");
		float dir_rot_y = PlayerPrefs.GetFloat ("Dir_rY");
		float dir_rot_z = PlayerPrefs.GetFloat ("Dir_rZ");

		Quaternion dir_target = Quaternion.Euler(dir_rot_x, dir_rot_y, dir_rot_z);
		direction.transform.position = new Vector3 (pos_x, pos_y, pos_z);
		direction.transform.rotation = dir_target;
	}


	private void Fire(){
		//start the animation
		anim.SetBool("unstability", true);

		curvedAmount = curveSlider.value;
		if (!isFired) {
			//discRigidBody.isKinematic = false;
			//discRigidBody.AddForce(center.transform.forward * power,ForceMode.Impulse);	
			//discRigidBody.AddForce(transform.forward * power,ForceMode.Impulse);
			discRigidBody.isKinematic = false;
			discRigidBody.AddForce(direction.transform.forward * power,ForceMode.Impulse);
			isFired = true;
			targetLineRenderer.enabled = false;
		}

	}


	void FixedUpdate () {


		if(continueToRotate)
			Rotate ();


		if (!didTouchTheGround) {  
			// Curve force added each frame

			if (isFired) {
				// Curve force added each frame
				Vector3 sideDir = Vector3.Cross (direction.transform.up, discRigidBody.velocity).normalized;
				discRigidBody.AddForce (sideDir * curvedAmount);

			}
		} else {
			//stop the animation
			Debug.Log("Touched the ground");
			anim.SetBool("unstability", false);
			discRigidBody.mass = massAfterGround;
			discRigidBody.AddForce(Vector3.down * 9.8f * discRigidBody.mass);
		}
	}

	// Update is called once per frame
	void Update () {
		Debug.DrawLine(discRigidBody.position, discRigidBody.transform.forward, Color.red);
	}


	void OnCollisionEnter(Collision collision) {
		didTouchTheGround = true;

		if (collision.collider.tag == "Target" && !panelSet) {
			nextLevelPanel.SetActive (true);
			SetLevel ();
			panelSet = true;
		} else if (!panelSet) {
			retryPanel.SetActive (true);
			panelSet = true;
		}

		Debug.Log ("Touched ground : OnCollisionEnter");
	}


	void OnTriggerEnter(Collider other) {
		didTouchTheGround = true;
		anim.updateMode = AnimatorUpdateMode.AnimatePhysics;
		Debug.Log ("Touched ground : OnTriggerEnter");
	}

	void decorateTargetLineRenderer(){
		// A simple 2 color gradient with a fixed alpha of 1.0f.
		float alpha = 1.0f;
		Gradient gradient = new Gradient();
		gradient.SetKeys(
			new GradientColorKey[] { new GradientColorKey(startCol, 0.0f), new GradientColorKey(Color.gray, 1.0f) },
			new GradientAlphaKey[] { new GradientAlphaKey(alpha, 0.0f), new GradientAlphaKey(alpha, 1.0f) }
		);
		targetLineRenderer.colorGradient = gradient;
	}

	void Rotate() {
		direction.transform.Rotate (Vector3.up, targetSlider.value * Time.deltaTime);
		renderTargetLine ();
	}
		
	private void renderTargetLine(){
		targetLineRenderer.startWidth = 0.5f;
		targetLineRenderer.endWidth = 0f;
		targetLineRenderer.positionCount = 2;
		targetLineRenderer.SetPosition(0, direction.transform.position);
		targetLineRenderer.SetPosition(1, direction.transform.forward * 5 + direction.transform.position);

		decorateTargetLineRenderer ();
	}
		

	public void OnEndDrag()
	{
		continueToRotate = false;
		Debug.Log("OnBeginDrag called.");

		//reset slider to initial position
		targetSlider.value = 0;
	}

	public void OnStartDrag()
	{
		continueToRotate = true;
		Debug.Log("OnStartDrag called.");
	}


	private void SetLevel(){
		int atLevel = PlayerPrefs.GetInt ("LevelCompleted");
		PlayerPrefs.SetInt ("LevelCompleted", atLevel + 1);
	}
}
