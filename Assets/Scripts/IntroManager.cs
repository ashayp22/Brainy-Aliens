using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroManager : MonoBehaviour {

    public Star starPrefab;
    public Star starPrefab2;

    private List<Star> starList = new List<Star>();

    public List<GameObject> firstList = new List<GameObject>();
    public List<GameObject> secondList = new List<GameObject>();

    private bool up = false;
    private bool up2 = true;

    public static int STARNUMBER = 50;

	// Use this for initialization
	void Start () {

        //Player Prefs
        if(!PlayerPrefs.HasKey("points"))
        {
            PlayerPrefs.SetInt("points", 0);
            PlayerPrefs.SetString("ship", "1000000");
            PlayerPrefs.SetString("selectedShip", "red");
            PlayerPrefs.SetInt("totalpoints", 0);
            PlayerPrefs.SetInt("alienskilled", 0);
            PlayerPrefs.SetInt("aliensevolved", 0);

        }


        for (int i = 0; i < STARNUMBER/4; i++)
        {
            Star oneStar = Instantiate(starPrefab) as Star;
            oneStar.SetLocation(new Vector2(randomFloat(-12f, 12f), randomFloat(-5f, 5f)));
            Star twoStar = Instantiate(starPrefab2) as Star;
            twoStar.SetLocation(new Vector2(randomFloat(-12f, 12f), randomFloat(-5f, 5f)));
            Star threeStar = Instantiate(starPrefab2) as Star;
            threeStar.SetLocation(new Vector2(randomFloat(-12f, 12f), randomFloat(-5f, 5f)));
            Star fourStar = Instantiate(starPrefab2) as Star;
            fourStar.SetLocation(new Vector2(randomFloat(-12f, 12f), randomFloat(-5f, 5f)));
        }
	}

    private float randomFloat(float min, float max)
    {
        return Random.Range(min, max);
    }

    private void Update()
    {
        for(int i = 0; i < firstList.Count; i++)
        {
            if(up)
            {
                firstList[i].transform.position += new Vector3(0, 0.05f, 0);
            } else
            {
                firstList[i].transform.position -= new Vector3(0, 0.05f, 0);
            }
        }

        if(firstList[0].transform.position.y > 1.5)
        {
            up = false;
        } else if(firstList[0].transform.position.y < -1.5)
        {
            up = true;
        }

        for (int i = 0; i < secondList.Count; i++)
        {
            if (up2)
            {
                secondList[i].transform.position += new Vector3(0, 0.05f, 0);
            }
            else
            {
                secondList[i].transform.position -= new Vector3(0, 0.05f, 0);
            }
        }

        if (secondList[0].transform.position.y > 1.5)
        {
            up2 = false;
        }
        else if (secondList[0].transform.position.y < -1.5)
        {
            up2 = true;
        }

    }

    public void startGame()
    {
        SceneManager.LoadScene(1);
    }

    public void infoScene()
    {
        SceneManager.LoadScene(2);
    }

    public void shopScene()
    {
        SceneManager.LoadScene(3);
    }

    public void statScene()
    {
        SceneManager.LoadScene(4);
    }

}
