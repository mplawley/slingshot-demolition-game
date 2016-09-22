using UnityEngine;
using System.Collections;

public class CloudCrafter : MonoBehaviour
{
	#region Fields
	[Header("Inspector fields")]
	public GameObject cloudAnchor;
	public int numClouds = 40;
	public GameObject[] cloudPrefabs;
	public Vector3 cloudPosMin;
	public Vector3 cloudPosMax;
	public float cloudScaleMin = 1;
	public float cloudScaleMax = 5;
	public float cloudSpeedMult = 0.5f;

	[Header("Dynamic fields")]
	public GameObject[] cloudInstances;

	#endregion

	#region Methods

	void Awake()
	{
		//Make an array large enough to hold all of the Cloud instances
		cloudInstances = new GameObject[numClouds];

		//Iterate through and make clouds
		GameObject cloud;

		for (int i = 0; i < numClouds; i++)
		{
			//Pick an int between 0 and cloudPrefabs.Length-1. Random.Range will never pick as high as the top number
			int prefabNum = Random.Range(0, cloudPrefabs.Length);

			//Make an instance
			cloud = Instantiate(cloudPrefabs[prefabNum]) as GameObject;

			//Position cloud
			Vector3 cPos = Vector3.zero;
			cPos.x = Random.Range(cloudPosMin.x, cloudPosMax.x);
			cPos.y = Random.Range(cloudPosMin.y, cloudPosMax.y);

			//Scale cloud
			float scaleU = Random.value;
			float scaleVal = Mathf.Lerp(cloudScaleMin, cloudScaleMax, scaleU);

			//Smaller clouds (with smaller scaleU) should be nearer the ground
			cPos.y = Mathf.Lerp (cloudPosMin.y, cPos.y, scaleU);

			//Smaller clouds shoudl be further away
			cPos.z = 100 - 90 * scaleU;

			//Apply these transforms to the cloud
			cloud.transform.position = cPos;
			cloud.transform.localScale = Vector3.one * scaleVal;

			//Make a cloud a child of the anchor
			cloud.transform.parent = cloudAnchor.transform;

			//Add the cloud to cloudInstances
			cloudInstances[i] = cloud;
		}
	}
		
	// Update is called once per frame
	void Update()
	{
		//Iterate over each cloud that was created
		foreach (GameObject cloud in cloudInstances)
		{
			//Get the cloud scale and position
			float scaleVal = cloud.transform.localScale.x;
			Vector3 cPos = cloud.transform.position;

			//Move larger clouds faster
			cPos.x -= scaleVal * Time.deltaTime * cloudSpeedMult;

			//If a cloud has moved too far to the left...
			if (cPos.x <= cloudPosMin.x)
			{
				//Move it to the far right
				cPos.x = cloudPosMax.x;
			}

			//Apply the new position to cloud
			cloud.transform.position = cPos;
		}
	}
	#endregion
}
