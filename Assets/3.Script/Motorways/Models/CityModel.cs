using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Motorways.Models {
	public static class CityModel {
		public static int LatestLaneChangeFrame = -1;
		public static HashSet<Lane> ChangedLanes = new HashSet<Lane>();
	}
}

