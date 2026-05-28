using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;
using ooparts.dungen;
								// Start 함수가 없다? - 대신 다른곳에서 호출되는 듯한 함수가 잔뜩 있다
namespace ooparts.dungen		// RoomMapManager.cs 의 Start->BeginGame() 로 호출되서 생성됨
{
	[System.Serializable]
	public struct MinMax		// 구조체 타입으로 최소값과 최대값을 만들어둔다 (왜?) (추측 : 방 최소 최대 개수?)
	{
		public int Min;
		public int Max;
	}

	public enum TileType		// 타일의 타입을 Enum 으로 받는다 
	{
		Empty,
		Room,
		Corridor,
		Wall
	}

	public class Map : MonoBehaviour
	{
		public Room RoomPrefab;
		[HideInInspector] public int RoomCount;
		public RoomSetting[] RoomSettings;
		[HideInInspector] public IntVector2 MapSize;
		[HideInInspector] public MinMax RoomSize;
		public float GenerationStepDelay;

		private List<Room> _rooms;
		private List<Corridor> _corridors;

		private TileType[,] _tilesTypes;

		private bool _hasPlayer = false;

		public void SetTileType(IntVector2 coordinates, TileType tileType)
		{
			_tilesTypes[coordinates.x, coordinates.z] = tileType;
		}

		public TileType GetTileType(IntVector2 coordinates)
		{
			return _tilesTypes[coordinates.x, coordinates.z];
		}


		// Generate Rooms and Corridors
		public IEnumerator Generate()					// 코루틴 사용..? // 잠깐만요 이거 뭐에요
		{
			Stopwatch stopwatch = new Stopwatch();		// 스탑워치 생성
			stopwatch.Start();							// 스탑워치 활성화

			{
				_tilesTypes = new TileType[MapSize.x, MapSize.z];		// 방에 있는 타일이 어떤 타입일지 정하는 듯 하다
				_rooms = new List<Room>();

				// Generate Rooms
				for (int i = 0; i < RoomCount; i++)						// 방 개수보다 적게 방을 만들어낸다?
				{
					Room roomInstance = CreateRoom();					// 방 인스턴스를 만들어낸다 - Room 클래스가 뭐지?
					if (roomInstance == null)								// 만약 룸 인스턴스가 존재하지 않는다면? - 예외처리
					{
						RoomCount = _rooms.Count;						// 방 카운트를 위에 만든 리스트의 길이만큼 만든다
						Debug.Log("Cannot make more rooms!");			// 방을 더 만든 수 없다고 로그 띄우기
						Debug.Log("Created Rooms : " + RoomCount);		// 방이 몇개 만들어졌는지 알려주기
						break;
					}
					roomInstance.Setting = RoomSettings[Random.Range(0, RoomSettings.Length)];
					StartCoroutine(roomInstance.Generate());

					// Generate Player or Monster
					if (_hasPlayer)
						yield return roomInstance.CreateMonsters();
					else
					{
						yield return roomInstance.CreatePlayer();
						_hasPlayer = true;
					}
					yield return null;
				}
				Debug.Log("Every rooms are generated");

				// Delaunay Triangulation
				yield return BowyerWatson();

				// Minimal Spanning Tree
				yield return PrimMST();
				Debug.Log("Every rooms are minimally connected");

				// Generate Corridors
				foreach (Corridor corridor in _corridors)
				{
					StartCoroutine(corridor.Generate());
					yield return null;
				}
				Debug.Log("Every corridors are generated");

				// Generate Walls
				yield return WallCheck();
				foreach (Room room in _rooms)
				{
					yield return room.CreateWalls();
				}
				foreach (Corridor corridor in _corridors)
				{
					yield return corridor.CreateWalls();
				}
				Debug.Log("Every walls are generated");
			}

			stopwatch.Stop();
			print("Done in :" + stopwatch.ElapsedMilliseconds / 1000f + "s");
		}

		private IEnumerator WallCheck()
		{
			for (int x = 0; x < MapSize.x; x++)
			{
				for (int z = 0; z < MapSize.z; z++)
				{
					if (_tilesTypes[x, z] == TileType.Empty && IsWall(x, z))
					{
						_tilesTypes[x, z] = TileType.Wall;
					}
				}
			}
			yield return null;
		}

		private bool IsWall(int x, int z)
		{
			for (int i = x - 1; i <= x + 1; i++)
			{
				if (i < 0 || i >= MapSize.x)
				{
					continue;
				}
				for (int j = z - 1; j <= z + 1; j++)
				{
					if (j < 0 || j >= MapSize.z || (i == x && j == z))
					{
						continue;
					}
					if (_tilesTypes[i, j] == TileType.Room || _tilesTypes[i, j] == TileType.Corridor)
					{
						return true;
					}
				}
			}

			return false;
		}

