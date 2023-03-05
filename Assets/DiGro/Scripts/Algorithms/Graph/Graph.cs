using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace DiGro.Algorithms.Graph {

	public enum Direction { Up, Right, Down, Left }

	public interface INode<Value> {
		Value value { get; set; }
		ref Value valueRef();
		NodeInfo<Value> GetLink(Direction dir);
		bool HasLink(Direction dir);
		bool HasLinkTo(NodeInfo<Value> nodeInfo);
	}

	public class Node<Value> : INode<Value> {
		public NodeInfo<Value>[] links = new NodeInfo<Value>[4];
		private Value m_value;

		public NodeInfo<Value> info { get; }
		public ref Value valueRef() { return ref m_value; }
		public Value value {
			get { return m_value; }
			set { m_value = value; }
		}

		public Node() {
			info = new NodeInfo<Value>(this);
		}

		public void SetLink(Direction dir, NodeInfo<Value> nodeInfo) {
			if (links == null)
				links = new NodeInfo<Value>[4];
			links[(int)dir] = nodeInfo;
		}
		public NodeInfo<Value> GetLink(Direction dir) {
			return links[(int)dir];
		}
		public bool HasLink(Direction dir) {
			return links[(int)dir] != null;
		}
		public bool HasLinkTo(NodeInfo<Value> nodeIndex) {
			foreach (var node in links)
				if (node == nodeIndex)
					return true;
			return false;
		}
		public void RemoveLink(Direction dir) {
			links[(int)dir] = null;
		}
		public void RemoveLinkTo(NodeInfo<Value> nodeIndex) {
			for (int i = 0; i < links.Length; i++)
				if (links[i] == nodeIndex)
					links[i] = null;
		}
	}

	public class NodeInfo<Value> : INode<Value> {
		private Node<Value> m_node;

		public NodeInfo(Node<Value> node) { m_node = node; }
		public ref Value valueRef() { return ref m_node.valueRef(); }
		public Value value {
			get { return m_node.value; }
			set { m_node.value = value; }
		}
		public NodeInfo<Value> GetLink(Direction dir) { return m_node.GetLink(dir); }
		public bool HasLink(Direction dir) { return m_node.HasLink(dir); }
		public bool HasLinkTo(NodeInfo<Value> nodeInfo) { return m_node.HasLinkTo(nodeInfo); }
	}

	public class Graph<Value> {

		#region Fields

		private List<Node<Value>> m_nodes = new List<Node<Value>>();
		private Dictionary<NodeInfo<Value>, int> m_infos = new Dictionary<NodeInfo<Value>, int>();

		#endregion Fields
		#region PublicMethods

		public Direction ReverseDirection(Direction dir) {
			int rDir = ((int)dir + 2) % 4;
			return (Direction)rDir;
		}

		public int Count() { return m_nodes.Count; }

		public NodeInfo<Value> CreateNode() {
			Node<Value> node = new Node<Value>();
			int index = m_nodes.Count;
			m_nodes.Add(node);
			m_infos.Add(node.info, index);
			return node.info;
		}

		public bool Contains(NodeInfo<Value> nodeInfo) {
			return m_infos.Keys.Contains(nodeInfo);
		}

		public void SetLink(NodeInfo<Value> from, NodeInfo<Value> to, Direction direction) {
			CheckNodeInfo(from);
			CheckNodeInfo(to);

			int fromIndex = m_infos[from];
			int toIndex = m_infos[to];
			m_nodes[fromIndex].SetLink(direction, to);
			m_nodes[toIndex].SetLink(ReverseDirection(direction), from);
		}

		public void RemoveNode(NodeInfo<Value> nodeInfo) {
			CheckNodeInfo(nodeInfo);

			int index = m_infos[nodeInfo];
			for (int i = m_nodes[index].links.Length - 1; i >= 0; i--) {
				Direction dir = (Direction)i;
				if (m_nodes[index].HasLink(dir)) {
					int linked = m_infos[m_nodes[index].GetLink(dir)];
					m_nodes[index].RemoveLink(dir);
					m_nodes[linked].RemoveLink(ReverseDirection(dir));
				}
			}
			foreach (Node<Value> n in m_nodes) {
				for (int i = 0; i < n.links.Length; i++)
					if (m_infos[n.links[i]] > index)
						m_infos[n.links[i]]--;
			}
			foreach (NodeInfo<Value> value in m_infos.Keys) {
				if (m_infos[value] > index)
					m_infos[value]--;
			}
			m_nodes.RemoveAt(index);
			m_infos.Remove(nodeInfo);
		}

		public void RemoveLinks(NodeInfo<Value> first, NodeInfo<Value> second) {
			int firstIndex = m_infos[first];
			int secondIndex = m_infos[second];
			m_nodes[firstIndex].RemoveLinkTo(second);
			m_nodes[secondIndex].RemoveLinkTo(first);
		}

		public NodeInfo<Value>[] getAdjacent(NodeInfo<Value> nodeInfo) {
			CheckNodeInfo(nodeInfo);

			int index = m_infos[nodeInfo];
			List<NodeInfo<Value>> adjacent = new List<NodeInfo<Value>>();
			for (int i = 0; i < m_nodes[index].links.Length; i++) {
				Direction dir = (Direction)i;
				if (m_nodes[index].HasLink(dir)) {
					int linked = m_infos[m_nodes[index].GetLink(dir)];
					adjacent.Add(m_nodes[linked].info);
				}
			}
			return adjacent.ToArray();
		}

		public delegate void Action(NodeInfo<Value> k);
		public delegate bool BinPred(NodeInfo<Value> a, NodeInfo<Value> b);

		public void forEach(Action action, int index = 0) {
			CheckIndex(index);
			for (int i = index; i < m_nodes.Count; i++)
				action(m_nodes[i].info);
		}

		public void traverse(NodeInfo<Value> nodeInfo, BinPred transitionCondition, Action visitAction) {
			CheckNodeInfo(nodeInfo);

			int index = m_infos[nodeInfo];
			
			bool[] visited = Enumerable.Repeat(false, m_nodes.Count).ToArray();
			Queue<int> q = new Queue<int>();
			q.Enqueue(index);

			while (q.Count > 0) {
				int current = q.Dequeue();

				if (visited[current])
					continue;

				visitAction(m_nodes[current].info);
				visited[current] = true;

				NodeInfo<Value>[] links = m_nodes[current].links;
				for (int i = 0; i < links.Length; i++) {
					if (links[i] != null) {
						int next = m_infos[links[i]];
						bool canTransition = transitionCondition(m_nodes[current].info, m_nodes[next].info);
						if (!visited[next] && canTransition)
							q.Enqueue(next);
					}
				}
			}
		}

		#endregion PublicMethods
		#region PrivateMethods

		private void CheckIndex(int index) {
			if (index < 0 || index >= m_nodes.Count)
				throw new IndexOutOfRangeException("Bad node index.");
		}

		private void CheckNodeInfo(NodeInfo<Value> info) {
			if(!Contains(info))
				throw new IndexOutOfRangeException("Graph not contains specified node.");
		}

		#endregion PrivateMethods

	}
}