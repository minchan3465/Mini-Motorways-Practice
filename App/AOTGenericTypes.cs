using System;
using System.Collections.Generic;
using Factory;
using FixMath;
using Motorways;
using Motorways.Models;
using Motorways.Processes;
using Server;
using UnityEngine;

// Token: 0x0200006B RID: 107
public static class AOTGenericTypes
{
	// Token: 0x060000B7 RID: 183 RVA: 0x0000388C File Offset: 0x00001A8C
	public static void DontCall_AOTWorkaround()
	{
		new SerializerLibrary.ListSerializer<IAppCommand>();
		new SerializerLibrary.DictionarySerializer<RoadTileSignature, RoadTileDefinition>();
		new SerializerLibrary.ListSerializer<RoadTileDefinition>();
		new SerializerLibrary.DictionarySerializer<RoadTileConnection, RoadTileConnectionStrokePath>();
		new SerializerLibrary.ListSerializer<RoadTileConnection>();
		new SerializerLibrary.ListSerializer<RoadTileNode>();
		new SerializerLibrary.DictionarySerializer<RoadTileConnection, RoadTilePath>();
		new SerializerLibrary.ArraySerializer<Vector2>();
		new SerializerLibrary.ListSerializer<RoadTilePath.Piece>();
		new SerializerLibrary.ListSerializer<Vector2Fixed>();
		new MeshSerializer();
		new SerializerLibrary.ListSerializer<Vector2>();
		new SerializerLibrary.ListSerializer<IModel>();
		new SerializerLibrary.DictionarySerializer<Type, List<IModel>>();
		new SerializerLibrary.ListSerializer<IProcess>();
		new SerializerLibrary.ListSerializer<Command>();
		new SerializerLibrary.ListSerializer<Vector2Int>();
		new SerializerLibrary.ArraySerializer<ChallengeData>();
		new SerializerLibrary.ListSerializer<Fix64>();
		new SerializerLibrary.ListSerializer<VehicleDispatchRecord>();
		new SerializerLibrary.ListSerializer<AdjacentTileConnection>();
		new SerializerLibrary.ListSerializer<Passage>();
		new SerializerLibrary.DictionarySerializer<Vector2Int, TileDirection>();
		new SerializerLibrary.ListSerializer<int>();
		new SerializerLibrary.ListSerializer<bool>();
		new SerializerLibrary.ListSerializer<DestinationModel>();
		new SerializerLibrary.ArraySerializer<bool>();
		new SerializerLibrary.ListSerializer<UpgradePackageDefinition>();
		new SerializerLibrary.ListSerializer<VehicleModel>();
		new SerializerLibrary.DictionarySerializer<TutorialIdentifier, int>();
		new ModelFrameSerializer();
		new SerializerLibrary.ListSerializer<CityPlanModel.ScheduledBuilding>();
		new SerializerLibrary.DictionarySerializer<int, Fix64>();
		new SerializerLibrary.ArraySerializer<int>();
		new SerializerLibrary.DictionarySerializer<int, int>();
		new SerializerLibrary.DictionarySerializer<int, TileMatrixInt>();
		new SerializerLibrary.ListSerializer<LaneModel>();
		new SerializerLibrary.DictionarySerializer<Vector2Int, TileModel>();
		new SerializerLibrary.DictionarySerializer<CornerAdjacencyReference, TileCornerModel>();
		new SerializerLibrary.DictionarySerializer<int, MotorwayModel>();
		new SerializerLibrary.ArraySerializer<RoadState>();
		new SerializerLibrary.ArraySerializer<Fix64>();
		new SerializerLibrary.ListSerializer<CornerAdjacencyReference>();
		new SerializerLibrary.ListSerializer<UpgradeChoice>();
		new SerializerLibrary.ListSerializer<ChallengeData>();
		new SerializerLibrary.ListSerializer<IntersectionEntryDecision>();
		new SerializerLibrary.DictionarySerializer<VehicleModel, List<IntersectionEntryDecision>>();
		new SerializerLibrary.ListSerializer<IntersectionEntryVehicleContext>();
		new SerializerLibrary.ListSerializer<TileModel>();
		new SerializerLibrary.ListSerializer<CarparkModel.ParkingSpace>();
		new SerializerLibrary.ListSerializer<RoadChunkModel.InboundVehicle>();
		new SerializerLibrary.ListSerializer<TileDirectionBitfield>();
		new SerializerLibrary.ListSerializer<TrainModel>();
		new SerializerLibrary.ListSerializer<RailTileModel>();
		new SerializerLibrary.ListSerializer<BoatModel>();
		new SerializerLibrary.ListSerializer<BoatPathTileModel>();
		new SerializerLibrary.DictionarySerializer<string, bool>();
		new SerializerLibrary.DictionarySerializer<string, int>();
		new SerializerLibrary.DictionarySerializer<string, string>();
		new SerializerLibrary.DictionarySerializer<string, Fix64>();
	}
}
