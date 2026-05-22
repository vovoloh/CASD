using System;
using System.Collections;
using System.Collections.Generic;
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

    public MyHashMap() : this(16, 0.75f)
    {
    }

    public MyHashMap(int initialCapacity) : this(initialCapacity, 0.75f)
    {
    }

    public MyHashMap(int initialCapacity, float loadFactor)
    {
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
        return Math.Abs(hashCode) % length;
    }
    private bool KeysEqual(K a, K b)
    {
        if (a == null)
        {
            return b == null;
        }
        return a.Equals(b);
    }
    public void Clear()
    {
        for (int i = 0; i < table.Length; i++)
        {
            table[i] = null;
        }
        count = 0;
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
            if (entry.Hash ==hash && KeysEqual(entry.Key, k))
            {
                return true;
            }
            entry = entry.Next;
        }

        return false;
    }
    public ISet<K> KeySet()
    {
        HashSet<K> set = new HashSet<K>();

        for (int i = 0; i < table.Length; i++)
        {
            Entry entry = table[i];
            while (entry != null)
            {
                set.Add(entry.Key);
                entry = entry.Next;
            }
        }
        return set;
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
            Entry entry =oldTable[i];

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
    public int Size()
    {
        return count;
    }
    public bool IsEmpty()
    {
        return count == 0;
    }
}
public class MyHashSet<E> : IEnumerable<E>
{
    private MyHashMap<E, object> map;
    private static readonly object PRESENT = new object();
    public MyHashSet()
    {
        map = new MyHashMap<E, object>();
    }
    public MyHashSet(E[] a)
    {
        map = new MyHashMap<E, object>();
        AddAll(a);
    }
    public MyHashSet(int initialCapacity, float loadFactor)
    {
        map = new MyHashMap<E, object>(initialCapacity, loadFactor);
    }

    public MyHashSet(int initialCapacity)
    {
        map = new MyHashMap<E, object>(initialCapacity);
    }
    public bool Add(E e)
    {
        return map.Put(e, PRESENT) == null;
    }
    public void AddAll(E[] a)
    {
        foreach (E e in a)
        {
            Add(e);
        }
    }
    public void Clear()
    {
        map.Clear();
    }
    public bool Contains(object o)
    {
        return map.ContainsKey(o);
    }
    public bool ContainsAll(E[] a)
    {
        foreach (E e in a)
        {
            if (!Contains(e))
            {
                return false;
            }
        }
        return true;
    }
    public bool IsEmpty()
    {
        return map.IsEmpty();
    }
    public bool Remove(object o)
    {
        return map.Remove(o) != null;
    }
    public void RemoveAll(E[] a)
    {
        foreach (E e in a)
        {
            Remove(e);
        }
    }
    public void RetainAll(E[] a)
    {

        HashSet<E> toKeep = new HashSet<E>(a);
        List<E> toRemove = new List<E>();
        foreach (E e in this)
        {
            if (!toKeep.Contains(e))
            {
                toRemove.Add(e);
            }
        }
        foreach (E e in toRemove)
        {
            Remove(e);
        }
    }
    public int Size()
    {
        return map.Size();
    }

    public E[] ToArray()
    {
        ISet<E> keys = map.KeySet();
        E[] res = new E[keys.Count];
        int i = 0;
        foreach (E key in keys)
        {
            res[i++] = key;
        }
        return res;
    }
    public E[] ToArray(E[] a)
    {
        E[] arr = ToArray();
        if (a == null || a.Length < arr.Length)
        {
            return arr;
        }
        Array.Copy(arr, 0, a, 0, arr.Length);
        if (a.Length > arr.Length)
        {
            a[arr.Length] = default(E);
        }

        return a;
    }
    public IEnumerator<E> GetEnumerator()
    {
        foreach (E key in map.KeySet())
        {
            yield return key;
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
        MyHashSet<int> set = new MyHashSet<int>();
        Console.WriteLine("Добав элементы 1, 2, 3, 4, 5");
        set.Add(1);
        set.Add(2);
        set.Add(3);
        set.Add(4);
        set.Add(5);
        Console.WriteLine("Размер: " + set.Size());
        Console.WriteLine("Пусто?? " + set.IsEmpty());
        Console.WriteLine();
        Console.WriteLine("добавить еще раз 2");
        bool added = set.Add(2);
        Console.WriteLine("Было ли добавлено 2 " + added);
    }
}
