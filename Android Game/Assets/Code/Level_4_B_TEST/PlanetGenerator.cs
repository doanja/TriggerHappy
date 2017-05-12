using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlanetGenerator : MonoBehaviour {

    public GameObject[] Planets;

    Queue<GameObject> availablePlanets = new Queue<GameObject>();

	// Use this for initialization
	void Start () {
        
        for (int i = 0; i < Planets.Length; i++)
        {
            availablePlanets.Enqueue(Planets[i]);
        }

        // call MovePlanetDown function every 20 seconds
        InvokeRepeating("MovePlanetDown", 0, 20f);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    /*
     * Function to dequeue a planet, and set its isMoving flag to true
     * so that the planet starts scrolling down the screen
     */
     void MovePlanetDown()
    {
        EnqueuePlanets();

        if (availablePlanets.Count == 0)
            return;

        GameObject aPlanet = availablePlanets.Dequeue();

        aPlanet.GetComponent<BackgroundPlanet>().isMoving = true;
    }

    /*
     * Function to enqueue planets that are below the screen and are not moving
     * 
     */
     void EnqueuePlanets()
    {
        foreach(GameObject aPlanet in Planets)
        {
            // if the planet is below the screen & not moving
            if((aPlanet.transform.position.y < 0) && (!aPlanet.GetComponent<BackgroundPlanet>().isMoving))
            {
                // reset the planet position
                aPlanet.GetComponent<BackgroundPlanet>().ResetPosition();

                // enqueue the planet
                availablePlanets.Enqueue(aPlanet);
            }
        }
    }
}
