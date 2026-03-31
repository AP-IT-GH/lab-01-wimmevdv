using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using System.Collections.Generic;

public class ObelixAgent : Agent
{
    [Header("Referenties")]
    public Transform speelveld;              // referentie naar het Plane
    public GameObject menhirPrefab;          // de Menhir prefab
    public GameObject destinationPrefab;     // de Destination prefab

    [Header("Instellingen")]
    public float moveSpeed = 5f;
    public float rotateSpeed = 200f;
    public int aantalMenhirs = 6;            // aantal menhirs EN destinations
    public float spawnRange = 8f;            // hoe ver menhirs random kunnen spawnen
    public float circleRadius = 6f;          // radius van de destination-cirkel

    [Header("Materialen")]
    public Material vrijeMaterial;           // kleur voor vrije destination (bijv. blauw/grijs)
    public Material bezetteMaterial;         // kleur voor bezette destination (bijv. groen/goud)

    // Interne state
    private bool dragtMenhir = false;        // heeft Obelix een menhir op zijn rug?
    private GameObject vastgehoudeMenhir;     // referentie naar de opgepakte menhir
    private Rigidbody rb;

    // Gespawnde objecten bijhouden
    private List<GameObject> gespawndeMenhirs = new List<GameObject>();
    private List<GameObject> destinations = new List<GameObject>();
    private HashSet<GameObject> bezetteDestinations = new HashSet<GameObject>();


    public override void Initialize()
    {
        rb = GetComponent<Rigidbody>();
        MaxStep = 10000; // episode eindigt na 10000 stappen (meer tijd nodig voor 6 menhirs)
    }


    public override void OnEpisodeBegin()
    {

        dragtMenhir = false;
        vastgehoudeMenhir = null;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;


        transform.localPosition = new Vector3(0f, 0.5f, 0f);
        transform.localRotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);


        foreach (GameObject obj in gespawndeMenhirs)
        {
            if (obj != null) Destroy(obj);
        }
        foreach (GameObject obj in destinations)
        {
            if (obj != null) Destroy(obj);
        }
        gespawndeMenhirs.Clear();
        destinations.Clear();
        bezetteDestinations.Clear();

        // Spawn destinations in een cirkel (Stonehenge-stijl) ---
        for (int i = 0; i < aantalMenhirs; i++)
        {
            // Bereken positie op de cirkel (gelijkmatig verdeeld)
            float angle = i * (360f / aantalMenhirs) * Mathf.Deg2Rad;
            Vector3 pos = new Vector3(
                Mathf.Cos(angle) * circleRadius,
                1f,     // rustend op het speelveld
                Mathf.Sin(angle) * circleRadius
            );

            GameObject dest = Instantiate(destinationPrefab, transform.parent);
            dest.transform.localPosition = pos;

            // Zet vrije kleur
            Renderer rend = dest.GetComponent<Renderer>();
            if (rend != null && vrijeMaterial != null)
            {
                rend.material = vrijeMaterial;
            }

            destinations.Add(dest);
        }


        for (int i = 0; i < aantalMenhirs; i++)
        {
            Vector3 spawnPos = new Vector3(
                Random.Range(-spawnRange, spawnRange),
                3f,     // boven het veld, valt naar beneden door gravity
                Random.Range(-spawnRange, spawnRange)
            );

            GameObject menhir = Instantiate(menhirPrefab, transform.parent);
            menhir.transform.localPosition = spawnPos;
            gespawndeMenhirs.Add(menhir);
        }
    }


    public override void CollectObservations(VectorSensor sensor)
    {

        sensor.AddObservation(dragtMenhir);


        sensor.AddObservation(transform.localPosition.x / spawnRange);
        sensor.AddObservation(transform.localPosition.z / spawnRange);


    }


    public override void OnActionReceived(ActionBuffers actions)
    {
        // Twee continue acties: vooruit/achteruit + links/rechts draaien
        float moveInput = actions.ContinuousActions[0];
        float rotateInput = actions.ContinuousActions[1];

        // Beweeg vooruit/achteruit
        Vector3 movement = transform.forward * moveInput * moveSpeed * Time.deltaTime;
        rb.MovePosition(transform.position + movement);

        // Draai links/rechts
        float rotation = rotateInput * rotateSpeed * Time.deltaTime;
        transform.Rotate(0f, rotation, 0f);

        // Kleine negatieve beloning per stap (stimuleert snelheid)
        AddReward(-0.001f);

        // Straf als Obelix van het veld valt
        if (transform.localPosition.y < -1f)
        {
            AddReward(-1f);
            EndEpisode();
        }
    }

    // belonging aan de collider van Obelix, detect
    private void OnTriggerEnter(Collider other)
    {
        // --- MENHIR OPPAKKEN ---
        if (other.gameObject.CompareTag("Menhir"))
        {
            if (!dragtMenhir)
            {
                // Goed: pak de menhir op
                dragtMenhir = true;
                vastgehoudeMenhir = other.gameObject;
                vastgehoudeMenhir.SetActive(false); // verberg de menhir
                AddReward(0.5f);
            }
            else
            {
                // Fout: je draagt al een menhir!
                AddReward(-0.2f);
            }
        }

        // --- MENHIR AFLEVEREN OP BESTEMMING ---
        if (other.gameObject.CompareTag("Destination"))
        {
            if (dragtMenhir && !bezetteDestinations.Contains(other.gameObject))
            {
                // === SUCCES: afleveren op een VRIJE destination ===
                dragtMenhir = false;
                Destroy(vastgehoudeMenhir);
                vastgehoudeMenhir = null;

                // Markeer destination als bezet
                bezetteDestinations.Add(other.gameObject);

                // Verander kleur naar bezet
                Renderer rend = other.gameObject.GetComponent<Renderer>();
                if (rend != null && bezetteMaterial != null)
                {
                    rend.material = bezetteMaterial;
                }

                // Beloning voor aflevering
                AddReward(1.0f);

                // Check of ALLE destinations bezet zijn
                if (bezetteDestinations.Count >= aantalMenhirs)
                {
                    // Bonus: alle menhirs afgeleverd!
                    AddReward(2.0f);
                    EndEpisode();
                }
            }
            else if (dragtMenhir && bezetteDestinations.Contains(other.gameObject))
            {
                // Fout: deze destination is al bezet
                AddReward(-0.3f);
            }
            else if (!dragtMenhir)
            {
                // Fout: je hebt geen menhir bij je
                AddReward(-0.1f);
            }
        }
    }


    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActions = actionsOut.ContinuousActions;
        continuousActions[0] = Input.GetAxis("Vertical");    // W/S of pijltjes
        continuousActions[1] = Input.GetAxis("Horizontal");  // A/D of pijltjes
    }
}