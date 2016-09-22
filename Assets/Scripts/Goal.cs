using UnityEngine;
using System.Collections;

public class Goal : MonoBehaviour {
	#region Fields
	public static bool goalMet = false;
	#endregion

	#region Methods

	void OnTriggerEnter(Collider other)
	{
		//When the trigger is hit by something
		//Check to see if it's a projectile
		if (other.gameObject.tag == "Projectile")
		{
			//If so, set goalMet to true
			Goal.goalMet = true;

			//Also set the alpha of the color to higher opacity
			Color c = GetComponent<Renderer>().material.color;
			c.a = 1;
			GetComponent<Renderer>().material.color = c;
		}
	}
}
	#endregion
