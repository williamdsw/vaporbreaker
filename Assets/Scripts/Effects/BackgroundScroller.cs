﻿using System;
using UnityEngine;

namespace Effects
{
    public class BackgroundScroller : MonoBehaviour
    {
        // || Inspector References

        [Header("Required Configuration")]
        [SerializeField] private bool chooseRandomMaterial;
        [SerializeField] private Material[] materials;
        [SerializeField] private bool chooseRandomMovementSpeed;
        [SerializeField] private float movementSpeedInX = 0.1f;
        [SerializeField] private float movementSpeedInY = 0.1f;

        // || CONFIG

        private const float TEXTURE_OFFSET_VALUE = 0.2f;

        // || State
        private bool canOffsetTexture = false;

        // || Cached

        private Material material;
        private Renderer myRenderer;
        private Vector2 offset;

        private void Awake()
        {
            GetRequiredComponents();
            Config();
        }

        private void FixedUpdate()
        {
            if (canOffsetTexture)
            {
                material.mainTextureOffset += (offset * Time.fixedDeltaTime);
            }
        }

        /// <summary>
        /// Get required components
        /// </summary>
        private void GetRequiredComponents()
        {
            try
            {
                myRenderer = GetComponent<Renderer>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Config settings
        /// </summary>
        private void Config()
        {
            try
            {
                if (chooseRandomMaterial && materials != null && materials.Length != 0)
                {
                    int index = UnityEngine.Random.Range(0, materials.Length);
                    myRenderer.material = materials[index];
                }

                if (chooseRandomMovementSpeed)
                {
                    movementSpeedInX = UnityEngine.Random.Range(-TEXTURE_OFFSET_VALUE, TEXTURE_OFFSET_VALUE);
                    movementSpeedInY = UnityEngine.Random.Range(-TEXTURE_OFFSET_VALUE, TEXTURE_OFFSET_VALUE);
                }

                material = myRenderer.material;
                offset = new Vector2(movementSpeedInX, movementSpeedInY);
                canOffsetTexture = (!material.name.Contains("Flat"));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}