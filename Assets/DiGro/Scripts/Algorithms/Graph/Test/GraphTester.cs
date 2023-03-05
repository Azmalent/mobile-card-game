using System;
using System.Collections.Generic;
using UnityEngine;

using DiGro.Algorithms.Graph;


public class GraphTester : MonoBehaviour {

	[Serializable]
	public class Value {
		public int index = 0;
		public int color = 0;

		public Value(int index, int color = 0) {
			this.index = index;
			this.color = color;
		}
	}

	public Graph<Value> graph = new Graph<Value>();
	public List<NodeInfo<Value>> nodes = new List<NodeInfo<Value>>();

	[Header("Set values")]
	public List<Value> values = new List<Value>();
	public bool i_setValues = false;
	[Header("Search line")]
	public int startIndex = 0;
	public bool i_searchLine = false;

	private void Awake() {
		for (int i = 0; i < 12; i++) {
			var node = graph.CreateNode();
			node.value = new Value(i, 0);
			nodes.Add(node);
		}
		SetLink(0, 3, Direction.Down);
		SetLink(1, 2, Direction.Right);
		SetLink(2, 3, Direction.Right);
		SetLink(3, 4, Direction.Right);
		SetLink(1, 5, Direction.Down);
		SetLink(3, 6, Direction.Down);
		SetLink(5, 7, Direction.Down);
		SetLink(6, 9, Direction.Down);
		SetLink(7, 8, Direction.Right);
		SetLink(8, 9, Direction.Right);
		SetLink(9, 10, Direction.Right);
		SetLink(9, 11, Direction.Down);

		PrintGraph();
	}

	private void SetLink(int from, int to, Direction dir) {
		graph.SetLink(nodes[from], nodes[to], dir);
	}

	private void Update() {
		if (i_setValues) {
			i_setValues = false;
			for (int i = 0; i < nodes.Count; i++)
				nodes[i].value = new Value(i, 0);
			for (int i = 0; i < values.Count; i++) {
				if (values[i].index < nodes.Count)
					nodes[values[i].index].value.color = values[i].color;
			}
			values.Clear();
			PrintGraph();
		}
		if (i_searchLine) {
			i_searchLine = false;

			List<NodeInfo<Value>> connectedNodes = new List<NodeInfo<Value>>();

			graph.traverse(
				nodes[startIndex],
				delegate (NodeInfo<Value> a, NodeInfo<Value> b) {
					return a.value.color != 0 && b.value.color != 0 && a.value.color == b.value.color;
				},
				delegate (NodeInfo<Value> node) {
					if (node.value.color != 0)
						connectedNodes.Add(node);
				}
			);

			string str = "";
			foreach (var node in connectedNodes)
				str += node.value.index.ToString() + ", ";
			Debug.Log(str);
		}
	}

	private bool TraverseCondition(NodeInfo<Value> a, NodeInfo<Value> b) {
		return a.value.color == b.value.color;
	}
	private void TraverseAction(NodeInfo<Value> node) {

	}

	private void PrintGraph() {
		string str = "";
		graph.forEach(delegate (NodeInfo<Value> node) {
			str += node.value.index.ToString() + "(" + node.value.color.ToString() + "): ";
			for (int i = 0; i < 4; i++) {
				var dir = (Direction)i;
				if (node.HasLink(dir))
					str += node.GetLink(dir).value.index.ToString() + ", ";
			}
			str += "\n";
		});

		Debug.Log(str);
	}

}
