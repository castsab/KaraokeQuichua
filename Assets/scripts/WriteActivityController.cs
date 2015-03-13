using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class WriteActivityController : MonoBehaviour {
	public Button exitButton;
	public PhraseUI phraseUI;
	public WordUI wordUI;
	public GameObject WinMessage;
	private Phrase phrase;

	public Action BackActionExecuted {
		get;
		set;
	}

	void Start () {
		exitButton.onClick.AddListener(delegate {
			if(BackActionExecuted != null){
				WinMessage.SetActive(false);
				BackActionExecuted();
			}
		});
		wordUI.LetterButtonSelected += HandleLetterButtonSelected;
		phraseUI.WordFinished += HandleWordFinished;
		phraseUI.PhraseFinished += HandlePhraseFinished;
	}

	void HandleLetterButtonSelected (Button letterButton) {
		string letter = letterButton.transform.GetChild (0).GetComponent<Text> ().text;
		letterButton.interactable = !phraseUI.CheckCorrectLetter (letter);
	}

	void HandleWordFinished (int indexNextWord)	{
		DestroyWord ();
		wordUI.DrawWord (GetHiddenWord(phrase.words, indexNextWord));
	}

	void HandlePhraseFinished () {
		WinMessage.SetActive(true);
	}
	
	public void SetActive(){
		gameObject.SetActive (true);
	}
	
	public void SetInactive(){
		gameObject.SetActive (false);
	}

	public void Reset(Song song){
		DestroyPhrase ();
		DestroyWord ();
		phrase = GetRandomPhraseFromSong (song);
		phraseUI.DrawPhrase (phrase);
		wordUI.DrawWord (GetHiddenWord(phrase.words, 0));
	}

	private Phrase GetRandomPhraseFromSong(Song song){
		if (song.phrases.Count != 0) {
			int randomIndex = UnityEngine.Random.Range(0, song.phrases.Count);
			return song.phrases[randomIndex];		
		}
		throw new Exception ("La cancion no tiene frases");
	}

	private void SetPhrase(Phrase phrase){
		this.phrase = phrase;
	}

	private string GetHiddenWord (List<Word> words, int hiddenWordNumber){
		int hiddenWordCounter = 0;
		foreach (Word word in words) {
			if (word.isHidden){
				if (hiddenWordNumber == hiddenWordCounter)
					return word.text;

				hiddenWordCounter++;
			}

		}
		return null;
	}

	private void DestroyPhrase(){
		phraseUI.ClearHiddenWords ();
		foreach(Transform  child in phraseUI.transform ) {
			Destroy (child.gameObject);
		}
	}

	private void DestroyWord(){
		foreach(Transform  child in wordUI.transform ) {
			Destroy (child.gameObject);
		}
	}
}