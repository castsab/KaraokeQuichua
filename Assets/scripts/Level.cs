using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class Level : MonoBehaviour {
	//Data level
	public int numberLevel;
	public int scoreLevel;
	public string nameLevel;
	public bool isUnlocked;
	[SerializeField]
	private KaraokeController karaoke;
	[SerializeField]
	private List<Activity> activities;
	[SerializeField]
	private bool introScreenItsOpened;
	private static readonly object syncLock = new object();

	//GUI objects
	[SerializeField]
	private Button activityButtonPrefab;
	private Text levelNumber;
	private Text levelName;
	[SerializeField]
	private IntroController introController;
	private GameObject activityListUI;
	private ProgressBarController barLevel;
	private Button introButton;
	protected GameStateBehaviour gameStateBehaviour;

	public Action<Level> UnlockNextLevel;

	void Awake(){
		FindObjectInScene ();
		SetUpActivities ();
		ClearActivitysList ();

		introButton.onClick.AddListener (delegate {
			gameStateBehaviour.GameState = GameState.ShowingIntro;
		});
	}

	public void BeginLevel () {
		SetUpIntroScreen ();
		OpenIntroScreenFirstTime ();
		ShowActivitysList ();
		PlayPreviewWordActivity ();
		SetUpLevelProperties ();

		if (IsAllActivitiesCompleted()) {
			if (UnlockNextLevel != null)
				UnlockNextLevel (this);
		}
	}
	
	public void StopPreviewWordActivity(){
		WordActivity wordActivity = FindWordActivity ();
		if (wordActivity != null)
			wordActivity.StopSong ();
	}

	public bool IsAllActivitiesCompleted(){
		foreach (Activity activity in activities)
			if (!activity.IsCompleted)
				return false;

		return true;
	}

	public int GetTotalScore(){
		int scoreLevel = 0;
		foreach (Activity activity in activities)
			scoreLevel += activity.ScoreObtained;
		GameSettings.Instance.scoreByCurrentLevel = scoreLevel;
		return scoreLevel;
	}

	private void FindObjectInScene ()	{
		activityListUI = GameObject.FindGameObjectWithTag ("ActivityList");
		introButton = GameObject.FindGameObjectWithTag ("IntroButton").GetComponent (typeof(Button)) as Button;
		barLevel = GameObject.FindGameObjectWithTag ("ProgressBarLevel").GetComponent (typeof(ProgressBarController)) as ProgressBarController;
		gameStateBehaviour = GameObject.FindGameObjectWithTag ("GameState").GetComponent (typeof(GameStateBehaviour)) as GameStateBehaviour;
		levelNumber = GameObject.FindGameObjectWithTag ("MainLevelNumber").GetComponent (typeof(Text)) as Text;
		levelName = GameObject.FindGameObjectWithTag ("MainLevelName").GetComponent (typeof(Text)) as Text;
	}

	private void SetUpLevelProperties()	{
		levelNumber.text = "NIVEL " + numberLevel;
		levelName.text = nameLevel;
		GameSettings.Instance.nameLevel [0] = numberLevel.ToString ();
		GameSettings.Instance.nameLevel [1] = nameLevel;
		scoreLevel = GetTotalScore ();
		barLevel.SetFillerSize (scoreLevel);
	}

	private void OpenIntroScreenFirstTime(){
		lock (syncLock) {
			if (CanOpenIntroScreen ()) { 
				gameStateBehaviour.GameState = GameState.ShowingIntro;
				introScreenItsOpened = true;
			}
		}
	}

	public bool CanOpenIntroScreen ()	{
		return introController != null && !introScreenItsOpened;
	}

	private void SetUpIntroScreen(){
		introScreenItsOpened = LevelDataPersistent.IsLevelIntroOpened (numberLevel);

		if(!introScreenItsOpened)
			introController.ContinueButtonClicked = HandleFirstTimeContinueButtonClicked;
		else
			introController.ContinueButtonClicked = HandleContinueButtonClicked;
	}

	private void SetUpActivities(){	
		int index = 0;
		foreach (Activity activity in activities) {
			if(activity != null){
				activity.SetLevel(numberLevel);
				activity.ResetData();
				activity.ActivityCompleted = HandleActivityCompleted;
				DrawActivity(activity, index);
				index++;
			}
		}
	}

	private void DrawActivity (Activity activity, int index){
		Button newItem = Instantiate(activityButtonPrefab) as Button;
		newItem.name = GetActivityType(activity);
		newItem.transform.FindChild("Title").GetComponent<Text>().text = GetActivityName(activity, 0);
		newItem.transform.FindChild("Description").GetComponent<Text>().text = GetActivityName(activity, 1);
		newItem.transform.SetParent(activityListUI.gameObject.transform, false);	
		newItem.onClick.AddListener(delegate {
			HandleClickButtonActivity(newItem.gameObject);
		});
		if (activity.IsDataFound ()) {
			newItem.interactable = true;
		} else {
			newItem.interactable = false;
		}
	}

	private string GetActivityType(Activity activity){
		return activity.GetType ().ToString();
	}

	private string GetActivityName (Activity activity, int index){
		string name = "";

		if(activity is  WordActivity)
			name =  GameSettings.Instance.wordActivityTag[index];
		if(activity is WriteActivity)
			name = GameSettings.Instance.writeActivityTag[index];

		return name;
	}

	private void ShowActivitysList () {
		ClearActivitysList ();
		SetUpActivities ();
	}

	private void ClearActivitysList () {
		foreach(Transform  child in activityListUI.transform ) {
			Destroy (child.gameObject);
		}
	}

	private WordActivity FindWordActivity(){
		foreach (Activity activity in activities) {
			if(activity is WordActivity)
				return (WordActivity) activity;
		}
		return null;
	}

	private void PlayPreviewWordActivity(){
		WordActivity wordActivity = FindWordActivity ();
		if (wordActivity != null && wordActivity.IsDataFound ())
				wordActivity.PlayPreview ();
	}

	private void HandleFirstTimeContinueButtonClicked (){
		LevelData data = new LevelData();
		data.level = numberLevel;
		data.isUnlocked = true;
		data.isIntroOpened = true;
		LevelDataPersistent.SaveLevelData(data);

		introController.ContinueButtonClicked = HandleContinueButtonClicked;

		WordActivity wordActivity = FindWordActivity ();
		if (wordActivity != null && wordActivity.IsDataFound ()) {
			wordActivity.StopSong ();
			wordActivity.StartActivity ();
		} else {
			gameStateBehaviour.GameState = GameState.SelectingLevel;
		}
	}

	private void HandleContinueButtonClicked (){
		gameStateBehaviour.GameState = GameState.SelectingLevel;
	}

	private void HandleClickButtonActivity (GameObject activityButton) {
		foreach (Activity activity in activities) {
			if(activityButton.name == GetActivityType(activity)){
				StopPreviewWordActivity();
				activity.StartActivity();
			}
		}
	}
	
	private void HandleActivityCompleted ()	{
		scoreLevel = GetTotalScore ();
		barLevel.SetFillerSize (scoreLevel);

		if (IsAllActivitiesCompleted()) {
			if (UnlockNextLevel != null)
				UnlockNextLevel (this);
		}
	}
}