
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Tile
{
	public GameObject tileObj;
	public string type;

	public Tile (GameObject obj, string tile)
	{
		tileObj = obj;
		type = tile;
	}
}


public class CreateGame : MonoBehaviour
{

	GameObject tile1 = null;
	GameObject tile2 = null;

	public static int remainingTurns = 40;
	public static int score = 0;

	public GameObject[] tile;
	List<GameObject> tileBank = new List<GameObject> ();

	static int rows = 10;
	static int cols = 10;
	bool renewBoard = false;

	Tile[,] tiles = new Tile[cols, rows];

	void ShuffleList ()
	{
		System.Random rand = new System.Random ();
		int gemsCount = tileBank.Count;
		while (gemsCount > 1) {
			gemsCount--;
			int gemsNum = rand.Next (gemsCount + 1);
			GameObject val = tileBank [gemsNum];
			tileBank [gemsNum] = tileBank [gemsCount];
			tileBank [gemsCount] = val;
		}
	}



	// Use this for initialization
	void Start ()
	{

		int numCopies = (rows * cols) / 3;

		for (int i = 0; i < numCopies; i++) {
			for (int j = 0; j < tile.Length; j++) {
				GameObject objec = (GameObject)Instantiate (tile [j], new Vector3 (-10, -10, 0), tile [j].transform.rotation);
				objec.SetActive (false);
				tileBank.Add (objec);
			}
		}

		ShuffleList ();
		
		// create grid
		for (int rowNum = 0; rowNum < rows; rowNum++) {
			for (int colNum = 0; colNum < cols; colNum++) {
				Vector3 tilePos = new Vector3 (colNum, rowNum, 0);
			
				for (int gemsNum = 0; gemsNum < tileBank.Count; gemsNum++) {
					GameObject objec = tileBank [gemsNum];

					if (!objec.activeSelf) {
						objec.transform.position = new Vector3 (tilePos.x, tilePos.y, tilePos.z);
						objec.SetActive (true);
						tiles [colNum, rowNum] = new Tile (objec, objec.name);
						gemsNum = tileBank.Count + 1;
					}
				}
			}
		}
		CheckGrid ();
	}
	
	// Update is called once per frame
	void Update ()
	{
		
		if (Input.GetMouseButtonDown (0)) {
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			RaycastHit2D hit = Physics2D.GetRayIntersection (ray, 1000);

			if (hit) {
				tile1 = hit.collider.gameObject;
			}

			CreateGame.remainingTurns -= 1;

		} else if (Input.GetMouseButtonUp (0) && tile1) {
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			RaycastHit2D hit = Physics2D.GetRayIntersection (ray, 1000);

			if (hit) {
				tile2 = hit.collider.gameObject;
			}

			if (tile1 && tile2) {
				int horzDist = (int)Mathf.Abs (tile1.transform.position.x - tile2.transform.position.x);
				int vertDist = (int)Mathf.Abs (tile1.transform.position.y - tile2.transform.position.y);

				//Debug.Log ("horzDist: " + horzDist);
				//Debug.Log ("vertDist: " + vertDist);

				if ((horzDist == 1 && vertDist == 0) || (horzDist == 0 && vertDist == 1)) {
					Tile temp = tiles [(int)tile1.transform.position.x, (int)tile1.transform.position.y];
					tiles [(int)tile1.transform.position.x, (int)tile1.transform.position.y] = tiles [(int)tile2.transform.position.x, (int)tile2.transform.position.y];
					tiles [(int)tile2.transform.position.x, (int)tile2.transform.position.y] = temp;

					Vector3 tempPos = tile1.transform.position;
					tile1.transform.position = tile2.transform.position;
					tile2.transform.position = tempPos;

					tile1 = null;
					tile2 = null;

					CheckGrid ();
				} else {
					GetComponent<AudioSource> ().Play ();
				}

				
			}
			

		}
	}

