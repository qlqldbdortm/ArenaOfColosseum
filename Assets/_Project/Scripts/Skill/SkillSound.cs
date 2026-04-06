using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Colosseum
{
    public class SkillSound : MonoBehaviour
    {
        private AudioSource skillSound;

        void Awake()
        {
            skillSound = GetComponent<AudioSource>();
        }

        void OnEnable()
        {
            skillSound.Play();
        }

        void OnDisable()
        {
            skillSound?.Stop();
        }
    }
}
