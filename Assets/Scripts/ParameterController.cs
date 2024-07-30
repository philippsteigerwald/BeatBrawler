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
		
		BufferedMovement.moveDuration = WwiseClockSync.secondsPerBeat - BufferedMovement.movementDurationDecrease; // use this whenever BPM is updated 
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
	
	
	

}
