using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Motorways.Visuals {
    public enum PathType {
        Smooth, // 부드러운 베지어 곡선
        Sharp   // 45도, 90도 등 날카로운 직선 꺾임
    }

    public class RoadVisualPath {
        public List<Vector3> VisualPoints { get; private set; } = new List<Vector3>();
        public bool IsDeadEnd { get; set; } = false;

        public RoadTileNode StartNode { get; set; }
        public RoadTileNode EndNode { get; set; }

        public void AddPoint(Vector3 point) { VisualPoints.Add(point); }
    }
}