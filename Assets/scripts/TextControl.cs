using UnityEngine;
using System.Collections;

public class TextControl : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
		GetComponent<TextMesh>().text = "Turns : " + CreateGame.remainingTurns;

	}
}
