using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public enum GameMode
{
	idle,
	playing,
	levelEnd
}

public class MissionDemolition : MonoBehaviour
{
	#region Fields
	public static MissionDemolition S; //singleton

	[Header("Inspector fields")]
	public GameObject[] castles;
	public Text gtLevel;
	public Text gtScore;
	public Vector3 castlePos;

	[Header("Dynamic fields")]
	public int level;
	public int levelMax;
	public int shotsTaken;
	public GameObject castle;
	public GameMode mode = GameMode.idle;
	public string showing = "Slingshot"; //Followcam mode
	#endregion

	#region Methods
	void Start()
	{
		S = this; //Singleton creation

		level = 0;
		levelMax = castles.Length;
		StartLevel();
	}

	void StartLevel()
	{
		//Get rid of the old castle if one exists
		if (castle != null)
		{
			Destroy(castle);
		}

		//Destroy old projectiles if they exist
		GameObject[] gos = GameObject.FindGameObjectsWithTag("Projectile");
		foreach (GameObject pTemp in gos)
		{
			Destroy(pTemp);
		}

		//Instantiate the new castle
		castle = Instantiate(castles[level]) as GameObject;
		castle.transform.position = castlePos;
		shotsTaken = 0;

		//Reset the camera
		SwitchView("Both");
		ProjectileLine.S.Clear();

		//Reset the goal
		Goal.goalMet = false;

		ShowGT();

		mode = GameMode.playing;
	}

	void ShowGT()
	{
		//Show the data in the Texts on the UI
		gtLevel.text = "Level: " + (level + 1) + " of " + levelMax;
		gtScore.text = "Shots Taken: " + shotsTaken;
	}

	void Update()
	{
		ShowGT();

		//Check for level end
		if (mode == GameMode.playing && Goal.goalMet)
		{
			//Change mode to stop checking for level end
			mode = GameMode.levelEnd;

			//Zoom out
			SwitchView("Both");

			//Start the next level in 2 seconds
			Invoke("NextLevel", 2f);
		}
	}

	void NextLevel()
	{
		level++;
		if (level == levelMax)
		{
			level = 0;
		}
		StartLevel();
	}

	void OnGUI()
	{
		//Draw the GUI Button for view switching at the top of the screen
		Rect buttonRect = new Rect((Screen.width/2) - 50, 10, 100, 24 );

		switch (showing)
		{
		case "Slingshot":
			if (GUI.Button(buttonRect, "Show Castle"))
			{
				SwitchView("Castle");
			}
			break;

		case "Castle":
			if (GUI.Button(buttonRect, "Show Both"))
			{
				SwitchView("Both");
			}
			break;

		case "Both":
			if (GUI.Button(buttonRect, "Show Slingshot"))
			{
				SwitchView("Slingshot");
			}
			break;
		}
	}

	//Static method that allows code anywhere to request a view change
	public static void SwitchView(string eView)
	{
		S.showing = eView;
		switch (S.showing)
		{
		case "Slingshot":
			FollowCam.S.poi = null;
			break;

		case "Castle":
			FollowCam.S.poi = S.castle;
			break;

		case "Both":
			FollowCam.S.poi = GameObject.Find("ViewBoth");
			break;
		}
	}

	//Static method that allows code anywhere to increment shotsTaken
	public static void ShotFired()
	{
		S.shotsTaken++;
	}
}
	#endregion