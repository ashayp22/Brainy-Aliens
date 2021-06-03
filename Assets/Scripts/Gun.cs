using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour {

    //bullets
    public Bullet bulletPrefab;

    private List<Bullet> bulletList = new List<Bullet>();

    private int maxBullets;
    private int bulletsFired = 0;

    private bool autoRun = true; //set to true if autorun is on

    //joystick objects
    protected Joystick joystick;
    protected Joybutton joybutton;

    //stats
    private int score = 0;
    private int aliensKilled = 0;

    public void Start()
    {
        //only one type, so finds it
        joystick = FindObjectOfType<Joystick>();
        joybutton = FindObjectOfType<Joybutton>();
    }

    public void Init(int maxBullets, Vector2 pos)
    {
        transform.position = pos;
        //initialize stuff
        for (int i = 0; i < maxBullets; i++)
        {
            bulletList.Add(Instantiate(bulletPrefab) as Bullet);
            bulletList[i].Init(transform.position, 4.5f, -4, "red");
        }
        this.maxBullets = maxBullets;
    }

    public void moveGun(bool auto, List<double> alienXPos) //auto is the boolean deciding if there is autogun, xPosList is the x positions of the aliens
    {
        if (!auto || !autoRun) //either there isn't autogun or the person turned off autogun
        {
            //keyboard controls
            if (Input.GetButton("left")) //for moving to the left
            {
                transform.position += new Vector3(-0.1f, 0, 0);
            }
            else if (Input.GetButton("right")) //for moving to the right
            {
                transform.position += new Vector3(0.1f, 0, 0);
            }
            else if (Input.GetKeyDown("space")) //for firing
            {
                fireBullet(); //fires one bullet
            }


            //joystick/joybutton controls

            transform.position += new Vector3(0.1f * joystick.Horizontal, 0, 0);
            
            //moves the gun to other side of the screen if it goes past x bounds
            if(transform.position.x > 12)
            {
                transform.position = new Vector3(-12, transform.position.y, 0);
            } else if (transform.position.x < -12)
            {
                transform.position = new Vector3(12, transform.position.y, 0);
            }

            if (joybutton.Pressed)
            {
                joybutton.Pressed = false;
                fireBullet();
            }
            
        }
        else
        {
            int randIndex = Random.RandomRange(0, alienXPos.Count-1);

            transform.position = new Vector3((float)alienXPos[randIndex], -4, 0);

            fireBullet();
        }

        //updates the positions of the bullets, along with calculating how many bullets have been fired
        updateBulletPositions();
        updateBulletsFired();

    }

    private void updateBulletPositions() //updates the positions of bullets not fired
    {
        for (int i = 0; i < bulletList.Count; i++)
        {
            if (!(bulletList[i].Fired))
            {
                bulletList[i].updateX(transform.position.x);
            }
        }
    }
	
    public void fireBullet() //fires the first bullet available
    {
        if(bulletsFired != maxBullets)
        {
            for(int i = 0; i < bulletList.Count; i++)
            {
                if(!(bulletList[i].Fired))
                {
                    bulletList[i].fireBullet();
                    return;
                }
            }
        }
    }

    private void updateBulletsFired() //updates the number of bullets fired
    {
            bulletsFired = 0;
            for (int i = 0; i < bulletList.Count; i++)
            {
                if (bulletList[i].Fired)
                {
                    bulletsFired += 1;
                }
            }
    }

    public List<Vector2> getPositionList() //returns a list of vectors containing the bullet's positions
    {
        List<Vector2> list = new List<Vector2>();
        for(int i = 0; i < bulletList.Count; i++)
        {
            list.Add(bulletList[i].getPosition());
        }

        return list;
    }
    
    public void BulletHit(int bullet) //bullet hit something
    {
        bulletList[bullet].HitSomething(transform.position.x);
    }

    //for stats

    public void increaseScore(double newScore)
    {
        score += (int)newScore;
    }

    public int getScore()
    {
        return score;
    }

    public void alienKilled()
    {
        aliensKilled += 1;
    }

    public int getAliensKilled()
    {
        return aliensKilled;
    }

    //restarts the gun

    public void restart()
    {

        //resets variables
        bulletsFired = 0;
        score = 0;
        aliensKilled = 0;

        //moves bullets and gun to center
        transform.position = new Vector2(0, -4);

        for(int i = 0; i < bulletList.Count; i++)
        {
            bulletList[i].HitSomething(transform.position.x);
        }
    }  

    public void Update()
    {
        if (Input.GetKeyDown("t")) //switch controls
        {
            autoRun = !autoRun;
        }
        
    }

}

