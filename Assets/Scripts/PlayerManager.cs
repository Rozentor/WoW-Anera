using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class PlayerManager : MonoBehaviourPun
{
    // Start is called before the first frame update
    void Start()
    {
        var camera = GetComponent<CameraFollow>();


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

    // Update is called once per frame
    void Update()
    {
        
    }
}
