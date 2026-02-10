using UnityEngine;
using System.Collections.Generic;

public class FollowPath : MonoBehaviour
{
    public GameObject wpManagerObj; 
    public float speed = 5.0f;
    public float rotSpeed = 2.0f;
    public float accuracy = 1.5f; // Iets verhoogd om cirkelen te voorkomen

    GameObject[] wps;
    Graph g;
    int currentNode = 0;

    void Start()
    {
        if (wpManagerObj == null)
        {
            Debug.LogError("[FollowPath] WPManager Object is niet toegewezen!");
            return;
        }

        WPManager manager = wpManagerObj.GetComponent<WPManager>();
        wps = manager.waypoints;
        g = manager.graph;
        Debug.Log("[FollowPath] Script gestart. Aantal waypoints in manager: " + wps.Length);
    }

    public void GoToPalm(int index)
    {
        if (index < 0 || index >= wps.Length) return;

        GameObject startNode = GetClosestWaypoint();
        GameObject targetNode = wps[index];

        Debug.Log($"[A*] Bereken pad van {startNode.name} naar {targetNode.name}");

        if (g.AStar(startNode, targetNode))
        {
            currentNode = 0;
            string pathString = "";
            foreach (Node n in g.pathList) pathString += n.getID().name + " -> ";
            Debug.Log("[A*] Pad gevonden: " + pathString + "DONE");
        }
        else
        {
            Debug.LogWarning("[A*] Kon geen pad vinden tussen deze twee punten!");
        }
    }

    GameObject GetClosestWaypoint()
    {
        GameObject closest = wps[0];
        float lastDist = Vector3.Distance(transform.position, wps[0].transform.position);
        foreach (GameObject wp in wps)
        {
            float dist = Vector3.Distance(transform.position, wp.transform.position);
            if (dist < lastDist)
            {
                closest = wp;
                lastDist = dist;
            }
        }
        return closest;
    }

    void LateUpdate()
{
    if (g == null || g.pathList.Count == 0 || currentNode >= g.pathList.Count) return;

    GameObject currentGoalObj = g.pathList[currentNode].getID();
    
    // FIX: We pakken de positie maar zetten de Y op dezelfde hoogte als de tank
    Vector3 goalPos = currentGoalObj.transform.position;
    Vector3 targetPosFlat = new Vector3(goalPos.x, transform.position.y, goalPos.z); 
    
    Vector3 direction = targetPosFlat - transform.position;

    // Teken de rode lijn in Scene View
    Debug.DrawLine(transform.position, targetPosFlat, Color.red);

    // Gebruik de 'flat' direction voor de afstand
    if (direction.magnitude > accuracy)
    {
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotSpeed);
        transform.Translate(0, 0, speed * Time.deltaTime);
    }
    else
    {
        Debug.Log($"[FollowPath] Waypoint {currentGoalObj.name} bereikt! Op naar de volgende.");
        currentNode++;
    }
}
}