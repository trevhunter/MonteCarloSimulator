# Monte carlo sports clip simulator

This is an example of a solution to a lab that calls for an efficient monty
carlo simulation of determining the probabiltiy of randomly generating 
a movie from a sequence of clips where two adjacent clips share the same 
person (principal).

### Some Notes:
* Only minimal data is used by the simulation (names or secondary 
data set isn't read). This means the assumption is 
that the same person doesn't appear under more than one principal id.

* A number of algorithms are presented in the Algorithms folder. The
fastest one is currently selected in code.

### Results
On a 4 processor Intel Xeon E5-2673 v3 @ 2.4GHz (Azure VM) = 1.8m iterations/sec 
On a 4 processor Intel core i7-3667U @ 3GHz (Laptop) = 1.3m iterations/sec
