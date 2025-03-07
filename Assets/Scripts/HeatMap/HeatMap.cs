using System.Collections.Generic;
using UnityEngine;

public class HeatMap : MonoBehaviour
{
    public Gradient gradient;
    public int spreadRange = 5;
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

    public void GenerateHeatMap(List<Vector3> dataPoints, List<float> totalLosses, int textureSize)
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

            //float lossValue = Mathf.Clamp01((- totalLosses[i]) / 60.0f);
            float lossValue = Mathf.Clamp01((-totalLosses[i] - 75) / 60.0f);

            int px = Mathf.Clamp(
                (int)((-worldPoint.x + worldPosition.x + rangeX / 2) / rangeX * textureSize),
                0, textureSize - 1);

            int py = Mathf.Clamp(
                (int)((-worldPoint.z + worldPosition.z + rangeZ / 2) / rangeZ * textureSize),
                0, textureSize - 1);

            ApplyMosaicDensity(density, px, py, lossValue, spreadRange, textureSize);
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

    void ApplyMosaicDensity(float[,] density, int x, int y, float value, int gridSize, int textureSize)
    {
        int startX = Mathf.Clamp(x - gridSize / 2, 0, textureSize - 1);
        int startY = Mathf.Clamp(y - gridSize / 2, 0, textureSize - 1);
        int endX = Mathf.Min(startX + gridSize, textureSize);
        int endY = Mathf.Min(startY + gridSize, textureSize);

        for (int nx = startX; nx < endX; nx++)
        {
            for (int ny = startY; ny < endY; ny++)
            {
                density[nx, ny] = value; 
            }
        }
    }

}
