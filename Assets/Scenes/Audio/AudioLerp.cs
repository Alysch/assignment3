// UMD IMDM290 
// Instructor: Myungin Lee
// All the same Lerp but using audio

using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AudioLerp : MonoBehaviour
{
    GameObject[] spheres;
    static int numSphere = 200; 
    float time = 0f;
    float Realtime = 0f;
    Vector3[] initPos;
    Vector3[] startPosition, LPos, OPos, VPos, EPos, endPosition;
    float lerpFraction; // Lerp point between 0~1
    float t , phase;
    GameObject mother;
    float direction = 1f;

    // Start is called before the first frame update
    void Start()
    {
        // Assign proper types and sizes to the variables.
        spheres = new GameObject[numSphere];
        initPos = new Vector3[numSphere]; // Start positions
        startPosition = new Vector3[numSphere];
        LPos = new Vector3[numSphere];
        OPos = new Vector3[numSphere];
        VPos = new Vector3[numSphere];
        EPos = new Vector3[numSphere]; 
        endPosition = new Vector3[numSphere];
        mother = GameObject.Find("leafBody");
        phase = 0f;
        // Define target positions. Start = random, End = spheres 
        for (int i =0; i < numSphere; i++){
            // Random start positions
            float r = 10f;
            startPosition[i] = new Vector3(r * Random.Range(-1f, 1f), r * Random.Range(-1f, 1f), r * Random.Range(-1f, 1f));

            if (i < numSphere * 2 / 3) {
                LPos[i] = new Vector3(0, 20 * i / (numSphere * 2 / 3) - 10, 0);
            } else {
                LPos[i] = new Vector3(9 * (i - numSphere * 2 / 3) / (numSphere * 1 / 3), -10, 0);
            }

            // Circular end position
            OPos[i] = new Vector3(r * Mathf.Sin(i * 2 * Mathf.PI / numSphere), r * Mathf.Cos(i * 2 * Mathf.PI / numSphere));

            if (i < numSphere / 2) {
                VPos[i] = new Vector3(-20 * i / (numSphere / 2), Mathf.Abs(-20 * i / (numSphere / 2)) - 10, 0);
            } else {
                VPos[i] = new Vector3(20 * (i - numSphere / 2) / (numSphere / 2), Mathf.Abs(20 * (i - numSphere / 2) / (numSphere / 2)) - 10, 0);
            }

            if (i < numSphere / 4) {
                EPos[i] = new Vector3(9 * i / (numSphere / 4), 10, 0);
            } else if (i < numSphere / 2) {
                EPos[i] = new Vector3(9 * (i - numSphere / 4) / (numSphere / 4), 0, 0);
            } else if (i < 3 * numSphere / 4) {
                EPos[i] = new Vector3(9 * (i - numSphere / 2) / (numSphere / 4), -10, 0);
            } else {
                EPos[i] = new Vector3(0, 20 * (i - 3 * numSphere / 4) / (numSphere / 4) - 10, 0);
            }

            r = 3f; // radius of the circle
            // Circular end position
            endPosition[i] = new Vector3(r * Mathf.Sin(i * 2 * Mathf.PI / numSphere), r * Mathf.Cos(i * 2 * Mathf.PI / numSphere));
        }
        // Let there be spheres..
        for (int i =0; i < numSphere; i++){
            // Draw primitive elements:
            // https://docs.unity3d.com/6000.0/Documentation/ScriptReference/GameObject.CreatePrimitive.html
            //spheres[i] = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            spheres[i] = Instantiate(mother);

            // Position
            initPos[i] = startPosition[i];
            spheres[i].transform.position = initPos[i];
            spheres[i].transform.localRotation = Quaternion.EulerAngles(Random.Range(-180f, 180f), Random.Range(-180f, 180f), Random.Range(-180f, 180f));
            spheres[i].transform.localScale = new Vector3(Random.Range(0.2f, 0.3f), Random.Range(0.2f, 0.3f), Random.Range(0.2f, 0.3f));
            // Color
            // Get the renderer of the spheres and assign colors.
            Renderer sphereRenderer = spheres[i].GetComponent<Renderer>();
            // HSV color space: https://en.wikipedia.org/wiki/HSL_and_HSV
            float hue = (float)i / numSphere; // Hue cycles through 0 to 1
            Color color = Color.HSVToRGB(1f, 1f, 1f); // Full saturation and brightness
            sphereRenderer.material.color = color;
        }
    }

    // Update is called once per frame
    void Update()
    {
        // ***Here, we use audio Amplitude, where else do you want to use?
        // Measure Time 
        // Time.deltaTime = The interval in seconds from the last frame to the current one
        // but what if time flows according to the music's amplitude?
        Realtime += Time.deltaTime;
        Debug.Log(Realtime);
        time += Time.deltaTime * AudioSpectrum.audioAmp;
        // time and phase is different.
        phase += Time.deltaTime * direction;

        

        // Change trigger happens when..
        if (phase >= 10.0f | phase <= 0f)
        {
            Debug.Log("switch");

            // change phase sequence direction 
            direction = direction * (-1f);

        }

        if (Realtime <= 6.7f) {
            // what to update over time?
            for (int i =0; i < numSphere; i++){
                // Lerp logic. Update position       
                t = i* 2 * Mathf.PI / numSphere;
                spheres[i].transform.position = Vector3.Lerp(startPosition[i], LPos[i], Realtime / 6.7f);
                // float scale = (1f + AudioSpectrum.audioAmp) * 0.2f;
                float scale = 1f + AudioSpectrum.audioAmp;

                //spheres[i].transform.localScale = new Vector3(scale, 0.2f, 0.2f);
                spheres[i].transform.localScale = new Vector3(scale, 1f, 1f);
                spheres[i].transform.Rotate(AudioSpectrum.audioAmp, 1f, 1f);
                
                // Color Update over time
                Renderer sphereRenderer = spheres[i].GetComponent<Renderer>();
            }
        } 
        else if (Realtime > 6.7f && Realtime < 12.6f)
        {
            for (int i =0; i < numSphere; i++){
                // Lerp logic. Update position       
                t = i* 2 * Mathf.PI / numSphere;
                spheres[i].transform.position = Vector3.Lerp(LPos[i], OPos[i], (Realtime - 6.7f) / 5.9f);
                // float scale = (1f + AudioSpectrum.audioAmp) * 0.2f;
                float scale = 1f + AudioSpectrum.audioAmp;

                //spheres[i].transform.localScale = new Vector3(scale, 0.2f, 0.2f);
                spheres[i].transform.localScale = new Vector3(scale, 1f, 1f);
                spheres[i].transform.Rotate(AudioSpectrum.audioAmp, 1f, 1f);

                // color update
                Renderer sphereRenderer = spheres[i].GetComponent<Renderer>();
                float hue = (float)i / numSphere; // Hue cycles through 0 to 1
                Color color = Color.HSVToRGB(1f, 1f, 1f); // Full saturation and brightness
                sphereRenderer.material.color = color;
            }
            Debug.Log("2nd stage");
        }
        else if (Realtime > 12.6f && Realtime < 18.5f)
        {
            for (int i = 0; i < numSphere; i++)
            {
                // Lerp logic. Update position       
                t = i* 2 * Mathf.PI / numSphere;
                spheres[i].transform.position = Vector3.Lerp(OPos[i], VPos[i], (Realtime - 12.6f) / 5.9f);
                // float scale = (1f + AudioSpectrum.audioAmp) * 0.2f;
                float scale = 1f + AudioSpectrum.audioAmp;

                //spheres[i].transform.localScale = new Vector3(scale, 0.2f, 0.2f);
                spheres[i].transform.localScale = new Vector3(scale, 1f, 1f);
                spheres[i].transform.Rotate(AudioSpectrum.audioAmp, 1f, 1f);
                
                // color update
                Renderer sphereRenderer = spheres[i].GetComponent<Renderer>();
                float hue = (float)i / numSphere; // Hue cycles through 0 to 1
                Color color = Color.HSVToRGB(0.5f, 1f, 1f); // Full saturation and brightness
                sphereRenderer.material.color = color;

            }
            Debug.Log("3rd stage");

        }
        else if (Realtime > 18.5f && Realtime < 25f)
        {
            for (int i = 0; i < numSphere; i++)
            {
                // Lerp logic. Update position     
                spheres[i].transform.position = Vector3.Lerp(VPos[i], EPos[i], (Realtime - 18.5f) / 6.5f);
                // float scale = (1f + AudioSpectrum.audioAmp) * 0.2f;
                float scale = 1f + AudioSpectrum.audioAmp;

                //spheres[i].transform.localScale = new Vector3(scale, 0.2f, 0.2f);
                spheres[i].transform.localScale = new Vector3(scale, 1f, 1f);
                spheres[i].transform.Rotate(AudioSpectrum.audioAmp, 1f, 1f);

                // color update
                Renderer sphereRenderer = spheres[i].GetComponent<Renderer>();
                float hue = (float)i / numSphere; // Hue cycles through 0 to 1
                Color color = Color.HSVToRGB(0.3f, 1f, 1f); // Full saturation and brightness
                sphereRenderer.material.color = color;
            }
            Debug.Log("4th stage");

        }
        else if (Realtime > 25f && Realtime < 40f)
        {
            for (int i = 0; i < numSphere; i++)
            {
                // Lerp logic. Update position     
                spheres[i].transform.position = Vector3.Lerp(EPos[i], startPosition[i], (Realtime - 25f) / 15f);
                // float scale = (1f + AudioSpectrum.audioAmp) * 0.2f;
                float scale = 1f + AudioSpectrum.audioAmp;

                //spheres[i].transform.localScale = new Vector3(scale, 0.2f, 0.2f);
                spheres[i].transform.localScale = new Vector3(scale, 1f, 1f);
                spheres[i].transform.Rotate(AudioSpectrum.audioAmp, 1f, 1f);

                // color update
                Renderer sphereRenderer = spheres[i].GetComponent<Renderer>();
                float hue = (float)i / numSphere; // Hue cycles through 0 to 1
                Color color = Color.HSVToRGB(Mathf.Lerp(0.3f, hue * Mathf.Cos(40), (Realtime - 25f) / 15f), 1f, Mathf.Lerp(1f, (1f + Mathf.Cos(40f)) / 2f, (Realtime - 25f) / 15f)); // Full saturation and brightness
                sphereRenderer.material.color = color;
            }
            Debug.Log("5th stage");

        } 
        else if (Realtime > 40f && Realtime < 70f) 
        {
            // what to update over time?
            for (int i =0; i < numSphere; i++){
                // lerpFraction variable defines the point between startPosition and endPosition (0~1)
                float start = 0f;
                float end = 1f;

                lerpFraction = Mathf.Lerp(start, end, phase/10f);

                // Lerp logic. Update position
                spheres[i].transform.position = Vector3.Lerp(startPosition[i], endPosition[i], lerpFraction);
                // float scale = (1f + AudioSpectrum.audioAmp) * 0.2f;
                float scale = 1f + AudioSpectrum.audioAmp;

                //spheres[i].transform.localScale = new Vector3(scale, 0.2f, 0.2f);
                spheres[i].transform.localScale = new Vector3(scale, 1f, 1f);
                spheres[i].transform.Rotate(AudioSpectrum.audioAmp, 1f, 1f);
                
                // Color Update over time
                Renderer sphereRenderer = spheres[i].GetComponent<Renderer>();
                float hue = (float)i / numSphere; // Hue cycles through 0 to 1
                Color color = Color.HSVToRGB(Mathf.Abs(hue * Mathf.Cos(Realtime)), Mathf.Cos(AudioSpectrum.audioAmp / 10f) / 2f, (1f + Mathf.Cos(Realtime)) / 2f); // Full saturation and brightness
                sphereRenderer.material.color = color;
            }
            Debug.Log("6th stage");
        } else if (Realtime > 70f && Realtime < 81.5f) {
            // what to update over time?
            for (int i =0; i < numSphere; i++){
                // Lerp logic. Update position       
                t = i* 2 * Mathf.PI / numSphere;
                spheres[i].transform.position = Vector3.Lerp(startPosition[i], LPos[i], (Realtime - 70f) / 11.5f);
                // float scale = (1f + AudioSpectrum.audioAmp) * 0.2f;
                float scale = 1f + AudioSpectrum.audioAmp;

                //spheres[i].transform.localScale = new Vector3(scale, 0.2f, 0.2f);
                spheres[i].transform.localScale = new Vector3(scale, 1f, 1f);
                spheres[i].transform.Rotate(AudioSpectrum.audioAmp, 1f, 1f);
                
                // Color Update over time
                Renderer sphereRenderer = spheres[i].GetComponent<Renderer>();
                float hue = (float)i / numSphere; // Hue cycles through 0 to 1
                Color color = Color.HSVToRGB(Mathf.Lerp(hue, 1f, (Realtime - 70f) / 11.5f), Mathf.Lerp(0, 1f, (Realtime - 70f) / 11.5f) / 2f, Mathf.Lerp((1f + Mathf.Cos(70f)) / 2f, 1f, (Realtime - 70f) / 11.5f)); // Full saturation and brightness
                sphereRenderer.material.color = color;
            }
            Debug.Log("6th stage");
        } 
        else if (Realtime > 81.5f && Realtime < 87.4)
        {
            for (int i =0; i < numSphere; i++){
                // Lerp logic. Update position      
                spheres[i].transform.position = Vector3.Lerp(LPos[i], OPos[i], (Realtime - 81.5f) / 5.9f);
                // float scale = (1f + AudioSpectrum.audioAmp) * 0.2f;
                float scale = 1f + AudioSpectrum.audioAmp;

                //spheres[i].transform.localScale = new Vector3(scale, 0.2f, 0.2f);
                spheres[i].transform.localScale = new Vector3(scale, 1f, 1f);
                spheres[i].transform.Rotate(AudioSpectrum.audioAmp, 1f, 1f);

                // color update
                Renderer sphereRenderer = spheres[i].GetComponent<Renderer>();
                Color color = Color.HSVToRGB(1f, 1f, 1f); // Full saturation and brightness
                sphereRenderer.material.color = color;
            }
            Debug.Log("8th stage");
        }
        else if (Realtime > 87.4f && Realtime < 93.2f)
        {
            for (int i = 0; i < numSphere; i++)
            {
                // Lerp logic. Update position       
                t = i* 2 * Mathf.PI / numSphere;
                spheres[i].transform.position = Vector3.Lerp(OPos[i], VPos[i], (Realtime - 87.4f) / 5.8f);
                // float scale = (1f + AudioSpectrum.audioAmp) * 0.2f;
                float scale = 1f + AudioSpectrum.audioAmp;

                //spheres[i].transform.localScale = new Vector3(scale, 0.2f, 0.2f);
                spheres[i].transform.localScale = new Vector3(scale, 1f, 1f);
                spheres[i].transform.Rotate(AudioSpectrum.audioAmp, 1f, 1f);
                
                // color update
                Renderer sphereRenderer = spheres[i].GetComponent<Renderer>();
                float hue = (float)i / numSphere; // Hue cycles through 0 to 1
                Color color = Color.HSVToRGB(0.5f, 1f, 1f); // Full saturation and brightness
                sphereRenderer.material.color = color;
            }
            Debug.Log("9th stage");

        }
        else if (Realtime > 93.2f && Realtime < 99.5f)
        {
            for (int i = 0; i < numSphere; i++)
            {
                // Lerp logic. Update position     
                spheres[i].transform.position = Vector3.Lerp(VPos[i], EPos[i], (Realtime - 93.2f) / 6.3f);
                // float scale = (1f + AudioSpectrum.audioAmp) * 0.2f;
                float scale = 1f + AudioSpectrum.audioAmp;

                //spheres[i].transform.localScale = new Vector3(scale, 0.2f, 0.2f);
                spheres[i].transform.localScale = new Vector3(scale, 1f, 1f);
                spheres[i].transform.Rotate(AudioSpectrum.audioAmp, 1f, 1f);

                // color update
                Renderer sphereRenderer = spheres[i].GetComponent<Renderer>();
                float hue = (float)i / numSphere; // Hue cycles through 0 to 1
                Color color = Color.HSVToRGB(0.3f, 1f, 1f); // Full saturation and brightness
                sphereRenderer.material.color = color;
            }
            Debug.Log("10th stage");

        }
        else if (Realtime > 99.5f && Realtime < 120f)
        {
            for (int i = 0; i < numSphere; i++)
            {
                // Lerp logic. Update position     
                spheres[i].transform.position = Vector3.Lerp(EPos[i], startPosition[i], (Realtime - 99.5f) / 20.5f);
                // float scale = (1f + AudioSpectrum.audioAmp) * 0.2f;
                float scale = 1f + AudioSpectrum.audioAmp;

                //spheres[i].transform.localScale = new Vector3(scale, 0.2f, 0.2f);
                spheres[i].transform.localScale = new Vector3(scale, 1f, 1f);
                spheres[i].transform.Rotate(AudioSpectrum.audioAmp, 1f, 1f);

                // color update
                Renderer sphereRenderer = spheres[i].GetComponent<Renderer>();
                float hue = (float)i / numSphere; // Hue cycles through 0 to 1
                Color color = Color.HSVToRGB(Mathf.Lerp(0.3f, hue * Mathf.Cos(120), (Realtime - 99.5f) / 20.5f), 1f, Mathf.Lerp(1f, (1f + Mathf.Cos(120f)) / 2f, (Realtime - 99.5f) / 20.5f)); // Full saturation and brightness
                sphereRenderer.material.color = color;
            }
            Debug.Log("11th stage");
        }
        else if (Realtime > 120f) 
        {
            // what to update over time?
            for (int i =0; i < numSphere; i++){
                // lerpFraction variable defines the point between startPosition and endPosition (0~1)
                float start = 0f;
                float end = 1f;

                lerpFraction = Mathf.Lerp(start, end, phase/10f);

                // Lerp logic. Update position
                spheres[i].transform.position = Vector3.Lerp(startPosition[i], endPosition[i], lerpFraction);
                // float scale = (1f + AudioSpectrum.audioAmp) * 0.2f;
                float scale = 1f + AudioSpectrum.audioAmp;

                //spheres[i].transform.localScale = new Vector3(scale, 0.2f, 0.2f);
                spheres[i].transform.localScale = new Vector3(scale, 1f, 1f);
                spheres[i].transform.Rotate(AudioSpectrum.audioAmp, 1f, 1f);
                
                // Color Update over time
                Renderer sphereRenderer = spheres[i].GetComponent<Renderer>();
                float hue = (float)i / numSphere; // Hue cycles through 0 to 1
                Color color = Color.HSVToRGB(Mathf.Abs(hue * Mathf.Cos(Realtime)), Mathf.Cos(AudioSpectrum.audioAmp / 10f) / 2f, (1f + Mathf.Cos(Realtime)) / 2f); // Full saturation and brightness
                sphereRenderer.material.color = color;
            }
            Debug.Log("6th stage");
        }
    }
}