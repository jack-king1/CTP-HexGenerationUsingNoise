using UnityEngine;
using UnityEngine.UI;


public class HexGridChunk : MonoBehaviour {

	public HexTerrain terrain;

	HexCell[] cells;
    Canvas gridCanvas;

    float noiseResolution;
    Vector2 noiseOffset;
    float noiseScale; // larger = more islands & lakes
    public ElevationHeights[] evh;

    void Awake () {
		gridCanvas = GetComponentInChildren<Canvas>();

		cells = new HexCell[HexData.chunkSizeX * HexData.chunkSizeZ];
		ShowUI(false);
        terrain.gameObject.layer = 8;
	}

	public void AddCell (int index, HexCell cell, bool autoGen) {
		cells[index] = cell;
		cell.chunk = this;
		cell.transform.SetParent(transform, false);
		cell.uiRect.SetParent(gridCanvas.transform, false);

	}

	public void Refresh () {
		enabled = true;
	}

	public void ShowUI (bool visible) {
		gridCanvas.gameObject.SetActive(visible);
	}

	void LateUpdate () {
		Triangulate();
		enabled = false;
	}

	public void Triangulate () {
		terrain.Clear();
		for (int i = 0; i < cells.Length; i++) {
			Triangulate(cells[i]);
		}
		terrain.Apply();
	}

