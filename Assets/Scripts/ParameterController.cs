using System;
using System.Collections;
using System.Collections.Generic;
using AK.Wwise;
using UnityEngine;


public class ParameterController : MonoBehaviour
{
	
public BufferedMovement BufferedMovement;


public static int comboCounter; 
public static int windowStatus;
public WwiseClockSync wwiseClockSync;


[HideInInspector] public float secondsPerBeat;
	
[HideInInspector] public float okWindowTimeStart;
[HideInInspector] public float goodWindowTimeStart;
[HideInInspector] public float perfectWindowTimeStart;
	
[HideInInspector] public float okWindowTimeEnd;
[HideInInspector] public float goodWindowTimeEnd;
[HideInInspector] public float perfectWindowTimeEnd;
public float okTime;
public float goodTime;
public float perfectTime;	
[HideInInspector] public float cachedPositionInSong;
public Queue<float> CacheTimingQueue = new Queue<float>();
	

[HideInInspector] public static float playbackSpeed = 1;


	// Start is called before the first frame update
	void Start()
	{
		AkSoundEngine.SetRTPCValue("PitchController",playbackSpeed);
		
	}

	// Update is called once per frame
	void Update()
	{
		if (Input.GetKeyDown(KeyCode.O)) 
		{
			playbackSpeed++;
			AkSoundEngine.SetRTPCValue("PitchController",playbackSpeed);
		}
		if (Input.GetKeyDown(KeyCode.I) && playbackSpeed != 1) 
		{
			playbackSpeed--;
			AkSoundEngine.SetRTPCValue("PitchController",playbackSpeed);
		}
		
		BufferedMovement.moveDuration = wwiseClockSync.UpdateBPM(wwiseClockSync.mainThemeID) - BufferedMovement.movementDurationDecrease; // use this whenever BPM is updated // assumes that WwiseCLockSync permanently updates its bpm, maybe its better to manually update it here
		
		
		//WwiseClockSync.updateBPM();
		//Debug.Log("playbackspeed = " + playbackSpeed);
		
		
		if (comboCounter >= 10 && comboCounter < 20)
		{
			AkSoundEngine.SetState("MainTheme","Below20");
		} 
		
		if (comboCounter >= 20)
		{
			AkSoundEngine.SetState("MainTheme","Above20");
		}
		
		if (comboCounter < 10 )
		{
			AkSoundEngine.SetState("MainTheme","Below10");
		}
	}
	
	
	// This function is called on every beat or cue and caches the window for the next beat in secondsPerBeat + 1000. The values we store here are checked against the InputTimings in WindowChecker()
	// we need to substract okTime which is the time we waited in the Buffer to store the next value, so its fits again. 
	public void UpdateWindow()
	{
		if (CacheTimingQueue.Count == 0)
		{
			CacheTimingQueue.Enqueue(wwiseClockSync.UpdatePositionInSong(wwiseClockSync.mainThemeID)); // writes a 0 to the queue
		}
		CacheTimingQueue.Enqueue(wwiseClockSync.UpdatePositionInSong(wwiseClockSync.mainThemeID) + wwiseClockSync.UpdateBPM(wwiseClockSync.mainThemeID) * 1000); 
		Debug.Log("This Value is added to the Queue: " + (wwiseClockSync.UpdatePositionInSong(wwiseClockSync.mainThemeID) + wwiseClockSync.UpdateBPM(wwiseClockSync.mainThemeID) * 1000));
		cachedPositionInSong = CacheTimingQueue.Dequeue();
		Debug.Log("This Value is removed from the Queue: " + cachedPositionInSong);
		 
					
					
		// secondsPerBeat = wwiseClockSync.UpdateBPM(wwiseClockSync.mainThemeID);
		
		// okWindowTimeStart = cachedPositionInSong + secondsPerBeat * 1000 - okTime;
		// okWindowTimeEnd = cachedPositionInSong +  secondsPerBeat * 1000 + okTime;
		// goodWindowTimeStart = cachedPositionInSong + secondsPerBeat * 1000 - goodTime;
		// goodWindowTimeEnd = cachedPositionInSong +  secondsPerBeat * 1000 + goodTime ;
		// perfectWindowTimeStart = cachedPositionInSong + secondsPerBeat * 1000 - perfectTime;
		// perfectWindowTimeEnd = cachedPositionInSong +  secondsPerBeat * 1000 + perfectTime;		

		okWindowTimeStart = cachedPositionInSong  - okTime;
		okWindowTimeEnd = cachedPositionInSong  + okTime;
		goodWindowTimeStart = cachedPositionInSong - goodTime;
		goodWindowTimeEnd = cachedPositionInSong  + goodTime ;
		perfectWindowTimeStart = cachedPositionInSong  - perfectTime;
		perfectWindowTimeEnd = cachedPositionInSong  + perfectTime;		
	}
	

}
