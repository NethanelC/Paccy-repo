using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SidePorters : MonoBehaviour
{
    [SerializeField] private Vector2 _posToTeleport;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        collision.gameObject.transform.position = _posToTeleport;
    }
}
