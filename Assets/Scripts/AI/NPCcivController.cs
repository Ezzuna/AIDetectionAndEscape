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
    public GameObject civilianContainer;
    public List<GameObject> civilianList;
    public CivilianManager civManager;

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

        civManager = civilianContainer.GetComponent<CivilianManager>();

        civilianList = new List<GameObject>();  //Add all civilians to list
        civilianList = civManager.UpdateCivilianList();

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
        GameObject toDelete = new GameObject();    //creating a placeholder as to not delete mid enumeration of for each loop
        if (other.tag == "Extraction" && NPCStateCurrent == NPCState.Evacuating)
        {
            foreach (GameObject child in civilianList)
            {
                if (child.gameObject == this.gameObject)
                {
                    toDelete = child;
                    
                }
            }

            civManager.DeleteCiv(toDelete);
        }
    }


    void CheckOtherCivs()
    {
        foreach (GameObject child in civilianList)
        {
            if(child.gameObject != this.gameObject)
            {
                if (child.gameObject.GetComponent<NPCcivController>().GetNPCState() == NPCState.Evacuating)
                {
                    if ((child.transform.position - this.transform.position).magnitude < detectionRange) //within range
                    {
                        if (Vector3.Angle(this.transform.forward, child.transform.position - this.transform.position) <= (120 / 2)) // AI can see AI
                        {

                            RaycastHit hit;
                            if (Physics.Linecast(this.transform.position, child.transform.position, out hit))
                            {
                                if (hit.transform.gameObject == child.gameObject)
                                {
                                    NPCStateCurrent = NPCState.Alerted;
                                }
                            }

                        }
                    }

                }
            }

        }
    }


    void Update()
    {
        if (NPCStateCurrent == NPCState.Alerted)
        {
            NPCStateCurrent = NPCState.Evacuating;
            SetColor(Color.red);
            NPCMoveToEscape();
        }

        civilianList = civManager.UpdateCivilianList();
        if (NPCStateCurrent == NPCState.Exploring)
        {
            if (playerController.GetPlayerDangerous())
            {
                if ((player.transform.position - this.transform.position).magnitude < detectionRange)
                {

                    Debug.Log("Player within range");
                    if (Vector3.Angle(this.transform.forward, player.transform.position - this.transform.position) <= (120 / 2)) // AI can see player
                    {
                        Debug.Log("Player within FOV");
                        RaycastHit hit;
                        if (Physics.Linecast(this.transform.position, player.transform.position, out hit))
                        {
                            Debug.Log("Pass");
                            Debug.Log(hit.transform.tag);
                            if (hit.transform.tag == "Player")
                            {
                                NPCStateCurrent = NPCState.Alerted;
                            }
                        }

                    }
                }
            }
            CheckOtherCivs();
        }


        
        if (playerController.GetAlertAll())
        {
            NPCStateCurrent = NPCState.Alerted;
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

    }
}
