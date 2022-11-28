using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dreamteck.Splines;
using UnityEngine.UI;

public class FlowerGrow_v2 : MonoBehaviour
{
    [SerializeField] GameObject flowerPart;
    [SerializeField] GameObject flowerBud;
    [SerializeField] GameObject flowerPetal;
    [SerializeField] GameObject flowerThorn;
    [SerializeField] GameObject[] flowerLeaf;
    [SerializeField] float growSpeed = 1;
    [SerializeField] float incrementSize = 0.001f;
    [SerializeField] float size = 1;
    [SerializeField] float pointRadiusMultiplier = 1;
    [SerializeField] float minPointDistance = 1f;
    [SerializeField] float maxPointDistance = 2f;
    [SerializeField] int petalCount = 40;
    [SerializeField] float petalSpread = 60;
    [SerializeField] int thornProbabilityDivisor = 50;
    [SerializeField] int leafProbabilityDivisor = 50;

    float waterLevel = 0;
    float growth = 0;
    int pointCount = 4;
    int budCount = 4;
    GameObject[] buds;
    GameObject[] petals;
    float[] leanRatio;

    GameObject flowerGO;
    SplineComputer flowerSC;
    SplineMesh flowerSM;

    Quaternion rotation;

    Transform leavesStore;
    Transform thornsStore;
    Transform budsStore;
    Transform petalsStore;

    public static Text score;
    string color;

    public void spawnFlower(Vector3 coords)
    {

        buds = new GameObject[budCount];
        petals = new GameObject[petalCount];
        leanRatio = new float[petalCount];

        SplinePoint[] points = flowerSC.GetPoints();
        Vector3 position = coords;
        float distance = 0;

        color = RandomRoseColor();
        flowerPetal.GetComponent<MeshRenderer>().material = Resources.Load("RoseColors/Rose" + color, typeof(Material)) as Material;

        for (int i = 0; i < pointCount; i++)
        {
            points[i].position = position;
            points[i].normal = Vector3.forward;

            distance += Random.Range(minPointDistance, maxPointDistance);
            position = coords + Random.insideUnitSphere * pointRadiusMultiplier;
            position.y = coords.y + distance;
        }

        flowerSC.SetPoints(points);

        for (int b = 0; b < budCount; b++)
        {
            buds[b] = Instantiate(flowerBud, coords, flowerBud.transform.rotation * Quaternion.AngleAxis(b * 360 / budCount, Vector3.up), budsStore);
            buds[b].transform.localScale *= 0.3f;
        }

        for (int p = 0; p < petalCount; p++)
        {
            leanRatio[p] = Random.Range(1, 10) / 10f;

            petals[p] = Instantiate(flowerPetal, coords - new Vector3(0, 0.3f, 0), flowerPetal.transform.rotation * Quaternion.AngleAxis(p * 360 / petalCount, Vector3.up), petalsStore);
            petals[p].transform.localScale *= 0;
        }

        //ready = true;
    }

    // Start is called before the first frame update
    void Awake()
    {
        flowerGO = Instantiate(flowerPart, gameObject.transform, gameObject.transform);
        flowerSC = flowerGO.GetComponent<SplineComputer>();
        flowerSM = flowerGO.GetComponent<SplineMesh>();

        flowerSM.GetChannel(0).clipTo = 0;

        leavesStore = gameObject.transform.Find("Leaves");
        thornsStore = gameObject.transform.Find("Thorns");
        budsStore = gameObject.transform.Find("Buds");
        petalsStore = gameObject.transform.Find("Petals");

        if (score == null)
        {
            // If this line results in an error, check the following: Is the ScoreText named exactly "ScoreText"? does it have a Text Component in it? Does it exist, and is it enabled, by the time the first Pot appears?
            score = GameObject.Find("Score").GetComponentInChildren<Text>();
        }
    }

    public void Water()
    {
        waterLevel += 0.03f;
    }

