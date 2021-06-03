using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ShopManager : MonoBehaviour {

    public Text pointText;
    private bool isSelect = true; //true is select, false is buy
    public Button switchButton;

    public List<Text> textList;

    // Use this for initialization
    void Start() {
        updateText();
    }

    private void updateText()
    {
        int points = PlayerPrefs.GetInt("points");
        string ship = PlayerPrefs.GetString("selectedShip");

        pointText.text = "Click on a ship to buy it with points\nPoints: " + points + "\nSelected Ship: " + ship;

        string allShips = PlayerPrefs.GetString("ship");
        Debug.Log(allShips);
        for(int i = 0; i < 7; i++)
        {
            if(allShips.Substring(i, 1).Equals("1"))
            {
                Debug.Log(i);
                textList[i].text = "Already\nOwn";
            }
        }
    }

    public void Decide(int type)
    {
        if (isSelect)
        {
            select(type);
        } else
        {
            buy(type);
        }

    }

    private void select(int type)
    {
        string list = PlayerPrefs.GetString("ship");
        string selected = PlayerPrefs.GetString("selectedShip");
        switch (type)
        {
            case 0:
                if (list.Substring(0, 1).Equals("1"))
                {
                    selected = "red";
                }
                break;
            case 1:
                if (list.Substring(1, 1).Equals("1"))
                {
                    selected = "orange";
                }
                break;
            case 2:
                if (list.Substring(2, 1).Equals("1"))
                {
                    selected = "yellow";
                }
                break;
            case 3:
                if (list.Substring(3, 1).Equals("1"))
                {
                    selected = "green";
                }
                break;
            case 4:
                if (list.Substring(4, 1).Equals("1"))
                {
                    selected = "blue";
                }
                break;
            case 5:
                if (list.Substring(5, 1).Equals("1"))
                {
                    selected = "purple";
                }
                break;
            case 6:
                if (list.Substring(6, 1).Equals("1"))
                {
                    selected = "pink";
                }
                break;
            default:
                break;
        }
        PlayerPrefs.SetString("selectedShip", selected);
        updateText();
    }

    private void buy(int type)
    {

        int points = PlayerPrefs.GetInt("points");
        string ships = PlayerPrefs.GetString("ship");
        switch (type)
        {
            case 0:
                if (points >= 10000 && ships.Substring(0, 1).Equals("0"))
                {
                    points -= 10000;
                    ships = ships.Substring(0, type) + "1" + ships.Substring(type + 1, 7 - (type + 1));

                }
                break;
            case 1:
                if (points >= 30000 && ships.Substring(1, 1).Equals("0"))
                {
                    points -= 30000;
                    ships = ships.Substring(0, type) + "1" + ships.Substring(type + 1, 7 - (type + 1));
                }
                break;
            case 2:
                if (points >= 50000 && ships.Substring(2, 1).Equals("0"))
                {
                    points -= 50000;
                    ships = ships.Substring(0, type) + "1" + ships.Substring(type + 1, 7 - (type + 1));

                }
                break;
            case 3:
                if (points >= 75000 && ships.Substring(3, 1).Equals("0"))
                {
                    points -= 75000;
                    ships = ships.Substring(0, type) + "1" + ships.Substring(type + 1, 7 - (type + 1));

                }
                break;
            case 4:
                if (points >= 100000 && ships.Substring(4, 1).Equals("0"))
                {
                    points -= 100000;
                    ships = ships.Substring(0, type) + "1" + ships.Substring(type + 1, 7 - (type + 1));

                }
                break;
            case 5:
                if (points >= 200000 && ships.Substring(5, 1).Equals("0"))
                {
                    points -= 200000;
                    ships = ships.Substring(0, type) + "1" + ships.Substring(type + 1, 7 - (type + 1));

                }
                break;
            case 6:
                if (points >= 500000 && ships.Substring(6, 1).Equals("0"))
                {
                    points -= 500000;
                    ships = ships.Substring(0, type) + "1" + ships.Substring(type + 1, 7 - (type + 1));

                }
                break;
            default:
                break;
        }
        PlayerPrefs.SetInt("points", points);
        PlayerPrefs.SetString("ship", ships);
        updateText();
    }

    public void change() {
        isSelect = !isSelect;
        if(isSelect)
        {
            Text buttonText = switchButton.GetComponentInChildren<Text>();
            buttonText.text = "SELECT";
        } else
        {
            Text buttonText = switchButton.GetComponentInChildren<Text>();
            buttonText.text = "BUY";
        }
    }

    public void back()
    {
        SceneManager.LoadScene(0);
    }

}
