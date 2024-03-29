﻿using UnityEngine;
using System.Collections;

public class Slingshot : MonoBehaviour
{
	#region Fields
	public static Slingshot S; //Singleton

	[Header("Inspector fields")]
	public GameObject prefabProjectile;
	public float velocityMult = 4f;

	[Header("Dynamic fields")]
	public GameObject launchPoint;
	public Vector3 launchPos;
	public GameObject projectile;
	public bool aimingMode;

	#endregion

	#region Methods

	void Awake()
	{
		//Set the slingshot singleton
		S = this;

		//Set launch fields
		Transform launchPointTrans = transform.Find("LaunchPoint");
		launchPoint = launchPointTrans.gameObject;
		launchPoint.SetActive(false);
		launchPos = launchPointTrans.position;
	}
		
	void Start()
	{

	}
		
	void OnMouseEnter()
	{
		launchPoint.SetActive(true);
	}

	void OnMouseExit()
	{
		launchPoint.SetActive(false);
	}

	void OnMouseDown()
	{
		//The player has pressed the mouse button while over the slingshot
		aimingMode = true;

		//Instantiate a projectile
		projectile = Instantiate(prefabProjectile) as GameObject;

		//Start it at the launch point
		projectile.transform.position = launchPos;

		//Set it to isKinematic for now
		projectile.GetComponent<Rigidbody>().isKinematic = true;
	}
	
	// Update is called once per frame
	void Update()
	{
		//If slingshot is in aiming mode, don't run this code...
		if (!aimingMode)
		{
			return;
		}

		//Get the current mouse position in 2D screen coordinates
		Vector3 mousePos2D = Input.mousePosition;

		//Convert the mouse position to 3D world coordinates
		mousePos2D.z = -Camera.main.transform.position.z;
		Vector3 mousePos3D = Camera.main.ScreenToWorldPoint(mousePos2D);

		//Find the delta from the launchPos to the mousePos3D
		Vector3 mouseDelta = mousePos3D - launchPos;

		//Limit mouseDelta to the radius of the Slingshot SphereCollider
		float maxMagnitude = this.GetComponent<SphereCollider>().radius;
		if (mouseDelta.magnitude > maxMagnitude)
		{
			//Turn it into a unit vector and multiple by the max magnitude
			mouseDelta.Normalize();
			mouseDelta *= maxMagnitude;
		}

		//Move the projectile to this new position
		Vector3 projPos = launchPos + mouseDelta;
		projectile.transform.position = projPos;

		//TODO: OnMouseUp
		if (Input.GetMouseButtonUp(0))
		{
			//The mouse has been released
			aimingMode = false;
			projectile.GetComponent<Rigidbody>().isKinematic = false;
			projectile.GetComponent<Rigidbody>().velocity = -mouseDelta * velocityMult;
			FollowCam.S.poi = projectile;
			projectile = null;
			MissionDemolition.ShotFired();
		}
	}

	#endregion
}
