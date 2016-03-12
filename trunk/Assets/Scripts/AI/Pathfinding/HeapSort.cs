using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

//implement Heap Sort
class HeapSort
{
	//PUT NUMBERS HERE;
    //algorithm implementation
    public void heapSortMethod(Tile[] arr, int start, int end)
    {
        //place array in max-heap order
        //get index of last parent node
        // >> is a right shift operator - effect is to divide by 2
        int left = start + ((end - start) >> 1);
        int right = end;
        int offset = start;

        //loop until array is organized as a maz-heap
        while (left >= start)
        {
            //use the siftDown Method to move nodes into the
            //correct location for a max-heap
            siftDown(arr, offset, left, end);
            left -= 1;
        }

        //all values in max-heap order, do swaps to finish the sort
        while (right > start)
        {
            //swap the max value to the end of the array
            swap(arr, right, start);

            //decrease the heap size by 1
            right -= 1;
            siftDown(arr, offset, start, right);

        }
    }

    private void swap(Tile[] arr, int right, int start)
    {
        //swaps two values
        Tile temp = arr[right];
        arr[right] = arr[start];
        arr[start] = temp;
    }

    private void siftDown(Tile[] arr, int offset, int start, int end)
    {
        int root = start; //root of heap
        int child;

        //keep going while root has at least 1 child
        while (((root - offset) * 2 + 1) <= (end - offset))
        {
            //get left child value
            child = ((root - offset) * 2 + 1) + offset;

            //if child has a sibling and child's value < sibling's
            if (child < end && arr[child].F < arr[child + 1].F )
            {
                //point to sibling instead
                child++;
            }
            //see if selected child > parent
            if (arr[root].F < arr[child].F )
            {
                //swap the out of order values
                swap(arr, root, child);

                //repeat the sifting process
                root = child;
            }
            else
            {
                //no other values to swap
                break;
            }

        }
    }



}
