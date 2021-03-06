﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyUtil;

public class SnakeHeadCollider : MonoBehaviour
{
    private GameSceneController gameSceneController;
    PlayerController head;
    private void Start()
    {
        gameSceneController = FindObjectOfType<GameSceneController>();
        head = transform.parent.gameObject.GetComponent<PlayerController>();
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        Tagger tagger = collider.gameObject.GetComponent<Tagger>();
        if (tagger)
        {
            if (gameSceneController.gameOver == false)
            {
                if (tagger.containsCustomTag("obstacle"))
                {
                    head.onChildObstacleHit();
                }
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Tagger tagger = collision.gameObject.GetComponent<Tagger>();
        if (tagger)
        {
            if (gameSceneController.gameOver == false)
            {
                if (tagger.containsCustomTag("obstacle"))
                {
                    head.onChildObstacleHit();
                }
                else if (tagger.containsCustomTag("teleporter"))
                {
                    head.teleport();
                }
                else if (tagger.containsCustomTag("accelerator"))
                {
                    head.accelerate(2);
                }
                else if (tagger.containsCustomTag("decelerator"))
                {
                    head.accelerate(0.5f);
                }
            }
        }
    }
}
