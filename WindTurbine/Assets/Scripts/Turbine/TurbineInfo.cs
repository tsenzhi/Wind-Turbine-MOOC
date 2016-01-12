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

	public float timeForPowerLossShow;
	public float timeAfterPowerLossShow;
	
	public bool isWorking;
	public bool	isReparing; 
	
	public int costForRepair;
	public AudioClip click;
	public AudioClip breakdown;
	public AudioClip repair;
	public AudioClip construct;
	private AudioSource working;


	bool healthShow = false;
	bool powerLossShow;
	
	
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
		timeForPowerLossShow = 3f;
		
		healthShow = true;
		powerLossShow = true;
	}
	
	void Update()
	{
		if (isWorking) {
			working.mute = false;
			//CalculateOutput();
			if (Application.loadedLevelName == "Level1")
				powerLoss = 0;
			else{
				powerLoss = (int)(lossK * originalOutput * originalOutput * powerLineInfo.length(transform.position, gameObject.transform.GetComponent<TurbineWorking>().transformerForTurbine.position));
				Math.Min(originalOutput, powerLoss);
			}
			
			output = originalOutput - powerLoss;
			
			timeAfterWork += Time.deltaTime;
			timeAfterPowerLossShow += Time.deltaTime;

			if (Application.loadedLevelName != "Level1" && Application.loadedLevelName != "Level2")
				health = 100 - (int)(100 * timeAfterWork/timeForWork);
			
			if(health <= 0)
			{
				AudioSource.PlayClipAtPoint(breakdown, Camera.main.transform.position, 0.6f);
				isWorking = false;
				isReparing = true;
			}

			if(timeAfterPowerLossShow >= timeForPowerLossShow)
			{
				powerLossShow = false;
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
		
		if (transform == VisualizationManager.visualizedObject || health <= 10) {
			
			healthShow = true;
			
		} else {
			
			healthShow = false;
			
		}
		
		if (powerLossShow) {
			
			if(Application.loadedLevelName == "Level1")
				transform.GetChild (1).GetChild (0).GetComponent<TurbineHealth> ().enter ();
			else
				transform.GetChild (1).GetChild (0).GetComponent<TurbineHealth> ().enterWithPowerLoss ();
			
		}
		
		else if (healthShow) {
			
			transform.GetChild (1).GetChild (0).GetComponent<TurbineHealth> ().enter ();
			
		}
		
		else if (!healthShow) {
			
			transform.GetChild (1).GetChild (0).GetComponent<TurbineHealth> ().exit ();
			
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
			return "Turbine\n\n\n\nTurbine is waiting to be repaired.\nTime remaining until total loss: " + (int)timeRemains + "s\nRepair Cost: " + costForRepair + " TC";
			
		}
		
		return "Turbine\n\n\n\n" + "\nPower Output: " + originalOutput + "\nSelling Price: " + cost/2 + " TC";
		
	}
	
	void OnMouseDown()
	{
		if (LockUI.OverGui) return;

		AudioSource.PlayClipAtPoint (click, Camera.main.transform.position, 0.4f);
		GameObject.FindGameObjectWithTag ("screens").GetComponent<CustomizationSwitch> ().toSelectionP ();
		GameObject.FindGameObjectWithTag ("selectionPanel").GetComponent<InfoPanel> ().UpdateInfo (gameObject.transform.GetComponent<TurbineInfo>());

		if (Application.loadedLevelName != "Level1" && Application.loadedLevelName != "Level2") {
		

			GameObject repairButton = GameObject.FindGameObjectWithTag ("repairButton");
			repairButton.GetComponent<RepairManager> ().proposeRepairTurbine (transform);
		
		}

		GameObject sellButton = GameObject.FindGameObjectWithTag ("sellButton");
		sellButton.GetComponent<SellManager>().proposeSellTurbine(this.gameObject);
		
		Debug.Log(GetInfo ());
		
		VisualizationManager.visualizedObject = transform;
		
	}
	
	
	//For repair the windTurbine
	public void Repair(int cost)
	{
		AudioSource.PlayClipAtPoint (repair, Camera.main.transform.position, 0.4f);
		MoneyManager.money -= cost;
		timeAfterWork = 0;
		timeAfterRepair = 0;
		timeAfterPowerLossShow = 0;
		isWorking = true;
		isReparing = false;
		powerLossShow = true;
	}
}
