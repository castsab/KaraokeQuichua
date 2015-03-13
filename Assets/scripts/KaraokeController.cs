﻿using UnityEngine;
using System;
using System.Collections;
using UnityEngine.UI;

public class KaraokeController : MonoBehaviour {
	public Button finishButton;
	public Button pauseButton;
	public LyricSyncManager lyricSync;
	public string songLyricsText;

	public Action SongFinished {
		get;
		set;
	}

	public Action SongPaused {
		get;
		set;
	}
	
	void Start () {
		finishButton.onClick.AddListener(delegate {
			if(SongFinished != null){
				SongFinished();
				ChangePauseElements("Pausa", false);
			}
		});
		pauseButton.onClick.AddListener(delegate {
			if(SongPaused != null){
				SongPaused();
				ChangePauseState();
			}
		});
	}

	public void SetActive(){
		gameObject.SetActive (true);
	}
	
	public void SetInactive(){
		gameObject.SetActive (false);
	}

	private void ChangePauseState(){
		if(pauseButton.GetComponentInChildren<Text> ().text == "Pausa")
			ChangePauseElements("Continuar", true);
		else
			ChangePauseElements("Pausa", false);
	}

	private void ChangePauseElements(string pauseState, bool isVisibleFinishButton){
		pauseButton.GetComponentInChildren<Text> ().text = pauseState;
		finishButton.gameObject.SetActive(isVisibleFinishButton);
	}

	public void BeginSubtitles(string songLyric, AudioSource clip){
		lyricSync.BeginDialogue(songLyric, clip);
	}
}