using Unity.Collections;
using UnityEngine;
using UnityEngine.UIElements;

// adapted from: https://docs.unity3d.com/6000.0/Documentation/Manual/UIE-radial-progress.html
public class EllipseMesh
{
    int _numSteps;
    float _width;
    float _height;
    Color _color;
    float _borderSize;
    bool _useSegmentedColors;

    bool _isDirty;
    public bool isDirty => _isDirty;
    public Vertex[] vertices { get; private set; }
    public ushort[] indices { get; private set; }

    public int numSteps
    {
        get => _numSteps;
        set
        {
            _isDirty = value != _numSteps;
            _numSteps = value;
        }
    }

    public float width
    {
        get => _width;
        set => CompareAndWrite(ref _width, value);
    }

    public float height
    {
        get => _height;
        set => CompareAndWrite(ref _height, value);
    }

    public Color color
    {
        get => _color;
        set
        {
            _isDirty = value != _color;
            _color = value;
        }
    }

    public float borderSize
    {
        get => _borderSize;
        set => CompareAndWrite(ref _borderSize, value);
    }

    public bool useSegmentedColors
    {
        get => _useSegmentedColors;
        set
        {
            _isDirty = value != _useSegmentedColors;
            _useSegmentedColors = value;
        }
    }

    public EllipseMesh(int numSteps)
    {
        this.numSteps = numSteps;
        _isDirty = true;
    }

    void CompareAndWrite(ref float field, float newValue)
    {
        if (Mathf.Abs(field - newValue) > float.Epsilon)
        {
            _isDirty = true;
            field = newValue;
        }
    }

    public void UpdateMesh()
    {
        if (!_isDirty)
            return;
        
        int numVertices = numSteps * 2;
        int numIndices = numVertices * 6;

        if (vertices == null || vertices.Length != numVertices)
            vertices = new Vertex[numVertices];

        if (indices == null || indices.Length != numIndices)
            indices = new ushort[numIndices];

        float stepSize = 360.0f / (float)numSteps;
        float angle = -180.0f;

        for (int i = 0; i < numSteps; ++i)
        {
            angle -= stepSize;
            float radians = Mathf.Deg2Rad * angle;

            Color segmentColor;
            if (_useSegmentedColors)
            {
                float normalizedAngle = angle;
                while (normalizedAngle < 0)
                    normalizedAngle += 360f;
                while (normalizedAngle >= 360f)
                    normalizedAngle -= 360f;

                if (normalizedAngle >= 315f || normalizedAngle < 45f)
                    segmentColor = Color.white;
                else if (normalizedAngle >= 45f && normalizedAngle < 135f)
                    segmentColor = Color.yellow;
                else if (normalizedAngle >= 135f && normalizedAngle < 225f)
                    segmentColor = Color.red;
                else
                    segmentColor = Color.black;
            }
            else
            {
                segmentColor = _color;
            }

            float outerX = Mathf.Sin(radians) * _width;
            float outerY = Mathf.Cos(radians) * _height;
            Vertex outerVertex = new Vertex();
            outerVertex.position = new Vector3(_width + outerX, _height + outerY, Vertex.nearZ);
            outerVertex.tint = segmentColor;
            vertices[i * 2] = outerVertex;

            float innerX = Mathf.Sin(radians) * (_width - _borderSize);
            float innerY = Mathf.Cos(radians) * (_height - _borderSize);
            Vertex innerVertex = new Vertex();
            innerVertex.position = new Vector3(_width + innerX, _height + innerY, Vertex.nearZ);
            innerVertex.tint = segmentColor;
            vertices[i * 2 + 1] = innerVertex;

            indices[i * 6] = (ushort)(i == 0 ? vertices.Length - 2 : (i - 1) * 2); // previous outer vertex
            indices[i * 6 + 1] = (ushort)(i * 2); // current outer vertex
            indices[i * 6 + 2] = (ushort)(i * 2 + 1); // current inner vertex

            indices[i * 6 + 3] = (ushort)(i == 0 ? vertices.Length - 2 : (i - 1) * 2); // previous outer vertex
            indices[i * 6 + 4] = (ushort)(i * 2 + 1); // current inner vertex
            indices[i * 6 + 5] = (ushort)(i == 0 ? vertices.Length - 1 : (i - 1) * 2 + 1);
        }
        _isDirty = false;
    }
}


[UxmlElement]
public partial class UIProgressRadial : UIComponent
{