	void CheckGrid ()
	{
		int counter = 1;
		// check in colums
		for (int rowNum = 0; rowNum < rows; rowNum++) {
			counter = 1;
			for (int colNum = 1; colNum < cols; colNum++) {
				if (tiles [colNum, rowNum] != null && tiles [colNum - 1, rowNum] != null){
					// if tiles exist
					if (tiles [colNum, rowNum].type == tiles [colNum - 1, rowNum].type) {
						counter++;
						if (colNum == (rows - 1) && counter >= 3) { 
							markRow (colNum + 1, rowNum, counter); 
						}
					} else {
						//if three found remove
						if (counter >= 3) {
							markRow (colNum, rowNum, counter);
						}
						counter = 1; // reset counter
					}
				}
			}
		}
		//check in rows
		for (int colNum = 0; colNum < rows; colNum++) { 
			counter = 1; 
			for (int rowNum = 1; rowNum < cols; rowNum++) { 
				if (tiles [colNum, rowNum] != null && tiles [colNum, rowNum - 1] != null){ 
					// if tiles exist 
					if (tiles [colNum, rowNum].type == tiles [colNum, rowNum - 1].type) { 
						counter++; 
						if (rowNum == (cols - 1) && counter >= 3) { 
							markCol (colNum, rowNum + 1, counter);
					}
						} else { 
							if (counter >= 3) { 
								markCol (colNum, rowNum, counter); 
							} 
							counter = 1; // reset counter 
						} 
					 
				} 
			} 
		}

		if (renewBoard) {
			RenewGrid ();
			renewBoard = false;
		}
	}

	void RenewGrid ()
	{
		bool anyMoved = false;
		ShuffleList ();

		for (int rowNum = 1; rowNum < rows; rowNum++) {
			for (int colNum = 0; colNum < cols; colNum++) {
				if (rowNum == rows - 1 && tiles [colNum, rowNum] == null) {
					Vector3 tilePos = new Vector3 (colNum, rowNum, 0);

					for (int gemsNum = 0; gemsNum < tileBank.Count; gemsNum++) {
						GameObject objec = tileBank [gemsNum];

						if (!objec.activeSelf) {
							objec.transform.position = new Vector3 (tilePos.x, tilePos.y, tilePos.z);
							objec.SetActive (true);
							tiles [colNum, rowNum] = new Tile (objec, objec.name);
							gemsNum = tileBank.Count + 1;
						}
					}
				}

				if (tiles [colNum, rowNum] != null) {
					// drop down if space below is empty
					if (tiles [colNum, rowNum - 1] == null) {
						tiles [colNum, rowNum - 1] = tiles [colNum, rowNum];
						tiles [colNum, rowNum - 1].tileObj.transform.position = new Vector3 (colNum, rowNum - 1, 0);
						tiles [colNum, rowNum] = null;
						anyMoved = true;
					}
				}
			}
		}

		if (anyMoved) {
			Invoke ("RenewGrid", 0.6f); 
		} else {
			Invoke ("CheckGrid", 0.2f);
		}
	}

	private void markRow (int startCol, int rowNum, int count)
	{
		for (int i = 1; i <= count; i++) {
			if (tiles [startCol - i, rowNum] != null)
				tiles [startCol - i, rowNum].tileObj.SetActive (false);						
			tiles [startCol - i, rowNum] = null;
		}
		Debug.Log ("counter: " + count);
		renewBoard = true;
		CreateGame.score += 1;			
	}

	private void markCol (int colNum, int startRow, int count) 
	{ 
		for (int i = 1; i <= count; i++) { 
			if (tiles [colNum, startRow - i] != null) 
				tiles [colNum, startRow - i].tileObj.SetActive (false); 
			tiles [colNum, startRow - i] = null; 
		} 
		Debug.Log ("counterCol: " + count); 
		renewBoard = true; 
		CreateGame.score += 1; 
	}

}

