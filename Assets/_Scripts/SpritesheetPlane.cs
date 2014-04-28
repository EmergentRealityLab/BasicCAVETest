using UnityEngine;
using System.Collections;

public enum FrameOrder { LeftToRight, TopToBottom };
public enum BillboardMode { None, YAxis, FaceCamera };

[ExecuteInEditMode]
public class SpritesheetPlane : MonoBehaviour
{
	Mesh plane;
	public int rowCount = 1;
	public int columnCount = 1;
	public float framesPerSecond = 30f;
	public int currentFrame = 0;
	public bool animating = false;
	public FrameOrder order = FrameOrder.LeftToRight;
	public BillboardMode billboardMode = BillboardMode.YAxis;
	public bool flipX = false;
	public bool flipY = false;
	
	public Color vertexColor = Color.white;
	
	float timeInCurFrame = 0f;
	
	void Start ()
	{
		plane = new Mesh();
		Vector3[] vertices = new Vector3[] { 
			new Vector3(-0.5f, 0.5f, 0),
			new Vector3(0.5f, 0.5f, 0),
			new Vector3(-0.5f, -0.5f, 0),
			new Vector3(0.5f, -0.5f, 0)
		};
		Vector2[] uvs = new Vector2[] {
			new Vector2(0f, 1f),
			new Vector2(1f, 1f),
			new Vector2(0f, 0f),
			new Vector2(1f, 0f)
		};
		int[] triangles = new int[] {
			0, 2, 1, 1, 2, 3
		};
		Color[] colors = new Color[] {
			vertexColor, vertexColor, vertexColor, vertexColor
		};
		
		plane.vertices = vertices;
		plane.uv = uvs;
		plane.triangles = triangles;
		plane.colors = colors;
		plane.RecalculateNormals();
		
		MeshFilter filter = GetComponent<MeshFilter>();
		if (!filter)
			filter = gameObject.AddComponent<MeshFilter>();
		filter.mesh = plane;
	}
	
	void Update ()
	{
		if (animating)
		{
			float time = Time.deltaTime;
			if (time == 0f)
				time = 1f / 60f;
			timeInCurFrame += Time.deltaTime;
			float timePerFrame = 1f / framesPerSecond;
			
			int numFramesPassed = (int)(timeInCurFrame / timePerFrame);
			timeInCurFrame -= timePerFrame * numFramesPassed;
			currentFrame += numFramesPassed;
			
			currentFrame %= (rowCount * columnCount);
			
			float width = 1f / columnCount;
			float height = 1f / rowCount;
			
			int x, y, x2, y2;
			
			if (order == FrameOrder.LeftToRight)
			{
				x = currentFrame % columnCount;
				y = currentFrame / columnCount;
			}
			else
			{
				x = currentFrame / rowCount;
				y = currentFrame % rowCount;
			}
			x2 = x + 1;
			y2 = y + 1;
			if (flipX)
			{
				x2 ^= x;
				x ^= x2;
				x2 ^= x;
			}
			if (flipY)
			{
				y2 ^= y;
				y ^= y2;
				y2 ^= y;
			}
			
			Vector2[] uvs = new Vector2[] {
				new Vector2(width * x, height * y2),
				new Vector2(width * x2, height * y2),
				new Vector2(width * x, height * y),
				new Vector2(width * x2, height * y)
			};
			Color[] colors = new Color[] {
				vertexColor, vertexColor, vertexColor, vertexColor
			};
			plane.uv = uvs;
			plane.colors = colors;
			plane.RecalculateNormals();
		}
	}
	
	void OnWillRenderObject()
	{
		if (billboardMode != BillboardMode.None)
		{
			Vector3 cameraPos = Camera.current.transform.position;
			if (billboardMode == BillboardMode.YAxis)
				cameraPos.y = transform.position.y;
			
			Vector3 faceDir = cameraPos - transform.position;
			transform.forward = faceDir;
		}
	}
}
