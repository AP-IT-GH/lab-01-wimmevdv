using UnityEngine;

public class FollowWaypoint : MonoBehaviour
{
    // Array om alle waypoints (bijv. de palmboom prefabs) in op te slaan
    public GameObject[] waypoints;
    int currentWaypoint = 0;

    public float speed = 10.0f;      // Bewegingssnelheid
    public float rotSpeed = 1.0f;    // Draaisnelheid
    public float lookAhead = 2.0f;   // Afstand tot waypoint voordat we wisselen

    void Update()
    {
        // 1. Controleer de afstand: zijn we dicht genoeg bij het huidige waypoint?
        if (Vector3.Distance(transform.position, waypoints[currentWaypoint].transform.position) < lookAhead)
        {
            // Ga naar het volgende waypoint in de lijst
            currentWaypoint++;
            
            // Als we bij het einde zijn, begin weer bij de eerste (indefinite loop)
            if (currentWaypoint >= waypoints.Length)
            {
                currentWaypoint = 0;
            }
        }

        // 2. Rotatie: Draai geleidelijk richting het volgende waypoint
        Vector3 direction = waypoints[currentWaypoint].transform.position - transform.position;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotSpeed);

        // 3. Beweging: Beweeg het object vooruit over de lokale Z-as
        transform.Translate(0, 0, speed * Time.deltaTime);
    }
}