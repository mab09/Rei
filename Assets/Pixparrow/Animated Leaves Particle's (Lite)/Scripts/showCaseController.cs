using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class showCaseController : MonoBehaviour
{

    public bool treesShow;
    public Vector3 deployPosition = new Vector3(0, 3, 0);
    public GameObject[] objects;
   
    public GameObject[] edgeParticles;
    public GameObject[] headers;
    public float camSpeed = 1;

    public GameObject camBase;
    int objectId = 0;
  
    public void Start()
    {
     
        foreach(GameObject obj in objects)
        {
            obj.SetActive(false);
        }
        objects[objectId].SetActive(true);

        if (!treesShow)
        {
            foreach (GameObject head in headers)
            {
                head.SetActive(false);
            }
            headers[objectId].SetActive(true);

            foreach (GameObject part in edgeParticles)
            {
                part.SetActive(false);
            }
            edgeParticles[objectId].SetActive(true);
        }
    }
    public void FixedUpdate()
    {
        camBase.transform.Rotate(0, camSpeed / 10, 0);
    }
    public void Update()
    {
       
        if (Input.GetKeyDown("right"))
        {
            Next();
        }
        if (Input.GetKeyDown("left"))
        {
            Back();
        }

    }

   

    public void Next()
    {

        objectId += 1;
        if (objectId >= objects.Length)
        {
            objectId = 0;
        }
        foreach (GameObject obj in objects)
        {
            obj.SetActive(false);
        }
        objects[objectId].SetActive(true);

        if (!treesShow)
        {
            foreach (GameObject head in headers)
            {
                head.SetActive(false);
            }
            headers[objectId].SetActive(true);

            foreach (GameObject part in edgeParticles)
            {
                part.SetActive(false);
            }
            edgeParticles[objectId].SetActive(true);
        }

    }

    public void Back()
    {
        objectId -= 1;
        if (objectId < 0)
        {
            objectId = objects.Length-1;
        }

        foreach (GameObject obj in objects)
        {
            obj.SetActive(false);
        }
        objects[objectId].SetActive(true);

        if (!treesShow)
        {
            foreach (GameObject head in headers)
            {
                head.SetActive(false);
            }
            headers[objectId].SetActive(true);

            foreach (GameObject part in edgeParticles)
            {
                part.SetActive(false);
            }
            edgeParticles[objectId].SetActive(true);

        }
    }
}
