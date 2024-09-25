using UnityEngine;

public class CTerrainRampGenerator : MonoBehaviour
{
    public Terrain terrain;
    public Vector3 startPoint;
    public Vector3 endPoint;
    public float startHeight;
    public float endHeight;
    public int width = 10;

    //void Start()
    //{
    //    GenerateRamp();
    //}

    public void GenerateRamp()
    {
        TerrainData terrainData = terrain.terrainData;
        int heightmapWidth = terrainData.heightmapResolution;
        int heightmapHeight = terrainData.heightmapResolution;

        float[,] heights = terrainData.GetHeights(0, 0, heightmapWidth, heightmapHeight);

        var startP = this.transform.position + startPoint;
        var endP = this.transform.position + endPoint;

        int startX = Mathf.RoundToInt(startP.x / terrainData.size.x * heightmapWidth);
        int startZ = Mathf.RoundToInt(startP.z / terrainData.size.z * heightmapHeight);
        int endX = Mathf.RoundToInt(endP.x / terrainData.size.x * heightmapWidth);
        int endZ = Mathf.RoundToInt(endP.z / terrainData.size.z * heightmapHeight);

        for (int x = startX; x <= endX; x++)
        {
            for (int z = startZ; z <= endZ; z++)
            {
                float t = Mathf.InverseLerp(startX, endX, x);
                float height = Mathf.Lerp(startHeight, endHeight, t);
                heights[z, x] = height / terrainData.size.y;
            }
        }

        terrainData.SetHeights(0, 0, heights);

        Debug.Log("=====MAKE SLOPE=====");
    }

    void OnDrawGizmos()
    {
        if (terrain == null) return;

        Gizmos.color = Color.red;
        Vector3 start = this.transform.position + new Vector3(startPoint.x, startHeight, startPoint.z);
        Vector3 end = this.transform.position + new Vector3(endPoint.x, endHeight, endPoint.z);

        // Draw the main line of the ramp
        Gizmos.DrawLine(start, end);

        // Draw the width of the ramp
        Vector3 perp = Vector3.Cross(end - start, Vector3.up).normalized * width * 0.5f;
        Gizmos.DrawLine(start - perp, start + perp);
        Gizmos.DrawLine(end - perp, end + perp);
        Gizmos.DrawLine(start - perp, end - perp);
        Gizmos.DrawLine(start + perp, end + perp);
    }
}