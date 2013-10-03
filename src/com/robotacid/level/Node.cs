using System;
using System.Collections.Generic;
using flash;

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
		public Boolean visited;
		public Vector<Node> connections;
		public Vector<bool> connectionsActive;
		public Boolean drop;
		
		public Node(int x, int y) {
			this.x = x;
			this.y = y;
			visited = false;
			connections = new Vector<Node>();
			connectionsActive = new Vector<Boolean>();
			drop = false;
		}
		
	}

}