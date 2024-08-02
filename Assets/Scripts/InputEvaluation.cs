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
	

	[HideInInspector] private int windowTiming;
	

	
	public WwiseClockSync wwiseClockSync;
	
	public ParameterController parameterController;
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
		//Debug.Log("Seconds per Beat: " + wwiseClockSync.UpdateBPM(wwiseClockSync.mainThemeID));
	}
	


	
	
	// is called when ever the player does a moveinput and checks whether the move should register depending on the type of window we hit
	public bool WindowChecker()	
	{

		//float secondsPerBeat = wwiseClockSync.UpdateBPM(wwiseClockSync.mainThemeID);
		
		float currentPositionInSong = wwiseClockSync.UpdatePositionInSong(wwiseClockSync.mainThemeID); 

		if (!WwiseClockSync.windowUpdateDone) // if this is true the inputs was earlier then the beat and the inputs are therefore checked before the queue update is activated. 
																				// current position is 1690 while Queue is (1800/1700) and cached position is 1600 as opposed to 1710 when Queue is (1900/1800) and cached is 1700
			{
				currentPositionInSong = currentPositionInSong - wwiseClockSync.UpdateBPM(wwiseClockSync.mainThemeID) * 1000; // parameterController.cachedPositionInSong - wwiseClockSync.UpdateBPM(wwiseClockSync.mainThemeID) * 1000;	// this is why we have reduce the currentposition w
				Debug.Log("WindowUpdate wasnt done and its currentposition is reduced by 1000");
			}
			
			
		//currentPositionInSong = parameterController.cachedPositionInSong;	
				
		// fetch current Position in Song as Callback from mainTheme
		//float secondsPerBeat = wwiseClockSync.UpdateBPM(wwiseClockSync.mainThemeID);
		
		Debug.Log("okWindowStart :" + parameterController.okWindowTimeStart);
		Debug.Log("SongPosition On Buttonpress :" + currentPositionInSong);
		Debug.Log("okWindowEnd :" + parameterController.okWindowTimeEnd);

		if (IsValueInRange(currentPositionInSong, parameterController.perfectWindowTimeStart, parameterController.perfectWindowTimeEnd))
		{	
			ParameterController.windowStatus = 4; 
			Debug.Log("Value is within the PERFECT Window!");
			return true;
		}
		
		else if (IsValueInRange(currentPositionInSong, parameterController.goodWindowTimeStart, parameterController.goodWindowTimeEnd))
		{
			Debug.Log("Value is within the GOOD Window!");
			ParameterController.windowStatus = 3; 
			return true;
		}
		
		else if (IsValueInRange(currentPositionInSong, parameterController.okWindowTimeStart, parameterController.okWindowTimeEnd))
		{
			Debug.Log("Value is within the OK Window!");
			ParameterController.windowStatus = 2; 
			return true;
		}
		
		else 	// The input was not inside a Window in which we register moves
		{
			Debug.Log("Value is OUTSIDE the any Window!");
			ParameterController.windowStatus = 1; 
			return false;
		} 
	}
	
	//public void CalculateTimingWindow() 
	//{
		// currentPositionInSong = wwiseClockSync.UpdatePositionInSong(wwiseClockSync.mainThemeID) - okTime - bufferedMovement.inputBufferTime;
		// secondsPerBeat = wwiseClockSync.UpdateBPM(wwiseClockSync.mainThemeID);
		
		// okWindowTimeStart = currentPositionInSong + secondsPerBeat * 1000 - okTime;
		// okWindowTimeEnd = currentPositionInSong +  secondsPerBeat * 1000 + okTime;
		// goodWindowTimeStart = currentPositionInSong + secondsPerBeat * 1000 - goodTime;
		// goodWindowTimeEnd = currentPositionInSong +  secondsPerBeat * 1000 + goodTime ;
		// perfectWindowTimeStart = currentPositionInSong + secondsPerBeat * 1000 - perfectTime;
		// perfectWindowTimeEnd = currentPositionInSong +  secondsPerBeat * 1000 + perfectTime;
		
		//currentPositionInSong = wwiseClockSync.UpdatePositionInSong(wwiseClockSync.mainThemeID);
	//}
	
	// public IEnumerator InputBuffer()
	// {
	// 	float waitTime = okTime / 1000; // okTime is the longest window time and we have currentPosition stored exactly one bar before we need to track in runtime but we only print it to the window value once then prior value is obsolete
	// 	yield return new WaitForSeconds(waitTime); // The window for the next Input only gets updated after the input is already to late anyway 
	// 	CacheTimingWindow();
	// 	//Debug.Log("I enter buffer now Buffer");
	// }	
	
	
	// This Function is ment for SFX and VFX depedning on the window we hit
	public void UpdateAccordingToWindow()
	{
		interfaceController.ComboCounterUpdate(ParameterController.windowStatus);
		interfaceController.WindowUpdaterText(ParameterController.windowStatus);
		float punchScaleX; 
		float punchScaleY; 
		
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
}
