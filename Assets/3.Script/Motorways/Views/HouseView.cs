using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Motorways.Views {
	public class HouseView : MonoBehaviour {
		[SerializeField] private MeshRenderer HouseRoof;
		[SerializeField] private MeshRenderer HouseRoof2;

		public void UpdateColor(int groupIndex) {
			Color color = GroupColor.GetGroupColor(groupIndex);
			HouseRoof.material.color = color;
			HouseRoof2.material.color = color;
		}
	}
}
