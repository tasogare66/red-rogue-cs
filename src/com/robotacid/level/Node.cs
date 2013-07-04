using System.Collections.Generic;

namespace com.robotacid.level {
	/**
	 * This is a graph vertex
	 * 
	 * Its edges are the connections vector which we keep the same size as
	 * the connectionsActive vector for the graph pruning stage of map building
	 * 
	 * @author Aaron Steed, robotacid.com
	 */
	public class Node{
		
		public int x;
		public int y;
		public bool visited;
		public List<Node> connections;
		public List<bool> connectionsActive;
		public bool drop;
		
		public Node(int x, int y) {
			this.x = x;
			this.y = y;
			visited = false;
			connections = new List<Node>();
			connectionsActive = new List<bool>();
			drop = false;
		}
		
	}

}