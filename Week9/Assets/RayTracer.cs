using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RayTracer : MonoBehaviour
{
    public Vector2Int targetTextureSize;
    public LayerMask raytraceable;
    public Color skyBoxColour;
    public Color ambientLightColour;
    public float FOVAngle = 45;
    public int maxRayReflectionCount = 2;
    public RawImage uiOutput;
    public Light[] lights;

    Texture2D texture;

    private void Start()
    {
        //Create the texture for use as render output
        texture = new Texture2D(targetTextureSize.x, targetTextureSize.y);
        //set the ui element to use it
        uiOutput.texture = texture;
        rayTrace();
        texture.Apply();
    }


    void FillTexture(Color color)
    {
        for (int y = 0; y < texture.height; y++)
        {
            for (int x = 0; x < texture.width; x++)
            {
                texture.SetPixel(x, y, color);
            }
        }
        texture.Apply();
    }

    void rayTrace()
    {
        float cameraZ = -1 / (Mathf.Tan(Mathf.Deg2Rad * FOVAngle));
        Vector3 relCamCoord = new Vector3(0.5f, 0.5f, cameraZ); // pos of camera centre relative to top left of projection plane

        Vector2 outputSize = uiOutput.rectTransform.sizeDelta;
        float aspectRatio = outputSize.x / outputSize.y;

        for(int y = 0; y < texture.height; y++)
        {
            for(int x = 0; x < texture.width; x++)
            {
                Vector3 rayDirection = new Vector3((float)x / texture.width, (float)y / texture.height, 0) - relCamCoord;
                rayDirection.x *= aspectRatio;
                rayDirection.Normalize();

                rayDirection = Camera.main.transform.TransformVector(rayDirection);
                Ray ray = new Ray(Camera.main.transform.position, rayDirection);
                texture.SetPixel(x, y, doRayTrace(ray));
            }
        }
    }

    // Takes a ray as input and a depth count
    Color doRayTrace(Ray ray)
    {
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, raytraceable))
        {
            Renderer renderer = hit.transform.gameObject.GetComponentInChildren<Renderer>();
            if (renderer != null)
            {
                Color texelColor = Color.black;// = renderer.material.color * ambientLightColor;
                foreach (Light light in lights)
                {
                    if(light.enabled)
                    {
                        switch (light.type)
                        {
                            case LightType.Directional:
                                texelColor += phongLighting(light.transform.forward, -hit.normal, ray.direction, light.color, renderer.material.color);
                                break;
                        }
                    }
                }
                return texelColor;
            }
        }
        skyBoxColour.a = 1;
        return skyBoxColour;
    }

    Color phongLighting(Vector3 lightDirection, Vector3 surfaceNormal, Vector3 viewRay, Color lightColour, Color materialColour)
    {
        Color diffuse = materialColour * lightColour * Mathf.Clamp01(Vector3.Dot(lightDirection, surfaceNormal));

        float shininess = 30;
        float specIntens = 0.4f;
        Vector3 reflectedRay = Vector3.Reflect(-viewRay, surfaceNormal);
        Color specular = Mathf.Pow(Mathf.Clamp01(Vector3.Dot(reflectedRay, lightDirection)), shininess) * lightColour * specIntens;
        return diffuse + specular + ambientLightColour;
    }
}