	void Triangulate (HexCell cell) {
		for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++) {
			Triangulate(d, cell);
		}
	}

	void Triangulate (HexDirection direction, HexCell cell) {
        if(cell)
        {
            Vector3 center = cell.Position;
            EdgeVertices e = new EdgeVertices(
                center + HexData.GetFirstSolidCorner(direction),
                center + HexData.GetSecondSolidCorner(direction)
            );

                TriangulateEdgeFan(center, e, cell.Colour);

            if (direction <= HexDirection.SE)
            {
                TriangulateConnection(direction, cell, e);
            }
        }
	}


	void TriangulateConnection (
		HexDirection direction, HexCell cell, EdgeVertices e1
	) {
		HexCell neighbor = cell.GetNeighbor(direction);
		if (neighbor == null) {
			return;
		}

		Vector3 bridge = HexData.GetBlendZone(direction);
		bridge.y = neighbor.Position.y - cell.Position.y;
		EdgeVertices e2 = new EdgeVertices(
			e1.v1 + bridge,
			e1.v5 + bridge
		);


		if (cell.GetEdgeType(direction) == HexEdgeClassification.Slope) {
			TriangulateEdgeTerraces(e1, cell, e2, neighbor);
		}
		else {
			TriangulateEdgeStrip(e1, cell.Colour, e2, neighbor.Colour);
		}

		HexCell nextNeighbor = cell.GetNeighbor(direction.Next());
		if (direction <= HexDirection.E && nextNeighbor != null) {
			Vector3 v5 = e1.v5 + HexData.GetBlendZone(direction.Next());
			v5.y = nextNeighbor.Position.y;
			if (cell.Elevation <= neighbor.Elevation) {
				if (cell.Elevation <= nextNeighbor.Elevation) {
					TriangulateCorner(
						e1.v5, cell, e2.v5, neighbor, v5, nextNeighbor
					);
				}
				else {
					TriangulateCorner(
						v5, nextNeighbor, e1.v5, cell, e2.v5, neighbor
					);
				}
			}
			else if (neighbor.Elevation <= nextNeighbor.Elevation) {
				TriangulateCorner(
					e2.v5, neighbor, v5, nextNeighbor, e1.v5, cell
				);
			}
			else {
				TriangulateCorner(
					v5, nextNeighbor, e1.v5, cell, e2.v5, neighbor
				);
			}
		}
	}

	void TriangulateCorner (
		Vector3 bottom, HexCell bottomCell,
		Vector3 left, HexCell leftCell,
		Vector3 right, HexCell rightCell
	) {
		HexEdgeClassification leftEdgeType = bottomCell.GetEdgeType(leftCell);
		HexEdgeClassification rightEdgeType = bottomCell.GetEdgeType(rightCell);

		if (leftEdgeType == HexEdgeClassification.Slope) {
			if (rightEdgeType == HexEdgeClassification.Slope) {
				TriangulateCornerTerraces(
					bottom, bottomCell, left, leftCell, right, rightCell
				);
			}
			else if (rightEdgeType == HexEdgeClassification.Flat) {
				TriangulateCornerTerraces(
					left, leftCell, right, rightCell, bottom, bottomCell
				);
			}
			else {
				TriangulateCornerTerracesCliff(
					bottom, bottomCell, left, leftCell, right, rightCell
				);
			}
		}
		else if (rightEdgeType == HexEdgeClassification.Slope) {
			if (leftEdgeType == HexEdgeClassification.Flat) {
				TriangulateCornerTerraces(
					right, rightCell, bottom, bottomCell, left, leftCell
				);
			}
			else {
				TriangulateCornerCliffTerraces(
					bottom, bottomCell, left, leftCell, right, rightCell
				);
			}
		}
		else if (leftCell.GetEdgeType(rightCell) == HexEdgeClassification.Slope) {
			if (leftCell.Elevation < rightCell.Elevation) {
				TriangulateCornerCliffTerraces(
					right, rightCell, bottom, bottomCell, left, leftCell
				);
			}
			else {
				TriangulateCornerTerracesCliff(
					left, leftCell, right, rightCell, bottom, bottomCell
				);
			}
		}
		else {
			terrain.AddTriangle(bottom, left, right);
			terrain.AddTriangleColor(
				bottomCell.Colour, leftCell.Colour, rightCell.Colour
			);
		}
	}

	void TriangulateEdgeTerraces (
		EdgeVertices begin, HexCell beginCell,
		EdgeVertices end, HexCell endCell
	) {
		EdgeVertices e2 = EdgeVertices.TerraceLerp(begin, end, 1);
		Color c2 = HexData.StepOffset(beginCell.Colour, endCell.Colour, 1);

		TriangulateEdgeStrip(begin, beginCell.Colour, e2, c2);

		for (int i = 2; i < HexData.terraceSteps; i++) {
			EdgeVertices e1 = e2;
			Color c1 = c2;
			e2 = EdgeVertices.TerraceLerp(begin, end, i);
			c2 = HexData.StepOffset(beginCell.Colour, endCell.Colour, i);
			TriangulateEdgeStrip(e1, c1, e2, c2);
		}

		TriangulateEdgeStrip(e2, c2, end, endCell.Colour);
	}

	void TriangulateCornerTerraces (
		Vector3 begin, HexCell beginCell,
		Vector3 left, HexCell leftCell,
		Vector3 right, HexCell rightCell
	) {
		Vector3 v3 = HexData.StepOffset(begin, left, 1);
		Vector3 v4 = HexData.StepOffset(begin, right, 1);
		Color c3 = HexData.StepOffset(beginCell.Colour, leftCell.Colour, 1);
		Color c4 = HexData.StepOffset(beginCell.Colour, rightCell.Colour, 1);

		terrain.AddTriangle(begin, v3, v4);
		terrain.AddTriangleColor(beginCell.Colour, c3, c4);

		for (int i = 2; i < HexData.terraceSteps; i++) {
			Vector3 v1 = v3;
			Vector3 v2 = v4;
			Color c1 = c3;
			Color c2 = c4;
			v3 = HexData.StepOffset(begin, left, i);
			v4 = HexData.StepOffset(begin, right, i);
			c3 = HexData.StepOffset(beginCell.Colour, leftCell.Colour, i);
			c4 = HexData.StepOffset(beginCell.Colour, rightCell.Colour, i);
			terrain.AddQuad(v1, v2, v3, v4);
			terrain.AddQuadColour(c1, c2, c3, c4);
		}

		terrain.AddQuad(v3, v4, left, right);
		terrain.AddQuadColour(c3, c4, leftCell.Colour, rightCell.Colour);
	}

	void TriangulateCornerTerracesCliff (
		Vector3 begin, HexCell beginCell,
		Vector3 left, HexCell leftCell,
		Vector3 right, HexCell rightCell
	) {
		float b = 1f / (rightCell.Elevation - beginCell.Elevation);
		if (b < 0) {
			b = -b;
		}
		Vector3 boundary = Vector3.Lerp(
			HexData.AddRealismNoiseToBaseMesh(begin), HexData.AddRealismNoiseToBaseMesh(right), b
		);
		Color boundaryColour = Color.Lerp(beginCell.Colour, rightCell.Colour, b);

		TriangulateBoundaryTriangle(
			begin, beginCell, left, leftCell, boundary, boundaryColour
		);

		if (leftCell.GetEdgeType(rightCell) == HexEdgeClassification.Slope) {
			TriangulateBoundaryTriangle(
				left, leftCell, right, rightCell, boundary, boundaryColour
			);
		}
		else {
			terrain.AddTriangleUnperturbed(
				HexData.AddRealismNoiseToBaseMesh(left), HexData.AddRealismNoiseToBaseMesh(right), boundary
			);
			terrain.AddTriangleColor(
				leftCell.Colour, rightCell.Colour, boundaryColour
			);
		}
	}

	void TriangulateCornerCliffTerraces (
		Vector3 begin, HexCell beginCell,
		Vector3 left, HexCell leftCell,
		Vector3 right, HexCell rightCell
	) {
		float b = 1f / (leftCell.Elevation - beginCell.Elevation);
		if (b < 0) {
			b = -b;
		}
		Vector3 boundary = Vector3.Lerp(
			HexData.AddRealismNoiseToBaseMesh(begin), HexData.AddRealismNoiseToBaseMesh(left), b
		);
		Color boundaryColor = Color.Lerp(beginCell.Colour, leftCell.Colour, b);

		TriangulateBoundaryTriangle(
			right, rightCell, begin, beginCell, boundary, boundaryColor
		);

		if (leftCell.GetEdgeType(rightCell) == HexEdgeClassification.Slope) {
			TriangulateBoundaryTriangle(
				left, leftCell, right, rightCell, boundary, boundaryColor
			);
		}
		else {
			terrain.AddTriangleUnperturbed(
				HexData.AddRealismNoiseToBaseMesh(left), HexData.AddRealismNoiseToBaseMesh(right), boundary
			);
			terrain.AddTriangleColor(
				leftCell.Colour, rightCell.Colour, boundaryColor
			);
		}
	}

	void TriangulateBoundaryTriangle (
		Vector3 begin, HexCell beginCell,
		Vector3 left, HexCell leftCell,
		Vector3 boundary, Color boundaryColor
	) {
		Vector3 v2 = HexData.AddRealismNoiseToBaseMesh(HexData.StepOffset(begin, left, 1));
		Color c2 = HexData.StepOffset(beginCell.Colour, leftCell.Colour, 1);

		terrain.AddTriangleUnperturbed(HexData.AddRealismNoiseToBaseMesh(begin), v2, boundary);
		terrain.AddTriangleColor(beginCell.Colour, c2, boundaryColor);

		for (int i = 2; i < HexData.terraceSteps; i++) {
			Vector3 v1 = v2;
			Color c1 = c2;
			v2 = HexData.AddRealismNoiseToBaseMesh(HexData.StepOffset(begin, left, i));
			c2 = HexData.StepOffset(beginCell.Colour, leftCell.Colour, i);
			terrain.AddTriangleUnperturbed(v1, v2, boundary);
			terrain.AddTriangleColor(c1, c2, boundaryColor);
		}

		terrain.AddTriangleUnperturbed(v2, HexData.AddRealismNoiseToBaseMesh(left), boundary);
		terrain.AddTriangleColor(c2, leftCell.Colour, boundaryColor);
	}

	void TriangulateEdgeFan (Vector3 center, EdgeVertices edge, Color color) {
		terrain.AddTriangle(center, edge.v1, edge.v2);
		terrain.AddTriangleColor(color);
		terrain.AddTriangle(center, edge.v2, edge.v3);
		terrain.AddTriangleColor(color);
		terrain.AddTriangle(center, edge.v3, edge.v4);
		terrain.AddTriangleColor(color);
		terrain.AddTriangle(center, edge.v4, edge.v5);
		terrain.AddTriangleColor(color);
	}

	void TriangulateEdgeStrip (
		EdgeVertices e1, Color c1,
		EdgeVertices e2, Color c2
	) {
		terrain.AddQuad(e1.v1, e1.v2, e2.v1, e2.v2);
		terrain.AddQuadColor(c1, c2);
		terrain.AddQuad(e1.v2, e1.v3, e2.v2, e2.v3);
		terrain.AddQuadColor(c1, c2);
		terrain.AddQuad(e1.v3, e1.v4, e2.v3, e2.v4);
		terrain.AddQuadColor(c1, c2);
		terrain.AddQuad(e1.v4, e1.v5, e2.v4, e2.v5);
		terrain.AddQuadColor(c1, c2);
	}
}