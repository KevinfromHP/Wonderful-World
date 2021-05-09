using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace ForgottenFoes.Utils
{
    /// <summary>
    /// Attach this component to a gameObject and pass a meshrenderer in. It'll attempt to find the correct shader controller from the meshrenderer material, attach it if it finds it, and destroy itself.
    /// </summary>
    public class HGControllerFinder : MonoBehaviour
    {
        public Renderer Renderer;
        public Material Material;

        public void Start()
        {
            if (Renderer)
            {
                Material = Renderer.material;
                if (Material)
                {
                    switch (Material.shader.name)
                    {
                        case "Hopoo Games/Deferred/Standard":
                            var standardController = gameObject.AddComponent<HGStandardController>();
                            standardController.Material = Material;
                            standardController.Renderer = Renderer;
                            break;
                        case "Hopoo Games/FX/Cloud Remap":
                            var cloudController = gameObject.AddComponent<HGCloudRemapController>();
                            cloudController.Material = Material;
                            cloudController.Renderer = Renderer;
                            break;
                        case "Hopoo Games/FX/Cloud Intersection Remap":
                            var intersectionController = gameObject.AddComponent<HGIntersectionController>();
                            intersectionController.Material = Material;
                            intersectionController.Renderer = Renderer;
                            break;
                    }
                }
            }
            Destroy(this);
        }
    }
}
