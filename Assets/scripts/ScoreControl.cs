﻿using UnityEngine;
using System.Collections;

public class ScoreControl : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

		GetComponent<TextMesh>().text = "Scores : " + CreateGame.score;

	
	}
}
