using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WwiseClockSync : MonoBehaviour
{
	public AK.Wwise.Event mainTheme;


	[HideInInspector] public static float secondsPerBeat;
	[HideInInspector] public static int currentPositionInSong;
	
	public InputEvaluation inputEvaluation;

	public UnityEvent OnCustomCueY;

	public UnityEvent OnLevelEnded;

	public UnityEvent OnEveryGrid;
	public UnityEvent OnEveryBeat;
	public UnityEvent OnEveryBar;
	
	public UnityEvent AfterQlimaxOnEveryBeat;

	public UnityEngine.UI.Text feedbackText;

	bool startedLevel = false;
	bool qlimaxReached = false;
	bool breakdown = false;


	public uint mainThemeID;



	private void Start()
	{
		//want to make sure we don't double-play the music event
		startedLevel = false;
		StartMusic();
		//Debug.Log("seconds per beats are: " + UpdateBPM(mainThemeID));		
	}

	private void Update()
	{

		//secondsPerBeat = UpdateBPM(mainThemeID); Rather update BPM whenever the RTCP for Pitch is called unless its a flowing movement then it has to be in the update loop
		//currentPositionInSong = UpdatePositionInSong(mainThemeID);
		
		//int songPosInBeats = currentPositionInSong / (int)secondsPerBeat;
		
		
		//Debug.Log("Seconds Per Beat: " + secondsPerBeat);
		//Debug.Log("BPM: " + 60/secondsPerBeat);
		//Debug.Log("Seconds Per Beat: " + secondsPerBeat);
		//Debug.Log("Current Position in Song: " + currentPositionInSong);
		
		
		
		
	}

	void StartMusic()
	{

		
		mainThemeID = mainTheme.Post(gameObject, (uint)(AkCallbackType.AK_MusicSyncAll | AkCallbackType.AK_EnableGetMusicPlayPosition | AkCallbackType.AK_MIDIEvent), MusicCallbackFunction);
			
			//we also want to get accurate playback position (my tests show it's usually within 5 ms, sometimes as high as 30 ms), which requires a callback as well.
			// syncall is a callback for all possible sync functions such as cue, beat, grid. This is why we need the switch container to understand which information is passed on. This is stored in in_info

	}
	
	
	void GETBPM(object in_cookie, AkCallbackType in_type, object in_info)
	{
	 AkMusicSyncCallbackInfo musicinfo = (AkMusicSyncCallbackInfo)in_info;
	 Debug.Log(60/musicinfo.segmentInfo_fBeatDuration);

	}


	void MusicCallbackFunction(object in_cookie, AkCallbackType in_type, AkCallbackInfo in_info) 
	{

		//in_cookie holds information that can be sent after the method  in the Post event
		//in_type holds inforamtion of the CallbackType that triggered the method, in this case MusicSyncAll, MusicPlayPosition, AK_MIDIevent
		//in_info holds information of the type of information that we retrieve from the Callback specified in the post event. In this case in_info of the event holds AkCallbackType.AK_MusicSyncUserCue or AkCallbackType.AK_MusicSyncBeat etc
		
		AkMusicSyncCallbackInfo _musicInfo;

		
		//check if it's music callback (beat, marker, bar, grid etc)
		if (in_info is AkMusicSyncCallbackInfo)
		{
			// store the actual info in a variable _musicInfo 
			_musicInfo = (AkMusicSyncCallbackInfo)in_info;

			//we're going to use this switchboard to fire off different events depending on wwise sends // Wwise sends every time a releveant info is retrieved? On bar beat etc
			
			// check the type of info that has been sent from wwise
			switch (_musicInfo.musicSyncType)
			{
				case AkCallbackType.AK_MusicSyncUserCue: // this is for markers

					CustomCues(_musicInfo.userCueName, _musicInfo);

					break;
					
					
				case AkCallbackType.AK_MusicSyncBeat:
					//secondsPerBeat = _musicInfo.segmentInfo_fBeatDuration;
					//secondsPerBeat = Mathf.Round(secondsPerBeat);
					inputEvaluation.startInputTimer();
					//Debug.Log("Beat is hit");

					OnEveryBeat.Invoke();
					
					if (qlimaxReached)
					{
						AfterQlimaxOnEveryBeat.Invoke();
					} // these are passed over in the inspector and can be delayed by a float from invoke
					
					
/* 					if (breakdown)
					{
						DuringBreakdownEveryBeat.Invoke();
					} // these are passed over in the inspector and can be delayed by a float from invoke */
					break; 
					
					
					
				case AkCallbackType.AK_MusicSyncBar:
					//I want to make sure that the secondsPerBeat is defined on our first measure.


					OnEveryBar.Invoke();
					
					break;
					

				case AkCallbackType.AK_MusicSyncGrid:
					//the grid is defined in Wwise - usually on your playlist.  It can be as small as a 32nd note

					OnEveryGrid.Invoke();
					
					break;
				default:
					break;
					
					



			}
		}
	
	}


	public void CustomCues(string cueName, AkMusicSyncCallbackInfo _musicInfo) // THIS IS THE SPAWNER
	{
		switch (cueName)
		{

			case "CustomCueY":
				//OnCustomCueY.Invoke();
				
				qlimaxReached = true;
				
				break;
				
			case "BreakDown":
			
				breakdown = true;
				DeactivateMovement(0.05f);
				break; 
				
			case "BreakDownOver":
				
				ReactivateMovement(0.5f, 0.3f);
				breakdown = false;
				break; 	

			case "Kick":
				//OnCustomCueY.Invoke();
				
				inputEvaluation.startInputTimer();
				
				break;

		}
	}

	//this is pretty straightforward - get the elapsed time      // WE NEED THIS FOR ELAPSED TIME I GUESS
	public int GetMusicTimeInMS()
	{

		AkSegmentInfo segmentInfo = new AkSegmentInfo();

		AkSoundEngine.GetPlayingSegmentInfo(mainThemeID, segmentInfo, true);

		return segmentInfo.iCurrentPosition;
	}


	public void DeactivateMovement(float xd)
	{
		
		float valueToSet = xd;
		 PunchAnimation[] allScripts = FindObjectsOfType<PunchAnimation>();

		// Iterate through all instances of MyScript
		foreach (PunchAnimation script in allScripts)
		{

				script.punchScaleX = valueToSet;
				script.punchScaleY = valueToSet;
				

		}	
		
		GameObject Player = GameObject.Find("Player");
		
		PunchAnimation specificScript = Player.GetComponent<PunchAnimation>();
		
		specificScript.punchScaleX = 1f;
		specificScript.punchScaleY = 1f;
		
		
		
	}
	
	
	public void ReactivateMovement(float xValue, float yValue)
	{
		
		float xValueToSet = xValue;
		float yValueToSet = yValue;
		 PunchAnimation[] allScripts = FindObjectsOfType<PunchAnimation>();

		// Iterate through all instances of MyScript
		foreach (PunchAnimation script in allScripts)
		{

				script.punchScaleX = xValueToSet;
				script.punchScaleY = yValueToSet;
				

		}	
		
		GameObject Player = GameObject.Find("Player");
		
		PunchAnimation specificScript = Player.GetComponent<PunchAnimation>();
		
		specificScript.punchScaleX = 0;
		specificScript.punchScaleY = 0;
	}
	
	
	public float UpdateBPM(uint trackID)
	{
		AkSegmentInfo segmentInfo = new AkSegmentInfo();
		AkSoundEngine.GetPlayingSegmentInfo(trackID, segmentInfo, true);
		secondsPerBeat = segmentInfo.fBeatDuration;
		//secondsPerBeat = Mathf.RoundToInt(secondsPerBeat);

		return secondsPerBeat;
	}
	
	
	public int UpdatePositionInSong(uint trackID)
	{
		
		AkSegmentInfo segmentInfo = new AkSegmentInfo();
		AkSoundEngine.GetPlayingSegmentInfo(trackID, segmentInfo, true);
		currentPositionInSong = segmentInfo.iCurrentPosition;
		
		return currentPositionInSong;
	}
	



}
