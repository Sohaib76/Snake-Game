using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;  //For Reloading
using UnityEngine.UI;

public class GameController : MonoBehaviour {

    public AudioSource Eat;
    public AudioSource Collide;

    //snake size / part of movement
   public int maxSize;
   public int currentSize;

    public GameObject SnakePrefab;
    public Snake head;
    public Snake tail;
    public int NESW;
    public Vector2 nextPosition;


    //For Food Spawn
    public int xBound;
    public int yBound;
    public GameObject FoodPrefab;
    public GameObject currentFood;


    //For Eating Food
    public int score;
    public Text scoreText;

    //Random increase Speed
    public float deltaTimer;

	// Use this for initialization
	void Start () {
        InvokeRepeating("TimerInvoke", 0, deltaTimer); //0.5f
        FoodFunction();
        
        
	}

    // Update is called once per frame
    void Update()
    {
        ComputerChangeDirection();


    }




    void TimerInvoke()  //executing every half a second
    {
        Movement();
        StartCoroutine(checkVisible());
        if(currentSize >= maxSize)
        {
            TailFunction();
        }
        else
        {
            currentSize++;
        }



    }

    void Movement()
    {
        GameObject temp;
        nextPosition = head.transform.position;
        switch (NESW)
        {
            case 0:
                nextPosition = new Vector2(nextPosition.x, nextPosition.y + 1);  //going up
                break;
            case 1:
                nextPosition = new Vector2(nextPosition.x+1, nextPosition.y);   //going right
                break;
            case 2:
                nextPosition = new Vector2(nextPosition.x, nextPosition.y-1);     //going down
                break;
            case 3:
                nextPosition = new Vector2(nextPosition.x-1, nextPosition.y);     //going left
                break;
        }
        temp = (GameObject)Instantiate(SnakePrefab, nextPosition, transform.rotation);
        head.Setnext(temp.GetComponent<Snake>());

        head = temp.GetComponent<Snake>();
        return;

    }



    void ComputerChangeDirection()
    {
        if(NESW != 2 && Input.GetKeyDown(KeyCode.W))              //if we not moving down and press W we go up.
        { 
            NESW = 0;
        }
        if (NESW != 0 && Input.GetKeyDown(KeyCode.S))              //if we not moving up and press S we go down.
        {
            NESW = 2;
        }
        if (NESW != 3 && Input.GetKeyDown(KeyCode.D))              //if we not moving left and press D we go right.
        {
            NESW = 1;
        }
        if (NESW != 1 && Input.GetKeyDown(KeyCode.A))              //if we not moving right and press A we go left.
        {
            NESW = 3;
        }
    }

    public void MobileChangeDirection(int direction)
    {
        if (NESW != 2 && direction == 0)              
        {
            NESW = direction;
        }
        if (NESW != 0 && direction == 2)              
        {
            NESW = direction;
        }
        if (NESW != 3 && direction == 1)              
        {
            NESW = direction;
        }
        if (NESW != 1 && direction == 3)              
        {
            NESW = direction;
        }
    }



    void TailFunction()
    {
        Snake tempSnake = tail;
        tail = tail.Getnext();
        tempSnake.RemoveTail();
    }

    void FoodFunction()
    {
        int xPos = Random.Range(-xBound, xBound);
        int yPos = Random.Range(-yBound, yBound);

        currentFood = (GameObject)Instantiate(FoodPrefab, new Vector2(xPos, yPos), transform.rotation);
        StartCoroutine(CheckRender(currentFood));

    }
   
    IEnumerator CheckRender(GameObject IN)
    {
        yield return new WaitForEndOfFrame();
        if(IN.GetComponent<Renderer>().isVisible == false)
        {
            if(IN.tag == "Food")
            {
                Destroy(IN);
                FoodFunction();
            }
        }
    }


    private void OnEnable()
    {
        Snake.Hit += Hit;
        
    }
    private void OnDisable()
    {
        Snake.Hit -= Hit;
    }

    void Hit(string WhatWasSend)
    {
        if (deltaTimer >= 0.09)
        {
            deltaTimer -= 0.05f;
            CancelInvoke("TimerInvoke");
            InvokeRepeating("TimerInvoke", 0, deltaTimer);
        }
        if (WhatWasSend == "Food")
        {
            FoodFunction();
            maxSize++;
            score++;
            Eat.Play();
            scoreText.text = score.ToString();
            int temp = PlayerPrefs.GetInt("High Score");
            if(score > temp)
            {
                PlayerPrefs.SetInt("High Score", score);    //check string
            }
        }

        if(WhatWasSend == "Snake")
        {
            Collide.Play();
            CancelInvoke("TimerInvoke");
           // Exit();
            StartCoroutine(ExitRoutine());
            
        }
    }

    public void Exit()
    {
        SceneManager.LoadScene(0);
   }

    IEnumerator ExitRoutine()
    {
        yield return new WaitForSeconds(2);
        SceneManager.LoadScene(0);
    }

    void wrap()
    {
        if(NESW == 0) //going to top
        {
            head.transform.position = new Vector2(head.transform.position.x, -(head.transform.position.y - 1));
        }
        else if (NESW == 1)  //going right
        {
            head.transform.position = new Vector2(-(head.transform.position.x - 1), head.transform.position.y );
        }
        else if (NESW == 2)  //going down
        {
            head.transform.position = new Vector2(head.transform.position.x, -(head.transform.position.y + 1));
        }
        else if (NESW == 3)
        {
            head.transform.position = new Vector2(-(head.transform.position.x + 1), head.transform.position.y);
        }

    }

    IEnumerator checkVisible()
    {
        yield return new WaitForEndOfFrame();
        if (!head.GetComponent<Renderer>().isVisible)
        {
            wrap();
        }
    }





}
