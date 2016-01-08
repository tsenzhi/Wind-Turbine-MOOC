﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TurbineHealth : MonoBehaviour {

	TurbineInfo myTurbineInfo;
	bool myTurbineIsWorking;
	bool myTurbineIsRepairing;

	// Use this for initialization
	void Start () {
		myTurbineInfo = transform.parent.parent.GetComponent<TurbineInfo> ();
	}
	
	// Update is called once per frame
	void Update () {
	
		myTurbineIsWorking = myTurbineInfo.isWorking;
		myTurbineIsRepairing = myTurbineInfo.isReparing;

		if (myTurbineIsWorking) {
		
			transform.GetChild(2).GetComponent<Image>().fillAmount = 0;
			transform.GetChild(1).GetComponent<Image>().fillAmount = (float)myTurbineInfo.health / 100.0f;
		
		}

		if (!myTurbineIsWorking) {
		
			transform.GetChild(1).GetComponent<Image>().fillAmount = 0;

			if(!myTurbineIsRepairing)
			{
				transform.GetChild(2).GetComponent<Image>().fillAmount = 0;
			}

			else
			{
				transform.GetChild(2).GetComponent<Image>().fillAmount = myTurbineInfo.timeAfterRepair / myTurbineInfo.timeForRepair;
			}
		
		}

		//transform.GetChild(3).GetComponent<Text>().text = myTurbineInfo.output + " kW";

	}

	public void enter(){

//		Debug.Log ("Mouse Enter");

		Color color = transform.GetChild (0).GetComponent<Image> ().color;
		transform.GetChild (0).GetComponent<Image> ().color = new Color(color.r, color.g, color.b, 1f);

		color = transform.GetChild (1).GetComponent<Image> ().color;
		transform.GetChild (1).GetComponent<Image> ().color = new Color(color.r, color.g, color.b, 1f);

		color = transform.GetChild (2).GetComponent<Image> ().color;
		transform.GetChild (2).GetComponent<Image> ().color = new Color(color.r, color.g, color.b, 1f);

		transform.GetChild(3).GetComponent<Text>().text = myTurbineInfo.output + " kW";

	}

	public void exit(){

//		Debug.Log ("Mouse Exit");

		Color color = transform.GetChild (0).GetComponent<Image> ().color;
		transform.GetChild (0).GetComponent<Image> ().color = new Color(color.r, color.g, color.b, 0f);
		
		color = transform.GetChild (1).GetComponent<Image> ().color;
		transform.GetChild (1).GetComponent<Image> ().color = new Color(color.r, color.g, color.b, 0f);
		
		color = transform.GetChild (2).GetComponent<Image> ().color;
		transform.GetChild (2).GetComponent<Image> ().color = new Color(color.r, color.g, color.b, 0f);

		transform.GetChild(3).GetComponent<Text>().text = "";
	}
}
