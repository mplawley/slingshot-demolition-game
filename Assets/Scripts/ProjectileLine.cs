﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ProjectileLine : MonoBehaviour
{
	#region Fields

	public static ProjectileLine S; //Singleton

	[Header("Inspector fields")]
	public float minDist = 0.1f;

	[Header("Dynamic fields")]
	public LineRenderer line;
	private GameObject _poi;
	public List<Vector3> points;

	#endregion

	#region Methods

	void Awake()
	{
		S = this; //Set the singleton

		//Get a reference to the line renderer
		line = GetComponent<LineRenderer>();

		//Disable the LineRenderer until it's needed
		line.enabled = false;

		//Initialize the points List
		points = new List<Vector3>();
	}

	//This is a property--i.e., a method masquerading as a field
	public GameObject poi
	{
		get
		{
			return (_poi);
		}
		set
		{
			_poi = value;
			if (_poi != null)
			{
				//When _poi is set to something new, it resets everything
				line.enabled = false;
				points = new List<Vector3>();
				AddPoint();
			}
		}
	}

	//This can be used to clear the line directly
	public void Clear()
	{
		_poi = null;
		line.enabled = false;
		points = new List<Vector3>();
	}

	public void AddPoint()
	{
		//This is called to add a point to the line
		Vector3 pt = _poi.transform.position;
		if (points.Count > 0 && (pt - lastPoint).magnitude < minDist)
		{
			//If the point isn't far enough frmo the last point, it returns
			return;
		}
		if (points.Count == 0)
		{
			//If this is the launch point...
			Vector3 launchPos = Slingshot.S.launchPoint.transform.position;
			Vector3 launchPosDiff = pt - launchPos;

			//...it adds an extra bit of line to aid aiming later
			points.Add(pt + launchPosDiff);
			points.Add(pt);
			line.SetVertexCount(2);

			//Sets the first two points
			line.SetPosition(0, points[0]);
			line.SetPosition(1, points[1]);

			//Enables the LineRenderer
			line.enabled = true;
		}
		else
		{
			//Normal behavior of adding a point
			points.Add(pt);
			line.SetVertexCount(points.Count);
			line.SetPosition(points.Count - 1, lastPoint);
			line.enabled = true;
		}
	}

	//Returns the location of the most recently added point
	public Vector3 lastPoint
	{
		get
		{
			if (points == null)
			{
				//if there are no points, returns Vector3.zero
				return(Vector3.zero);
			}
			return (points[points.Count - 1]);
		}
	}

	void FixedUpdate()
	{
		if (poi == null)
		{
			//If there is no poi, search for one
			if (FollowCam.S.poi != null)
			{
				if (FollowCam.S.poi.tag == "Projectile")
				{
					poi = FollowCam.S.poi;
				}
				else
				{
					return; //Return if we didn't find a poi
				}
			}
			else
			{
				return; //Return if we didn't find a poi
			}
		}

		//If there is a poi, it's loc is added every FixedUpdate
		AddPoint();
		if (poi.GetComponent<Rigidbody>().IsSleeping())
		{
			//Once the poi is sleeping, it is cleared
			poi = null;
		}
	}
	#endregion
}
