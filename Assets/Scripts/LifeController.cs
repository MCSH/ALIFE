﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifeController : MonoBehaviour
{

  private Rigidbody2D rb;
  private SpriteRenderer spriteRenderer;
  public float speed = 1;
  public float rotationSpeed = 100;

  public float visionRange = 20;

  public float hp = 100;
  public float huePenalty = 0.001f;
  public float forcePenalty = 0.1f;
  public float rotationPenalty = 0.001f;
  public float wallPenalty = 5;
  public float cellPenalty = 10;
  public float foodGain = 15;
  private Master master;
  public int VisionRays = 20;
  private int visionStep;

  private float[] vision;

  public const float FoodHue = 119.0f / 360.0f;
  public const float WallHue = 197.0f / 360.0f;

  private Brain brain;

  // Start is called before the first frame update
  void Start()
  {
    rb = this.GetComponent<Rigidbody2D>();
    spriteRenderer = this.GetComponent<SpriteRenderer>();

    master = GameObject.FindGameObjectsWithTag("GameController")[0].GetComponent<Master>();
    master.addCell(this);

    vision = new float[VisionRays];
    visionStep = 60 / VisionRays;

    brain = new Brain(this);

    // print(transform.rotation);
  }

  // Update is called once per frame
  void Update()
  {

    // living penalty

    hp -= Mathf.Abs(huePenalty * Hue);
    hp -= Mathf.Abs(forcePenalty * Force);
    hp -= Mathf.Abs(rotationPenalty * Rotation);


    if (hp <= 0)
    {
      master.removeCell(this);
      Destroy(gameObject);
    }
  }

  void FixedUpdate()
  {
    //   RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.Up);
    Quaternion.AngleAxis(30, Vector3.up);
    float vs = transform.eulerAngles.z - visionStep * VisionRays / 2;


    for (int i = 0; i < VisionRays; i++)
    {

      RaycastHit2D hit = Physics2D.Raycast(transform.position,
            Quaternion.AngleAxis(vs, transform.up).eulerAngles);

      if (hit.collider != null && hit.distance < visionRange)
      {
        if (hit.collider.tag == "cell")
        {
          vision[i] = hit.collider.gameObject.GetComponent<LifeController>().Hue;
        }
        else if (hit.collider.tag == "wall")
        {
          vision[i] = WallHue;
        }
        else if (hit.collider.tag == "food")
        {
          vision[i] = FoodHue;
        }

        vs += visionStep;
      }
      else
      {
        vision[i] = -1; // I saw nothing!
        //   print("no hit");
      }

    }

    brain.think(vision);

    float hue = Hue;

    float rotation = Rotation * rotationSpeed * Time.deltaTime;
    float force = Force * speed * Time.deltaTime;
    Vector3 movement = transform.up * force;

    transform.Rotate(0, 0, rotation);
    // rb.AddForce(movement);
    rb.MovePosition(transform.position + movement);

    spriteRenderer.color = Color.HSVToRGB(hue, 1, 1);
  }

  void OnCollisionEnter2D(Collision2D collision)
  {
    // print("collision");
    if (collision.gameObject.tag == "cell")
    {
      float penalty = Mathf.Abs(cellPenalty * collision.gameObject.GetComponent<LifeController>().Force);

      //   print("Penalty " + penalty);

      hp -= Mathf.Abs(cellPenalty * collision.gameObject.GetComponent<LifeController>().Force);
      // TODO
    }
    else if (collision.gameObject.tag == "wall")
    {
      hp -= Mathf.Abs(wallPenalty * Force);
      //   hp -= Force;
      // TODO
    }
    else if (collision.gameObject.tag == "food")
    {
      hp += foodGain;
      master.removeFood(collision.gameObject);
    }
  }

  public float Hue = 0;
  //   {
  //     get
  //     {
  //       return Random.value;
  //     }
  //   }

  public float Force = 0;
  //   {
  //     get
  //     {
  //       return Random.value * 2 - 1;
  //     }
  //   }

  public float Rotation = 0;
  //   {
  //     get
  //     {
  //       //   return 1
  //       return Random.value * 2 - 1;
  //     }
  //   }
}