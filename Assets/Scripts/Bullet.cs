using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {

    private bool isFired; //if it is fired
    private float upperBound; //its upperbound
    private float startingPosition; //its starting position

    public bool Fired { get { return isFired; } set { isFired = value; } } //acessor/mutator

    private string color = "";

    public double hit = 0; //will represent ticks before it gets reset, used for decreasing gun health


    public void Init(Vector2 position, float upperBound, float startingPosition, string color) //initializes
    {
        //initialize stuff
        gameObject.SetActive(false);
        transform.position = position;
        this.upperBound = upperBound;
        this.startingPosition = startingPosition;
        this.color = color;
        isFired = false;
    }

    public void updateX(float x) //updates the x pos
    {
        transform.position = new Vector3(x, transform.position.y, transform.position.z);
    }

    public void updateY(float y) //updates the y pos
    {
        transform.position = new Vector3(transform.position.x, y, transform.position.z);
    }

    public Vector2 getPosition() //gets position
    {
        return transform.position; 
    }

    public void fireBullet() //fire the bullet
    {
        gameObject.SetActive(true);
        isFired = true;
    }

    private void moveBullet() //move the bullet
    {
        transform.position += new Vector3(0, 0.2f, 0);
    }

    private void checkBounds() //checks its bound
    {
        if(transform.position.y >= upperBound)
        {
            isFired = false;
            transform.position = new Vector3(transform.position.x, startingPosition, 0);
            this.gameObject.SetActive(false);
        }
    }

    public void HitSomething(float x)
    {
        
        isFired = false;
        transform.position = new Vector2(x, startingPosition);
        this.gameObject.SetActive(false);
    }

    private void showColor()
    {
        if(color.Equals("red"))
        {
            SpriteRenderer sprite = GetComponent<SpriteRenderer>();
            sprite.color = new Color32(255, 0, 0, 255);
        }
    }

	void Update () { //either fires or not
        showColor();
		if(isFired)
        {
            if(color == "red") //if it is the gun's bullet
            {
                moveBullet();
                checkBounds();
            }
            
        } else
        {
            hit = 0;
            this.gameObject.SetActive(false);
        }
	}
}
