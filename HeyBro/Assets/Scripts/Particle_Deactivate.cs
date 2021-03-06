﻿using UnityEngine;
using System.Collections;

public class Particle_Deactivate : MonoBehaviour {

	public bool partVisible;
	public float attackTime = 2.0f;
	public ParticleSystem particle;
	public ParticleSystem particle2;
	
	// Use this for initialization
	void Start () {
		partVisible = false;
		particle.enableEmission = false;
		particle2.enableEmission = false;
	
	}
	
	// Update is called once per frame
	void Update () {

		if (partVisible == true)
		{
			particle.enableEmission = true;
			particle2.enableEmission = true;
			GameObject.Find ("Enemy_Face").GetComponent<Enemy_Faces>().SetSprite (1);
			attackTime -= Time.deltaTime;
		}
		if (attackTime <= 0)
		{
			partVisible = false;
			particle.enableEmission = false;
			particle2.enableEmission = false;
			GameObject.Find ("Enemy_Face").GetComponent<Enemy_Faces>().SetSprite (0);
			attackTime = 2.0f;
		}
	
	}
}
