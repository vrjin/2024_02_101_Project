using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//GridCell Ŭ������ �� �׸��� ���� ���¿� �����͸� ����
public class GridCell
{
    public Vector3Int Position;      //���� �׸��� �� ��ġ
    public bool IsOccupoed;          //���� �ǹ��� ���ִ��� ����
    public GameObject Building;      //���� ��ġ�� �ǹ� ��ü
    public GridCell(Vector3Int position)
    {
        Position = position;
        IsOccupoed = false;
        Building = null;
    }
}



public class GridBuildingSystem : MonoBehaviour
{
    [SerializeField] private int width = 10;    //�׸��� ���� ũ��
    [SerializeField] private int height = 10;   //�׸��� ���� ũ��
    [SerializeField] private float cellSize = 1;  //�� ���� ũ��

    [SerializeField] private GameObject cellPrefab;   //�� ������
    [SerializeField] private GameObject buildingPrefabs;  //�ǹ� ������
    [SerializeField] private PlayerController playerController;    //�÷��̾� ��Ʈ�ѷ� ����
    [SerializeField] private float maxBuildDistance = 5f;          //�Ǽ� ������ �ִ� �Ÿ�

    [SerializeField] private Grid grid;     //�׸��� ���� �� �޾ƿ´�.
    private GridCell[,] cells;               //2�� �迭�� ���� GridCell
    private Camera firstPersonCamera;       //�÷��̾��� 1��Ī ī�޶� �����´�.



    void Start()
    {
        grid = GetComponent<Grid>();          //������ �� �׸��带 �޾ƿ´�.
        CreateGrid();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        for(int x = 0; x  <  width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                Vector3 cellCentre = grid.GetCellCenterWorld(new Vector3Int(x, 0, z));  //�׸��� �߽� ���� �����´�.
                Gizmos.DrawWireCube(cellCentre, new Vector3(cellSize, 0.1f, cellSize));  //������ ĭ�� �׷��ش�.
            }

        }
    }

    private void CreateGrid()
    {
        grid.cellSize = new Vector3(cellSize, cellSize, cellSize);   //������ �� ����� �׸��� ���۳�Ʈ�� �ִ´�.
        cells = new GridCell[width, height];
        Vector3 gridCenter = playerController.transform.position;
        gridCenter.y = 0;
        transform.position = gridCenter = new Vector3(width * cellSize / 2.0f, 0, height * cellSize / 2);  //�׸��� �߽����� �̵� ��Ų��.
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                Vector3Int cellPosition = new Vector3Int(x, 0, z);
                Vector3 worIdposition = grid.GetCellCenterWorld(cellPosition);
                GameObject cellObject = Instantiate(cellPrefab, worIdposition, cellPrefab.transform.rotation);
                cellObject.transform.SetParent(transform);

                cells [x, z] = new GridCell(cellPosition);
            }

        }
    }

}
