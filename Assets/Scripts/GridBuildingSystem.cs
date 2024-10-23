using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//GridCell 클래스는 각 그리드 셀의 상태와 데이터를 관리
public class GridCell
{
    public Vector3Int Position;      //셀의 그리드 내 위치
    public bool IsOccupoed;          //셀의 건물로 차있는지 여부
    public GameObject Building;      //셀에 배치된 건물 객체
    public GridCell(Vector3Int position)
    {
        Position = position;
        IsOccupoed = false;
        Building = null;
    }
}



public class GridBuildingSystem : MonoBehaviour
{
    [SerializeField] private int width = 10;    //그리드 가로 크기
    [SerializeField] private int height = 10;   //그리드 세로 크기
    [SerializeField] private float cellSize = 1;  //각 셀의 크기

    [SerializeField] private GameObject cellPrefab;   //셀 프리맵
    [SerializeField] private GameObject buildingPrefabs;  //건물 프리맵
    [SerializeField] private PlayerController playerController;    //플레이어 컨트롤러 참조
    [SerializeField] private float maxBuildDistance = 5f;          //건설 가능한 최대 거리

    [SerializeField] private Grid grid;     //그리드 선언 후 받아온다.
    private GridCell[,] cell;               //2차 배열로 선언 GridCell
    private Camera firstPersonCamera;       //플레이어의 1인칭 카메라를 가져온다.



    void Start()
    {
        grid = GetComponent<Grid>();          //시작할 떄 그리드를 받아온다.
        CreateGrid();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        for(int x = 0; x  <  width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                Vector3 cellCentre = grid.GetCellCenterWorld(new Vector3Int(x, 0, z));  //그리드 중심 점을 가져온다.
                Gizmos.DrawWireCube(cellCentre, new Vector3(cellSize, 0.1f, cellSize));  //각각의 칸을 그려준다.
            }

        }
    }

    private void CreateGrid()
    {
        grid.cellSize = new Vector3(cellSize, cellSize, cellSize);   //설정한 셀 사이즈를 그리드 컴퍼넌트에 넣는다.
        cellSize = new GridCell[width, height];
        Vector3 gridCenter = playerController.transform.position;
        gridCenter.y = 0;
        transform.position = gridCenter = new Vector3(width * cellSize / 2.0f, 0, height * cellSize / 2);  //그리드 중심으로 이동 시킨다.
        for (int z = 0; z < width; z++)
        {
            for(int z = 0; z< height; z++)

        }
    }

}
