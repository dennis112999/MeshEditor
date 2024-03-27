using UnityEditor;
using UnityEngine;

namespace MeshEditor
{
    /// <summary>
    /// メッシュインスペクター
    /// </summary>
    [CustomEditor(typeof(MeshEdit))]
    public class MeshInspector : Editor
    {
        private MeshEdit _mesh;
        private Transform _handleTran;

        private void OnSceneGUI()
        {
            EditMesh();
        }

        /// <summary>
        /// メッシュ編集
        /// </summary>
        private void EditMesh()
        {
            _mesh = target as MeshEdit;

            _handleTran = _mesh.transform;
            for (int i = 0; i < _mesh.vertices.Length; i++)
            {
                ShowPoint(i);
            }
        }

        /// <summary>
        /// 頂点を表示する
        /// </summary>
        /// <param name="index">頂点索引</param>
        private void ShowPoint(int index)
        {
            if (_mesh.moveVertexPoint)
            {
                // Unityエディターでポイントを生成する
                Vector3 point = _handleTran.TransformPoint(_mesh.vertices[index]);
                Handles.color = _mesh.handleColor;
                point = Handles.FreeMoveHandle(point, _mesh.handleSize,　Vector3.zero, Handles.DotHandleCap);

                // GUI上で何か変更があった場合
                if (GUI.changed)
                {
                    _mesh.DoAction(index, _handleTran.InverseTransformPoint(point));
                }
            }
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            _mesh = target as MeshEdit;

            if (GUILayout.Button("Reset"))
            {
                _mesh.ResetMesh();
            }

            if (GUILayout.Button("Save"))
            {
                _mesh.EditMesh();
            }
        }
    }
}