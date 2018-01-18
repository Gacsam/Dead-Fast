using System.Collections;
using UnityEngine;
using System;
[Serializable]
public class Stat 
{
	public BarScript bar;
	public float maxVal;
	public float currentVal;

	void Start()
	{
		currentVal = 10;
		maxVal = 10;


	}

	public float CurrentVal {
		get {
			return currentVal;

		}

		set {
			currentVal = value;

			if (currentVal > maxVal)
				currentVal = maxVal;
		
			bar.Value = currentVal;
		}
	}
		public float MaxVal

		{
			get
			{
				return maxVal;

			}

			set
			{
				this.maxVal = value;
				bar.MaxValue = maxVal;
			}


	}

	public void Initialize()
	{
		bar = GameObject.FindGameObjectWithTag("UI").GetComponentInChildren<BarScript> ();
		this.MaxVal = maxVal;
		this.CurrentVal = currentVal;
	}

}

		
