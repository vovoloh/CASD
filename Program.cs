using System;
using System.Collections.Generic;
public class MyTreeMap<K, V>
{
    private class Node
    {
        public K Key;
        public V Value;
        public int Height = 1;
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
    //3 метод clear() для удаления всех пар «ключ-значение»
    public void Clear()
    {
        root = null;
        count = 0;
    }
    //4 метод containsKey(object key) для проверки, содержит ли отображение указанный ключ
    public bool ContainsKey(K key)
    {
        return GetNode(key) != null;
    }
    //5 метод containsValue(object value) для проверки, содержит ли отображение указанное значение
    public bool ContainsValue(V value)
    {
        return ContainsValue(root, value);
    }
    private bool ContainsValue(Node node, V value)
    {
        if (node == null)
        {
            return false;
        }
        if (AreEqual(value, node.Value))
        {
            return true;
        }
        return ContainsValue(node.Left, value) || ContainsValue(node.Right, value);
    }
    // 6 метод entrySet() для возврата множества (Set) всех пар «ключ-значение» 
    public ISet<KeyValuePair<K, V>> EntrySet()
    {
        var set = new HashSet<KeyValuePair<K, V>>();
        CollectEntries(root, set);
        return set;
    }
    // 7 метод get(object key) для возврата значения, связанного с указанным ключом, или null, если ключ не найден
    public V Get(K key)
    {
        var node = GetNode(key);
        if (node != null)
        {
            return node.Value;
        }
        return default(V);
    }
    // 8 метод isEmpty() для проверки, является ли отображение пустым
    public bool IsEmpty()
    {
        return count == 0;
    }
    private bool AreEqual(V a, V b)
    {
        if (a == null)
        {
            return b == null;
        }
        return a.Equals(b);
    }
    // 9 метод keySet() для возврата множества (Set) всех ключей в отображении
    public ISet<K> KeySet()
    {
        var set = new HashSet<K>();
        CollectKeys(root, set);
        return set;
    }
    // 10 метод put(K key, V value) для добавления пары «ключ-значение»
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
    // 11 метод remove(object key) для удаления пары «ключ-значение»
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
    // 12 метод size() для возврата количества пар «ключ-значение»
    public int Size()
    {
        return count;
    }
    // 13 метод firstKey() для возврата первого ключа отображения
    public K FirstKey()
    {
        if (root == null)
        {
            throw new InvalidOperationException("Empty tree");
        }
        return GetMin(root).Key;
    }
    // 14 метод lastKey() для возврата последнего ключа отображения
    public K LastKey()
    {
        if (root == null)
        {
            throw new InvalidOperationException("Empty tree");
        }
        return GetMax(root).Key;
    }
    // 15 метод headMap(K end) для возврата сортированного отображения, содержащего элементы, ключ которых меньше end
    public MyTreeMap<K, V> HeadMap(K end)
    {
        var map = new MyTreeMap<K, V>(comparer);
        AddToHeadMap(root, end, map);
        return map;
    }
    // 16 метод subMap(K start, K end) для возврата отображения, содержащего элементы, чей ключ больше или равен start и меньше end
    public MyTreeMap<K, V> SubMap(K start, K end)
    {
        var map = new MyTreeMap<K, V>(comparer);
        AddToSubMap(root, start, end, map);
        return map;
    }
    // 17 метод tailMap(K start) для возврата сортированного отображения, содержащего элементы, ключ которых больше start
    public MyTreeMap<K, V> TailMap(K start)
    {
        var map = new MyTreeMap<K, V>(comparer);
        AddToTailMap(root, start, map);
        return map;
    }
    // 18 метод lowerEntry(K key) для возврата пары «ключ-значение», где ключ меньше заданного;
    public KeyValuePair<K, V>? LowerEntry(K key)
    {
        var node = LowerNode(root, key);
        if (node != null)
        {
            return new KeyValuePair<K, V>(node.Key, node.Value);
        }
        return null;
    }
    // 19 метод floorEntry(K key) для возврата пары «ключ-значение», где ключ меньше или равен заданному
    public KeyValuePair<K, V>? FloorEntry(K key)
    {
        var node = FloorNode(root, key);
        if (node != null)
        {
            return new KeyValuePair<K, V>(node.Key, node.Value);
        }
        return null;
    }
    // 20 метод higherEntry(K key) для возврата пары «ключ-значение», где ключ больше заданного
    public KeyValuePair<K, V>? HigherEntry(K key)
    {
        var node = HigherNode(root, key);
        if (node != null)
        {
            return new KeyValuePair<K, V>(node.Key, node.Value);
        }
        return null;
    }
    // 21 метод ceilingEntry(K key) для возврата пары «ключ-значение», где ключ больше или равен заданному
    public KeyValuePair<K, V>? CeilingEntry(K key)
    {
        var node = CeilingNode(root, key);
        if (node != null)
        {
            return new KeyValuePair<K, V>(node.Key, node.Value);
        }
        return null;
    }
    // 22 метод lowerKey(K key) для возврата ключа, который меньше заданного
    public K LowerKey(K key)
    {
        var node = LowerNode(root, key);
        if (node != null)
        {
            return node.Key;
        }
        return default(K);
    }
    // 23 метод floorKey(K key) для возврата ключа, который меньше или равен заданному
    public K FloorKey(K key)
    {
        var node = FloorNode(root, key);
        if (node != null)
        {
            return node.Key;
        }
        return default(K);
    }
    // 24 метод higherKey(K key) для возврата ключа, который больше заданного
    public K HigherKey(K key)
    {
        var node = HigherNode(root, key);
        if (node != null)
        {
            return node.Key;
        }
        return default(K);
    }
    // 25 метод ceilingKey(K key) для возврата ключа, который больше или равен заданному
    public K CeilingKey(K key)
    {
        var node = CeilingNode(root, key);
        if (node != null)
        {
            return node.Key;
        }
        return default(K);
    }
    // 26 метод pollFirstEntry() для удаления и возврата первого элемента отображения
    public KeyValuePair<K, V>? PollFirstEntry()
    {
        var entry = FirstEntry();
        if (entry != null)
        {
            Remove(entry.Value.Key);
        }
        return entry;
    }
    // 27 метод pollLastEntry() для удаления и возврата последнего элемента отображения
    public KeyValuePair<K, V>? PollLastEntry()
    {
        var entry = LastEntry();
        if (entry != null)
        {
            Remove(entry.Value.Key);
        }
        return entry;
    }
    // 28 метод firstEntry() для возврата первого элемента отображения без удаления
    public KeyValuePair<K, V>? FirstEntry()
    {
        if (root == null)
        {
            return null;
        }
        var min = GetMin(root);
        return new KeyValuePair<K, V>(min.Key, min.Value);
    }
    // 29 метод lastEntry() для возврата последнего элемента отображения без удаления
    public KeyValuePair<K, V>? LastEntry()
    {
        if (root == null)
        {
            return null;
        }
        var max = GetMax(root);
        return new KeyValuePair<K, V>(max.Key, max.Value);
    }
    private int Compare(K a, K b)
    {
        return comparer.Compare(a, b);
    }
    private int Height(Node node)
    {
        if (node != null)
        {
            return node.Height;
        }
        return 0;
    }
    private void UpdateHeight(Node node)
    {
        if (node != null)
        {
            node.Height = 1 + Math.Max(Height(node.Left), Height(node.Right));
        }
    }
    private int BalanceFactor(Node node)
    {
        if (node != null)
        {
            return Height(node.Left) - Height(node.Right);
        }
        return 0;
    }
    private Node RotateRight(Node y)
    {
        var x = y.Left;
        var T2 = x.Right;
        x.Right = y;
        y.Left = T2;
        UpdateHeight(y);
        UpdateHeight(x);
        return x;
    }
    private Node RotateLeft(Node x)
    {
        var y = x.Right;
        var T2 = y.Left;
        y.Left = x;
        x.Right = T2;
        UpdateHeight(x);
        UpdateHeight(y);
        return y;
    }
    private Node Balance(Node node)
    {
        if (node == null)
        {
            return null;
        }
        UpdateHeight(node);
        int bf = BalanceFactor(node);

        if (bf > 1)
        {
            if (BalanceFactor(node.Left) < 0)
            {
                node.Left = RotateLeft(node.Left);
            }
            return RotateRight(node);
        }
        if (bf < -1)
        {
            if (BalanceFactor(node.Right) > 0)
            {
                node.Right = RotateRight(node.Right);
            }
            return RotateLeft(node);
        }
        return node;
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
        return Balance(node);
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
            if (node.Left == null || node.Right == null)
            {
                if (node.Left != null)
                {
                    return node.Left;
                }
                else
                {
                    return node.Right;
                }
            }
            else
            {
                var successor = GetMin(node.Right);
                node.Key = successor.Key;
                node.Value = successor.Value;
                node.Right = Delete(node.Right, successor.Key);
            }
        }
        return Balance(node);
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
    private Node GetMin(Node node)
    {
        while (node != null && node.Left != null)
        {
            node = node.Left;
        }
        return node;
    }
    private Node GetMax(Node node)
    {
        while (node != null && node.Right != null)
        {
            node = node.Right;
        }
        return node;
    }
    private Node FloorNode(Node node, K key)
    {
        Node result = null;
        while (node != null)
        {
            int cmp = Compare(key, node.Key);
            if (cmp >= 0)
            {
                result = node;
                if (cmp == 0)
                {
                    break;
                }
                node = node.Right;
            }
            else
            {
                node = node.Left;
            }
        }
        return result;
    }
    private Node CeilingNode(Node node, K key)
    {
        Node result = null;
        while (node != null)
        {
            int cmp = Compare(key, node.Key);
            if (cmp <= 0)
            {
                result = node;
                if (cmp == 0)
                {
                    break;
                }
                node = node.Left;
            }
            else
            {
                node = node.Right;
            }
        }
        return result;
    }
    private Node LowerNode(Node node, K key)
    {
        Node result = null;
        while (node != null)
        {
            int cmp = Compare(key, node.Key);
            if (cmp > 0)
            {
                result = node;
                node = node.Right;
            }
            else
            {
                node = node.Left;
            }
        }
        return result;
    }
    private Node HigherNode(Node node, K key)
    {
        Node result = null;
        while (node != null)
        {
            int cmp = Compare(key, node.Key);
            if (cmp < 0)
            {
                result = node;
                node = node.Left;
            }
            else
            {
                node = node.Right;
            }
        }
        return result;
    }
    private void AddToHeadMap(Node node, K end, MyTreeMap<K, V> map)
    {
        if (node == null)
        {
            return;
        }

        if (Compare(node.Key, end) < 0)
        {
            AddToHeadMap(node.Left, end, map);
            map.Put(node.Key, node.Value);
            AddToHeadMap(node.Right, end, map);
        }
        else
        {
            AddToHeadMap(node.Left, end, map);
        }
    }
    private void AddToTailMap(Node node, K start, MyTreeMap<K, V> map)
    {
        if (node == null)
        {
            return;
        }

        if (Compare(node.Key, start) >= 0)
        {
            AddToTailMap(node.Left, start, map);
            map.Put(node.Key, node.Value);
            AddToTailMap(node.Right, start, map);
        }
        else
        {
            AddToTailMap(node.Right, start, map);
        }
    }
    private void AddToSubMap(Node node, K start, K end, MyTreeMap<K, V> map)
    {
        if (node == null)
        {
            return;
        }

        if (Compare(node.Key, start) >= 0 && Compare(node.Key, end) < 0)
        {
            AddToSubMap(node.Left, start, end, map);
            map.Put(node.Key, node.Value);
            AddToSubMap(node.Right, start, end, map);
        }
        else if (Compare(node.Key, start) < 0)
        {
            AddToSubMap(node.Right, start, end, map);
        }
        else
        {
            AddToSubMap(node.Left, start, end, map);
        }
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
public class Program
{
    public static void Main()
    {
        var map = new MyTreeMap<int, string>();
        map.Put(10, "десять");
        map.Put(5, "пять");
        map.Put(15, "пятнадцать");
        map.Put(3, "три");
        map.Put(7, "семь");
        map.Put(12, "двенадцать");
        Console.WriteLine($"Размер: {map.Size()}, Первый ключ: {map.FirstKey()}, Последний ключ: {map.LastKey()}");
        Console.WriteLine("\nНавигация:");
        Console.WriteLine($"Floor(8): [{map.FloorEntry(8)?.Key}, {map.FloorEntry(8)?.Value}]");
        Console.WriteLine($"Ceiling(8): [{map.CeilingEntry(8)?.Key}, {map.CeilingEntry(8)?.Value}]");
        Console.WriteLine($"Lower(7): [{map.LowerEntry(7)?.Key}, {map.LowerEntry(7)?.Value}]");
        Console.WriteLine($"Higher(7): [{map.HigherEntry(7)?.Key}, {map.HigherEntry(7)?.Value}]");
        Console.WriteLine("\nДиапазоны:");
        Console.Write("До 12: ");
        foreach (var entry in map.HeadMap(12).EntrySet())
            Console.Write($"[{entry.Key}] ");

        Console.Write("\nОт 7: ");
        foreach (var entry in map.TailMap(7).EntrySet())
            Console.Write($"[{entry.Key}] ");

        Console.Write("\n[5, 15): ");
        foreach (var entry in map.SubMap(5, 15).EntrySet())
            Console.Write($"[{entry.Key}] ");
        Console.WriteLine("\n\nУдаление:");
        map.Remove(5);
        Console.WriteLine($"После удаления 5: размер = {map.Size()}");
        map.Clear();
        Console.WriteLine($"После очистки: размер = {map.Size()}");
    }
}
