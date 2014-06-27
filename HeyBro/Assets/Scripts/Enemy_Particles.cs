﻿using UnityEngine;
using System.Collections;

public class Enemy_Particles : MonoBehaviour {

	public bool partVisible;
	public ParticleSystem laser;
	public ParticleSystem laser2;
	public float attackTime = 2.0f;

	// Use this for initialization
	void Start () {
		partVisible = false;
		laser.enableEmission = false;
		laser2.enableEmission = false;
	}
	
	// Update is called once per frame
	void Update () {
		if (partVisible == true)
		{
			laser.enableEmission = true;
			laser2.enableEmission = true;
			GameObject.Find ("Enemy_Face").GetComponent<Enemy_Faces>().SetSprite (3);
			attackTime -= Time.deltaTime;
		}
		if (attackTime <= 0)
		{
			partVisible = false;
			laser.enableEmission = false;
			laser2.enableEmission = false;
			GameObject.Find("Forcefield").GetComponent<Display_Forcefield>().showField = false;
			GameObject.Find ("Enemy_Face").GetComponent<Enemy_Faces>().SetSprite (0);
			attackTime = 2.0f;
		}
	
	}
}
