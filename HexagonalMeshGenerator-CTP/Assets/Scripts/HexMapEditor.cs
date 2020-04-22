using UnityEngine;
using UnityEngine.EventSystems;

public class HexMapEditor : MonoBehaviour {

	public Color[] colors;

	public HexGrid hexGrid;

	int activeElevation;
    public string activePrefab;
    bool spawnFeatures = false;

	public Color activeColor;

	int brushSize;

	bool applyColour = true;
	bool applyElevation = true;
    float featuerRotation = 0;

	bool isDrag;
	HexDirection dragDirection;
	HexCell previousCell;

    public void ElevationActive(bool toggle)
    {
        applyElevation = toggle;
    }

    public void ColourActive(bool toggle)
    {
        applyColour = toggle;
    }

    public void SetSpawnFeatures(bool toggle)
    {
        spawnFeatures = toggle;
    }

	public void SetApplyElevation (bool toggle) {
		applyElevation = toggle;
	}

	public void SetElevation (float elevation) {
		activeElevation = (int)elevation;
	}

	public void SetBrushSize (float size) {
		brushSize = (int)size;
	}

	public void ShowUI (bool visible) {
		hexGrid.ShowUI(visible);
	}

	void Update () {
		if (
			Input.GetMouseButton(0) &&
			!EventSystem.current.IsPointerOverGameObject()
		) {
			HandleInput();
		}
		else {
			previousCell = null;
		}
	}

	void HandleInput () {
		Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		if (Physics.Raycast(inputRay, out hit)) {
			HexCell currentCell = hexGrid.GetCell(hit.point);

            //if (currentCell)
            //{
            //    if (Input.GetMouseButtonDown(0))
            //    {
            //        currentCell.ClearCellFeatures();
            //        GameObject tempFeature = Instantiate(Resources.Load("Forest"),
            //            currentCell.gameObject.transform.position, Quaternion.identity) as GameObject;
            //        tempFeature.transform.SetParent(currentCell.transform);
            //        currentCell.activeFeatures.Add(tempFeature);
            //    }
            //}

            if (previousCell && previousCell != currentCell) {
				ValidateDrag(currentCell);
			}
			else {
				isDrag = false;
			}
			EditCells(currentCell);
			previousCell = currentCell;
		}
		else {
			previousCell = null;
		}
	}

	void ValidateDrag (HexCell currentCell) {
		for (
			dragDirection = HexDirection.NE;
			dragDirection <= HexDirection.NW;
			dragDirection++
		) {
			if (previousCell.GetNeighbor(dragDirection) == currentCell) {
				isDrag = true;
				return;
			}
		}
		isDrag = false;
	}

	void EditCells (HexCell center) {
		int centerX = center.coordinates.X;
		int centerZ = center.coordinates.Z;

		for (int r = 0, z = centerZ - brushSize; z <= centerZ; z++, r++) {
			for (int x = centerX - r; x <= centerX + brushSize; x++) {
				EditCell(hexGrid.GetCell(new HexCoordinates(x, z)));


			}
		}
		for (int r = 0, z = centerZ + brushSize; z > centerZ; z--, r++) {
			for (int x = centerX - brushSize; x <= centerX + r; x++) {
				EditCell(hexGrid.GetCell(new HexCoordinates(x, z)));
			}
		}
	}

	void EditCell (HexCell cell) {
		if (cell) {
			if (applyColour) {
				cell.Colour = activeColor;
			}
			if (applyElevation) {
				cell.Elevation = activeElevation;
			}
			else if (isDrag) {
				HexCell otherCell = cell.GetNeighbor(dragDirection.Opposite());
                if (cell)
                {
                    cell.ClearCellFeatures();
                    if(spawnFeatures)
                    {
                        GameObject tempFeature = Instantiate(Resources.Load(activePrefab),
                        cell.gameObject.transform.position, Quaternion.identity) as GameObject;
                        tempFeature.transform.SetParent(cell.transform);
                        cell.activeFeatures.Add(tempFeature);
                    }

                }
            }

            if (Input.GetMouseButtonDown(0))
            {
                cell.ClearCellFeatures();
                //Debug.Log(activePrefab);
                if (spawnFeatures)
                {
                    GameObject tempFeature = Instantiate(Resources.Load(activePrefab),
                    cell.gameObject.transform.position, Quaternion.identity) as GameObject;
                    tempFeature.transform.SetParent(cell.transform);
                    cell.activeFeatures.Add(tempFeature);
                }
            }
        }
	}
}