		private Room CreateRoom()					// 맵이 만들어질 때 호출되는 방 생성 함수
		{
			Room newRoom = null;					// 빈 값으로 방을 선언한다

			// Try as many as we can.
			for (int i = 0; i < RoomCount * RoomCount; i++)
			{
				IntVector2 size = new IntVector2(Random.Range(RoomSize.Min, RoomSize.Max + 1), Random.Range(RoomSize.Min, RoomSize.Max + 1));		 // 랜덤 사이즈 설정
				IntVector2 coordinates = new IntVector2(Random.Range(1, MapSize.x - size.x), Random.Range(1, MapSize.z - size.z));					// 랜덤 좌표 설정
				if (!IsOverlapped(size, coordinates))			// 방이 겹치는지 확인하고 겹치지 않는다면 들여보낸다
				{
					newRoom = Instantiate(RoomPrefab);			// 방을 만든다
					_rooms.Add(newRoom);						// 리스트에 방을 추가
					newRoom.Num = _rooms.Count;					// 방 객체의 넘버 값을 조정
					newRoom.name = "Room " + newRoom.Num + " (" + coordinates.x + ", " + coordinates.z + ")";		// 오브젝트 이름 수정 - 위치 표기
					newRoom.Size = size;						// 사이즈 설정?
					newRoom.Coordinates = coordinates;			// 좌표 설정
					newRoom.transform.parent = transform;		// 부모 좌표 설정
					Vector3 position = CoordinatesToPosition(coordinates);
					position.x += size.x * 0.5f - 0.5f;			
					position.z += size.z * 0.5f - 0.5f;
					position *= RoomMapManager.TileSize;		// 포지션에 타일 사이즈를 곱한다
					newRoom.transform.localPosition = position;	// 정해진 위치로 방을 이동시킨다
					newRoom.Init(this);							// 방에게 방이 어떤 맵에 포함되어 있는지 알려준다
					break;										// 루프 나가기 -> 결과적으로 해당 포문 안에서 만들어지는 방은 하나다
				}
			}

			if (newRoom == null)
			{
				Debug.LogError("Too many rooms in map!! : " + _rooms.Count);			// 방이 너무 많을 경우
			}

			return newRoom;																// 그렇게 만들어진 방을 리턴한다
		}

		public IntVector2 RandomCoordinates
		{
			get { return new IntVector2(Random.Range(0, MapSize.x), Random.Range(0, MapSize.z)); }
		}

		private bool IsOverlapped(IntVector2 size, IntVector2 coordinates)
		{
			foreach (Room room in _rooms)
			{
				// Give a little space between two rooms
				if (Mathf.Abs(room.Coordinates.x - coordinates.x + (room.Size.x - size.x) * 0.5f) < (room.Size.x + size.x) * 0.7f &&			// 절대값 반환. 무언가를 확인한다?
					Mathf.Abs(room.Coordinates.z - coordinates.z + (room.Size.z - size.z) * 0.5f) < (room.Size.z + size.z) * 0.7f)
				{
					return true;
				}
			}
			return false;
		}

		// Big enough to cover the map
		private Triangle LootTriangle
		{
			get
			{
				Vector3[] vertexs = new Vector3[]
				{
					RoomMapManager.TileSize * new Vector3(MapSize.x * 2, 0, MapSize.z),
					RoomMapManager.TileSize * new Vector3(-MapSize.x * 2, 0, MapSize.z),
					RoomMapManager.TileSize * new Vector3(0, 0, -2 * MapSize.z)
				};

				Room[] tempRooms = new Room[3];
				for (int i = 0; i < 3; i++)
				{
					tempRooms[i] = Instantiate(RoomPrefab);
					tempRooms[i].transform.localPosition = vertexs[i];
					tempRooms[i].name = "Loot Room " + i;
					tempRooms[i].Init(this);
				}

				return new Triangle(tempRooms[0], tempRooms[1], tempRooms[2]);
			}
		}

