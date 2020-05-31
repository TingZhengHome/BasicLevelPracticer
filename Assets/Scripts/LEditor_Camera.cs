using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LEditor_Camera : Singleton<LEditor_Camera> {


    [SerializeField]
    float xMax, xMin, yMax, yMin;

    [SerializeField]
    float zMax, zMin;

    public float cameraSpeed;
    public float zoomInOutSpeed;

    public Vector3 startCameraPosition;

	// Use this for initialization
	void Start () {
        LevelEditor.LaunchedLevel += SetStartPositionAndShutDown;
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKey(KeyCode.W))
        {
            transform.position += Vector3.up * cameraSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.S))
        {
            transform.position += Vector3.down * cameraSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.A))
        {
            transform.position += Vector3.left * cameraSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.position += Vector3.right * cameraSpeed * Time.deltaTime;
        }
        transform.position += Vector3.forward * Input.GetAxis("Mouse ScrollWheel") * zoomInOutSpeed;

       transform.position = new Vector3(Mathf.Clamp(transform.position.x, xMin, xMax), Mathf.Clamp(transform.position.y, yMin, yMax), Mathf.Clamp(transform.position.z, zMin, zMax));
    }

    public void SetLimit(Vector3 minTile, Vector3 maxTile)
    {

        xMax = maxTile.x + maxTile.x * 0.3f;
        xMin = minTile.x - maxTile.x * 0.3f;

        yMin = maxTile.y + maxTile.y * 0.3f;
        yMax = minTile.y - maxTile.y * 0.3f;

        float z = maxTile.y * 3.5f;
        zMin = z;
        zMax = -2;
    }

    public void SetStartPositionAndShutDown()
    {
        transform.position = LEditor_Camera.Instance.startCameraPosition;
        GetComponent<LEditor_Camera>().enabled = false;
    }
}
