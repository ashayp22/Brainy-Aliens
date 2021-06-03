using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Stats : MonoBehaviour {

    public Text statText;

	// Use this for initialization
	void Start () {
        statText.text = "Total Points: " + PlayerPrefs.GetInt("totalpoints") + "\nAliens Killed: " + PlayerPrefs.GetInt("alienskilled") + "\nAliens Evolved: " + PlayerPrefs.GetInt("aliensevolved") + "\n\nDeveloper & Music: Ashay Parikh\nPlease give this game a rating and review!";
	}
}
