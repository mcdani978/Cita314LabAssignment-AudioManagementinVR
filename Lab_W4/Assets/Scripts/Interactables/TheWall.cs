using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.Events;

[ExecuteAlways]
public class TheWall : MonoBehaviour
{
    // UnityEvent for handling wall destruction
    public UnityEvent OnDestroy; // Fixed typo from OnDestory to OnDestroy

    [SerializeField] int columns;
    [SerializeField] int rows;

    [SerializeField] GameObject wallCubePrefab;
    [SerializeField] GameObject socketWallPrefab;

    [SerializeField] int SocketPosition = 1; // Fixed syntax error

    [SerializeField] XRSocketInteractor wallSocket;

    [SerializeField] List<GeneratedColumn> generatedColumn;

    [SerializeField] float cubeSpacing = 0.005f;

    private Vector3 cubeSize;
    private Vector3 spawnPosition;

    [SerializeField] bool buildWall;
    [SerializeField] bool deleteWall;
    [SerializeField] bool destroyWall;

    [SerializeField] int maxPower;

    [SerializeField] AudioClip destroyWallClip;

    public AudioClip GetDestroyClip => destroyWallClip;

    // Array to store wall cubes
    [SerializeField] GameObject[] wallCubes;

    void Start()
    {
        // Optionally initialize anything on start if necessary
    }

    private void BuildWall()
    {
        if (wallCubePrefab != null)
        {
            cubeSize = wallCubePrefab.GetComponent<Renderer>().bounds.size;
        }

        // Initialize spawn position based on the current position of the object
        spawnPosition = transform.position;
        for (int i = 0; i < columns; i++)
        {
            GenerateColumn(rows, true);
        }
    }

    private void GenerateColumn(int height, bool socketed)
    {
        GeneratedColumn tempColumn = new GeneratedColumn();
        tempColumn.InitializeColumn(transform, height, true);

        wallCubes = new GameObject[2];  // Set the number of cubes to 2, can be increased based on the wall design

        // Set each cube as a child of this GameObject
        for (int i = 0; i < wallCubes.Length; i++)
        {
            if (wallCubePrefab != null)
            {
                // Instantiate the first wall cube
                wallCubes[i] = Instantiate(socketWallPrefab, spawnPosition, transform.rotation);
            }
            if (wallCubes[i] != null)
            {
                wallCubes[i].transform.SetParent(transform);  // Set the parent to this object
            }
            spawnPosition.y += cubeSize.y + cubeSpacing;
        }

        // Instantiate the second wall cube if needed
        if (wallSocket != null)
        {
            wallCubes[1] = Instantiate(socketWallPrefab, spawnPosition, transform.rotation);
        }

        // Get the XRSocketInteractor component from the second wall cube
        wallSocket = wallCubes[1].GetComponentInChildren<XRSocketInteractor>();
        if (wallSocket != null)
        {
            wallSocket.selectEntered.AddListener(OnSocketEnter);
            wallSocket.selectExited.AddListener(OnSocketExited);
        }
    }

    private void AddSocketWall(GeneratedColumn socketedColumn)
    {
        // Logic for adding socket wall can be added here
    }

    private void OnSocketExited(SelectExitEventArgs arg0)
    {
        // Handle when something exits the socket
        for (int i = 0; i < wallCubes.Length; i++)
        {
            if (wallCubes[i] != null)
            {
                Rigidbody rb = wallCubes[i].GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.isKinematic = true;  // Make the Rigidbody kinematic
                }
            }
        }
    }

    private void OnSocketEnter(SelectEnterEventArgs arg0)
    {
        if (generatedColumn.Count >= 1)
        {
            for (int i = 0; i < wallCubes.Length; i++)
            {
                generatedColumn[i].DeleteColumn();
            }
        }

        // Invoke the OnDestroy event when the socketed wall is interacted with
        OnDestroy?.Invoke();  // Fixed typo from OnDestory to OnDestroy
    }

    // Method to destroy the wall, invoking the OnDestroy event
    private void DestroyWall()
    {
        OnDestroy?.Invoke(); // Trigger event before any other logic

        // Example of any other destruction logic you'd want to add (like destroying GameObjects)
        Destroy(gameObject);  // Destroy this wall object
    }

    // Update is called once per frame (empty for now)
    void Update()
    {
        if (buildWall)
        {
            buildWall = false;
            BuildWall();
        }

        // If the wall is set to be destroyed, call the DestroyWall method
        if (destroyWall)
        {
            destroyWall = false;
            DestroyWall();
        }
    }
}

[System.Serializable]
public class GeneratedColumn
{
    [SerializeField] GameObject[] wallCubes;

    [SerializeField] bool isSocketed;

    private Transform parentObject;

    private const string Column_Name = "column";

    public void InitializeColumn(Transform parent, int rows, bool socketed)
    {
        parentObject = parent;
        wallCubes = new GameObject[rows];
        isSocketed = socketed;
    }

    public void SetCube(GameObject cube)
    {
        for (int i = 0; i < wallCubes.Length; i++)
        {
            if (i == 0)
            {
                cube.name = Column_Name;
                cube.transform.SetParent(parentObject);
            }
            else
            {
                cube.transform.SetParent(wallCubes[0].transform);
            }

            if (wallCubes[i] != null)
            {
                wallCubes[i] = cube;
                break;
            }
        }
    }

    public void DeleteColumn()
    {
        for (int i = 0; i < wallCubes.Length; i++)
        {
            if (wallCubes[i] != null)
            {
                Object.DestroyImmediate(wallCubes[i]);
            }
        }
        wallCubes = new GameObject[0];
    }

    public void DestroyColumn()
    {
        for (int i = 0; i < wallCubes.Length; i++)
        {
            if (wallCubes[i] == null)
            {
                Rigidbody rb = wallCubes[i].GetComponent<Rigidbody>();
                rb.isKinematic = false;
            }
        }
    }
}
