# Path finder
implemented by Unity and C#


## A* pathfinding
[Demo](https://thecoderseye.github.io/demos/pathfinders/astar.html)

### Source code structure separates A* logic from its visualization
---------------

### Assets/Astar/Scripts
### `AstarAlgorithm.cs` _A* logic_

#### **StepFoward()** 
> _A* core algorithm_
> 1. Dequeue a single node from the open set
> 2. Get neighbour nodes
> 3. Evaluate each neighbour node's cost
> 4. Put neighbours to the open set
> 5. Put the dequeued node from the open set to the close set
   
#### **StartFindingPath()** 
> _Iterating StepFoward()_
> - Process all nodes in the open set until reaching the end node 
> - It stops the iteration when it reaches the end node even while the open set has remaining nodes.
> - Path to the end node does not exsist when there is no more node in the open set after iterating all the nodes on the map.

### `AstarPriorityQueue.cs` _custom priority queue_
- _Unity .net standard 2.1 does not include PriorityQueue._<br>
- It provides essential functions of PriorityQueue by wrapping **_SortedDictionary<TKey, TValue>_** class where TKey is the cost value F and TValue is either Stack<T> or Queue<T>. <br>
- Both Stack and Queue are implemented by LinkedList<T> because it frequently searches, removes, and inserts a single node due to **relocation** of the node
- PriorityQueue is used for the **open set** so that it returns a node of the lowest cost of F
- When using Stack, it returns the most recent added node among the nodes of the same cost of F
- When using Queue, it returns the oldest node among the nodes of the same cost of F

#### `AstarPriorityQueueFIFO.cs`
Stack is implemented by LinkedList<T>. It enforces its priority on the node closer to the end node so that it is expected that it possibly results in the narrower searches comparing to LIFO.

#### `AStarPriorityQueueLIFO.cs`
Queue is implemented by LinkedList<T>. It enforces dequeuing the oldest node in the open set, which is possibly closer to the start node. It is expected that it searches wider area on the map comparing to FIFO.
 

---
### Assets/Astar/Scripts/Behaviours
It contains source codes contributing rendering the map, progress of pathfinding, and UI interfaces to choose some available options which have effects on the pathfinding and the result. 