    // base class names for controll overall & the label
    public static readonly string ussClassName = "radial-progress";
    public static readonly string ussLabelClassName = "radial-progress-label";

    static CustomStyleProperty<Color> _trackColor = new CustomStyleProperty<Color>("--track-color");
    static CustomStyleProperty<Color> _progressColor = new CustomStyleProperty<Color>("--progress-color");
    static CustomStyleProperty<float> _trackWidth = new CustomStyleProperty<float>("--track-width");
    static CustomStyleProperty<float> _progressWidth = new CustomStyleProperty<float>("--progress-width");

    EllipseMesh _trackMesh;
    EllipseMesh _progressMesh;
    Label _label;
    float _progress;
    const int kNumSteps = 200;

    [UxmlAttribute]
    public float progress
    {
        get => _progress;
        set
        {
            _progress = value;
            MarkDirtyRepaint();
        }
    }

    int _labelValue;

    [UxmlAttribute]
    public int labelValue
    {
        get => _labelValue;
        set
        {
            _labelValue = value;
            _label.text = _labelValue.ToString();
            MarkDirtyRepaint();
        }
    }

    public UIProgressRadial()
    {
        _label = new Label();
        _label.AddToClassList(ussLabelClassName);
        Add(_label);

        _progressMesh = new EllipseMesh(kNumSteps);
        _trackMesh = new EllipseMesh(kNumSteps);

        AddToClassList(ussClassName);
        RegisterCallback<CustomStyleResolvedEvent>(e => CustomStylesResolved(e));
        generateVisualContent += context => GenerateVisualContent(context);
        progress = 0.0f;
        _progressMesh.borderSize = 10;
        _trackMesh.borderSize = 10;
    }

    static void CustomStylesResolved(CustomStyleResolvedEvent e)
    {
        UIProgressRadial element = (UIProgressRadial)e.currentTarget;
        element.UpdateStyles();
    }

    void UpdateStyles()
    {
        if (customStyle.TryGetValue(_progressColor, out var progressColor))
            _progressMesh.color = progressColor;
        if (customStyle.TryGetValue(_trackColor, out var trackColor))
            _trackMesh.color = trackColor;
        if (customStyle.TryGetValue(_trackWidth, out var trackWidth))
            _trackMesh.borderSize = trackWidth;
        if (customStyle.TryGetValue(_progressWidth, out var progressWidth))
            _progressMesh.borderSize = progressWidth;

        if (_progressMesh.isDirty || _trackMesh.isDirty)
            MarkDirtyRepaint();
    }

    // The GenerateVisualContent() callback method calls DrawMeshes()
    static void GenerateVisualContent(MeshGenerationContext context)
    {
        UIProgressRadial element = (UIProgressRadial)context.visualElement;
        element.DrawMeshes(context);
    }

    void DrawMeshes(MeshGenerationContext context)
    {
        float halfWidth = contentRect.width * 0.5f;
        float halfHeight = contentRect.height * 0.5f;

        if (halfWidth < 2.0f || halfHeight < 2.0f)
            return;

        _progressMesh.width = halfWidth;
        _progressMesh.height = halfWidth;
        _progressMesh.useSegmentedColors = true;
        _progressMesh.UpdateMesh();

        _trackMesh.width = halfWidth;
        _trackMesh.height =  halfHeight;
        _trackMesh.UpdateMesh();

        // Draw track mesh first
        var trackMeshWriteData = context.Allocate(_trackMesh.vertices.Length, _trackMesh.indices.Length);
        trackMeshWriteData.SetAllVertices(_trackMesh.vertices);
        trackMeshWriteData.SetAllIndices(_trackMesh.indices);

        float clampedProgress = Mathf.Clamp(_progress * 100, 0f, 100f);

        // Determine how many triangles are used to depending on progress, to achieve a partially filled circle
        int sliceSize = Mathf.FloorToInt((kNumSteps * clampedProgress) / 100f);

        if (sliceSize == 0)
            return;
        
        // Every step is 6 indices in the corresponding array.
        sliceSize *= 6;

        // Draw progress mesh
        var progressMeshWriteData = context.Allocate(_progressMesh.vertices.Length, sliceSize);
        progressMeshWriteData.SetAllVertices(_progressMesh.vertices);

        var tempIndicesArray = new NativeArray<ushort>(_progressMesh.indices, Allocator.Temp);
        progressMeshWriteData.SetAllIndices(tempIndicesArray.Slice(0, sliceSize));
        tempIndicesArray.Dispose();
    }
}
