using System;
using System.Collections.Generic;

namespace DiGro.Algorithms {

    public class PriorityQueue<Key, Value> where Key : IComparable<Key> {
        private List<Pair<Key, Value>> data;


        public PriorityQueue() {
            data = new List<Pair<Key, Value>>();
        }

        public PriorityQueue(int capacity) {
            data = new List<Pair<Key, Value>>(capacity);
        }

        public void push(Key p, Value v) {
            data.Add(new Pair<Key, Value>(p, v));
            updateHeap(0, data.Count);
        }

        public Value top() {
            if (data.Count == 0)
                throw new IndexOutOfRangeException("PriorityQueue is empty.");
            return data[0].second;
        }

        public Key topPriority() {
            if (data.Count == 0)
                throw new IndexOutOfRangeException("PriorityQueue is empty.");
            return data[0].first;
        }

        public void pop() {
            if (data.Count != 0) {
                data.RemoveAt(0);
                makeHeap(0, data.Count);
            }
        }

        public bool empty {
            get {
                return data.Count == 0;
            }
        }

        public int count {
            get {
                return data.Count;
            }
        }

        /**
         * end - индекс после последнего элемента
         * **/
        // O(NlogN)
        private void makeHeap(int begin, int end) {
            int size = end - begin;
            if (size < 2)
                return;
            int iMidle = size / 2 - 1;

            for (int midle = begin + iMidle; ; midle--) {
                makeHeap(begin, midle, end);
                if (midle == begin)
                    break;
            }
        }

        private void makeHeap(int begin, int midle, int end) {
            int size = (int)(end - begin);
            int iMidle = midle - begin;
            if (iMidle > size / 2 - 1)
                return;
            int iLeft = (int)(iMidle) * 2 + 1;
            int left = begin + iLeft;
            int right = left + 1;
            int max = midle;
            if (data[left].first.CompareTo(data[max].first) < 0)
                max = left;
            if (right != end && data[right].first.CompareTo(data[max].first) < 0)
                max = right;
            if (max != midle) {
                var t = data[midle];
                data[midle] = data[max];
                data[max] = t;
                makeHeap(begin, max, end);
            }
        }

        // O(logN)
        private void updateHeap(int begin, int end) {
            updateHeap(begin, end - 1, end);
        }

        private void updateHeap(int begin, int inserted, int end) {
            if (end <= begin || inserted == end)
                return;
            int perent = begin + ((inserted - begin) - 1) / 2;
            for (int it = inserted;
                 perent >= begin && data[it].first.CompareTo(data[perent].first) < 0;
                 it = perent, perent = begin + ((perent - begin) - 1) / 2) {
                var t = data[it];
                data[it] = data[perent];
                data[perent] = t;
            }
            updateHeap(begin, ++inserted, end);
        }

    }

}