using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HexGrid : MonoBehaviour {

	public int chunkCountX = 4, chunkCountZ = 3;
	public Color defaultColor = Color.white;
    public bool AutoGenerate;
    public HexCell cellPrefab;
	public Text cellLabelPrefab;
	public HexGridChunk chunkPrefab;

	public Texture2D noiseSource;

	HexGridChunk[] chunks;
	HexCell[] cells;

	int cellCountX, cellCountZ;

    //Continent
    public HexCell middleHexCell;
    public List<HexCell> continentNeighboursImmediate;
    public List<HexCell> continentNeighboursInner;
    public List<HexCell> continentNeighboursOuter;
    public List<HexCell> continentNeighboursOuterBeach;


    void Start () {
		HexData.noiseSource = noiseSource;

		cellCountX = chunkCountX * HexData.chunkSizeX;
		cellCountZ = chunkCountZ * HexData.chunkSizeZ;

		CreateChunks();
		CreateCells();
        //Create continent
        //CreateContinent();

        //Add Noise
        if (AutoGenerate)
        {
            for (int i = 0; i < cells.Length; ++i)
            {
                //Debug.Log((int)Mathf.PerlinNoise(cells[i].coordinates.X, cells[i].coordinates.Z));
                //float noise = NoiseFunc(cells[i]);
                Debug.Log((int)((NoiseAutoGen.Instance.ElevationNoise(ref cells[i], (float)cells.Length)) * 12));

                int addition = (int)((NoiseAutoGen.Instance.ElevationNoise(ref cells[i], (float)cells.Length)) * 12);
                cells[i].Elevation = addition;
                cells[i].Colour = NoiseAutoGen.Instance.evh[addition].colour;
            }
        }
    }

    void CreateContinent()
    {
        middleHexCell = cells[405];
        middleHexCell.Colour = Color.white;
        middleHexCell.Elevation = 6;
        for (int i = 0; i < System.Enum.GetNames(typeof(HexDirection)).Length; i++)
        {
            if (middleHexCell.GetNeighbor((HexDirection)i) != null)
            {
                continentNeighboursImmediate.Add(middleHexCell.GetNeighbor((HexDirection)i));
                middleHexCell.GetNeighbor((HexDirection)i).Elevation = (int)Random.Range(5f, 6f);
                middleHexCell.GetNeighbor((HexDirection)i).Colour = Color.grey;
            }
        }

        foreach (var cell in continentNeighboursImmediate)
        {
            for (int i = 0; i < System.Enum.GetNames(typeof(HexDirection)).Length; i++)
            {
                HexCell neighbourCell = cell.GetNeighbor((HexDirection)i);
                if (neighbourCell != null && neighbourCell != middleHexCell)
                {
                    if (!continentNeighboursImmediate.Contains(neighbourCell))
                    {
                        continentNeighboursInner.Add(neighbourCell);
                        neighbourCell.Colour = Color.grey;
                        neighbourCell.Elevation = (int)Random.Range(4f, 5f);
                    }
                }
            }
        }


        foreach (var cell in continentNeighboursInner)
        {
            for (int i = 0; i < System.Enum.GetNames(typeof(HexDirection)).Length; i++)
            {
                HexCell neighbourCell = cell.GetNeighbor((HexDirection)i);
                if (neighbourCell != null && neighbourCell != middleHexCell)
                {
                    if (!continentNeighboursInner.Contains(neighbourCell) && !continentNeighboursImmediate.Contains(neighbourCell))
                    {
                        continentNeighboursOuter.Add(neighbourCell);
                        neighbourCell.Colour = Color.green;
                        neighbourCell.Elevation = (int)Random.Range(3f, 4f);
                    }
                }
            }
        }

        foreach (var cell in continentNeighboursOuter)
        {
            for (int i = 0; i < System.Enum.GetNames(typeof(HexDirection)).Length; i++)
            {
                HexCell neighbourCell = cell.GetNeighbor((HexDirection)i);
                if (neighbourCell != null && neighbourCell != middleHexCell)
                {
                    if (!continentNeighboursOuter.Contains(neighbourCell) && !continentNeighboursInner.Contains(neighbourCell))
                    {
                        continentNeighboursOuterBeach.Add(neighbourCell);
                        neighbourCell.Colour = Color.green;
                        neighbourCell.Elevation = 3;
                    }
                }
            }
        }

        foreach (var cell in continentNeighboursOuterBeach)
        {
            for (int i = 0; i < System.Enum.GetNames(typeof(HexDirection)).Length; i++)
            {
                HexCell neighbourCell = cell.GetNeighbor((HexDirection)i);
                if (neighbourCell != null && neighbourCell != middleHexCell)
                {
                    if (!continentNeighboursOuterBeach.Contains(neighbourCell) && !continentNeighboursOuter.Contains(neighbourCell))
                    {
                        neighbourCell.Colour = Color.yellow;
                        neighbourCell.Elevation = 2;
                    }
                }
            }
        }

    }

	void CreateChunks () {
		chunks = new HexGridChunk[chunkCountX * chunkCountZ];

		for (int z = 0, i = 0; z < chunkCountZ; z++) {
			for (int x = 0; x < chunkCountX; x++) {
				HexGridChunk chunk = chunks[i++] = Instantiate(chunkPrefab);
				chunk.transform.SetParent(transform);
			}
		}
	}

	void CreateCells () {
		cells = new HexCell[cellCountZ * cellCountX];

		for (int z = 0, i = 0; z < cellCountZ; z++) {
			for (int x = 0; x < cellCountX; x++) {
				CreateCell(x, z, i++);
			}
		}

	}

	void OnEnable () {
		HexData.noiseSource = noiseSource;
	}

	public HexCell GetCell (Vector3 position) {
		position = transform.InverseTransformPoint(position);
		HexCoordinates coordinates = HexCoordinates.FromPosition(position);
		int index =
			coordinates.X + coordinates.Z * cellCountX + coordinates.Z / 2;
		return cells[index];
	}

	public HexCell GetCell (HexCoordinates coordinates) {
		int z = coordinates.Z;
		if (z < 0 || z >= cellCountZ) {
			return null;
		}
		int x = coordinates.X + z / 2;
		if (x < 0 || x >= cellCountX) {
			return null;
		}
		return cells[x + z * cellCountX];
	}

	public void ShowUI (bool visible) {
		for (int i = 0; i < chunks.Length; i++) {
			chunks[i].ShowUI(visible);
		}
	}

	void CreateCell (int x, int z, int i) {
		Vector3 position;
		position.x = (x + z * 0.5f - z / 2) * (HexData.innerRadius * 2f);
		position.y = 0f;
		position.z = z * (HexData.outerRadius * 1.5f);

		HexCell cell = cells[i] = Instantiate<HexCell>(cellPrefab);
		cell.transform.localPosition = position;
		cell.coordinates = HexCoordinates.FromOffsetCoordinates(x, z);
		cell.Colour = defaultColor;

		if (x > 0) {
			cell.SetNeighbor(HexDirection.W, cells[i - 1]);
		}
		if (z > 0) {
			if ((z & 1) == 0) {
				cell.SetNeighbor(HexDirection.SE, cells[i - cellCountX]);
				if (x > 0) {
					cell.SetNeighbor(HexDirection.SW, cells[i - cellCountX - 1]);
				}
			}
			else {
				cell.SetNeighbor(HexDirection.SW, cells[i - cellCountX]);
				if (x < cellCountX - 1) {
					cell.SetNeighbor(HexDirection.SE, cells[i - cellCountX + 1]);
				}
			}
		}

		Text label = Instantiate<Text>(cellLabelPrefab);
		label.rectTransform.anchoredPosition =
			new Vector2(position.x, position.z);
		label.text = cell.coordinates.ToStringOnSeparateLines();
		cell.uiRect = label.rectTransform;

		AddCellToChunk(x, z, cell);
	}

	void AddCellToChunk (int x, int z, HexCell cell) {
		int chunkX = x / HexData.chunkSizeX;
		int chunkZ = z / HexData.chunkSizeZ;
		HexGridChunk chunk = chunks[chunkX + chunkZ * chunkCountX];

		int localX = x - chunkX * HexData.chunkSizeX;
		int localZ = z - chunkZ * HexData.chunkSizeZ;
		chunk.AddCell(localX + localZ * HexData.chunkSizeX, cell, AutoGenerate);
	}

    float NoiseFunc(HexCell cell)
    {
        return Mathf.PerlinNoise((float)cell.coordinates.X, (float)cell.coordinates.Z);
    }
}