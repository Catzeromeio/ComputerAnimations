using System.Linq;
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

    MeshRenderer m_MeshRenderer;
    MeshRenderer MeshRenderer
    {
        get
        {
            if (m_MeshRenderer == null)
                m_MeshRenderer = GetComponent<MeshRenderer>();

            return m_MeshRenderer;
        }
    }


    public float boneLength = 1;
    public float boneHeight = 1;
    public float boneWidth = 1;

    public Transform bone0;
    public Transform bone1;

    Vector2[] m_Weights;
    Vector3[] m_VerticesUnderBone0;
    Vector3[] m_VerticesUnderBone1;

    void ApplyBindPoseAndFillVerticesUnderBones()
    {
        //模型坐标系下
        Vector3[] verticies =
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

        bone0.parent = transform;
        bone0.transform.localPosition = new Vector3(-boneLength, 0, 0);
        bone0.transform.localRotation = Quaternion.identity;
        bone0.transform.localScale = Vector3.one;

        bone1.parent = transform;
        bone1.transform.localPosition = new Vector3(0, 0, 0);
        bone1.transform.localRotation = Quaternion.identity;
        bone1.transform.localScale = Vector3.one;

        m_VerticesUnderBone0 = new Vector3[verticies.Length];
        m_VerticesUnderBone1 = new Vector3[verticies.Length];
        for (int i = 0; i < verticies.Length; i++)
        {
            m_VerticesUnderBone0[i] = bone0.InverseTransformPoint(transform.TransformPoint(verticies[i]));
            m_VerticesUnderBone1[i] = bone1.InverseTransformPoint(transform.TransformPoint(verticies[i]));
        }

        Vector2[] weights =
        {
            new Vector2(1, 0),
            new Vector2(1, 0),
            new Vector2(1, 0),
            new Vector2(1, 0),

            new Vector2(0.5f, 0.5f),
            new Vector2(0.5f, 0.5f),
            new Vector2(0.5f, 0.5f),
            new Vector2(0.5f, 0.5f),

            new Vector2( 0,1),
            new Vector2( 0,1),
            new Vector2( 0, 1),
            new Vector2( 0, 1),
        };
        m_Weights = weights;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ApplyBindPoseAndFillVerticesUnderBones();

    }

    // Update is called once per frame
    void Update()
    {
        Vector3[] v_bone0 = new Vector3[m_VerticesUnderBone0.Length];
        for (int i = 0; i < m_VerticesUnderBone0.Length; i++)
        {
            v_bone0[i] = transform.InverseTransformPoint(bone0.TransformPoint(m_VerticesUnderBone0[i]));
        }

        Vector3[] v_bone1 = new Vector3[m_VerticesUnderBone1.Length];
        for (int i = 0; i < m_VerticesUnderBone1.Length; i++)
        {
            v_bone1[i] = transform.InverseTransformPoint(bone1.TransformPoint(m_VerticesUnderBone1[i]));
        }


        Vector3[] vetices = new Vector3[m_VerticesUnderBone0.Length];
        for (int i = 0; i < vetices.Length; i++)
        {
            vetices[i] = Vector3.Lerp(v_bone0[i], v_bone1[i], m_Weights[i].x);
        }

        MeshFilter.mesh.vertices = vetices;

        //看向三角形时，顺时针
        int[] triangles =
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
        MeshFilter.mesh.triangles = triangles;

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
    }
}
