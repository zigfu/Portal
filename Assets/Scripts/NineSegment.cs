using UnityEngine;
using System.Collections;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
public class NineSegment : MonoBehaviour {

    public Texture2D Texture;
    public int[] BorderSizes = new int[4]; // left, top, right, bottom
    public Vector2 Size = new Vector2(1,1);
    Transform TopLeft, TopRight, TopCenter;
    Mesh data;
	// Use this for initialization
	void Start () {
        data = new Mesh();

        GenerateVertices();

        GenerateUVs();

        GenerateIndices();
        data.RecalculateNormals();

        GetComponent<MeshFilter>().mesh = data;
        renderer.material.mainTexture = Texture;
		//TODO: hack
		renderer.material.shader = Shader.Find("Transparent/Diffuse");
	}

    private void GenerateIndices()
    {
        int[] indices = new int[9 * 2 * 3]; // 9 quads, each composed of two triangles
        for (int y = 0; y < 3; y++) {
            for (int x = 0; x < 3; x++) {
                // top left part of quad
                indices[(x + y * 3) * 6 + 0] = x + y * 4 + 0;
                indices[(x + y * 3) * 6 + 1] = x + (y + 1) * 4 + 0;
                indices[(x + y * 3) * 6 + 2] = x + 1 + (y + 1) * 4 + 0;
                // bottom right part
                indices[(x + y * 3) * 6 + 3] = x + y * 4 + 0;
                indices[(x + y * 3) * 6 + 4] = x + 1 + (y + 1) * 4 + 0;
                indices[(x + y * 3) * 6 + 5] = x + 1 + y * 4 + 0;
            }
        }
        data.triangles = indices;
    }

    private void GenerateUVs()
    {
        int width = Texture.width;
        int height = Texture.height;

        float[] borderRatios = new float[] {
            ((float)BorderSizes[0]) / width, // left
            ((float)BorderSizes[1]) / height, // height
            ((float)BorderSizes[2]) / width, // right
            ((float)BorderSizes[3]) / height, // bottom
        };
        float[] us = new float[] {
            0, //leftmost
            borderRatios[0], // end of left border
            1.0f - borderRatios[2], // start of right border
            1.0f
        };
        // same logic but bottom up (notice flipped Y - smaller is lower Y)
        float[] vs = new float[] {
            0,
            borderRatios[3],
            1.0f - borderRatios[1],
            1.0f
        };

        Vector2[] uvs = new Vector2[16];

        for (int y = 0; y < 4; y++) {
            for (int x = 0; x < 4; x++) {
                uvs[x + y * 4] = new Vector2(us[x], vs[y]);
            }
        }
        data.uv = uvs;
    }
	
	// Update is called once per frame
	void Update () {
        GenerateVertices(); // update in case sizes were changed in runtime
	}

    private void GenerateVertices()
    {
        int width = Texture.width;
        int height = Texture.height;

        float[] borderRatios = new float[] {
            ((float)BorderSizes[0]) / width, // left
            ((float)BorderSizes[1]) / height, // height
            ((float)BorderSizes[2]) / width, // right
            ((float)BorderSizes[3]) / height, // bottom
        };
        float[] xs = new float[] {
            Size.x * (-0.5f), //leftmost
            Size.x * (-0.5f) + borderRatios[0], // end of left border
            Size.x * 0.5f - borderRatios[2], // start of right border
            Size.x * 0.5f
        };
        // same logic but bottom up (notice flipped Y - smaller is lower Y)
        float[] ys = new float[] {
            Size.y * (-0.5f),
            Size.y * (-0.5f) + borderRatios[3],
            Size.y * 0.5f - borderRatios[1],
            Size.y * 0.5f
        };


        Vector3[] vertices = new Vector3[16];

        for (int y = 0; y < 4; y++) {
            for (int x = 0; x < 4; x++) {
                vertices[x + y * 4] = new Vector3(xs[x], ys[y]);
            }
        }
        data.vertices = vertices;
    }
}
