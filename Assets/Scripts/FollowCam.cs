using UnityEngine;
using System.Collections;

public class FollowCam : MonoBehaviour
{
	#region Fields
	static public FollowCam S; //singleton

	[Header("Inspector fields")]
	public float easing = 0.05f;
	public Vector2 minXY;

	[Header("Dynamic fields")]
	public GameObject poi; //The point of interest
	public float camZ; //The desired Z pos of the camera
	#endregion

	#region Methods

	//Create the singleton
	void Awake()
	{
		S = this;
		camZ = this.transform.position.z;
	}
		
	void Start()
	{
	
	}

	void FixedUpdate()
	{
		Vector3 destination;

		//If there is no poi, return to P;[0.0.0]
		if (poi == null)
		{
			destination = Vector3.zero;
		}
		else
		{
			//Get the position of the poi
			destination = poi.transform.position;

			//If poi is a Projectile, check to see if it's at rest
			if (poi.tag == "Projectile")
			{
				//if it is sleeping/not moving
				if (poi.GetComponent<Rigidbody>().IsSleeping())
				{
					//return to default view
					poi = null;

					//in the next update
					return;
				}
			}
		}

		//Limit the X and Y to minimum values
		destination.x = Mathf.Max(minXY.x, destination.x);
		destination.y = Mathf.Max(minXY.y, destination.y);

		//Interpolate from the current Camera position toward destination
		destination = Vector3.Lerp(transform.position, destination, easing);

		//Retain a destination.z of camZ
		destination.z = camZ;

		//Set the camera to the destination
		transform.position = destination;

		//Set the orthographic size of the camera to keep the Ground in view
		this.GetComponent<Camera>().orthographicSize = destination.y + 10;
	}
	#endregion
}
