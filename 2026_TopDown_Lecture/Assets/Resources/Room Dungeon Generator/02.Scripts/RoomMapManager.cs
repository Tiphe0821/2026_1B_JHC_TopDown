using UnityEngine;
using System.Collections;
using ooparts.dungen;

namespace ooparts.dungen			// 가장 먼저 분석할 코드
{
	public class RoomMapManager : MonoBehaviour
	{
		public Map mapPrefap;
		private Map mapInstance;

		public int MapSizeX;
		public int MapSizeZ;
		public int MaxRooms;
		public int MinRoomSize;
		public int MaxRoomSize;

		public int TileSizeFactor = 1;		// 무언가 사이즈 설정?
		public static int TileSize;			// 스테틱(전역) 설정의 타일 사이즈 (아마 다른 곳에서 참조 없이 바로 볼 수 있도록 할 목적인 듯 하다)

		void Start()
		{
			TileSize = TileSizeFactor;		// 전역 값에 설정값을 넣어주는 모습
			BeginGame();					// 게임 시작 
		}

		void Update()
		{
			if (Input.GetKeyDown(KeyCode.Space))
			{
				RestartGame();			// 스페이스 누를때마다 맵 다시 만들기 (???)
			}
		}

		private void BeginGame()			// 게임 시작 함수 (Start)에서 호출
		{
			mapInstance = Instantiate(mapPrefap);		// 맵 프리팹 생성
			mapInstance.RoomCount = MaxRooms;					// 최대 방 수 설정 (아마 이 숫자만큼 방이 생설되는 것 같다
			mapInstance.MapSize = new IntVector2(MapSizeX, MapSizeZ);		// 맵이 생성될 수 있는 최대 크기 설정
			mapInstance.RoomSize.Min = MinRoomSize;				// 최소 방 사이즈 설정
			mapInstance.RoomSize.Max = MaxRoomSize;				// 최대 방 사이즈 설정
			TileSize = TileSizeFactor;							// 타일(방 크기 단위?) 설정

			StartCoroutine(mapInstance.Generate());	// 코루틴?? // Generate 함수 호출 (맵 생성되는 동안에도 게임이 멈추면 안되니 + 한번만 생성하기에 그런듯 하다)
		}

		private void RestartGame()
		{
			StopAllCoroutines();					// 코루틴 멈추기
			Destroy(mapInstance.gameObject);		// 맵 전부 없애기
			BeginGame();							// 다시 맵 만들기
		}
	}
}