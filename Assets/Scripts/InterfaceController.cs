using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InterfaceController : MonoBehaviour


{
	public TextMeshProUGUI ComboTextBox;
	
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
}
