using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {
	
	private AudioSource audio;
	public Slider volumeSlider;
	public  void Start()

	{
		audio = FindObjectOfType<AudioSource> ();
	}
	//start game button fucntion 
	public void buttonStart()
	{
		SceneManager.LoadScene (1);
		audio = FindObjectOfType<AudioSource> ();
		
	}
	public void buttonQuit()
	{
		Debug.Log ("has quit game");
		Application.Quit ();
	}
	// turns the music on and off 
	public void musicOnOff()
	{
		if (audio.mute == true)
			audio.mute = false;
		else
			audio.mute = true;
	}
	// changes the volume of the music
	public void changeVolume()
	{
		audio.volume = volumeSlider.value;
	}
}