using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]

public class SmoothSkinning : MonoBehaviour
{
    MeshFilter m_MeshFilter;
    MeshFilter MeshFilter
    {
        get
        {
            if (m_MeshFilter == null)
                m_MeshFilter = GetComponent<MeshFilter>();

            return m_MeshFilter;
        }
    }

    public float boneLength = 1;
    public float boneHeight = 1;
    public float boneWidth = 1;

    public Transform bone0;
    Matrix4x4 trs_bone0_to_root_bind;
    public Transform bone1;
    Matrix4x4 trs_bone1_to_root_bind;

    Vector3[] m_Verticies;
    Vector2[] m_Weights;
    int[] m_Triangles;

    void SetUp()
    {
        //模型坐标系下
        Vector3[] theVerticies =
        {
            new Vector3(-boneLength, -boneHeight / 2, -boneWidth / 2),
            new Vector3(-boneLength, boneHeight / 2, -boneWidth / 2),
            new Vector3(-boneLength, boneHeight / 2, boneWidth / 2),
            new Vector3(-boneLength, -boneHeight / 2, boneWidth / 2),

            new Vector3(0, -boneHeight / 2, -boneWidth / 2),
            new Vector3(0, boneHeight / 2, -boneWidth / 2),
            new Vector3(0, boneHeight / 2, boneWidth / 2),
            new Vector3(0, -boneHeight / 2, boneWidth / 2),

            new Vector3(boneLength, -boneHeight / 2, -boneWidth / 2),
            new Vector3(boneLength, boneHeight / 2, -boneWidth / 2),
            new Vector3(boneLength, boneHeight / 2, boneWidth / 2),
            new Vector3(boneLength, -boneHeight / 2, boneWidth / 2),
        };

        Mesh mesh = new Mesh();
        mesh.vertices = theVerticies;
        MeshFilter.mesh = mesh;

        //看向三角形时，顺时针
        int[] theTriangles =
        {
             0,3,1,
             3,2,1,

             0,1,5,
             0,5,4,

             1,2,6,
             1,6,5,

             3,6,2,
             3,7,6,

             0,7,3,
             0,4,7,

             4,5,9,
             4,9,8,

             5,6,10,
             5,10,9,

             7,10,6,
             7,11,10,

             4,11,7,
             4,8,11,

             11,9,10,
             11,8,9
        };
        MeshFilter.mesh.triangles = theTriangles;

        //Vector3[] normals =
        //{
        //    new Vector3(-1, -1, -1).normalized,
        //    new Vector3(-1, 1, -1).normalized,
        //    new Vector3(-1, 1, 1).normalized,
        //    new Vector3(-1, -1, 1).normalized,

        //    new Vector3(0, -1, -1).normalized,
        //    new Vector3(0, 1, -1).normalized,
        //    new Vector3(0, 1, 1).normalized,
        //    new Vector3(0, -1, 1).normalized,

        //    new Vector3(1, -1, -1).normalized,
        //    new Vector3(1, 1, -1).normalized,
        //    new Vector3(1, 1, 1).normalized,
        //    new Vector3(1, -1, 1).normalized
        //};

        bone0.parent = transform;
        bone0.transform.localPosition = new Vector3(-boneLength, 0, 0);
        bone0.transform.localRotation = Quaternion.identity;
        bone0.transform.localScale = Vector3.one;
        trs_bone0_to_root_bind = TRS(bone0);

        bone1.parent = transform;
        bone1.transform.localPosition = new Vector3(0, 0, 0);
        bone1.transform.localRotation = Quaternion.identity;
        bone1.transform.localScale = Vector3.one;
        trs_bone1_to_root_bind = TRS(bone1);

        m_Verticies = theVerticies;
        m_Triangles = theTriangles;

        m_Weights = new Vector2[12];
        m_Weights[0] = new Vector2(1, 0);
        m_Weights[1] = new Vector2(1, 0);
        m_Weights[2] = new Vector2(1, 0);
        m_Weights[3] = new Vector2(1, 0);
        m_Weights[4] = new Vector2(0.5f, 0.5f);
        m_Weights[5] = new Vector2(0.5f, 0.5f);
        m_Weights[6] = new Vector2(0.5f, 0.5f);
        m_Weights[7] = new Vector2(0.5f, 0.5f);
        m_Weights[8] = new Vector2(0, 1);
        m_Weights[9] = new Vector2(0, 1);
        m_Weights[10] = new Vector2(0, 1);
        m_Weights[11] = new Vector2(0, 1);
    }

    Matrix4x4 TRS(Transform trans)
    {
        return Matrix4x4.Translate(trans.localPosition) * Matrix4x4.Rotate(trans.localRotation) * Matrix4x4.Scale(trans.localScale);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SetUp();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3[] results = new Vector3[m_Verticies.Length];
        for (int i = 0; i < m_Verticies.Length; i++)
        {
            var lv_0 = trs_bone0_to_root_bind.inverse * (new Vector4(m_Verticies[i].x, m_Verticies[i].y, m_Verticies[i].z, 1));
            var v_0 = TRS(bone0) * lv_0;

            var lv_1 = trs_bone1_to_root_bind.inverse * (new Vector4(m_Verticies[i].x, m_Verticies[i].y, m_Verticies[i].z, 1));
            var v_1 = TRS(bone1) * lv_1;

            results[i] = Vector3.Lerp(v_0, v_1, m_Weights[i].y);
        }

        MeshFilter.mesh.vertices = results;
    }
}
