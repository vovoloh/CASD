using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

public class MyTreeMap<K, V>
{
    private class Node
    {
        public K Key;
        public V Value;
        public Node Left;
        public Node Right;
        public Node(K key, V value)
        {
            Key = key;
            Value = value;
        }
    }

    private Node root;
    private int count;
    private readonly IComparer<K> comparer;
    public MyTreeMap() : this(Comparer<K>.Default)
    {
    }

    public MyTreeMap(IComparer<K> comp)
    {
        if (comp == null)
        {
            comparer = Comparer<K>.Default;
        }
        else
        {
            comparer = comp;
        }
    }
    private int Compare(K a, K b)
    {
        return comparer.Compare(a, b);
    }

    public V Get(K key)
    {
        var node = GetNode(key);
        if (node != null)
        {
            return node.Value;
        }
        return default(V);
    }

    public V Put(K key, V value)
    {
        var node = GetNode(key);
        if (node != null)
        {
            var old = node.Value;
            node.Value = value;
            return old;
        }
        root = Insert(root, key, value);
        count++;
        return default(V);
    }
    public bool ContainsKey(K key)
    {
        return GetNode(key) != null;
    }
    public V Remove(K key)
    {
        if (!ContainsKey(key))
        {
            return default(V);
        }
        var old = Get(key);
        root = Delete(root, key);
        count--;
        return old;
    }

    private Node Insert(Node node, K key, V value)
    {
        if (node == null)
        {
            return new Node(key, value);
        }
        int cmp = Compare(key, node.Key);
        if (cmp < 0)
        {
            node.Left = Insert(node.Left, key, value);
        }
        else if (cmp > 0)
        {
            node.Right = Insert(node.Right, key, value);
        }
        else
        {
            node.Value = value;
        }
        return node;
    }

    private Node Delete(Node node, K key)
    {
        if (node == null)
        {
            return null;
        }
        int cmp = Compare(key, node.Key);
        if (cmp < 0)
        {
            node.Left = Delete(node.Left, key);
        }
        else if (cmp > 0)
        {
            node.Right = Delete(node.Right, key);
        }
        else
        {
            if (node.Left == null)
            {
                return node.Right;
            }
            if (node.Right == null)
            {
                return node.Left;
            }
            var successor = node.Right;
            while (successor.Left != null)
            {
                successor = successor.Left;
            }
            node.Key = successor.Key;
            node.Value = successor.Value;
            node.Right = Delete(node.Right, successor.Key);
        }
        return node;
    }

    private Node GetNode(K key)
    {
        var current = root;
        while (current != null)
        {
            int cmp = Compare(key, current.Key);
            if (cmp == 0)
            {
                return current;
            }
            else if (cmp < 0)
            {
                current = current.Left;
            }
            else
            {
                current = current.Right;
            }
        }
        return null;
    }
    private void CollectKeys(Node node, HashSet<K> set)
    {
        if (node == null)
        {
            return;
        }
        CollectKeys(node.Left, set);
        set.Add(node.Key);
        CollectKeys(node.Right, set);
    }

    private void CollectEntries(Node node, HashSet<KeyValuePair<K, V>> set)
    {
        if (node == null)
        {
            return;
        }
        CollectEntries(node.Left, set);
        set.Add(new KeyValuePair<K, V>(node.Key, node.Value));
        CollectEntries(node.Right, set);
    }
}
public class MyHashMap<K, V>
{
    private class Entry
    {
        public K Key;
        public V Value;
        public int Hash;
        public Entry Next;

        public Entry(K key, V value, int hash, Entry next)
        {
            Key = key;
            Value = value;
            Hash = hash;
            Next = next;
        }
    }

    private Entry[] table;
    private int count;
    private readonly float load;
    private const int Default_Cap = 16;
    private const float DefaultLoad = 0.75f;

    public MyHashMap() : this(Default_Cap, DefaultLoad)
    {

    }
    public MyHashMap(int initialCapacity) : this(initialCapacity, DefaultLoad)
    {

    }

