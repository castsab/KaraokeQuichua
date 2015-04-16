﻿using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class IntroController : MonoBehaviour {
	[SerializeField]
	private Image image;
	[SerializeField]
	private Text tittle;
	[SerializeField]
	private Text information;
	[SerializeField]
	private Button continueButton;

	public Action ContinueButtonClicked;

	void Awake () {
		continueButton.onClick.AddListener (delegate {
			if(ContinueButtonClicked != null)
				ContinueButtonClicked();
		});
	}

	public void SetActive(){
		gameObject.SetActive (true);
	}
	
	public void SetInactive(){
		gameObject.SetActive (false);
	}

	public void SetLevelTittle(int numberLevel, string nameLevel){
		if(tittle != null){
			tittle.text = "<color=#E5C507FF>NIVEL " + numberLevel + "</color><size=" + (Screen.width/16) + "><color=#FFFFFFFF><b> " + nameLevel + "</b></color></size>";
		}
	}

	public void SetInformation(string informationText){
		if(tittle != null){
			information.text = informationText;
		}
	}

	public void SetSpriteImage(Sprite newSprite){
		if (image != null) {
			image.sprite = newSprite;
		}
	}

}
