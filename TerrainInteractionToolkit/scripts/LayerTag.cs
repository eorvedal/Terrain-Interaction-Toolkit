/*
 * Written by Eric Orvedal. entropicsoul@gmail.com
 * No copyright is intended.
 */
using System.Collections;
using UnityEngine;

namespace booger
{
	public class LayerTag : MonoBehaviour
	{

		public int DetailLayer = -1;
		public Terrain DetailTerrain = null;
		public int range = 1;
		public bool explode = false;

		public void WasInteracted()
        {
			explode = true;
        }

		private void OnDestroy()
		{
			if (DetailLayer >= 0) // need to add sanity checks galore
				if (explode)
				{
					Vector3 DetailPos = new Vector3();
					DetailPos = ConvertWorldCoord2TerrCoord(gameObject.transform.position);
					RemoveLayerRange((int)DetailPos.x, (int)DetailPos.z, DetailTerrain, DetailLayer);
				}
		}

		private Vector3 ConvertWorldCoord2TerrCoord(Vector3 wordCor)
		{
			Vector3 vecRet = new Vector3();
			Terrain ter = DetailTerrain;
			Vector3 terPosition = ter.transform.position;
			vecRet.x = ((wordCor.x - terPosition.x) / ter.terrainData.size.x) * ter.terrainData.detailWidth;
			vecRet.z = ((wordCor.z - terPosition.z) / ter.terrainData.size.z) * ter.terrainData.detailHeight;
			return vecRet;
		}

		private void RemoveLayerRange(int x, int y, Terrain t, int layerIndex)
		{
			TerrainData terrainData = t.terrainData;
			int[,] map = terrainData.GetDetailLayer(0, 0, terrainData.detailWidth, terrainData.detailHeight, layerIndex);

			int xmin = x - range;
			int ymin = y - range;
			int xmax = x + range;
			int ymax = y + range;

			for (int i = xmin; i <= xmax; i++)
			{
				for (int j = ymin; j <= ymax; j++)
				{
					map[j, i] = 0;
				}
			}
			t.terrainData.SetDetailLayer(0, 0, layerIndex, map);
		}


	}
}