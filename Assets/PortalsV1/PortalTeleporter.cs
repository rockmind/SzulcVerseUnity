  using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalTeleporter : MonoBehaviour
{
    public Transform player;
    public Transform receiver;

    private bool _playerIsOverlapping = false;

    // Update is called once per frame
    void Update()
    {
        if (_playerIsOverlapping)
        {
            var teleport = transform;
            Vector3 portalToPlayer = player.position - teleport.position;
            float dotProduct = Vector3.Dot(teleport.up, portalToPlayer);

            if (dotProduct < 0f)
            {
                float rotationDiff = -Quaternion.Angle(transform.rotation, receiver.rotation);
                rotationDiff += 180;
                player.Rotate(Vector3.up, rotationDiff);

                Vector3 positionOffset = Quaternion.Euler(0f, rotationDiff, 0f) * portalToPlayer;
                player.position = receiver.position + positionOffset;

                _playerIsOverlapping = false;

            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _playerIsOverlapping = true;
        }
    }
    
    void OnTriggerExit (Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _playerIsOverlapping = false;
        }
    }
}
