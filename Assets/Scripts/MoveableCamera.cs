using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveableCamera : MonoBehaviour
{
  public float speed = 10;
  public float z_speed  = -10;

  private Camera cam;

  void Start()
  {
    cam = this.GetComponent<Camera>();
  }

  void Update()
  {
    float h = Input.GetAxis("Horizontal");
    float v = Input.GetAxis("Vertical");
    float z = Input.GetAxis("Mouse ScrollWheel");

    Vector3 tempVect = new Vector3(h, v, 0);
    tempVect = tempVect.normalized * speed * Time.deltaTime;

    this.transform.position += tempVect;
    cam.orthographicSize += z * z_speed;
  }
}
