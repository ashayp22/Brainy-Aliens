using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Alien : MonoBehaviour {

    public Bullet bulletPrefab; //bullet prefab

    private NeuralNet brain; //creates a neural network 

    private double age; //fitness
    private double ticks = 0; //how many ticks it lasts

    public double Fitness { get { return age; } }

   // private Rigidbody2D rb;

    private Vector2 startingPos;

    private byte rColor = 255;
    private byte gColor = 0;
    private byte bColor = 0;


    public void Start()
    {
        //rb = this.GetComponent<Rigidbody2D>();
        brain = new NeuralNet();
        brain.Init(8, 3, GameManager.HIDDENLAYERS, GameManager.NEURONSPERLAYER);
    }

    //constructor

    public void Init(Vector2 startingPosition)
    {
        startingPos = startingPosition;
        transform.position = startingPos;
    }

    //private

    public int checkDead(List<Vector2> bulletPositions) //returns whether the alien is dead or not
    {
        if(outofBounds())
        {
            age /= 2; //penalty for flying out of bounds
            return -1;
        } else if(checkCollision(bulletPositions) != -1)
        {
            return checkCollision(bulletPositions);
        }
        return -2;
    }

    private bool outofBounds() //checks if it is out of bounds
    {
        
        if(transform.position.y > 4.5 || transform.position.y < -3)
        {
            return true;
        }
        return false;
    }
    

    private int checkCollision(List<Vector2> bulletPositions) //checks if it collided with a bullet
    {
        for(int i = 0; i < bulletPositions.Count; i++)
        {
            var bulletX = bulletPositions[i].x;
            var bulletY = bulletPositions[i].y;

            var thisX = transform.position.x;
            var thisY = transform.position.y;

            if(bulletX > thisX - 0.5 && bulletX < thisX + 0.5 && bulletY > thisY - 0.5 && bulletY < thisY + 0.5)
            {
                return i;
            }
        }
        return -1;
    }


    //public

    public void ActionFromNetwork(List<Vector2> bulletPositions, Vector2 gunPosition) //moves based off of inputs
    {
        //for movement
        List<double> inputs = new List<double>();

        //adds in vectors to bullets from alien
        for(int i = 0; i < bulletPositions.Count; i++)
        {
            inputs.Add(bulletPositions[i].x - transform.position.x);
            inputs.Add(bulletPositions[i].y - transform.position.y);
        }

        //adds in vectors to gun from alien
        inputs.Add(gunPosition.x - transform.position.x);
        inputs.Add(gunPosition.y - transform.position.y);


        List<double> outputs = brain.FeedForward(inputs);

        double BiggestSoFar = 0;
        int action = 3; //0 is left, 1 is right, 2 is up, 3 is drift

        for(int i = 0; i < outputs.Count; i++)
        {
            if((outputs[i] > BiggestSoFar) && (outputs[i] > 0.9))
            {
                action = i;
                BiggestSoFar = outputs[i];
            }
        }

        //applies action in terms of moving it

        if(action == 0) //left
        {
            transform.position += new Vector3(-0.05f, 0, 0);
            //rb.AddForce(new Vector2(-2, 0));
        } else if(action == 1) //right
        {
            transform.position += new Vector3(0.05f, 0, 0);

            //rb.AddForce(new Vector2(2, 0));

        }
        else if(action == 2) //up
        {
            transform.position += new Vector3(0f, 0.05f, 0);

            //rb.AddForce(new Vector2(0, 2)); 
        } else
        {
            transform.position += new Vector3(0f, -0.05f, 0);
        }

        //will move to opposite side if the alien has passed the x bounds
        if(transform.position.x < -12)
        {
            transform.position = new Vector3(12, transform.position.y, 0);
        } else if(transform.position.x > 12)
        {
            transform.position = new Vector3(-12, transform.position.y, 0);
        }


        ticks += 1;
        //order 
        // increase g - r to o
        // decrease r - o to y
        // increase b - y to g
        // decrease g - g to b
        // increase r - b to p
        //total: 1275 increments
            if (ticks <= 255)
            {
                gColor++;
            }
            else if (ticks > 255 && ticks <= 510)
            {
                rColor--;
            }
            else if (ticks > 510 && ticks <= 765)
            {
                bColor++;
            }
            else if (ticks > 765 && ticks <= 1020)
            {
                gColor--;
            }
            else if (ticks > 1020 && ticks <= 1275)
            {
                rColor++;
            }
        
        setColor(rColor, gColor, bColor);
        
    }

    public void updateFitness()
    {
        age += 1;
    }

    private void setColor(byte r, byte g, byte b) //sets the color
    {
        SpriteRenderer sprite = GetComponent<SpriteRenderer>();
        sprite.color = new Color32(r, g, b, 255);
    }


    public void ResetAlien() //resets fitness and position
    {
        age = 0;
        ticks = 0;
        transform.position = startingPos;
        gameObject.SetActive(true);
        rColor = 255;
        gColor = 0;
        bColor = 0;
        setColor(rColor, gColor, bColor);
    }

    public void noLongerUsed() //alien is no longer used
    {
        gameObject.SetActive(false);
    }

    public void Mutate(double mutationRate) //mutates the movement NN
    {
        List<double> weights = brain.getAllWeights();
        
        for (int i = 0; i < weights.Count; i++)
        {
            if (Random.RandomRange(0, 100) > 50) //adds small value
              {
                  weights[i] += Random.RandomRange(1, 10) * 0.1;
              }
              else //subtracts small value
              {
                weights[i] -= Random.RandomRange(1, 10) * 0.1;
              }
        }

        brain.brainSurgery(weights); //adds back into the NN
    }

    public bool fullSize() //returns true if it has gone through all colors of the rainbow
    {
        return ticks >= 1275;
    }

    public double getTicks()
    {
        return ticks;
    }

    void Update()
    {
        
        if(Input.GetKeyDown("up"))
        {
        } else if(Input.GetKeyDown("left"))
        {
        } else if(Input.GetKeyDown("right"))
        {
        }
    }
}
