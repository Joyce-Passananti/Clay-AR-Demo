using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class GenerateControlPoints : MonoBehaviour
{
    FieldInfo[] fields;
    public static GenerateControlPoints instance;

    // initial coil parameters
    public GameObject cube;
    public GameObject rib;
    public Vector3 pos;
    public float radius;
    public float layerHeight;
    public int nbLayers;
    public int nbPoints; // points in layer

    private GameObject sphere;
    public float sphereSize;
    public List<GameObject> path = new List<GameObject>();
    public List<List<Vector3>> oldPath;

    private LineRenderer lineRenderer;

    private Boolean updatePath = true;

    public string sphereTag = "Sphere";


    public float inc;
    private int counter;


    private void Awake()
    {
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        Type scriptClass = this.GetType();
        fields = scriptClass.GetFields();

        lineRenderer = new GameObject("Line").AddComponent<LineRenderer>();
        lineRenderer.startColor = Color.white;
        lineRenderer.endColor = Color.white;
        lineRenderer.startWidth = 0.001f;
        lineRenderer.endWidth = 0.001f;
        lineRenderer.useWorldSpace = true;
        
        cube.transform.rotation = Quaternion.identity;
        cube.transform.GetChild(0).transform.rotation = Quaternion.identity;
        initialToolPath();

    }

    // Update is called once per frame
    void Update()
    {
        if (updatePath)
        {
            drawToolpath();
            updatePath = false;
        }

        if (counter%10 == 0)
        {
            rib.transform.position = new Vector3(rib.transform.position.x - inc, rib.transform.position.y, rib.transform.position.z);
            drawToolpath();
        }
        counter++;
    }

    public void initialToolPath()
    {
        path.ForEach(x => { Destroy(x); });
        path.Clear();
        pos = Vector3.zero;

        // vectors = []
        for (int j = 0; j < nbLayers; j++)
        {
            for (int i = 0; i < nbPoints; i++)
            {
                float angle = 360 / nbPoints;
                sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                sphere.transform.SetParent(cube.transform);
                sphere.transform.localPosition = new Vector3(pos.x + (radius * Mathf.Cos(i * angle * Mathf.PI / 180)) * .01f, pos.y + (layerHeight * j) * .01f, pos.z + (radius * Mathf.Sin(i * angle * Mathf.PI / 180)) * .01f);
                sphere.transform.localScale = new Vector3(sphereSize, sphereSize, sphereSize);
                addObjectComponents(sphere);
                path.Add(sphere);
            }
        }
        oldPath = new List<List<Vector3>>();
        // oldPath.Add(savePath(path));
        drawToolpath();
    }
    public void drawToolpath()
    {
        lineRenderer.positionCount = path.Count;

        for (int i = 0; i < path.Count; i++)
        {
            lineRenderer.SetPosition(i, path[i].transform.position); //x,y and z position of the starting point of the line
        }
    }

    private void addObjectComponents(GameObject obj)
    {
        // Add a Rigidbody component to the sphere
        Rigidbody rb = obj.AddComponent<Rigidbody>();

        // Disable gravity and make the Rigidbody kinematic
        rb.useGravity = false;
        rb.isKinematic = true;

        obj.tag = sphereTag;

        //rb.isKinematic = true;

        // Add a SphereCollisionHandler component to the sphere

        //obj.AddComponent<HighlightObject>();
        //obj.GetComponent<ObjectManipulator>().enabled = false;
    }

    void OnCollisionEnter(Collision collision)
    {
        // Check if the collided object is a sphere
        if (collision.gameObject.CompareTag(sphereTag))
        {
            // Get the Rigidbody component
            Rigidbody rb = collision.gameObject.GetComponent<Rigidbody>();

            // Make the Rigidbody non-kinematic
            if (rb != null && rb.isKinematic)
            {
                rb.isKinematic = false;
            }
        }
    }


    public List<Vector3> savePath(List<GameObject> path)
    {
        List<Vector3> oldP = new List<Vector3>();
        foreach (GameObject p in path)
        {
            oldP.Add(p.transform.localPosition);
        }

        return oldP;
    }
    public void undo()
    {
        // path = oldPath;
        if (oldPath.Count > 0)
        {
            for (int i = 0; i < path.Count; i++)
            {
                path[i].transform.localPosition = oldPath[oldPath.Count - 1][i];
            }
            oldPath.RemoveAt(oldPath.Count - 1);

            drawToolpath();
            print("undo!");
        }

    }

}



