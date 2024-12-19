using System.Collections.Generic;
using UnityEngine;

public class HeatMap : MonoBehaviour
{
    public Gradient gradient;      

    private Texture2D heatmapTexture;

    void Start()
    {
    }

    private void Update()
    {
        //GenerateHeatmap();
        //Renderer renderer = GetComponent<Renderer>();
        //renderer.material.mainTexture = heatmapTexture;
    }

    public void GenerateHeatmap(List<Vector3> dataPoints, List<float> totalLosses, int textureSize)
    {
        heatmapTexture = new Texture2D(textureSize, textureSize);
        float[,] density = new float[textureSize, textureSize];

        float rangeX = transform.lossyScale.x * 1f * 10;
        float rangeZ = transform.lossyScale.z * 1f * 10;
        Vector3 worldPosition = transform.position;

        for (int i = 0; i < dataPoints.Count; i++)
        {
            Vector3 point = dataPoints[i];
            Vector3 worldPoint = point;

            float lossValue = Mathf.Clamp01((- totalLosses[i]) / 40.0f);

            int px = Mathf.Clamp(
                (int)((-worldPoint.x + worldPosition.x + rangeX / 2) / rangeX * textureSize),
                0, textureSize - 1);

            int py = Mathf.Clamp(
                (int)((-worldPoint.z + worldPosition.z + rangeZ / 2) / rangeZ * textureSize),
                0, textureSize - 1);

            SpreadDensity(density, px, py, lossValue, 8, textureSize);
        }

        for (int x = 0; x < textureSize; x++)
        {
            for (int y = 0; y < textureSize; y++)
            {
                float value = Mathf.Clamp01(density[x, y]);
                Color color = gradient.Evaluate(value);
                heatmapTexture.SetPixel(x, y, color);
            }
        }

        heatmapTexture.Apply();
        Renderer renderer = GetComponent<Renderer>();
        renderer.material.mainTexture = heatmapTexture;
    }



    void SpreadDensity(float[,] density, int x, int y, float value, int radius ,int textureSize)
    {
        for (int dx = -radius; dx <= radius; dx++)
        {
            for (int dy = -radius; dy <= radius; dy++)
            {
                int nx = Mathf.Clamp(x + dx, 0, textureSize - 1);
                int ny = Mathf.Clamp(y + dy, 0, textureSize - 1);

                float distance = Mathf.Sqrt(dx * dx + dy * dy);
                if (distance <= radius)
                {
                    density[nx, ny] += value * (1 - distance / radius); 
                }
            }
        }
    }

}
