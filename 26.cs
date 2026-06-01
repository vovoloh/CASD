using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

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

    public MyHashMap() : this(16, 0.75f) { }

    public MyHashMap(int initialCapacity) : this(initialCapacity, 0.75f) { }

    public MyHashMap(int initialCapacity, float loadFactor)
    {
        table = new Entry[initialCapacity];
        load = loadFactor;
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
            if (entry.Hash == hash && KeysEqual(entry.Key, k))
            {
                return true;
            }
            entry = entry.Next;
        }

        return false;
    }
    public IEnumerable<K> Keys()
    {
        for (int i = 0; i < table.Length; i++)
        {
            Entry entry = table[i];
            while (entry != null)
            {
                yield return entry.Key;
                entry = entry.Next;
            }
        }
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
    private readonly MyHashMap<E, object> map;
    private static readonly object PRESENT = new object();

    public MyHashSet()
    {
        map = new MyHashMap<E, object>();
    }

    public bool Add(E e)
    {
        return map.Put(e, PRESENT) == null;
    }

    public void Clear()
    {
        map.Clear();
    }

    public bool Contains(object o)
    {
        return map.ContainsKey(o);
    }

    public bool Remove(object o)
    {
        return map.Remove(o) != null;
    }

    public int Size()
    {
        return map.Size();
    }

    public bool IsEmpty()
    {
        return map.IsEmpty();
    }

    public E[] ToArray()
    {
        E[] result = new E[map.Size()];
        int i = 0;
        foreach (E key in map.Keys())
            result[i++] = key;
        return result;
    }

    public IEnumerator<E> GetEnumerator()
    {
        foreach (E key in map.Keys())
            yield return key;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

public class Reading : IComparable<Reading>
{
    public List<string> len { get; }
    public string words { get; }

    public Reading(string line)
    {
        words = line ?? "";
        len = new List<string>(words.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries));
        len.Sort(CompareWords);
    }
    private static int CompareWords(string a, string b)
    {
        int c = a.Length.CompareTo(b.Length);
        if (c != 0) return c;
        return string.CompareOrdinal(a, b);
    }
    public int CompareTo(Reading other)
    {
        if (other == null)
            return 1;

        int n = Math.Min(len.Count, other.len.Count);
        for (int i = 0; i < n; i++)
        {
            int c = len[i].Length.CompareTo(other.len[i].Length);
            if (c != 0)
                return c;
        }

        return len.Count.CompareTo(other.len.Count);
    }
    public override bool Equals(object obj)
    {
        return obj is Reading other && CompareTo(other) == 0;
    }
    public override int GetHashCode()
    {
        {
            int hash = 17;
            for (int i = 0; i < len.Count; i++)
            {
                hash = hash * 31 +len[i].Length; 
            }
            return hash;
        }
    }
    public override string ToString()
    {
        return words;
    }
}
public class Program
{
    public static void Main()
    {
        MyHashSet<Reading> set = new MyHashSet<Reading>();

        if (!File.Exists("input.txt"))
        {
            Console.WriteLine("Файл input.txt не найден.");
            return;
        }

        foreach (string line in File.ReadAllLines("input.txt"))
        {
            set.Add(new Reading(line));
        }

        foreach (Reading line in set)
        {
            Console.WriteLine(line);
        }
    }
}
