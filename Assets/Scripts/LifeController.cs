using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class LifeController : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
{

  public GameObject corpsePrefab;

  private Rigidbody2D rb;
  private SpriteRenderer spriteRenderer;
  public float speed = 1;
  public float rotationSpeed = 100;

  public float visionRange = 20;

  public float hp;
  public float huePenalty = 0.001f;
  public float forcePenalty = 0.1f;
  public float rotationPenalty = 0.001f;
  public float wallPenalty = 5;
  public float cellPenalty = 10;
  public float scoreStep = 1;
  public float foodGain = 15;
  public float corpseGain = 50;
  private Master master;
  public int VisionRays = 20;
  private int visionStep;

  private float[] vision;

  public const float FoodHue = 119.0f / 360.0f;
  public const float WallHue = 197.0f / 360.0f;
  public const float CorpseHue = 10.0f / 360.0f;

  public Brain brain;
  public float score;

  // Start is called before the first frame update

  void Awake()
  {
    brain = new Brain(this);
  }

  void Start()
  {
    rb = this.GetComponent<Rigidbody2D>();
    spriteRenderer = this.GetComponent<SpriteRenderer>();

    master = GameObject.FindGameObjectsWithTag("GameController")[0].GetComponent<Master>();

    vision = new float[VisionRays];
    visionStep = 60 / VisionRays;

    hp = master.MaxHP;

    // brain = new Brain(this);

    // print(transform.rotation);
  }

  // Update is called once per frame
  void Update()
  {

    if(master.paused) return;
    // living penalty

    hp -= Mathf.Abs(huePenalty * Hue);
    hp -= Mathf.Abs(forcePenalty * Force);
    hp -= Mathf.Abs(rotationPenalty * Rotation);


    if (hp <= 0)
    {
      master.removeCell(gameObject);
      Instantiate(corpsePrefab, transform.position, Quaternion.identity);
      // Destroy(gameObject);
    } else {
      score += scoreStep * Time.deltaTime;
    }
  }

  void FixedUpdate()
  {
    if(master.paused) return;
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
        else if(hit.collider.tag == "corpse")
        {
            vision[i] = CorpseHue;
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
    if(master.paused) return;
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
    else if (collision.gameObject.tag == "corpse")
    {
      hp += corpseGain;
      Destroy(collision.gameObject);
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

  public void OnPointerClick(PointerEventData pointerEventData){
      master.selectCell(this);
  }

  public void OnPointerDown(PointerEventData eventData){
  }

  public void OnPointerUp(PointerEventData eventData){
  }
}
