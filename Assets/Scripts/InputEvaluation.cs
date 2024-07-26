using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using UnityEngine;
using UnityEngine.UI;

public class InputEvaluation : MonoBehaviour
{
	
	public float okTime;
	public float goodTime;
	public float perfectTime;
	
	public float currentPositionInSong;
	public float secondsPerBeat;
	
	[HideInInspector] public float okWindowTimeStart;
	[HideInInspector] public float goodWindowTimeStart;
	[HideInInspector] public float perfectWindowTimeStart;
	
	[HideInInspector] public float okWindowTimeEnd;
	[HideInInspector] public float goodWindowTimeEnd;
	[HideInInspector] public float perfectWindowTimeEnd;
	
	
	
	public WwiseClockSync wwiseClockSync;
	
	
	public GameObject goodParticles;
	public GameObject badParticles;

	//different colors for particles and feedback text;
	public Color perfectColor, goodColor, okColor, missedColor;
	
	// Start is called before the first frame update
	void Start()
	{

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
		
		Debug.Log("okWindowStart :" + okWindowTimeStart);
		Debug.Log("SongPosition On Buttonpress :" + currentPositionInSong);
		Debug.Log("okWindowEnd :" + okWindowTimeEnd);
		

		
		if (IsValueInRange(currentPositionInSong, perfectWindowTimeStart, perfectWindowTimeEnd))
		{
			Debug.Log("Value is within the PERFECT Window!");
			return true;
			
		}
		
		else if (IsValueInRange(currentPositionInSong, goodWindowTimeStart, goodWindowTimeEnd))
		{
			Debug.Log("Value is within the GOOD Window!");
			return true;
		}
		
		else if (IsValueInRange(currentPositionInSong, okWindowTimeStart, okWindowTimeEnd))
		{
			Debug.Log("Value is within the OK Window!");
			return true;
		}
		
		else 
		
		{
			Debug.Log("Value is OUTSIDE the any Window!");
			return false;
			
		} 
		
		
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
		
		currentPositionInSong = wwiseClockSync.UpdatePositionInSong(wwiseClockSync.mainThemeID) - okTime;
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
		yield return new WaitForSeconds(waitTime); // The window for the next Input only gets updated after the input is already to late anyway 
		CacheTimingWindow();
		//Debug.Log("I enter buffer now Buffer");
	}	
	
	
}
