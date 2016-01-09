﻿using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;

public class TurbineInfo : InfoItem
{
	public string[] directions = { "N", "NE", "E", "SE", "S", "SW", "W", "NW" };
	public int directionIndex = 0;
	public string direction;
	public int originalOutput = 0;		//maxOutput+ Elevation effect
	public int output = 0;				//After powerLoss
	public int maxOutput = 140;
	public int elevation;
	public int powerLoss;
	public int cost;
	
	public float lossK;
	
	public int x;
	public int z;
	
	public int health = 100;			//Health percentage
	public float timeForWork;		//Seconds before repair
	public float timeAfterWork = 0f;
	
	public float timeForRepair;
	public float timeAfterRepair = 0f;
	
	public bool isWorking;
	public bool	isReparing; 
	
	public int costForRepair;
	public AudioClip click;
	public AudioClip breakdown;
	public AudioClip repair;
	public AudioClip construct;
	private AudioSource working;


	bool healthShow = false;
	
	
	void Start()
	{
		working = GetComponent<AudioSource>();
		AudioSource.PlayClipAtPoint(construct, Camera.main.transform.position);
		direction = directions[directionIndex];
		lossK = 0.001f;
		timeAfterWork = 0;
		
		isWorking = true;
		isReparing = false;
		
		//timeForWork = 30f;
		timeForRepair = 15f;
		costForRepair = 25;
		
		healthShow = false;
	}
	
	void Update()
	{
		if (isWorking) {
			working.mute = false;
			//CalculateOutput();
			if (Application.loadedLevelName == "Level1")
				powerLoss = 0;
			else
				powerLoss = (int)(lossK * originalOutput * originalOutput * powerLineInfo.length(transform.position, gameObject.transform.GetComponent<TurbineWorking>().transformerForTurbine.position));
			
			output = originalOutput - powerLoss;
			
			timeAfterWork += Time.deltaTime;
			health = 100 - (int)(100 * timeAfterWork/timeForWork);
			
			if(health <= 0)
			{
				AudioSource.PlayClipAtPoint(breakdown, Camera.main.transform.position);
				isWorking = false;
				isReparing = true;
			}
			
		}
		
		if (!isWorking) {
			working.mute = true;
			powerLoss = originalOutput;
			output = 0;
			
			if (isReparing) {
				
				timeAfterRepair += Time.deltaTime;
				
				if(timeAfterRepair >= timeForRepair){
					
					isReparing = false;
					
				}
				
			}
			
			if(!isReparing)
			{
				working.mute = true;				
			}
			
		}
		
		
		if (!healthShow) {
			
			transform.GetChild (1).GetChild (0).GetComponent<TurbineHealth> ().exit ();
			
		}
		
		else if (healthShow) {
			
			transform.GetChild (1).GetChild (0).GetComponent<TurbineHealth> ().enter ();
			
		}
		
	}
	
	public void CalculateOutput(int elevation)
	{
		this.elevation = elevation;
		outputFromElevation ();
	}
	
	//calculate output with the influence of elevation	
	public void outputFromElevation(){
		
		originalOutput = maxOutput + this.elevation / 10;
		
	}
	
	public override string GetInfo()
	{
		//        return "\nCurrent Direction: " + direction + "\nCurrent Power: " + output
		//            + "\nMax Power: " + maxOutput;
		
		//		return "\nOriginal Power: " + originalOutput + "\nCurrent Power: " + output + "\nCurrent Elevation: " + elevation + "\nPower Loss: " + powerLoss.ToString("0.00")
		//			+ "\nCost: $ " + cost;
		
		float timeRemains;
		
		if (!isWorking && !isReparing)
			return "Turbine\n\n\n\nTurbine is not working any more.";
		
		else if (!isWorking && isReparing) {
			
			timeRemains = timeForRepair - timeAfterRepair;
			return "Turbine\n\n\n\nTurbine is waiting to be repaired.\nTime remains for repair: " + (int)timeRemains + "s\nRepair Cost: " + costForRepair + " TC";
			
		}
		
		return "Turbine\n\n\n\n" + "\nPower Output: " + originalOutput + "\nSelling Cost: " + cost/2 + " TC";
		
	}
	
	void OnMouseDown()
	{
		if (LockUI.OverGui) return;

		AudioSource.PlayClipAtPoint (click, Camera.main.transform.position);
		GameObject.FindGameObjectWithTag ("screens").GetComponent<CustomizationSwitch> ().toSelectionP ();
		GameObject.FindGameObjectWithTag ("selectionPanel").GetComponent<InfoPanel> ().UpdateInfo (gameObject.transform.GetComponent<TurbineInfo>());
		GameObject sellButton = GameObject.FindGameObjectWithTag ("sellButton");
		GameObject repairButton = GameObject.FindGameObjectWithTag ("repairButton");
		sellButton.GetComponent<SellManager>().proposeSellTurbine(this.gameObject);
		repairButton.GetComponent<RepairManager> ().proposeRepairTurbine (transform);
		
		Debug.Log(GetInfo ());
		
		if (healthShow) {
			
			healthShow = false;
			
		}
		
		else if (!healthShow) {
			
			healthShow = true;
			
		}
		
	}
	
	
	//For repair the windTurbine
	public void Repair()
	{
		AudioSource.PlayClipAtPoint (repair, Camera.main.transform.position);
		MoneyManager.money -= costForRepair;
		timeAfterWork = 0;
		timeAfterRepair = 0;
		isWorking = true;
		isReparing = false;
		
	}
}
