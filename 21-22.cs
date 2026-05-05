using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
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
    public ISet<KeyValuePair<K, V>> EntrySet()
    {
        HashSet<KeyValuePair<K, V>> set = new HashSet<KeyValuePair<K, V>>();

        for (int i = 0; i < table.Length; i++)
        {
            Entry entry = table[i];
            while (entry != null)
            {
                set.Add(new KeyValuePair<K, V>(entry.Key, entry.Value));
                entry = entry.Next;
            }
        }

        return set;
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
    public bool IsEmpty()
    {
        return count == 0;
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
    public int Capacity
    {
        get { return table.Length; }
    }
}
public class HtmlTag
{
    private MyHashMap<string, int> count;
    private static readonly Regex TagPattern = new Regex(@"<(/?[a-zA-Z][a-zA-Z0-9]*)>", RegexOptions.Compiled);
    public HtmlTag()
    {
        count = new MyHashMap<string, int>();
    }

    private string NormalizeTag(string tag)
    {
        string inner = tag.Substring(1, tag.Length - 2);
        if (inner.StartsWith("/"))
        {
            inner = inner.Substring(1);
        }
        return inner.ToLower();
    }
    public void ProcessLine(string line)
    {
        MatchCollection matches = TagPattern.Matches(line);
        foreach (Match match in matches)
        {
            string fullTag = match.Value;
            string norm = NormalizeTag(fullTag);
            int cur = count.Get(norm);
            count.Put(norm, cur + 1);
        }
    }
    public void ReadFile(string fileName)
    {
        if (!File.Exists(fileName))
        {
            throw new FileNotFoundException("Файл не найден: " + fileName);
        }

        using (StreamReader reader = new StreamReader(fileName))
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                ProcessLine(line);
            }
        }
    }
    public MyHashMap<string, int> GetTag()
    {
        return count;
    }
}
public class Program
{
    public static void Main()
    {
        string fileName = "input.txt";
        Console.WriteLine(fileName);
        string[] lines = File.ReadAllLines(fileName);
        for (int i = 0; i < lines.Length; i++)
        {
            Console.WriteLine(lines[i]);
        }
        Console.WriteLine();

        HtmlTag file = new HtmlTag();
        file.ReadFile(fileName);

        Console.WriteLine();
        MyHashMap<string, int> map = file.GetTag();
        Console.WriteLine("Содержит ключ 'html' " + map.ContainsKey("html"));
        Console.WriteLine("Значение для 'div': " + map.Get("div"));
        Console.WriteLine("Размер карты: " + map.Size());
        Console.WriteLine("Карта пуста " + map.IsEmpty());

        Console.WriteLine();
        Console.WriteLine("Все уникальные теги:");
        foreach (string key in map.KeySet())
        {
            Console.Write("<" + key + "> ");
        }
        Console.WriteLine();
    }
}
