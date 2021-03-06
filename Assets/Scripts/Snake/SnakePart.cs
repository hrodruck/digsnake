﻿using MyUtil;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

[RequireComponent(typeof(Tagger))]
public class SnakePart : MonoBehaviour
{

    #region variables

    [Range(1, 10)]
    [SerializeField]
    private int editorSpeed = 6;
    protected float speed;

    [Range(0, 1)]
    [SerializeField]
    private float distanceFactor; //the fraction of the snake part's total size that they use as distance from the snake in front

    protected Vector3 currentDirection;

    protected GameObject childPart;
    public GameObject partToSpawn;
    protected SnakeBodyController childScript;
    protected SnakePart front;

    private Queue<Vector3> positionsAfterTurn;

    protected SpriteRenderer myRenderer;
    protected Rigidbody2D myRigidbody;
    protected Light2D myLight;

    protected int snakeLength;
    protected GameSceneController gameSceneController;
    #endregion

    #region movement

    private void recursiveMovement()
    {
        if (front == null)
        {
            IndependentMove();
        }
        else
        {

            if (front.positionsAfterTurn.Count > 0)
            {
                Vector3 frontPeekPosition = front.positionsAfterTurn.Peek();
                float frontDistance = Mathf.Abs(frontPeekPosition.y - front.transform.position.y) + Mathf.Abs(frontPeekPosition.x - front.transform.position.x);
                Vector3 newPos=transform.position;
                while(front.positionsAfterTurn.Count>0 && frontDistance > (1.0f + distanceFactor) * myRenderer.bounds.size.x)
                {
                    newPos = front.positionsAfterTurn.Dequeue();
                    frontPeekPosition = front.positionsAfterTurn.Peek();
                    frontDistance = Mathf.Abs(frontPeekPosition.y - front.transform.position.y) + Mathf.Abs(frontPeekPosition.x - front.transform.position.x);
                }
                myRigidbody.MovePosition(newPos);
            }
            else
            {
                //stay still
            }
        }

        if (childScript)
        {
            positionsAfterTurn.Enqueue(transform.position);
            childScript.recursiveMovement();
        }
    }

    protected virtual void IndependentMove()
    {

    }

    #endregion

    #region basics
    protected virtual void Start()
    {
        gameSceneController = FindObjectOfType<GameSceneController>();

        if (partToSpawn == null)
        {
            Debug.LogError("Assign a part to spawn to the snake head");
        }

        positionsAfterTurn = new Queue<Vector3>();

        myRigidbody = GetComponent<Rigidbody2D>();
        myRenderer = GetComponentInChildren<SpriteRenderer>();
        myLight = GetComponentInChildren<Light2D>();


        if (speed < Mathf.Epsilon && editorSpeed > Mathf.Epsilon)
        {
            speed = editorSpeed / 2.0f;
        }


        if (front != null)
        {
            if (((snakeLength-2)/2) % 3 == 1)
            {
                foreach (Light2D light in GetComponentsInChildren<Light2D>())
                {
                    light.color = Color.HSVToRGB(316f/360, 0.8f, 1);
                }
            }
            else if (((snakeLength - 2) / 2) % 3 == 2)
            {
                foreach (Light2D light in GetComponentsInChildren<Light2D>())
                {
                    light.color = Color.HSVToRGB(76f/360, 0.8f, 1);
                }
            }
            else
            {
                foreach (Light2D light in GetComponentsInChildren<Light2D>())
                {
                    light.color = Color.HSVToRGB(196f / 360, 0.8f, 1);
                }
            }
        }
    
        
    }

    protected virtual void FixedUpdate()
    {
        if (front == null)
        {
            recursiveMovement();
        }
    }

    private void Update()
    {
        //print(((1.0f + distanceFactor) * myRenderer.bounds.size.x) / (Time.fixedDeltaTime * speed));
        //print(((1.0f + distanceFactor) * myRenderer.bounds.size.x));
    }

    #endregion

    #region other recursive methods
    protected void growParts()
    {
        snakeLength++;
        if (childScript == null )
        {
            if (front == null)
            {
                childPart = Instantiate(partToSpawn, transform.position + currentDirection * (-1.0f - distanceFactor) * myRenderer.bounds.size.x, Quaternion.identity);
            }
            else
            {
                childPart = Instantiate(partToSpawn, transform.position + currentDirection * (- distanceFactor) * myRenderer.bounds.size.x, Quaternion.identity);
            }
            childScript = childPart.GetComponentInChildren<SnakeBodyController>();
            childScript.front = this;
            childScript.speed = speed;
            childScript.partToSpawn = partToSpawn;
            childScript.currentDirection = currentDirection;
            childScript.distanceFactor = distanceFactor;
            childScript.snakeLength = snakeLength;
        }
        else if (!gameSceneController.gameOver && childScript!=null) //just in case the child object gets destroyed prior to this call
        {
            childScript.growParts();
        }
    }

    protected void changeSnakeSpeed(float newSpeed)
    {
        speed = newSpeed;
        if (childScript)
        {
            childScript.changeSnakeSpeed(newSpeed);
        }
    }

    public void setLength(int newLength)
    {
        if (newLength>snakeLength)
        {
            StartCoroutine(growAtOnce(newLength-snakeLength));
            
        }
        else if (newLength<snakeLength)
        {
            destroyParts(snakeLength - newLength, 0);
        }
    }

    private IEnumerator growAtOnce(int numberToGrow)
    {
        for (int i = numberToGrow; i > 0; i--)
        {
            growParts();
            yield return new WaitForSeconds(0.2f); //to give the part time to grow a trail
        }
    }

    private void destroyParts(int numberToDestroy, int myIndex)
    {
        if (childScript)
        {
            childScript.destroyParts(numberToDestroy, myIndex + 1);
        }
        if (snakeLength - myIndex <= numberToDestroy)
        {
            Destroy(gameObject);
        }
        else
        {
            snakeLength = snakeLength - numberToDestroy;
        }
    }

    #endregion

    #region other
    public int getLength()
    {
        return snakeLength;
    }

    #endregion
}
