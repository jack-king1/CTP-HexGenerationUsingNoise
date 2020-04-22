using UnityEngine;

public static class HexData {

	public const float outerToInner = 0.866025404f;
	public const float innerToOuter = 1f / outerToInner;

	public const float outerRadius = 10f;

	public const float innerRadius = outerRadius * outerToInner;

	public const float solidFactor = 0.8f;

	public const float blendFactor = 1f - solidFactor;

	public const float elevationStep = 3f;

	public const int terracesPerSlope = 2;

	public const int terraceSteps = terracesPerSlope * 2 + 1;

	public const float horizontalTerraceStepSize = 1f / terraceSteps;

	public const float verticalTerraceStepSize = 1f / (terracesPerSlope + 1);

	public const float cellPerturbStrength = 4f;

	public const float elevationPerturbStrength = 1.5f;

	public const float streamBedElevationOffset = -1.75f;

	public const float noiseScale = 0.003f;

	public const int chunkSizeX = 5, chunkSizeZ = 5;

    //Gets the corners of the hex for each side.
	static Vector3[] hexCorners = {
		new Vector3(0f, 0f, outerRadius),
		new Vector3(innerRadius, 0f, 0.5f * outerRadius),
		new Vector3(innerRadius, 0f, -0.5f * outerRadius),
		new Vector3(0f, 0f, -outerRadius),
		new Vector3(-innerRadius, 0f, -0.5f * outerRadius),
		new Vector3(-innerRadius, 0f, 0.5f * outerRadius),
		new Vector3(0f, 0f, outerRadius)
	};

	public static Texture2D noiseSource;

    //Samples 2d noise from a png texture returning an x and y value based on a vector 2 (x, z) position.
	public static Vector4 GetNoiseFromTexture (Vector3 position) {
		return noiseSource.GetPixelBilinear(
			position.x * noiseScale,
			position.z * noiseScale
		);
	}

    //Returns a vector 3 of the first corner for mesh creation.
	public static Vector3 GetFirstCorner (HexDirection direction) {
		return hexCorners[(int)direction];
	}

    //Returns a vector 3 of the first corner for mesh creation.
    public static Vector3 GetSecondCorner (HexDirection direction) {
		return hexCorners[(int)direction + 1];
	}

	public static Vector3 GetFirstSolidCorner (HexDirection direction) {
		return hexCorners[(int)direction] * solidFactor;
	}

	public static Vector3 GetSecondSolidCorner (HexDirection direction) {
		return hexCorners[(int)direction + 1] * solidFactor;
	}

	public static Vector3 GetSolidEdgeMiddle (HexDirection direction) {
		return
			(hexCorners[(int)direction] + hexCorners[(int)direction + 1]) *
			(0.5f * solidFactor);
	}

    //Creates a blend zone between 2 hexes so the colours can blend togethor.
	public static Vector3 GetBlendZone (HexDirection direction) {
		return (hexCorners[(int)direction] + hexCorners[(int)direction + 1]) *
			blendFactor;
	}

	public static Vector3 StepOffset (Vector3 a, Vector3 b, int step) {
		float h = step * HexData.horizontalTerraceStepSize;
		a.x += (b.x - a.x) * h;
		a.z += (b.z - a.z) * h;
		float v = ((step + 1) / 2) * HexData.verticalTerraceStepSize;
		a.y += (b.y - a.y) * v;
		return a;
	}

	public static Color StepOffset (Color a, Color b, int step) {
		float h = step * HexData.horizontalTerraceStepSize;
		return Color.Lerp(a, b, h);
	}

	public static HexEdgeClassification GetHexCellEdgeType (int elevation1, int elevation2) {
		if (elevation1 == elevation2) {
			return HexEdgeClassification.Flat;
		}
		int delta = elevation2 - elevation1;
		if (delta == 1 || delta == -1) {
			return HexEdgeClassification.Slope;
		}
		return HexEdgeClassification.Cliff;
	}

	public static Vector3 AddRealismNoiseToBaseMesh (Vector3 position) {
		Vector4 sample = GetNoiseFromTexture(position);
		position.x += (sample.x * 2f - 1f) * cellPerturbStrength;
		position.z += (sample.z * 2f - 1f) * cellPerturbStrength;
		return position;
	}
}