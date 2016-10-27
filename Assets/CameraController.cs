using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CameraController : MonoBehaviour {
    public WebCamTexture mCamera = null;
    // Use this for initialization
    void Start () {

        Debug.Log("Initialize");
        //Renderer renderer = gameObject.GetComponent<Renderer>();

        //set up camera
        WebCamDevice[] devices = WebCamTexture.devices;
        string backCamName = "";
        for (int i = 0; i < devices.Length; i++)
        {
            Debug.Log("Device:" + devices[i].name + "IS FRONT FACING:" + devices[i].isFrontFacing);

                backCamName = devices[i].name;
        }
        mCamera = new WebCamTexture(backCamName);//, 1280, 720, 30);
        mCamera.Play();
        //renderer.material.mainTexture = mCamera;
        GetComponent<Image>().material.mainTexture = mCamera;
    }

    // Update is called once per frame
    void Update () {
	
	}

    void TriggerAR()
    {
    }
    public void Initialize()
    {
    }
}
