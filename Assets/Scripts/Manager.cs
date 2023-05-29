using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace Com.Albert.Kalashnikova
{
    public class Manager : MonoBehaviour
    {
        public string player_prefab;
        public Transform[] spawn_points;

        private void Start()
        {
            Spawn();
        }

        // Update is called once per frame
        public void Spawn()
        {
            Transform spawn_point = spawn_points[Random.Range(0, spawn_points.Length)];
            PhotonNetwork.Instantiate(player_prefab, spawn_point.position, spawn_point.rotation);
        }
    }
}