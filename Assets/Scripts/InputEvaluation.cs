using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using UnityEditor.PackageManager.UI;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

using UnityEngine.UIElements;

public class InputEvaluation : MonoBehaviour
{
	
	public float okTime;
	public float goodTime;
	public float perfectTime;	
	

	
	[HideInInspector] public float currentPositionInSong;
	[HideInInspector] public float secondsPerBeat;
	
	[HideInInspector] public float okWindowTimeStart;
	[HideInInspector] public float goodWindowTimeStart;
	[HideInInspector] public float perfectWindowTimeStart;
	
	[HideInInspector] public float okWindowTimeEnd;
	[HideInInspector] public float goodWindowTimeEnd;
	[HideInInspector] public float perfectWindowTimeEnd;
	
	[HideInInspector] private int windowTiming;
	
	
	
	public WwiseClockSync wwiseClockSync;
	public BufferedMovement bufferedMovement;
	
	
	public GameObject goodParticles;
	public GameObject badParticles;
	public InterfaceController interfaceController;

	//different colors for particles and feedback text;
	public Color perfectColor, goodColor, okColor, missedColor;
	
	
	
	// Start is called before the first frame update
	void Start()
	{
		this.gameObject.AddComponent<AkGameObj>(); // we need this to be able to post events 
	}

	// Update is called once per frame
	void Update()
	{
		if (Input.anyKeyDown)
		{
			
		}
	}
	
	
	public void startInputTimer()
	
	{

		StartCoroutine(Buffer());
		

	}
	
	
	
	
	public bool WindowChecker()	
	{
		
		
		//float currentPositionInSong = wwiseClockSync.UpdatePositionInSong(wwiseClockSync.mainThemeID);
		// float secondsPerBeat = wwiseClockSync.UpdateBPM(wwiseClockSync.mainThemeID);
		
		currentPositionInSong = wwiseClockSync.UpdatePositionInSong(wwiseClockSync.mainThemeID);
		
		//Debug.Log("okWindowStart :" + okWindowTimeStart);
		//Debug.Log("SongPosition On Buttonpress :" + currentPositionInSong);
		//Debug.Log("okWindowEnd :" + okWindowTimeEnd);
		

		
		if (IsValueInRange(currentPositionInSong, perfectWindowTimeStart, perfectWindowTimeEnd))
		{	

			ParameterController.windowStatus = 4; 
			Debug.Log("Value is within the PERFECT Window!");
			return true;
			
			// Make a sound that fits the track and ducks the maintheme/ the btter the timing the better the filter curve
			// give it some particles
			// make the combo score go up 3 
			// make character bump
			// bring additional heat after some score --> more drums
			
			
			
		}
		
		else if (IsValueInRange(currentPositionInSong, goodWindowTimeStart, goodWindowTimeEnd))
		{
			Debug.Log("Value is within the GOOD Window!");
			ParameterController.windowStatus = 3; 
			return true;
		}
		
		else if (IsValueInRange(currentPositionInSong, okWindowTimeStart, okWindowTimeEnd))
		{
			Debug.Log("Value is within the OK Window!");
			ParameterController.windowStatus = 2; 
			return true;
		}
		
		else 
		
		{
			Debug.Log("Value is OUTSIDE the any Window!");
			ParameterController.windowStatus = 1; 
			return false;
			
		} 
	}
	
	public void UpdateAccordingToWindow()
	{
		
		//Debug.Log("I am called");
		
		interfaceController.ComboCounterUpdate(ParameterController.windowStatus);
		float punchScaleX; // = windowStatus / 2 
		float punchScaleY; // = windowStatus / 2 
		//PunchAnimation(windowStatus / 2, windowStatus / 2);
		
		if (ParameterController.windowStatus > 1) 
		{
			AkSoundEngine.SetRTPCValue("WindowLFOHit", ParameterController.windowStatus);
			AkSoundEngine.PostEvent("Play_PlayerMoveInput" , this.gameObject);
			
			if (ParameterController.windowStatus == 2)
			{

				punchScaleX = ParameterController.windowStatus / 2; 
				punchScaleY = 1; 
				PunchAnimation(punchScaleX, punchScaleY);
				
			
			}
			
			else if (ParameterController.windowStatus == 3)
			{
				punchScaleX = 1; 
				punchScaleY = ParameterController.windowStatus / 2; 
				PunchAnimation(punchScaleX, punchScaleY);
					
				
			}
			
			else if (ParameterController.windowStatus == 4)
			{
				
				punchScaleX = ParameterController.windowStatus / 2; 
				punchScaleY = ParameterController.windowStatus / 2; 
				PunchAnimation(punchScaleX, punchScaleY);
				
				
			}
			
		}
		
		else  // everything that happens on missed Inputs
		{
			AkSoundEngine.PostEvent("Play_PlayerMissedInput", this.gameObject);

		}
		
		
	}
	
	private void PunchAnimation(float punchScaleX, float punchScaleY)
	{
		transform.DOPunchScale(new Vector3(punchScaleX, punchScaleY, 1), 0.25f / ParameterController.playbackSpeed , 10, 1f);
	}
	
	
	bool IsValueInRange(float value, float min, float max)
	{
		if (value >= min && value <= max) 
		{
			return true;
		}
		
		return false;
	}
	
	void CacheTimingWindow() // we need to substract okTime so its fits again
	{
		
		currentPositionInSong = wwiseClockSync.UpdatePositionInSong(wwiseClockSync.mainThemeID) - okTime - bufferedMovement.inputBufferTime;
		secondsPerBeat = wwiseClockSync.UpdateBPM(wwiseClockSync.mainThemeID);
		
		okWindowTimeStart = currentPositionInSong + secondsPerBeat * 1000 - okTime;
		okWindowTimeEnd = currentPositionInSong +  secondsPerBeat * 1000 + okTime;
		goodWindowTimeStart = currentPositionInSong + secondsPerBeat * 1000 - goodTime;
		goodWindowTimeEnd = currentPositionInSong +  secondsPerBeat * 1000 + goodTime ;
		perfectWindowTimeStart = currentPositionInSong + secondsPerBeat * 1000 - perfectTime;
		perfectWindowTimeEnd = currentPositionInSong +  secondsPerBeat * 1000 + perfectTime;
	}
	
		IEnumerator Buffer()
	{
		float waitTime = okTime / 1000;
		yield return new WaitForSeconds(waitTime); // The window for the next Input only gets updated after the input is already to late anyway // this can surely be improved upon
		CacheTimingWindow();
		//Debug.Log("I enter buffer now Buffer");
	}	
	
	
}
