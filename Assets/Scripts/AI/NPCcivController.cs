using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPCcivController : MonoBehaviour
{
   
    public enum NPCState
    {
        Exploring,
        Alerted,
        Evacuating
    }

    
    public NavMeshAgent agent;
    public float wanderRadius;
    public float wanderTimer;
    public GameObject extractionPoint;
    public GameObject player;
    public PlayerController playerController;
    public int detectionRange;


    private Transform target;
    private Transform originalPosition;
    private float timer;
    private NPCState NPCStateCurrent;
    private MeshRenderer meshRenderer;

    public NPCState GetNPCState()
    {
        return this.NPCStateCurrent;
    }

    public void SetNPCState(NPCState inState)
    {
        this.NPCStateCurrent = inState;
    }

    public static Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask)
    {
        Vector3 randDirection = Random.insideUnitSphere * dist;

        randDirection += origin;

        NavMeshHit navHit;

        NavMesh.SamplePosition(randDirection, out navHit, dist, layermask);

        return navHit.position;
    }


    private void OnEnable()
    {
        meshRenderer = this.transform.GetChild(0).gameObject.GetComponent<MeshRenderer>();
    }


    void Start()
    {
        Debug.Log("Start");
        originalPosition = this.transform;
        NPCStateCurrent = NPCState.Exploring;

        
    }


    public void SetColor(Color newColor)
    {
        meshRenderer.material.color = newColor;
    }

    void NPCMoveToEscape()
    {
        agent.SetDestination(extractionPoint.transform.position);
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Extraction" && NPCStateCurrent == NPCState.Evacuating)
        {
            Destroy(this.gameObject);
        }
    }


    void Update()
    {


        if (playerController.GetAlertAll())
        {
            NPCStateCurrent = NPCState.Alerted;
        }

        if (playerController.GetPlayerDangerous())
        {
            if ((player.transform.position - this.transform.position).magnitude < detectionRange)
            {
                if (Vector3.Angle(this.transform.forward, player.transform.position - this.transform.position) <= (120 / 2)) // AI can see player
                {

                    RaycastHit hit;
                    if (Physics.Linecast(this.transform.position, player.transform.position, out hit))
                    {
                        if (hit.transform.tag == "Player")
                        {
                            NPCStateCurrent = NPCState.Alerted;
                        }
                    }

                }
            }
        }



        timer += Time.deltaTime;
        if(NPCStateCurrent == NPCState.Exploring ) {
            if (timer >= wanderTimer)
            {
                Vector3 newPos = RandomNavSphere(originalPosition.position, wanderRadius, -1);
                agent.SetDestination(newPos);
                timer = 0;
            }
        }
        else if(NPCStateCurrent == NPCState.Alerted)
        {
            NPCStateCurrent = NPCState.Evacuating;
            SetColor(Color.red);
            NPCMoveToEscape();
        }
    }
}
