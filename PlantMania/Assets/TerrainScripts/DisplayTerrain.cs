using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayTerrain : MonoBehaviour
{
    public Renderer textureRenderer;
    public MeshFilter meshFilter;
    public MeshRenderer meshRenderer;
    public MeshCollider meshCollider;

    public int objectCount = 0;
    public int vineCount = 0;

    public GameObject beeHive;
    public GameObject spiderCocoon;
    public GameObject vineSeed;
    public GameObject vineSpawnerPosition;

    public LayerMask groundMask;

    public void DrawTexture(Texture2D texture) //applying the texture
    {
        textureRenderer.sharedMaterial.mainTexture = texture;
        textureRenderer.transform.localScale = new Vector3(texture.width, 1, texture.height);
    }

    public void DrawMesh(MeshData meshData, Texture2D texture) //creating and applying mesh
    {
        Mesh mesh = meshData.CreateMesh();
        meshFilter.sharedMesh = mesh;
        meshCollider.sharedMesh = mesh;
        meshRenderer.sharedMaterial.mainTexture = texture;

        //adding beehives randomly
        for(int i = 0; i < objectCount; i++)
        {
            float randomX = Random.Range(meshRenderer.bounds.min.x / 1.2f, meshRenderer.bounds.max.x / 1.2f);
            float randomZ = Random.Range(meshRenderer.bounds.min.z / 1.2f, meshRenderer.bounds.max.z / 1.2f);

            RaycastHit hit;
            if (Physics.Raycast(new Vector3(randomX, meshRenderer.bounds.max.y + 5f, randomZ), -Vector3.up, out hit, groundMask))
            {
                Instantiate(beeHive, hit.point, Quaternion.Euler(0, Random.Range(0, 360), 0));
            }
            else
            {
                i--; //did not hit ground, try again
            }
        }
        //adding spider cocoons randomly
        for (int i = 0; i < objectCount; i++)
        {
            float randomX = Random.Range(meshRenderer.bounds.min.x / 1.2f, meshRenderer.bounds.max.x / 1.2f);
            float randomZ = Random.Range(meshRenderer.bounds.min.z / 1.2f, meshRenderer.bounds.max.z / 1.2f);

            RaycastHit hit;
            if (Physics.Raycast(new Vector3(randomX, meshRenderer.bounds.max.y + 5f, randomZ), -Vector3.up, out hit, groundMask))
            {
                Instantiate(spiderCocoon, hit.point, Quaternion.Euler(0, Random.Range(0, 360), 0));
            }
            else
            {
                i--; //did not hit ground, try again
            }
        }

        //adding vines randomly
        for (int i = 0; i < vineCount; i++)
        {
            float randomX = Random.Range(meshRenderer.bounds.min.x / 1.2f, meshRenderer.bounds.max.x / 1.2f);
            float randomZ = Random.Range(meshRenderer.bounds.min.z / 1.2f, meshRenderer.bounds.max.z / 1.2f);

            RaycastHit hit;
            if (Physics.Raycast(new Vector3(randomX, meshRenderer.bounds.max.y + 5f, randomZ), -Vector3.up, out hit, groundMask))
            {
                GameObject vineSeedUnit = Instantiate(vineSeed, hit.point, Quaternion.identity);
                vineSeedUnit.GetComponent<VineSeed>().SpawnVine(hit.point, hit.normal);
            }
            else
            {
                i--; //did not hit ground, try again
            }
        }
    }
}