    public MyHashMap(int initialCapacity, float loadFactor)
    {
        if (initialCapacity <= 0)
        {
            throw new ArgumentException("Начальная ёмкость должна быть положительной.");
        }
        table = new Entry[initialCapacity];
        this.load = loadFactor;
        count = 0;
    }
    private int Hash(K key)
    {
        if (key == null)
        {
            return 0;
        }
        return key.GetHashCode();
    }
    private int IndexFor(int hashCode, int length)
    {
        return (hashCode & 0x7FFFFFFF) % length;
    }
    private bool KeysEqual(K a, K b)
    {
        if (a == null)
        {
            return b == null;
        }
        return a.Equals(b);
    }
    private bool ValuesEqual(V a, V b)
    {
        if (a == null)
        {
            return b == null;
        }
        return a.Equals(b);
    }
    public bool ContainsKey(object key)
    {
        if (!(key is K) && key != null)
        {
            return false;
        }

        K k = (K)key;
        int hash = Hash(k);
        int index = IndexFor(hash, table.Length);

        Entry entry = table[index];
        while (entry != null)
        {
            if (entry.Hash == hash && KeysEqual(entry.Key, k))
            {
                return true;
            }
            entry = entry.Next;
        }

        return false;
    }
    public bool ContainsValue(object value)
    {
        V v;
        if (value == null)
        {
            v = default(V);
        }
        else if (value is V)
        {
            v = (V)value;
        }
        else
        {
            return false;
        }

        for (int i = 0; i < table.Length; i++)
        {
            Entry entry = table[i];
            while (entry != null)
            {
                if (ValuesEqual(entry.Value, v))
                {
                    return true;
                }
                entry = entry.Next;
            }
        }

        return false;
    }
    public V Get(object key)
    {
        if (!(key is K) && key != null)
        {
            return default(V);
        }

        K k = (K)key;
        int hash = Hash(k);
        int index = IndexFor(hash, table.Length);

        Entry entry = table[index];
        while (entry != null)
        {
            if (entry.Hash == hash && KeysEqual(entry.Key, k))
            {
                return entry.Value;
            }
            entry = entry.Next;
        }

        return default(V);
    }

    public V Put(K key, V value)
    {
        int hash = Hash(key);
        int index = IndexFor(hash, table.Length);
        Entry entry = table[index];
        while (entry != null)
        {
            if (entry.Hash == hash && KeysEqual(entry.Key, key))
            {
                V oldValue = entry.Value;
                entry.Value = value;
                return oldValue;
            }
            entry = entry.Next;
        }
        AddEntry(hash, key, value, index);
        return default(V);
    }
    private void AddEntry(int hash, K key, V value, int index)
    {
        Entry head = table[index];
        table[index] = new Entry(key, value, hash, head);
        count++;
        if (count > table.Length * load)
        {
            Resize(table.Length * 2);
        }
    }
    private void Resize(int newCapacity)
    {
        Entry[] oldTable = table;
        Entry[] newTable = new Entry[newCapacity];

        for (int i = 0; i < oldTable.Length; i++)
        {
            Entry entry = oldTable[i];

            while (entry != null)
            {
                Entry next = entry.Next;
                int newIndex = IndexFor(entry.Hash, newCapacity);
                entry.Next = newTable[newIndex];
                newTable[newIndex] = entry;

                entry = next;
            }
        }

        table = newTable;
    }

    public V Remove(object key)
    {
        if (!(key is K) && key != null)
        {
            return default(V);
        }

        K k = (K)key;
        int hash = Hash(k);
        int index = IndexFor(hash, table.Length);

        Entry entry = table[index];
        Entry prev = null;

        while (entry != null)
        {
            if (entry.Hash == hash && KeysEqual(entry.Key, k))
            {
                if (prev == null)
                {
                    table[index] = entry.Next;
                }
                else
                {
                    prev.Next = entry.Next;
                }
                count--;
                return entry.Value;
            }

            prev = entry;
            entry = entry.Next;
        }

        return default(V);
    }
}
public class Program
{
    private const int n = 20;
    private static readonly int[] Sizes = { 100000, 1000000, 10000000 };

    private static Random random = new Random(42);
    private static int[] GenerateRandomKeys(int size)
    {
        HashSet<int> set = new HashSet<int>();
        while (set.Count < size)
            set.Add(random.Next());
        int[] keys = new int[size];
        set.CopyTo(keys);
        return keys;
    }

