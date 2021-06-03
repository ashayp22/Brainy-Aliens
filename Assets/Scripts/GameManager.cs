using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    //PREFABS

    public Gun gunPrefab;
    public Alien alienPrefab;

    //INSTANCES
    private Gun gunInstance;

    private List<Alien> alienPopulation = new List<Alien>(); //list of aliens in the population

    private List<Alien> currentAliens = new List<Alien>(); //list of aliens currently in the game
    
    //INTs

    private int aliensCreatedSoFar; //how many aliens have been spawned(instantaniated)

    private int gameTicks; //ticks for the game;

    //booleans

    private bool gameOver = false; //if the game is over
    private bool alreadySetPanel = false; //if the panel has been set before the game starts

    //UI
    public Image autoBar;
    public Text scoreText; //score
    public GameObject startPanel;
    public Text panelText;
    public Text leftText; //time left

    //constant/static project settings
    public static int POPSIZE = 5;
    public static double MUTATIONRATE = 0.2;
    public static int TOURNAMENTNUM = 10;
    public static int ALIENSINGAME = 5;

    public static int HIDDENLAYERS = 1;
    public static int NEURONSPERLAYER = 15;

    public static int TOTALGAMETICKS = 2505;
    //other
    private List<string> panelTextList = new List<string>();

    //Audio
    public AudioSource hitAudio;
    public AudioSource startAudio;
    public AudioSource restartAudio;
    public AudioSource losingAudio;
    public AudioSource waitingAudio;

    //for continuing and exiting the game
    private Vector2 firstPressPos;
    private Vector2 currentSwipe;

    // Use this for initialization
    void Start () {
        gunInstance = Instantiate(gunPrefab) as Gun;
        gunInstance.Init(3, new Vector2(0, -4));

        string gun = PlayerPrefs.GetString("selectedShip");
        if(gun.Equals("red"))
        {
            SpriteRenderer renderer = gunInstance.GetComponent<SpriteRenderer>();
            renderer.color = new Color32(255, 0, 0, 255);
        } else if(gun.Equals("orange"))
        {
            SpriteRenderer renderer = gunInstance.GetComponent<SpriteRenderer>();
            renderer.color = new Color32(255, 136, 0, 255);
        }
        else if (gun.Equals("yellow"))
        {
            SpriteRenderer renderer = gunInstance.GetComponent<SpriteRenderer>();
            renderer.color = new Color32(255, 255, 0, 255);
        }
        else if (gun.Equals("green"))
        {
            SpriteRenderer renderer = gunInstance.GetComponent<SpriteRenderer>();
            renderer.color = new Color32(0, 255, 0, 255);
        }
        else if (gun.Equals("blue"))
        {
            SpriteRenderer renderer = gunInstance.GetComponent<SpriteRenderer>();
            renderer.color = new Color32(0, 0, 255, 255);
        }
        else if (gun.Equals("purple"))
        {
            SpriteRenderer renderer = gunInstance.GetComponent<SpriteRenderer>();
            renderer.color = new Color32(136, 0, 255, 255);
        }
        else if (gun.Equals("pink"))
        {
            SpriteRenderer renderer = gunInstance.GetComponent<SpriteRenderer>();
            renderer.color = new Color32(255, 0, 255, 255);
        }

        for (int i = 0; i < ALIENSINGAME; i++)
        {
            currentAliens.Add(Instantiate(alienPrefab) as Alien);
            currentAliens[i].Init(new Vector2(Random.Range(-11, 11), Random.Range(0, 4)));
        }

        panelTextList.Add("The Aliens are getting\nready to destroy you.");
        panelTextList.Add("The Aliens are evolving\n so wait for a while.");

        panelTextList.Add("Wait for the Aliens\nto get warmed up.");
        panelTextList.Add("Hold up, the Aliens\nare preparing themselves\nbefore you lose.");
        waitingAudio.Play();
    }


    private void UpdateAliens()
    {
        for(int i = 0; i < currentAliens.Count; i++) //makes an update for every alien in the game
        {
            currentAliens[i].ActionFromNetwork(gunInstance.getPositionList(), gunInstance.transform.position); //moves based on NN
            currentAliens[i].updateFitness();

            if(currentAliens[i].fullSize()) //if the alien is full size, player score decreases
            {
                losingAudio.Play();
                gunInstance.increaseScore(-1);
            }

            int deadType = currentAliens[i].checkDead(gunInstance.getPositionList()); //int representing if the alien is dead
            if(deadType >= -1) //alien is dead and must be replaced 
            {
                
                //increase the score
                if (aliensCreatedSoFar > POPSIZE && deadType != -1 && gameTicks > 30) //makes sure the game has been started for a while, and the player shot the alien
                {
                    hitAudio.Play();
                    gunInstance.increaseScore((1275 - currentAliens[i].getTicks()) / 4);
                    gunInstance.alienKilled();
                }

                if (deadType >= 0) //bullet hit something and must stop moving
                {
                    gunInstance.BulletHit(deadType); //updates the bullet
                }

                currentAliens[i].noLongerUsed(); //alien is invisible now

                alienPopulation.Add(currentAliens[i]); //adds to the population
                currentAliens.RemoveAt(i); //removes from the current aliens

                alienPopulation.Sort((alien1, alien2) => alien1.Fitness.CompareTo(alien2.Fitness)); //sorts the population

                if(alienPopulation.Count >= POPSIZE) { //if the population size has been reached or exceeded, it removes the worst alien
                    //Debug.Log("removed worst one");
                    Destroy(alienPopulation[0]);
                    alienPopulation.RemoveAt(0);
                }

                if(aliensCreatedSoFar <= POPSIZE) //enough aliens haven't been created yet
                {
                    //Debug.Log("adding: " + aliensCreatedSoFar);
                    //creates new alien and adds it to the current alien population(on the screen)
                    currentAliens.Insert(i, Instantiate(alienPrefab) as Alien);
                    currentAliens[i].Init(new Vector2(Random.Range(-11, 11), Random.Range(0, 4)));
                    aliensCreatedSoFar++;
                    if(aliensCreatedSoFar > POPSIZE) //once enough aliens have been created, the game will reset and then start 
                    {
                        waitingAudio.mute = true;
                        restart();
                    }
                } else //chooses from population
                {
                    //Debug.Log("multiset");
                    int newAlienIndex = TournamentSelection(alienPopulation);
                    currentAliens.Insert(i, alienPopulation[newAlienIndex]); //adds to current aliens
                    alienPopulation.RemoveAt(newAlienIndex); // removes from population
                    currentAliens[i].ResetAlien(); //resets the alien

                    if(Random.RandomRange(0, 100) < 80) //mutates the alien
                    {
                        PlayerPrefs.SetInt("aliensevolved", PlayerPrefs.GetInt("aliensevolved") + 1); //updates the aliens evolved
                        currentAliens[i].Mutate(MUTATIONRATE);
                    }
                }

                //Debug.Log(aliensCreatedSoFar);

            }
        }
    }

    private int TournamentSelection(List<Alien> population) //n random individuals are selected, and the one with the highest fitness is returned
    {
        double bestFitness = 0;
        int chosenOne = 0;
        int N = TOURNAMENTNUM; //n number

        for(int i = 0; i < N; i++)
        {
            int lower = (int)((4.0 / 5.0) * population.Count);
            int upper = population.Count - 1;

            int ThisTry = Random.Range(lower, upper); //random indivdual

            if(population[ThisTry].Fitness > bestFitness) //checks fitness to see if higher
            {
                chosenOne = ThisTry;

                bestFitness = population[ThisTry].Fitness;
            }
        }

        //Debug.Log(population[chosenOne].Fitness);

        return chosenOne;
    }

    private void checkGameOver()
    {
        if(gameTicks >= TOTALGAMETICKS) //if there are no more aliens, game is over
        {
            PlayerPrefs.SetInt("points", PlayerPrefs.GetInt("points") + gunInstance.getScore()); //updates the players score
            PlayerPrefs.SetInt("totalpoints", PlayerPrefs.GetInt("totalpoints") + gunInstance.getScore()); //updates the players score
            PlayerPrefs.SetInt("alienskilled", PlayerPrefs.GetInt("alienskilled") + gunInstance.getAliensKilled()); //updates the players score

            restartAudio.Play();
            gameOver = true;
        }
    }

    private void UpdatePanel ()
    {
        if (aliensCreatedSoFar > POPSIZE) //not in auto mode
        {
            startPanel.SetActive(false);
        }
        else //auto mode, will update the bar
        {
            if(!alreadySetPanel) //panel text hasn't been set
            {
                string text = panelTextList[Random.Range(0, panelTextList.Count - 1)];
                text += "\n\nWhen the bar below is\ngone, the game will start.\n\nGood Luck and Have Fun!";
                panelText.text = text;
                alreadySetPanel = true;
            }

            //updates the bar
            RectTransform rectTransform = autoBar.transform as RectTransform;
            double width = 800 * ((POPSIZE - aliensCreatedSoFar) / (double)POPSIZE);
            rectTransform.sizeDelta = new Vector2((float)width, rectTransform.sizeDelta.y);
        }
    }
	
	// Update is called once per frame
	void Update () {
        if (!gameOver)
        {
            //makes a list of the x coordinates of the aliens
            List<double> xPos = new List<double>();
            for (int i = 0; i < currentAliens.Count; i++)
            {
                xPos.Add(currentAliens[i].transform.position.x);
            }
            gunInstance.moveGun(aliensCreatedSoFar <= POPSIZE, xPos); //moves the gun
            UpdateAliens(); //updates the aliens
            UpdatePanel(); //updates the panel/autobar

            if(aliensCreatedSoFar > POPSIZE)
            {
                leftText.text = "" + (((TOTALGAMETICKS - 5) / 5) - (gameTicks / 5));
                scoreText.text = "" + gunInstance.getScore();
                gameTicks++;
            }
            checkGameOver();
        } else
        {
            showStats();

            if (Input.GetMouseButtonDown(0))
            {
                firstPressPos = Input.mousePosition;
            }
            if (Input.GetMouseButtonUp(0))
            {
                currentSwipe = (Vector2)Input.mousePosition - firstPressPos;
                if(currentSwipe.y >= 0)
                {
                    restart();
                } else
                {
                    SceneManager.LoadScene(0);
                }
            }

        }
    }

    private void showStats() //shows the stats
    {
        startPanel.gameObject.SetActive(true);
        panelText.text = "Score: " + gunInstance.getScore() + "\nAliens Killed: " + gunInstance.getAliensKilled() + "\n\n SWIPE UP TO PLAY AGAIN\nSWIPE DOWN TO EXIT"; 
    }

    private void restart() //reset the game
    {

        startAudio.Play();

        //restart gamemanager class

        gameTicks = 0; ; 

        gameOver = false;

        //create a new population of aliens

        int c = currentAliens.Count;

        for (int i = 0; i < c; i++) //adds each alien to population
        {
            currentAliens[0].noLongerUsed();
            alienPopulation.Add(currentAliens[0]);
            currentAliens.RemoveAt(0);
            Debug.Log(currentAliens.Count);
        }

        currentAliens = new List<Alien>();

        alienPopulation.Sort((alien1, alien2) => alien1.Fitness.CompareTo(alien2.Fitness)); //sorts the population 

        for (int i = 0; i < ALIENSINGAME; i++)
        {
            int newAlienIndex = TournamentSelection(alienPopulation);
            currentAliens.Insert(i, alienPopulation[newAlienIndex]); //adds to current aliens
            alienPopulation.RemoveAt(newAlienIndex); // removes from population
            currentAliens[i].ResetAlien(); //resets the alien

            if (Random.RandomRange(0, 100) < 80) //mutates the alien
            {
                currentAliens[i].Mutate(MUTATIONRATE);
            }
        }

        //restart gun class
        gunInstance.restart();

        //fix the ui
        startPanel.gameObject.SetActive(false); //hides panel
    }
}
