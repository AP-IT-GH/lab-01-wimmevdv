using UnityEngine;

public class FollowWaypoint : MonoBehaviour
{
    
    public GameObject[] waypoints;
    int currentWaypoint = 0;

    public float speed = 10.0f;      
    public float rotSpeed = 1.0f;   
    public float lookAhead = 2.0f;   

    void Update()
    {
        
        if (Vector3.Distance(transform.position, waypoints[currentWaypoint].transform.position) < lookAhead)
        {
           
            currentWaypoint++;
            
            
            if (currentWaypoint >= waypoints.Length)
            {
                currentWaypoint = 0;
            }
        }

       
        Vector3 direction = waypoints[currentWaypoint].transform.position - transform.position;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotSpeed);

        
        transform.Translate(0, 0, speed * Time.deltaTime);
    }
}