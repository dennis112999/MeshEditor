using System.Collections.Generic;
using UnityEngine;

namespace MeshEditor
{
    /// <summary>
    /// メッシュ編集
    /// </summary>
    [ExecuteInEditMode]
    public class MeshEdit : MonoBehaviour
    {
        private Mesh _originalMesh;
        private Mesh _clonedMesh;
        private MeshFilter _meshFilter;
        private MeshCollider _meshCollider;

        private int[] triangles;

        [HideInInspector]
        public Vector3[] vertices;

        [Header("エディター専用")]
        [Space(5)]
        [Tooltip("ハンドルサイズ")]
        public float handleSize = 0.03f;
        [Tooltip("ハンドルの色")]
        public Color handleColor;

        [HideInInspector]
        public bool moveVertexPoint = true;

        void Start()
        {
            Init();

            InitMesh();
        }

        private void Init()
        {
            _meshFilter = GetComponent<MeshFilter>();
            _meshCollider = GetComponent<MeshCollider>();
        }

        /// <summary>
        /// 初期化メッシュ
        /// </summary>
        private void InitMesh()
        {
            // 既存のメッシュを保存する
            _originalMesh = _meshFilter.sharedMesh;

            // 新しいメッシュを生成し、既存のメッシュデータを読み込む
            _clonedMesh = new Mesh();
            _clonedMesh.name = "clone";
            _clonedMesh.vertices = _originalMesh.vertices;
            _clonedMesh.triangles = _originalMesh.triangles;
            _clonedMesh.normals = _originalMesh.normals;
            _clonedMesh.uv = _originalMesh.uv;
            _meshFilter.mesh = _clonedMesh;

            vertices = _clonedMesh.vertices;
            triangles = _clonedMesh.triangles;

            if (_meshCollider != null)
            {
                // 新しいメッシュをメッシュコライダーに使用する
                _meshCollider.sharedMesh = _clonedMesh;
            }
            else
            {
                Debug.LogError("Missing MeshCollider Component");
            }
        }

        /// <summary>
        /// 何が動いたら、アクションする
        /// </summary>
        /// <param name="index">動いた頂点索引</param>
        /// <param name="localPos">動いた頂点の新しいローカル座標</param>
        public void DoAction(int index, Vector3 localPos)
        {
            PullSimilarVertices(index, localPos);
        }

        /// <summary>
        /// 関連頂点を探す
        /// </summary>
        /// <param name="targetPt">目標頂点</param>
        /// <param name="findConnected"></param>
        /// <returns></returns>
        private List<int> FindRelatedVertices(Vector3 targetPt)
        {
            // 見つかった頂点の頂点索引を記録する
            List<int> relatedVertices = new List<int>();

            int idx = 0;
            Vector3 pos;

            // メッシュ内の全てのトライアングルをループする
            for (int t = 0; t < triangles.Length; t++)
            {
                idx = triangles[t];
                pos = vertices[idx];

                // 現在の位置が目標頂点と同じ場合
                if (pos == targetPt)
                {
                    relatedVertices.Add(idx);
                }
            }

            return relatedVertices;
        }

        /// <summary>
        /// 似ている頂点を引き寄せる
        /// </summary>
        /// <param name="index">変更した頂点の索引</param>
        /// <param name="newPos">頂点の新しい座標</param>
        private void PullSimilarVertices(int index, Vector3 newPos)
        {
            Vector3 targetVertexPos = vertices[index];
            List<int> relatedVertices = FindRelatedVertices(targetVertexPos);
            foreach (int i in relatedVertices)
            {
                vertices[i] = newPos;
            }

            // メッシュのノーマルを再計算する
            _clonedMesh.vertices = vertices;
            _clonedMesh.RecalculateNormals();

            if (_meshCollider != null)
            {
                _meshCollider.sharedMesh = null;
                _meshCollider.sharedMesh = _clonedMesh;
            }
        }

        /// <summary>
        /// リセットメッシュ
        /// </summary>
        public void ResetMesh()
        {
            if (_clonedMesh != null && _originalMesh != null)
            {
                _clonedMesh.vertices = _originalMesh.vertices;
                _clonedMesh.triangles = _originalMesh.triangles;
                _clonedMesh.normals = _originalMesh.normals;
                _clonedMesh.uv = _originalMesh.uv;
                _meshFilter.mesh = _clonedMesh;

                vertices = _clonedMesh.vertices;
                triangles = _clonedMesh.triangles;

                _meshCollider.sharedMesh = _originalMesh;
            }
        }

        /// <summary>
        /// 新しいメッシュ保存
        /// </summary>
        public void EditMesh()
        {
            _originalMesh = _clonedMesh;
            InitMesh();
        }
    }
}
