# Canopy
A Behaviour Tree Editor for Unity largely based off of [Kiwi Coder's BT editor tutorials](https://www.youtube.com/c/TheKiwiCoder).

# Features
- Create Behaviour Trees and Blackboards in Unity visually
- Extend node types via inheritence
- Basic AI perception and navigation wrappers
- Modular Blackboard focused on ease of use


# Helper Node Types
- Concurrency Node (Single Threaded): Run all children at the same time. If one fails, the whole node fails.
- Exists In Blackboard Node: If the specified entry exists in the blackboard, run the child.
- Selector Node: Randomly select a child of the node and run it.
- Sequencer Node: Runs all children in sequence. If `allChildrenMustSucceed` is true, all children must succeed for the node to succeed.

# Sample User Types
- DebugLogNode: Exactly what you think it does.
- Move To Random Nearby Pos Node: Uses AINavigation wrapper to find a nearby random position to move to.
- Move To Spotted Node: Moves to the spotted position written to the blackboard by AIPerception.
- Repeat Node: Loops child's behaviour forever.
- Stop In Place Node: Stops the agent in place.
- Wait Node: Waits for a specified amount of time before returning success.
