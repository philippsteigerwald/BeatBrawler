using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class PunchAnimation : MonoBehaviour
{
	public float punchScaleX = 1f;
	public float punchScaleY = 1f;
	//simple punch scale effect from DoTween
	public void PunchAnimationEffect()
	{
		transform.DOPunchScale(new Vector3(punchScaleX, punchScaleY, 1), 0.25f / ParameterController.playbackSpeed , 10, 0f);
	}
	/// 
	
}