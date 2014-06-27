using UnityEngine;
using System.Collections;

public class Display_Forcefield : MonoBehaviour {

	public bool showField;
	public SpriteRenderer forcefield;

	// Use this for initialization
	void Start () {
		forcefield.renderer.enabled = false;
		showField = false;
	
	}
	
	// Update is called once per frame
	void Update () {
		if (showField == true)
		{
			forcefield.renderer.enabled = true;
		}

	
	}
}
