using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map {
	
	public Vector2 dimensions;	
	//string[,] Tablero = new string[3,3];
	public Tile[,] tiles; //array of tiles
	public static GameObject tileHolder;

	Vector2 currentPos;


	public void RefreshMap(){
		int W = (int) dimensions.x;
		int H = (int) dimensions.y;

		for (int row = 0; row < H; row++) {
			for (int col = 0; col < W; col++) {
				if (!tiles [row, col].isLocked) {					
					tiles [row, col].RefreshTile ();
				}
			}
		}

		for (int row = 0; row < H; row++) {
			for (int col = 0; col < W; col++) {
				tiles[row,col].ApplyRefresh();
			}
		}
	}

	public void GenerateMap(int W, int H){
		dimensions.x = (float) W;
		dimensions.y= (float) H;

		bool isFirstMap = false;

		if (tileHolder == null) {
			tileHolder = new GameObject ("Tile Holder");
			tiles = new Tile[H+1,W+1];
			isFirstMap = true;
		}

		for (int row = 0; row < H; row++) {
			for (int col = 0; col < W; col++) {
				currentPos = new Vector2(row,col);
				if (isFirstMap) {
					tiles [row, col] = tileHolder.AddComponent<Tile> ();//new Tile ();
					tiles [row, col].Initialise (currentPos, row, col);
				} else {					
					tiles [row, col].Initialise (currentPos, row, col);
				}
			}
		}

		for (int row = 0; row < H; row++) {
			for (int col = 0; col < W; col++) {
				tiles [row, col].AddNeighbors(W-1,H-1);
			}
		}
	}
}
