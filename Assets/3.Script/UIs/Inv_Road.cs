using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Motorways.UI {
	public class Inv_Road : MonoBehaviour {
		[SerializeField] private TextMeshProUGUI Road_Text;

		public void ChangeRoadCount() {
			Road_Text.text = ResourceManager.Instance.GetCount(ItemType.Road).ToString();
		}
	}
}

