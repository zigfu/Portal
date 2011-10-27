using UnityEngine;
using System.Collections;
using System.Linq;

public class VisualizerGradient2 : MonoBehaviour
{

    public int Width = 128;
    public int Height = 128;
    [System.Serializable]
    public class GradientColor
    {
        public float position;
        public Color color;
        public GradientColor(float position, Color color)
        {
            this.position = position;
            this.color = color;
        }
        public GradientColor() : this(0.0f, Color.black) { }
    }
    //NOTE: assumed that the points are sorted according to the intensity field
    public GradientColor[] Scale = { new GradientColor(0.0f, Color.black), new GradientColor(1.0f, Color.white) };


    //public Material Target;
    //public string TargetTexture;
    public Renderer Target;
    Color GetIntensityForPosition(float position, GradientColor[] array)
    {
        ////TODO: clamp positions between 0.0 to 1.0

        float preposition = 0.0f;
        Color precolor = Color.black;

        float postposition = 0.0f;
        Color postcolor = Color.white;

        foreach (var pt in array) {
            if (pt.position >= position) {
                postposition = pt.position;
                postcolor = pt.color;
                break;
            }
            else {
                preposition = pt.position;
                precolor = pt.color;
            }
        }

        float length = postposition - preposition;
        float offset = position - preposition;
        return Color.Lerp(precolor, postcolor, offset / length);

        // find the minimal item that is larger than the current item
        //TODO: finish bicubic interpolation
        //int largerThanCurrentIndex = Enumerable.Range(0, array.Length).Where((int i) => (array[i] >= position)).Min();
    }
    // Use this for initialization
    void Start()
    {
        Texture2D gradientTexture = new Texture2D(Width, Height);
        Color[] pixels = gradientTexture.GetPixels();

        for (int y = 0; y < Height / 2; y++) {
            for (int x = 0; x < Width / 2; x++) {
                float xPos = Mathf.Pow(1.0f - ((float)x) / (Width / 2), 3.0f);
                float yPos = Mathf.Pow(1.0f - ((float)y) / (Height / 2), 0.25f);

                float finalIntensity = 1.0f - Mathf.Sqrt(xPos * xPos + yPos * yPos) / (Mathf.Sqrt(2));//xPos * xPos * yPos * yPos;//xPos * yPos;

                float position = Mathf.Clamp01(finalIntensity);
                Color result = GetIntensityForPosition(position, Scale);

                // symmetry around the center of the texture
                pixels[x + y * Width] = pixels[(Width - 1 - x) + y * Width] =
                    pixels[x + (Height - 1 - y) * Width] = pixels[(Width - 1 - x) + (Height - 1 - y) * Width] = result;
            }
        }
        gradientTexture.SetPixels(pixels);
        gradientTexture.Apply(true);
        gradientTexture.wrapMode = TextureWrapMode.Clamp;
        //Target.SetTexture(TargetTexture, gradientTexture);
        Target.material.mainTexture = gradientTexture;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
