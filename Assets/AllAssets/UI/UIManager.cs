using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {
	
	private AudioSource theAudio;
	public Slider volumeSlider;

	// GET MENU OPTIONS
	public int zombieCount = 15;
	public int maxZombies = 50;
	public int maxTime = 120;

	public  void Start()
	{
		theAudio = FindObjectOfType<AudioSource> ();
		DontDestroyOnLoad (this);
	}

	//start game button fucntion 
	public void buttonStart()
	{
		SceneManager.LoadScene (1);
	}

	void OnLevelWasLoaded(int level){
		if (level == 1) {
			GameUI theUI = FindObjectOfType<GameUI> ();
			theAudio.volume = 0.5f; // GET MENU OPTIONS
		}
	}

	public void buttonQuit()
	{
		Debug.Log ("has quit game");
		Application.Quit ();
	}
	// turns the music on and off 
	public void musicOnOff()
	{
		if (theAudio.mute == true)
			theAudio.mute = false;
		else
			theAudio.mute = true;
	}
	// changes the volume of the music
	public void changeVolume()
	{
		theAudio.volume = volumeSlider.value;
	}
}