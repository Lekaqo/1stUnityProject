using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Scripts.ShootGame
{
    public class Mat : MonoBehaviour
    {
        public bool mat = false;
        public Text daba;
        public int HitNumber = 0;
        public int Score = 0;
        public float ResetTime = 4;
        public GameObject Powercell;

        private void Update()
        {           
            if (HitNumber == 3)
            {
                Score = Score + 20;
                ResetTime = ResetTime - 0.2f;
                HitNumber = 0;
                daba.text = "全部击中标靶时得20分，累计100分获得能量源，当前得分" + Score;

                if(Score == 100)
                {
                    Powercell.GetComponent<CapsuleCollider>().enabled = true;
                }
            }
        }

        private void OnTriggerEnter(Collider collider)
        {
            if (collider.gameObject.tag == "Player")
            {
                mat = true;
                daba.text = "全部击中标靶时得20分，累计100分获得能量源，当前得分" + Score;
            }
        }

        private void OnTriggerExit(Collider collider)
        {
            if (collider.gameObject.tag == "Player")
            {
                mat = false;
                Score = 0;
                ResetTime = 4;
                daba.text = "";
            }
        }
    }
}
