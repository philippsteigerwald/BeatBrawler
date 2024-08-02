using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Data;

public class InterfaceController : MonoBehaviour


{
	public TextMeshProUGUI ComboTextBox;
	public TextMeshProUGUI WindowFeedback;
	
	// Start is called before the first frame update
	void Start()
	{
		
	}

	// Update is called once per frame
	void Update()
	{
		
	}
	
	
	public void ComboCounterUpdate(int window)
	
	{
		
		if ( window == 1)
		{
			ParameterController.comboCounter = 0;
			
		}
		
		
		ParameterController.comboCounter +=  window - 1; 
		
		ComboTextBox.text = ParameterController.comboCounter.ToString();
	}
	
	public void WindowUpdaterText(int window)
	{
		WindowFeedback.enabled = true;
		
		if ( window == 1 )
		{
			WindowFeedback.text = "Missed";
		}
		
		if ( window == 2 )
		{
			WindowFeedback.text = "Ok! + 1 Score";
		}
		
		if ( window == 3 )
		{
			WindowFeedback.text = "Good! + 2 Score";
		}
		
		if ( window == 4 )
		{
			WindowFeedback.text = "Perfect! + 3 Score";
		}
		
		Invoke("DeactivateWindowFeedback", 0.7f);
	}
	
	private void DeactivateWindowFeedback()
	{
		WindowFeedback.enabled = false;
	}
}
