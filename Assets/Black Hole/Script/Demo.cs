using UnityEngine;
using System.Collections;

public class Demo : MonoBehaviour
{
	public Material m_Mat = null;
	[Range(0.01f, 0.2f)] public float m_DarkRange = 0.1f;
	[Range(-2.5f, -1f)] public float m_Distortion = -2f;
	[Range(0.05f, 0.3f)] public float m_Form = 0.2f;
	private float m_MouseX = 0f;
	private float m_MouseY = 0f;
	private bool m_TraceMouse = false;
	
	void Start ()
	{
		if (!SystemInfo.supportsImageEffects)
			enabled = false;
		m_MouseX = m_MouseY = 0.5f;
	}
	void OnRenderImage (RenderTexture sourceTexture, RenderTexture destTexture)
	{
		m_Mat.SetVector ("_Center", new Vector4 (m_MouseX, m_MouseY, 0f, 0f));
		m_Mat.SetFloat ("_DarkRange", m_DarkRange);
		m_Mat.SetFloat ("_Distortion", m_Distortion);
		m_Mat.SetFloat ("_Form", m_Form);
		Graphics.Blit (sourceTexture, destTexture, m_Mat);
	}
	void Update ()
	{
		if (Input.GetMouseButtonDown (1))
		{
			m_TraceMouse = true;
		}
		else if (Input.GetMouseButtonUp (1))
		{
			m_TraceMouse = false;
		}
		else if (Input.GetMouseButton (1))
		{
			if (m_TraceMouse)
			{
				m_MouseX = Input.mousePosition.x / Screen.width;
#if UNITY_5
				m_MouseY = 1f - Input.mousePosition.y / Screen.height;
#else
				m_MouseY = Input.mousePosition.y / Screen.height;
#endif
			}
		}
	}
	void OnGUI ()
	{
		GUI.Box (new Rect (10, 10, 200, 25), "Black Hole Demo");
	}
}