		private IEnumerator BowyerWatson()
		{
			List<Triangle> triangulation = new List<Triangle>();

			Triangle loot = LootTriangle;
			triangulation.Add(loot);

			foreach (Room room in _rooms)
			{
				List<Triangle> badTriangles = new List<Triangle>();

				foreach (Triangle triangle in triangulation)
				{
					if (triangle.IsContaining(room))
					{
						badTriangles.Add(triangle);
					}
				}

				List<Corridor> polygon = new List<Corridor>();
				foreach (Triangle badTriangle in badTriangles)
				{
					foreach (Corridor corridor in badTriangle.Corridors)
					{
						if (corridor.Triangles.Count == 1)
						{
							polygon.Add(corridor);
							corridor.Triangles.Remove(badTriangle);
							continue;
						}

						foreach (Triangle triangle in corridor.Triangles)
						{
							if (triangle == badTriangle)
							{
								continue;
							}

							// Delete Corridor which is between two bad triangles.
							if (badTriangles.Contains(triangle))
							{
								corridor.Rooms[0].RoomCorridor.Remove(corridor.Rooms[1]);
								corridor.Rooms[1].RoomCorridor.Remove(corridor.Rooms[0]);
								Destroy(corridor.gameObject);
							}
							else
							{
								polygon.Add(corridor);
							}
							break;
						}
					}
				}

				// Delete Bad Triangles
				for (int index = badTriangles.Count - 1; index >= 0; --index)
				{
					Triangle triangle = badTriangles[index];
					badTriangles.RemoveAt(index);
					triangulation.Remove(triangle);
					foreach (Corridor corridor in triangle.Corridors)
					{
						corridor.Triangles.Remove(triangle);
					}
				}

				foreach (Corridor corridor in polygon)
				{
					// TODO: Edge sync
					Triangle newTriangle = new Triangle(corridor.Rooms[0], corridor.Rooms[1], room);
					triangulation.Add(newTriangle);
				}
			}
			yield return null;

			for (int index = triangulation.Count - 1; index >= 0; index--)
			{
				if (triangulation[index].Rooms.Contains(loot.Rooms[0]) || triangulation[index].Rooms.Contains(loot.Rooms[1]) ||
					triangulation[index].Rooms.Contains(loot.Rooms[2]))
				{
					triangulation.RemoveAt(index);
				}
			}

			foreach (Room room in loot.Rooms)
			{
				List<Corridor> deleteList = new List<Corridor>();
				foreach (KeyValuePair<Room, Corridor> pair in room.RoomCorridor)
				{
					deleteList.Add(pair.Value);
				}
				for (int index = deleteList.Count - 1; index >= 0; index--)
				{
					Corridor corridor = deleteList[index];
					corridor.Rooms[0].RoomCorridor.Remove(corridor.Rooms[1]);
					corridor.Rooms[1].RoomCorridor.Remove(corridor.Rooms[0]);
					Destroy(corridor.gameObject);
				}
				Destroy(room.gameObject);
			}
		}

		private IEnumerator PrimMST()
		{
			List<Room> connectedRooms = new List<Room>();
			_corridors = new List<Corridor>();

			connectedRooms.Add(_rooms[0]);

			while (connectedRooms.Count < _rooms.Count)
			{
				KeyValuePair<Room, Corridor> minLength = new KeyValuePair<Room, Corridor>();
				List<Corridor> deleteList = new List<Corridor>();

				foreach (Room room in connectedRooms)
				{
					foreach (KeyValuePair<Room, Corridor> pair in room.RoomCorridor)
					{
						if (connectedRooms.Contains(pair.Key))
						{
							continue;
						}
						if (minLength.Value == null || minLength.Value.Length > pair.Value.Length)
						{
							minLength = pair;
						}
					}
				}

				// Check Unnecessary Corridors.
				foreach (KeyValuePair<Room, Corridor> pair in minLength.Key.RoomCorridor)
				{
					if (connectedRooms.Contains(pair.Key) && (minLength.Value != pair.Value))
					{
						deleteList.Add(pair.Value);
					}
				}

				// Delete corridors
				for (int index = deleteList.Count - 1; index >= 0; index--)
				{
					Corridor corridor = deleteList[index];
					corridor.Rooms[0].RoomCorridor.Remove(corridor.Rooms[1]);
					corridor.Rooms[1].RoomCorridor.Remove(corridor.Rooms[0]);
					deleteList.RemoveAt(index);
					Destroy(corridor.gameObject);
				}

				connectedRooms.Add(minLength.Key);
				_corridors.Add(minLength.Value);
			}
			yield return null;
		}

		public Vector3 CoordinatesToPosition(IntVector2 coordinates)			//  방의 좌표를 설정하는 함수?
		{
			return new Vector3(coordinates.x - MapSize.x * 0.5f + 0.5f, 0f, coordinates.z - MapSize.z * 0.5f + 0.5f);
		}
	}
}