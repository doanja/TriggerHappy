using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/*
* Resource: https://www.youtube.com/watch?v=0kqjRn-5vSA&list=PLt_Y3Hw1v3QSFdh-evJbfkxCK_bjUD37n&index=24
*
* 
*/
public class PathDefinition : MonoBehaviour {

    public Transform[] Points;  // list of points to travel

    public IEnumerator<Transform> GetPathEnumerator()
    {
        // if there is no Points
        if (Points == null || Points.Length < 1)
            yield break;

        var direction = 1; // x-direction
        var index = 0; 

        while (true)
        {
            yield return Points[index];

            if (Points.Length == 1)
                continue;

            if (index <= 0)
                direction = 1;

            else if (index >= Points.Length - 1)
                direction = -1;

            index = index + direction;
        }

    }

    /*
    * Draws a visible line between Points
    */
    public void OnDrawGizmos()
    {
        // draws nothing if there is not two points or more
        if (Points == null || Points.Length < 2)
            return;

        var points = Points.Where(t => t != null).ToList();
        
        // if there are only 1 point
        if (points.Count < 2)
            return;
       
        // draws a line between each point
        for(var i = 1; i < points.Count; i++)
        {
            Gizmos.DrawLine(points[i - 1].position, points[i].position);
        }
    }
}
