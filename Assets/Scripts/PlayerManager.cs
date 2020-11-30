using Photon.Pun;
using UnityEngine;
using UnityEngine.AI;

public class PlayerManager : MonoBehaviourPun
{
    private NavMeshAgent agent;
    private Animator animator;
    Vector2 smoothDeltaPosition = Vector2.zero;
    Vector2 velocity = Vector2.zero;

    void Start()
    {
        var camera = GetComponent<CameraFollow>();
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        if (!photonView.IsMine)
        {
            agent.enabled = false;
            return;
        }

        if (camera != null)
        {
            camera.OnStartFollowing();
        }
        else
        {
            Debug.LogError("<Color=Red><a>Missing</a></Color> CameraWork Component on playerPrefab.", this);
        }
    }

    void Update()
    {
        if (!photonView.IsMine)
        {
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out var hit))
            {
                agent.SetDestination(hit.point);
            }
        }

        var direction = agent.destination - agent.nextPosition;
        animator.SetFloat("Speed", direction.normalized.magnitude);
        //animator.SetFloat("Direction", pos.magnitude, 0.25f, Time.deltaTime); TODO
    }

    void OnAnimatorMove()
    {
        if (!photonView.IsMine)
        {
            return;
        }

        transform.position = agent.nextPosition;
    }
}