    static void TestPut(int size)
    {
        int[] keys = GenerateRandomKeys(size);

        double hashMapTime = 0;
        for (int j = 0; j < n; j++)
        {
            MyHashMap<int, int> map = new MyHashMap<int, int>();
            Stopwatch sw = Stopwatch.StartNew();
            for (int i = 0; i < size; i++)
                map.Put(keys[i], i);
            sw.Stop();
            hashMapTime += sw.Elapsed.TotalMilliseconds;
        }
        hashMapTime /= n;
        Console.WriteLine($"MyHashMap Put: {hashMapTime:F2} мс");

        double treeMapTime = 0;
        for (int j = 0; j < n; j++)
        {
            MyTreeMap<int, int> map = new MyTreeMap<int, int>();
            Stopwatch sw = Stopwatch.StartNew();
            for (int i = 0; i < size; i++)
                map.Put(keys[i], i);
            sw.Stop();
            treeMapTime += sw.Elapsed.TotalMilliseconds;
        }
        treeMapTime /= n;
        Console.WriteLine($"MyTreeMap Put: {treeMapTime:F2} мс");
    }

    static void TestGet(int size)
    {
        int[] keys = GenerateRandomKeys(size);
        MyHashMap<int, int> hashMap = new MyHashMap<int, int>();
        for (int i = 0; i < size; i++)
        {
            hashMap.Put(keys[i], i);
        }

        double hashMapTime = 0;
        for (int j = 0; j < n; j++)
        {
            Stopwatch sw = Stopwatch.StartNew();
            for (int i = 0; i < size; i++)
                hashMap.Get(keys[i]);
            sw.Stop();
            hashMapTime += sw.Elapsed.TotalMilliseconds;
        }
        hashMapTime /= n;
        Console.WriteLine($"MyHashMap Get: {hashMapTime:F2} мс");

        MyTreeMap<int, int> treeMap = new MyTreeMap<int, int>();
        for (int i = 0; i < size; i++)
            treeMap.Put(keys[i], i);

        double treeMapTime = 0;
        for (int j = 0; j < n; j++)
        {
            Stopwatch sw = Stopwatch.StartNew();
            for (int i = 0; i < size; i++)
                treeMap.Get(keys[i]);
            sw.Stop();
            treeMapTime += sw.Elapsed.TotalMilliseconds;
        }
        treeMapTime /= n;
        Console.WriteLine($"MyTreeMap Get: {treeMapTime:F2} мс");
    }

    static void TestRemove(int size)
    {
        int[] keys = GenerateRandomKeys(size);

        double hashMapTime = 0;
        for (int j = 0; j < n; j++)
        {
            MyHashMap<int, int> map = new MyHashMap<int, int>();
            for (int i = 0; i < size; i++)
                map.Put(keys[i], i);
            Stopwatch sw = Stopwatch.StartNew();
            for (int i = 0; i < size; i++)
                map.Remove(keys[i]);
            sw.Stop();
            hashMapTime += sw.Elapsed.TotalMilliseconds;
        }
        hashMapTime /= n;
        Console.WriteLine($"MyHashMap Remove: {hashMapTime:F2} мс");

        double treeMapTime = 0;
        for (int j = 0; j < n; j++)
        {
            MyTreeMap<int, int> map = new MyTreeMap<int, int>();
            for (int i = 0; i < size; i++)
                map.Put(keys[i], i);
            Stopwatch sw = Stopwatch.StartNew();
            for (int i = 0; i < size; i++)
                map.Remove(keys[i]);
            sw.Stop();
            treeMapTime += sw.Elapsed.TotalMilliseconds;
        }
        treeMapTime /= n;
        Console.WriteLine($"MyTreeMap Remove: {treeMapTime:F2} мс");
    }

    static void Main()
    {
        Console.WriteLine($"Количество запусков для усреднения: {n}");
        Console.WriteLine("Размеры: 100000, 1000000, 10000000");

        foreach (int size in Sizes)
        {
            TestPut(size);
            TestGet(size);
            TestRemove(size);
        }
        Console.ReadKey();
    }
}
