using UnityEngine;
using System.Collections.Generic;
using System;

namespace ConcaveHull {
    public class Init{

        List<Node> dot_list = new List<Node>(); //Used only for the demo

        public string seed;
        public int scaleFactor=1;
        public int number_of_dots;
        public double concavity=1;
        public bool isSquareGrid=true;

		public List<Vector3> generateHull(List<Vector3> posList)
		{
	
			setDots(posList);
            Hull.setConvHull(dot_list);
            Hull.setConcaveHull(Math.Round(System.Convert.ToDecimal(concavity), 2), scaleFactor, isSquareGrid);

			List<Vector3> newPosList=new List<Vector3>();
			for (int i = 0; i < Hull.hull_concave_edges.Count; i++)
			{
				Vector3 pos = new Vector3((float)Hull.hull_concave_edges[i].nodes[1].x, posList[0].y, (float)Hull.hull_concave_edges[i].nodes[1].y);
				newPosList.Add(pos);
			}
			Debug.Log("Hull.hull_concave_edges.Count : "+Hull.hull_concave_edges.Count);
			return newPosList;
        }

        public void setDots(int number_of_dots) {
            //Used only for the demo
            System.Random pseudorandom = new System.Random(seed.GetHashCode());
            for (int x = 0; x < number_of_dots; x++) {
                dot_list.Add(new Node(pseudorandom.Next(0, 100), pseudorandom.Next(0, 100), x));
            }
            //Delete repeated nodes
            for (int pivot_position = 0; pivot_position < dot_list.Count; pivot_position++) {
                for (int position = 0; position < dot_list.Count; position++)
                    if (dot_list[pivot_position].x == dot_list[position].x && dot_list[pivot_position].y == dot_list[position].y
                        && dot_list[pivot_position].id != dot_list[position].id) {
                        dot_list.RemoveAt(position);
                        position--;
                    }
            }
        }
		public void setDots(List<Vector3> posList)
		{
			dot_list.Clear();
			for (int i = 0; i < posList.Count; i++)
			{
				dot_list.Add(new Node(posList[i].x, posList[i].z, i));
			}
			//Delete repeated nodes
			for (int pivot_position = 0; pivot_position < dot_list.Count; pivot_position++)
			{
				for (int position = 0; position < dot_list.Count; position++)
					if (dot_list[pivot_position].x == dot_list[position].x && dot_list[pivot_position].y == dot_list[position].y
						&& dot_list[pivot_position].id != dot_list[position].id)
					{
						dot_list.RemoveAt(position);
						position--;
					}
			}
		}
        void OnDrawGizmos() {

            //Convex hull
            Gizmos.color = Color.yellow;
            for (int i = 0; i < Hull.hull_edges.Count; i++) {
                Vector2 left = new Vector2((float)Hull.hull_edges[i].nodes[0].x, (float)Hull.hull_edges[i].nodes[0].y);
                Vector2 right = new Vector2((float)Hull.hull_edges[i].nodes[1].x, (float)Hull.hull_edges[i].nodes[1].y);
                Gizmos.DrawLine(left, right);
            }
            //Concave hull
            Gizmos.color = Color.blue;
            for (int i = 0; i < Hull.hull_concave_edges.Count; i++) {
                Vector2 left = new Vector2((float)Hull.hull_concave_edges[i].nodes[0].x, (float)Hull.hull_concave_edges[i].nodes[0].y);
                Vector2 right = new Vector2((float)Hull.hull_concave_edges[i].nodes[1].x, (float)Hull.hull_concave_edges[i].nodes[1].y);
                Gizmos.DrawLine(left, right);
            }

            Gizmos.color = Color.red;
            for (int i = 0; i < dot_list.Count; i++) {
                Gizmos.DrawSphere(new Vector3((float)dot_list[i].x, (float)dot_list[i].y, 0), 0.5f);
            }            
        }
    }

}
