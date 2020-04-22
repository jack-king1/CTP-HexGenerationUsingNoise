using System.Collections.Generic;
using UnityEngine;

public class HexCell : MonoBehaviour {

	public HexCoordinates coordinates;

	public RectTransform uiRect;

	public HexGridChunk chunk;

    public List<GameObject> activeFeatures;

	public Color Colour {
		get {
			return color;
		}
		set {
			if (color == value) {
				return;
			}
			color = value;
			Refresh();
		}
	}

    public void ClearCellFeatures()
    {
        if(activeFeatures.Count > 0)
        {
            foreach (var f in activeFeatures)
            {
                Destroy(f);
            }
        }
        activeFeatures.Clear();
    }

	public int Elevation {
		get {
			return elevation;
		}
		set {
			if (elevation == value) {
				return;
			}
			elevation = value;
			Vector3 position = transform.localPosition;
			position.y = value * HexData.elevationStep;
			position.y +=
				(HexData.GetNoiseFromTexture(position).y * 2f - 1f) *
				HexData.elevationPerturbStrength;
			transform.localPosition = position;

			Vector3 uiPosition = uiRect.localPosition;
			uiPosition.z = -position.y;
			uiRect.localPosition = uiPosition;

			Refresh();
		}
	}

	public Vector3 Position {
		get {
			return transform.localPosition;
		}
	}

	Color color;

	int elevation = int.MinValue;


	[SerializeField]
	HexCell[] neighbors;

	public HexCell GetNeighbor (HexDirection direction) {
		return neighbors[(int)direction];
	}

	public void SetNeighbor (HexDirection direction, HexCell cell) {
		neighbors[(int)direction] = cell;
		cell.neighbors[(int)direction.Opposite()] = this;
	}

	public HexEdgeClassification GetEdgeType (HexDirection direction) {
		return HexData.GetHexCellEdgeType(
			elevation, neighbors[(int)direction].elevation
		);
	}

	public HexEdgeClassification GetEdgeType (HexCell otherCell) {
		return HexData.GetHexCellEdgeType(
			elevation, otherCell.elevation
		);
	}

	void Refresh () {
		if (chunk) {
			chunk.Refresh();
			for (int i = 0; i < neighbors.Length; i++) {
				HexCell neighbor = neighbors[i];
				if (neighbor != null && neighbor.chunk != chunk) {
					neighbor.chunk.Refresh();
				}
			}
		}
	}

	void RefreshSelfOnly () {
		chunk.Refresh();
	}
}