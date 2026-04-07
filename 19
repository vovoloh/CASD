using System;
using System.Collections;
using System.Collections.Generic;
public class MyTreeMap<K, V> : IEnumerable<KeyValuePair<K, V>>
{
    private enum Color
    {
        Red, Black
    }

    private class Node
    {
        public K Key;
        public V Value;
        public Node Left;
        public Node Right;
        public Node Parent;
        public Color Color;

        public Node(K key, V value, Node parent)
        {
            Key = key;
            Value = value;
            Parent = parent;
            Color = Color.Red;
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

    public IComparer<K> Comparer
    {
        get { return comparer; }
    }

    private int Compare(K a, K b)
    {
        return comparer.Compare(a, b);
    }

    private Color GetColor(Node node)
    {
        if (node == null)
        {
            return Color.Black;
        }
        return node.Color;
    }

    private void SetColor(Node node, Color color)
    {
        if (node != null)
        {
            node.Color = color;
        }
    }

    public void clear()
    {
        root = null;
        count = 0;
    }

    public bool containsKey(K key)
    {
        return getNode(key) != null;
    }

    public V get(K key)
    {
        Node node = getNode(key);
        if (node != null)
        {
            return node.Value;
        }
        return default(V);
    }

    public V put(K key, V value)
    {
        if (root == null)
        {
            root = new Node(key, value, null);
            root.Color = Color.Black;
            count = 1;
            return default(V);
        }

        Node current = root;
        Node parent = null;
        int cmp = 0;

        while (current != null)
        {
            parent = current;
            cmp = Compare(key, current.Key);

            if (cmp < 0)
            {
                current = current.Left;
            }
            else if (cmp > 0)
            {
                current = current.Right;
            }
            else
            {
                V old = current.Value;
                current.Value = value;
                return old;
            }
        }

        Node newNode = new Node(key, value, parent);

        if (cmp < 0)
        {
            parent.Left = newNode;
        }
        else
        {
            parent.Right = newNode;
        }

        fixAfterInsert(newNode);
        count++;
        return default(V);
    }

    public V remove(K key)
    {
        Node node = getNode(key);
        if (node == null)
        {
            return default(V);
        }

        V old = node.Value;
        deleteNode(node);
        return old;
    }

    public int size()
    {
        return count;
    }

    public bool isEmpty()
    {
        return count == 0;
    }

    public K firstKey()
    {
        Node current = root;
        while (current.Left != null)
        {
            current = current.Left;
        }
        return current.Key;
    }
    public K LastKey()
    {
        Node current = root;
        while (current.Right != null)
        {
            current = current.Right;
        }
        return current.Key;
    }

    public K ceilingKey(K key)
    {
        Node node = ceilingNode(root, key);
        if (node != null)
        {
            return node.Key;
        }
        return default(K);
    }

    public K floorKey(K key)
    {
        Node node = floorNode(root, key);
        if (node != null)
        {
            return node.Key;
        }
        return default(K);
    }

    public K higherKey(K key)
    {
        Node node = higherNode(root, key);
        if (node != null)
        {
            return node.Key;
        }
        return default(K);
    }

    public K lowerKey(K key)
    {
        Node node = lowerNode(root, key);
        if (node != null)
        {
            return node.Key;
        }
        return default(K);
    }

    public K[] keysToArray()
    {
        K[] array = new K[count];
        int index = 0;
        fillKeys(root, array, ref index);
        return array;
    }

    public K[] keysToArrayDesc()
    {
        K[] array = new K[count];
        int index = 0;
        fillKeysDesc(root, array, ref index);
        return array;
    }

    private void fillKeys(Node node, K[] array, ref int index)
    {
        if (node == null)
        {
            return;
        }

        fillKeys(node.Left, array, ref index);
        array[index++] = node.Key;
        fillKeys(node.Right, array, ref index);
    }

    private void fillKeysDesc(Node node, K[] array, ref int index)
    {
        if (node == null)
        {
            return;
        }

        fillKeysDesc(node.Right, array, ref index);
        array[index++] = node.Key;
        fillKeysDesc(node.Left, array, ref index);
    }

    private Node getNode(K key)
    {
        Node current = root;

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

    private Node MinNode(Node node)
    {
        while (node.Left != null)
        {
            node = node.Left;
        }
        return node;
    }

    private Node MaxNode(Node node)
    {
        while (node.Right != null)
        {
            node = node.Right;
        }
        return node;
    }

    private Node successor(Node node)
    {
        if (node == null)
        {
            return null;
        }

        if (node.Right != null)
        {
            return MinNode(node.Right);
        }

        Node parent = node.Parent;
        while (parent != null && node == parent.Right)
        {
            node = parent;
            parent = parent.Parent;
        }
        return parent;
    }

    private void rotateLeft(Node x)
    {
        Node y = x.Right;
        x.Right = y.Left;

        if (y.Left != null)
        {
            y.Left.Parent = x;
        }

        y.Parent = x.Parent;

        if (x.Parent == null)
        {
            root = y;
        }
        else if (x == x.Parent.Left)
        {
            x.Parent.Left = y;
        }
        else
        {
            x.Parent.Right = y;
        }

        y.Left = x;
        x.Parent = y;
    }

    private void rotateRight(Node y)
    {
        Node x = y.Left;
        y.Left = x.Right;

        if (x.Right != null)
        {
            x.Right.Parent = y;
        }

        x.Parent = y.Parent;

        if (y.Parent == null)
        {
            root = x;
        }
        else if (y == y.Parent.Left)
        {
            y.Parent.Left = x;
        }
        else
        {
            y.Parent.Right = x;
        }

        x.Right = y;
        y.Parent = x;
    }

    private void fixAfterInsert(Node z)
    {
        while (z != root && GetColor(z.Parent) == Color.Red)
        {
            if (z.Parent == z.Parent.Parent.Left)
            {
                Node y = z.Parent.Parent.Right;

                if (GetColor(y) == Color.Red)
                {
                    SetColor(z.Parent, Color.Black);
                    SetColor(y, Color.Black);
                    SetColor(z.Parent.Parent, Color.Red);
                    z = z.Parent.Parent;
                }
                else
                {
                    if (z == z.Parent.Right)
                    {
                        z = z.Parent;
                        rotateLeft(z);
                    }

                    SetColor(z.Parent, Color.Black);
                    SetColor(z.Parent.Parent, Color.Red);
                    rotateRight(z.Parent.Parent);
                }
            }
            else
            {
                Node y = z.Parent.Parent.Left;

                if (GetColor(y) == Color.Red)
                {
                    SetColor(z.Parent, Color.Black);
                    SetColor(y, Color.Black);
                    SetColor(z.Parent.Parent, Color.Red);
                    z = z.Parent.Parent;
                }
                else
                {
                    if (z == z.Parent.Left)
                    {
                        z = z.Parent;
                        rotateRight(z);
                    }

                    SetColor(z.Parent, Color.Black);
                    SetColor(z.Parent.Parent, Color.Red);
                    rotateLeft(z.Parent.Parent);
                }
            }
        }

        SetColor(root, Color.Black);
    }

    private void transplant(Node u, Node v)
    {
        if (u.Parent == null)
        {
            root = v;
        }
        else if (u == u.Parent.Left)
        {
            u.Parent.Left = v;
        }
        else
        {
            u.Parent.Right = v;
        }

        if (v != null)
        {
            v.Parent = u.Parent;
        }
    }

    private void deleteNode(Node z)
    {
        Node y = z;
        Node x;
        Node xParent;
        Color originalColor = y.Color;

        if (z.Left == null)
        {
            x = z.Right;
            xParent = z.Parent;
            transplant(z, z.Right);
        }
        else if (z.Right == null)
        {
            x = z.Left;
            xParent = z.Parent;
            transplant(z, z.Left);
        }
        else
        {
            y = MinNode(z.Right);
            originalColor = y.Color;
            x = y.Right;

            if (y.Parent == z)
            {
                xParent = y;
                if (x != null)
                {
                    x.Parent = y;
                }
            }
            else
            {
                xParent = y.Parent;
                transplant(y, y.Right);
                y.Right = z.Right;
                y.Right.Parent = y;
            }

            transplant(z, y);
            y.Left = z.Left;
            y.Left.Parent = y;
            y.Color = z.Color;
        }

        count--;

        if (originalColor == Color.Black)
        {
            fixAfterDelet(x, xParent);
        }
    }

    private void fixAfterDelet(Node x, Node parent)
    {
        while (x != root && GetColor(x) == Color.Black)
        {
            if (x == (parent != null ? parent.Left : null))
            {
                Node w = parent.Right;

                if (GetColor(w) == Color.Red)
                {
                    SetColor(w, Color.Black);
                    SetColor(parent, Color.Red);
                    rotateLeft(parent);
                    w = parent.Right;
                }

                if (GetColor(w.Left) == Color.Black && GetColor(w.Right) == Color.Black)
                {
                    SetColor(w, Color.Red);
                    x = parent;
                    parent = x.Parent;
                }
                else
                {
                    if (GetColor(w.Right) == Color.Black)
                    {
                        SetColor(w.Left, Color.Black);
                        SetColor(w, Color.Red);
                        rotateRight(w);
                        w = parent.Right;
                    }

                    SetColor(w, GetColor(parent));
                    SetColor(parent, Color.Black);
                    SetColor(w.Right, Color.Black);
                    rotateLeft(parent);
                    x = root;
                }
            }
            else
            {
                Node w = parent.Left;

                if (GetColor(w) == Color.Red)
                {
                    SetColor(w, Color.Black);
                    SetColor(parent, Color.Red);
                    rotateRight(parent);
                    w = parent.Left;
                }

                if (GetColor(w.Right) == Color.Black && GetColor(w.Left) == Color.Black)
                {
                    SetColor(w, Color.Red);
                    x = parent;
                    parent = x.Parent;
                }
                else
                {
                    if (GetColor(w.Left) == Color.Black)
                    {
                        SetColor(w.Right, Color.Black);
                        SetColor(w, Color.Red);
                        rotateLeft(w);
                        w = parent.Left;
                    }

                    SetColor(w, GetColor(parent));
                    SetColor(parent, Color.Black);
                    SetColor(w.Left, Color.Black);
                    rotateRight(parent);
                    x = root;
                }
            }
        }

        SetColor(x, Color.Black);
    }

    private Node floorNode(Node node, K key)
    {
        Node result = null;

        while (node != null)
        {
            int cmp = Compare(key, node.Key);

            if (cmp == 0)
            {
                return node;
            }

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

    private Node ceilingNode(Node node, K key)
    {
        Node result = null;

        while (node != null)
        {
            int cmp = Compare(key, node.Key);

            if (cmp == 0)
            {
                return node;
            }

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

    private Node lowerNode(Node node, K key)
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

    private Node higherNode(Node node, K key)
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

    public IEnumerator<KeyValuePair<K, V>> GetEnumerator()
    {
        Stack<Node> stack = new Stack<Node>();
        Node current = root;

        while (stack.Count > 0 || current != null)
        {
            while (current != null)
            {
                stack.Push(current);
                current = current.Left;
            }

            current = stack.Pop();
            yield return new KeyValuePair<K, V>(current.Key, current.Value);
            current = current.Right;
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
public class MyTreeSet<E> : IEnumerable<E>
{
    protected MyTreeMap<E, object> m;
    protected static readonly object PRESENT = new object();

    public MyTreeSet()
    {
        m = new MyTreeMap<E, object>();
    }
    public MyTreeSet(MyTreeMap<E, object> map)
    {
        if (map == null)
        {
            throw new ArgumentNullException("");
        }
        m = map;
    }
    public MyTreeSet(IComparer<E> comparator)
    {
        m = new MyTreeMap<E, object>(comparator);
    }

    public MyTreeSet(E[] a)
    {
        m = new MyTreeMap<E, object>();
        addAll(a);
    }

    public MyTreeSet(SortedSet<E> s)
    {
        if (s == null)
        {
            throw new ArgumentNullException("");
        }

        m = new MyTreeMap<E, object>(s.Comparer);

        foreach (E e in s)
        {
            add(e);
        }
    }

    // 1 метод add(T e) для добавления элемента в множество
    public virtual bool add(E e)
    {
        return m.put(e, PRESENT) == null;
    }

    // 2 метод addAll(T[] a) для добавления элементов из массива
    public virtual void addAll(E[] a)
    {
        if (a == null)
        {
            throw new ArgumentNullException("a");
        }

        foreach (E e in a)
        {
            add(e);
        }
    }

    // 3 метод clear() для удаления всех элементов из множества
    public virtual void clear()
    {
        m.clear();
    }

    // 4 метод contains(object o) для проверки наличия объекта в множестве
    public virtual bool contains(object o)
    {
        if (o is E)
        {
            return m.containsKey((E)o);
        }
        return false;
    }

    // 5 метод containsAll(T[] a) для проверки, содержатся ли все элементы массива во множестве
    public virtual bool containsAll(E[] a)
    {
        if (a == null)
        {
            throw new ArgumentNullException("a");
        }

        foreach (E e in a)
        {
            if (!contains(e))
            {
                return false;
            }
        }
        return true;
    }

    // 6 метод isEmpty() для проверки, пусто ли множество
    public virtual bool isEmpty()
    {
        return m.isEmpty();
    }

    // 7 метод remove(object o) для удаления элемента из множества
    public virtual bool remove(object o)
    {
        if (o is E)
        {
            return m.remove((E)o) != null;
        }
        return false;
    }

    // 8 метод removeAll(T[] a) для удаления указанных элементов
    public virtual void removeAll(E[] a)
    {
        if (a == null)
        {
            throw new ArgumentNullException("a");
        }

        foreach (E e in a)
        {
            remove(e);
        }
    }

    // 9 метод retainAll(T[] a) для оставления только указанных элементов
    public virtual void retainAll(E[] a)
    {
        if (a == null)
        {
            throw new ArgumentNullException("a");
        }

        HashSet<E> keep = new HashSet<E>(a);
        List<E> toRemove = new List<E>();

        foreach (E e in this)
        {
            if (!keep.Contains(e))
            {
                toRemove.Add(e);
            }
        }

        foreach (E e in toRemove)
        {
            remove(e);
        }
    }

    // 10 метод size() для получения количества элементов
    public virtual int size()
    {
        return m.size();
    }

    // 11 метод toArray() для возврата массива всех элементов
    public virtual E[] toArray()
    {
        return m.keysToArray();
    }

    // 12 метод toArray(T[] a) для возврата массива элементов
    public virtual E[] toArray(E[] a)
    {
        E[] result = toArray();

        if (a == null)
        {
            return result;
        }

        if (a.Length < result.Length)
        {
            return result;
        }

        Array.Copy(result, 0, a, 0, result.Length);

        if (a.Length > result.Length)
        {
            a[result.Length] = default(E);
        }

        return a;
    }

    // 13 метод first() для возврата первого элемента
    public virtual E first()
    {
        return m.firstKey();
    }

    // 14 метод last() для возврата последнего элемента
    public virtual E last()
    {
        return m.LastKey();
    }

    // 15 метод subSet(E fromElement, E toElement)
    public virtual MyTreeSet<E> subSet(E fromElement, E toElement)
    {
        MyTreeSet<E> result = new MyTreeSet<E>(m.Comparer);

        foreach (E e in this)
        {
            if (m.Comparer.Compare(e, fromElement) >= 0 && m.Comparer.Compare(e, toElement) < 0)
            {
                result.add(e);
            }
        }

        return result;
    }

    // 16 метод headSet(E toElement)
    public virtual MyTreeSet<E> headSet(E toElement)
    {
        MyTreeSet<E> result = new MyTreeSet<E>(m.Comparer);

        foreach (E e in this)
        {
            if (m.Comparer.Compare(e, toElement) < 0)
            {
                result.add(e);
            }
        }

        return result;
    }

    // 17 метод tailSet(E fromElement)
    public virtual MyTreeSet<E> tailSet(E fromElement)
    {
        MyTreeSet<E> result = new MyTreeSet<E>(m.Comparer);

        foreach (E e in this)
        {
            if (m.Comparer.Compare(e, fromElement) >= 0)
            {
                result.add(e);
            }
        }

        return result;
    }

    // 18 метод ceiling(E obj)
    public virtual E ceiling(E obj)
    {
        return m.ceilingKey(obj);
    }

    // 19 метод floor(E obj)
    public virtual E floor(E obj)
    {
        return m.floorKey(obj);
    }

    // 20 метод higher(E obj)
    public virtual E higher(E obj)
    {
        return m.higherKey(obj);
    }

    // 21 метод lower(E obj)
    public virtual E lower(E obj)
    {
        return m.lowerKey(obj);
    }

    // 22 метод headSet(E upperBound, bool incl)
    public virtual MyTreeSet<E> headSet(E upperBound, bool incl)
    {
        MyTreeSet<E> result = new MyTreeSet<E>(m.Comparer);

        foreach (E e in this)
        {
            int cmp = m.Comparer.Compare(e, upperBound);

            if (cmp < 0 || (incl && cmp == 0))
            {
                result.add(e);
            }
        }

        return result;
    }

    // 23 метод subSet(E lowerBound, bool lowIncl, E upperBound, bool highIncl)
    public virtual MyTreeSet<E> subSet(E lowerBound, bool lowIncl, E upperBound, bool highIncl)
    {
        MyTreeSet<E> result = new MyTreeSet<E>(m.Comparer);

        foreach (E e in this)
        {
            int cmpLow = m.Comparer.Compare(e, lowerBound);
            int cmpHigh = m.Comparer.Compare(e, upperBound);

            bool okLow = cmpLow > 0 || (lowIncl && cmpLow == 0);
            bool okHigh = cmpHigh < 0 || (highIncl && cmpHigh == 0);

            if (okLow && okHigh)
            {
                result.add(e);
            }
        }

        return result;
    }

    // 24 метод tailSet(E fromElement, bool inclusive)
    public virtual MyTreeSet<E> tailSet(E fromElement, bool inclusive)
    {
        MyTreeSet<E> result = new MyTreeSet<E>(m.Comparer);

        foreach (E e in this)
        {
            int cmp = m.Comparer.Compare(e, fromElement);

            if (cmp > 0 || (inclusive && cmp == 0))
            {
                result.add(e);
            }
        }

        return result;
    }

    // 25 метод pollLast()
    public virtual E pollLast()
    {
        if (isEmpty())
        {
            return default(E);
        }

        E value = last();
        remove(value);
        return value;
    }

    // 26 метод pollFirst()
    public virtual E pollFirst()
    {
        if (isEmpty())
        {
            return default(E);
        }

        E value = first();
        remove(value);
        return value;
    }

    // 27 метод descendingIterator()
    public virtual IEnumerator<E> descendingIterator()
    {
        E[] array = m.keysToArrayDesc();

        for (int i = 0; i < array.Length; i++)
        {
            yield return array[i];
        }
    }

    // 28 метод descendingSet()
    public virtual MyTreeSet<E> descendingSet()
    {
        IComparer<E> reverseComparer = Comparer<E>.Create(
            delegate (E a, E b)
            {
                return m.Comparer.Compare(b, a);
            });

        MyTreeSet<E> result = new MyTreeSet<E>(reverseComparer);
        result.addAll(this.toArray());
        return result;
    }

    public virtual IEnumerator<E> GetEnumerator()
    {
        foreach (KeyValuePair<E, object> pair in m)
        {
            yield return pair.Key;
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
public class Program
{
    public static void Main()
    {
        MyTreeSet<int> set = new MyTreeSet<int>();

        set.add(10);
        set.add(5);
        set.add(15);
        set.add(3);
        set.add(7);
        set.add(12);

        Console.WriteLine("Размер: " + set.size());
        Console.WriteLine("Первый: " + set.first());
        Console.WriteLine("Последний: " + set.last());
        Console.WriteLine("Содержит 7? " + set.contains(7));
        Console.WriteLine("ceiling(8): " + set.ceiling(8));
        Console.WriteLine("floor(8): " + set.floor(8));
        Console.WriteLine("higher(7): " + set.higher(7));
        Console.WriteLine("lower(7): " + set.lower(7));

        Console.Write("Элементы: ");
        foreach (int x in set)
        {
            Console.Write(x + " ");
        }
        Console.WriteLine();

        MyTreeSet<int> head = set.headSet(12);
        Console.Write("headSet(12): ");
        foreach (int x in head)
        {
            Console.Write(x + " ");
        }
        Console.WriteLine();

        MyTreeSet<int> tail = set.tailSet(7);
        Console.Write("tailSet(7): ");
        foreach (int x in tail)
        {
            Console.Write(x + " ");
        }
        Console.WriteLine();

        MyTreeSet<int> sub = set.subSet(5, 15);
        Console.Write("subSet(5, 15): ");
        foreach (int x in sub)
        {
            Console.Write(x + " ");
        }
        Console.WriteLine();

        Console.Write("descendingSet(): ");
        foreach (int x in set.descendingSet())
        {
            Console.Write(x + " ");
        }
        Console.WriteLine();

        Console.Write("descendingIterator(): ");
        IEnumerator<int> it = set.descendingIterator();
        while (it.MoveNext())
        {
            Console.Write(it.Current + " ");
        }
        Console.WriteLine();

        Console.WriteLine("pollFirst(): " + set.pollFirst());
        Console.WriteLine("pollLast(): " + set.pollLast());

        Console.Write("После удаления: ");
        foreach (int x in set)
        {
            Console.Write(x + " ");
        }
        Console.WriteLine();
    }
}