    void Grow()
    {

        if (waterLevel > 0)
        {

            waterLevel -= incrementSize;
            growth += incrementSize;

            flowerSM.GetChannel(0).clipTo = growth;

            rotation = flowerSC.Evaluate(growth).rotation;

            for (int b = 0; b < budCount; b++)
            {
                buds[b].transform.position = flowerSC.EvaluatePosition(growth);
                buds[b].transform.rotation = rotation;
                buds[b].transform.Rotate(0, 0, b * 360 / budCount);
                buds[b].transform.Rotate(90 - 75 * growth, 0, 0);
            }

            for (int p = 0; p < petalCount; p++)
            {
                petals[p].transform.position = flowerSC.EvaluatePosition(growth) - rotation * Vector3.forward * 0.3f;
                petals[p].transform.rotation = rotation;
                petals[p].transform.Rotate(0, 0, p * 360 / petalCount);
                petals[p].transform.Rotate(90 - petalSpread * leanRatio[p] * growth, 0, 0);
                petals[p].transform.localScale = new Vector3(2f * leanRatio[p], 1f, 1.5f * leanRatio[p]) * growth / 3.5f;
            }

            if (Random.Range(0, thornProbabilityDivisor) == 0)
            {
                SpawnThorn(flowerSC.EvaluatePosition(growth), rotation, flowerSC.Evaluate(growth).size);
            }

            if (Random.Range(0, leafProbabilityDivisor) == 0 && 0.1f < growth && growth < 0.8f)
            {
                SpawnLeaf(flowerSC.EvaluatePosition(growth), rotation, flowerSC.Evaluate(growth).size);
            }

        }

    }

    // Update is called once per frame
    void Update()
    {
        if(growth < 1.0 - incrementSize)
        {
            Grow();
        }
        else
        {
            UpdateScore();
            Destroy(this);
        }
    }

    string RandomRoseColor()
    {
        int colorNumber = Random.Range(0, 13125);
        string color;

        //Debug.Log(colorNumber);

        if (colorNumber < 5000)
            color = "Red";
        else if (colorNumber < 10000)
            if (Random.Range(0, 2) == 0)
                color = "White";
            else
                color = "Yellow";
        else if (colorNumber < 11250)
            color = "Pink";
        else if (colorNumber < 12500)
            if (Random.Range(0, 2) == 0)
                color = "Purple";
            else
                color = "Orange";
        else
            color = "Blue";

        return color;
    }

    void SpawnThorn(Vector3 position, Quaternion rotation, float stemSize)
    {
        float thornSizeMiltiplier = Random.Range(0.5f, 1.5f);
        Quaternion thornRotation = Quaternion.AngleAxis(Random.Range(0, 360), Vector3.forward);
        Vector3 thornPosition = position + rotation * thornRotation * Vector3.up * -stemSize / 10;

        var thorn = Instantiate(flowerThorn, thornPosition, rotation * thornRotation, thornsStore);
        thorn.transform.localScale = thorn.transform.localScale * size * thornSizeMiltiplier / 8;
        thorn.transform.Rotate(90, 0, 90);
    }

    void SpawnLeaf(Vector3 position, Quaternion rotation, float stemSize)
    {
        float leafSizeMiltiplier = Random.Range(0.5f, 1.5f);
        Quaternion leafRotation = Quaternion.AngleAxis(Random.Range(0, 360), Vector3.forward);
        Vector3 leafPosition = position + rotation * leafRotation * Vector3.up * -stemSize / 10;

        var leaf = Instantiate(flowerLeaf[Random.Range(0, flowerLeaf.Length)], leafPosition, rotation * leafRotation, leavesStore);
        leaf.transform.localScale = leaf.transform.localScale * size * leafSizeMiltiplier / 5;
        leaf.transform.Rotate(-180, 0, 0);
        leaf.transform.Rotate(0, -180, 0);
        leaf.transform.Rotate(0, 0, 90);
    }

    void UpdateScore()
    {
        int addScore;

        if (color == "Red")
        {
            int.TryParse(score.text, out addScore);
            addScore++;
            score.text = addScore.ToString();
            //score++;
        }
        else if (color == "White" || color == "Yellow")
        {
            int.TryParse(score.text, out addScore);
            addScore += 2;
            score.text = addScore.ToString();
            //score += 2;
        }
        else if (color == "Pink")
        {
            int.TryParse(score.text, out addScore);
            addScore += 3;
            score.text = addScore.ToString();
            //score += 3;
        }
        else if (color == "Purple" || color == "Orange")
        {
            int.TryParse(score.text, out addScore);
            addScore += 4;
            score.text = addScore.ToString();
            //score += 4;
        }
        else if (color == "Blue")
        {
            int.TryParse(score.text, out addScore);
            addScore += 10;
            score.text = addScore.ToString();
            //score += 10;
        }
    }
}
