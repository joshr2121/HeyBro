using UnityEngine;
using System.Collections;

public class Enemy_Faces : MonoBehaviour {
	public Sprite normal;
	public Sprite angry;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		}

	public void SetSprite (int sprite) {
		switch (sprite) {
		case 0: 
			gameObject.GetComponent<SpriteRenderer>().sprite = normal;
			break;
		case 1:
			gameObject.GetComponent<SpriteRenderer>().sprite = angry;
			break;
		}
	
	}
}
