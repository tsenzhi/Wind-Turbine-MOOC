﻿using UnityEngine;
using System.Collections;

public class TransformerInfo : InfoItem
{
	
	public int power = 0;
	public int cost = 0;

	public int x;
	public int z;

    public override string GetInfo()
    {
		return "Transformer\n\n\n\n" + "Received Power: " + power;
    }

	void OnMouseDown()
	{
		GameObject.FindGameObjectWithTag ("screens").GetComponent<CustomizationSwitch> ().toSelectionP ();
		GameObject.FindGameObjectWithTag ("selectionPanel").GetComponent<InfoPanel> ().UpdateInfo (gameObject.transform.GetComponent<TransformerInfo>());
		Debug.Log(GetInfo ());
	}
}
