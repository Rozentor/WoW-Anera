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

        agent.updatePosition = false;

        if (camera != null)
        {
            if (photonView.IsMine)
            {
                camera.OnStartFollowing();
            }
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

        var worldDeltaPosition = agent.nextPosition - transform.position;

        var dx = Vector3.Dot(transform.right, worldDeltaPosition);
        var dy = Vector3.Dot(transform.forward, worldDeltaPosition);
        var deltaPosition = new Vector2(dx, dy);

        var smooth = Mathf.Min(1.0f, Time.deltaTime / 0.15f);
        smoothDeltaPosition = Vector2.Lerp(smoothDeltaPosition, deltaPosition, smooth);

        if (Time.deltaTime > 1e-5f)
            velocity = smoothDeltaPosition / Time.deltaTime;


        animator.SetFloat("Speed", velocity.x + velocity.y);
        //animator.SetFloat("Direction", pos.magnitude, 0.25f, Time.deltaTime); TODO
    }

    void OnAnimatorMove()
    {
        transform.position = agent.nextPosition;
    }
}
