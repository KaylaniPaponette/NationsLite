using UnityEngine;
using UnityEngine.UIElements;

public enum ProgressDirection
{
    Vertical,
    Horizontal
}

public class ProgressMesh
{
    float _width;
    float _height;
    float _fillAmount;
    Color _tint;
    ProgressDirection _direction;
    bool _isDirty;

    public bool isDirty => _isDirty;

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
    
    public float fillAmount
    {
        get => _fillAmount;
        set => CompareAndWrite(ref _fillAmount, Mathf.Clamp01(value));
    }

    public Color tint
    {
        get => _tint;
        set
        {
            _isDirty = value != _tint;
            _tint = value;
        }
    }

    public ProgressDirection direction
    {
        get => _direction;
        set
        {
            _isDirty = value != _direction;
            _direction = value;
        }
    }

    void CompareAndWrite(ref float field, float newValue)
    {
        if (Mathf.Abs(field - newValue) > float.Epsilon)
        {
            _isDirty = true;
            field = newValue;
        }
    }

    public Vertex[] vertices { get; private set; }
    public ushort[] indices { get; private set; }

    public ProgressMesh(ProgressDirection direction)
    {
        _direction = direction;
        _fillAmount = 1f;
        _isDirty = true;
    }

    public void UpdateMesh()
    {
        if (!_isDirty)
            return;

        if (vertices == null || vertices.Length != 4)
            vertices = new Vertex[4];
        if (indices == null || indices.Length != 6)
            indices = new ushort[6];

        if (direction == ProgressDirection.Horizontal)
        {
            float right = width * fillAmount;
            vertices[0] = new Vertex { position = new Vector3(0, 0, Vertex.nearZ), tint = this.tint };
            vertices[1] = new Vertex { position = new Vector3(right, 0, Vertex.nearZ), tint = this.tint };
            vertices[2] = new Vertex { position = new Vector3(0, height, Vertex.nearZ), tint = this.tint };
            vertices[3] = new Vertex { position = new Vector3(right, height, Vertex.nearZ), tint = this.tint };
        }
        else if (direction == ProgressDirection.Vertical)
        {
            float top = height * fillAmount;
            vertices[0] = new Vertex { position = new Vector3(0, 0, Vertex.nearZ), tint = this.tint };
            vertices[1] = new Vertex { position = new Vector3(width, 0, Vertex.nearZ), tint = this.tint };
            vertices[2] = new Vertex { position = new Vector3(0, top, Vertex.nearZ), tint = this.tint };
            vertices[3] = new Vertex { position = new Vector3(width, top, Vertex.nearZ), tint = this.tint };
        }

        indices[0] = 1;
        indices[1] = 2;
        indices[2] = 0;
        indices[3] = 1;
        indices[4] = 3;
        indices[5] = 2;

        _isDirty = false;
    }

}

[UxmlElement]
public partial class UIProgress : UIComponent
{
    public static readonly string ussClassName = "--progress-bar";
    static CustomStyleProperty<Color> _trackColor = new CustomStyleProperty<Color>("--progress-bar__track-color");
    static CustomStyleProperty<Color> _barColor = new CustomStyleProperty<Color>("--progress-bar__bar-color");

    ProgressMesh _trackMesh;
    ProgressMesh _progressMesh;
    ProgressDirection _direction;
    float _progress;

    [UxmlAttribute]
    public ProgressDirection direction
    {
        get => _direction;
        set
        {
            _direction = value;
            _trackMesh.direction = value;
            _progressMesh.direction = value;
            MarkDirtyRepaint();
        }
    }

    [UxmlAttribute, Range(0, 1)]
    public float progress
    {
        get => _progress;
        set
        {
            _progress = value;
            _progressMesh.fillAmount = Mathf.Clamp01(value);
            MarkDirtyRepaint();
        }
    }

    public UIProgress()
    {
        _progressMesh = new ProgressMesh(_direction);
        _trackMesh = new ProgressMesh(_direction);
        AddToClassList(ussClassName);
        RegisterCallback<CustomStyleResolvedEvent>(e => OnResolveCustomStyles(e));
        generateVisualContent += context => OnGenerateVisualContent(context);

        progress = 0.2f;
        _trackMesh.tint = Color.white;
        _progressMesh.tint = Color.red;
    }

    void OnResolveCustomStyles(CustomStyleResolvedEvent e)
    {
        UIProgress progressBar = (UIProgress)e.currentTarget;
        progressBar.UpdateStyles();
    }

    void UpdateStyles()
    {
        if (customStyle.TryGetValue(_trackColor, out var trackColor))
            _trackMesh.tint = trackColor;

        if (customStyle.TryGetValue(_barColor, out var barColor))
            _progressMesh.tint = barColor;
        
        if (_progressMesh.isDirty || _trackMesh.isDirty)
            MarkDirtyRepaint();
    }

    void OnGenerateVisualContent(MeshGenerationContext context)
    {
        UIProgress element = (UIProgress)context.visualElement;
        element.DrawMeshes(context);
    }

    void DrawMeshes(MeshGenerationContext context)
    {
        _trackMesh.width = contentRect.width;
        _trackMesh.height = contentRect.height;
        _trackMesh.fillAmount = 1f;
        _trackMesh.UpdateMesh();

        var trackMeshWriteData = context.Allocate(_trackMesh.vertices.Length, _trackMesh.indices.Length);
        trackMeshWriteData.SetAllVertices(_trackMesh.vertices);
        trackMeshWriteData.SetAllIndices(_trackMesh.indices);

        _progressMesh.width = contentRect.width;
        _progressMesh.height = contentRect.height;
        _progressMesh.UpdateMesh();

        var progressMeshWriteData = context.Allocate(_progressMesh.vertices.Length, _progressMesh.indices.Length);
        progressMeshWriteData.SetAllVertices(_progressMesh.vertices);
        progressMeshWriteData.SetAllIndices(_progressMesh.indices);
    }
}
