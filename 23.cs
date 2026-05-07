using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
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
    private int size;
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
        size = 0;
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
        size = 0;
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
        return size == 0;
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
        size++;

        if (size > table.Length * load)
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
                size--;
                return entry.Value;
            }

            prev = entry;
            entry = entry.Next;
        }

        return default(V);
    }

    public int Size()
    {
        return size;
    }

    public int Capacity
    {
        get { return table.Length; }
    }
}
public enum VarType
{
    Int,
    Float,
    Double
}
public class VarInfo
{
    public VarType Type;
    public string Value; 
    public VarInfo(VarType type, string value)
    {
        Type = type;
        Value = value;
    }
    public string TypeToString()
    {
        if (Type == VarType.Int)
        {
            return "int";
        }
        else if (Type == VarType.Float)
        {
            return "float";
        }
        else
        {
            return "double";
        }
    }
}
public class Variable
{
    private MyHashMap<string, VarInfo> variables;
    private List<string> errors;
    private static readonly Regex DefinitionPattern = new Regex(@"([a-zA-Z_][a-zA-Z0-9_]*)\s+([a-zA-Z_][a-zA-Z0-9_]*)\s*=\s*(\d+)\s*;",RegexOptions.Compiled);

    public Variable()
    {
        variables = new MyHashMap<string, VarInfo>();
        errors = new List<string>();
    }
    private bool TryParseType(string typeName, out VarType type)
    {
        if (typeName == "int")
        {
            type = VarType.Int;
            return true;
        }
        else if (typeName == "float")
        {
            type = VarType.Float;
            return true;
        }
        else if (typeName == "double")
        {
            type = VarType.Double;
            return true;
        }
        else
        {
            type = VarType.Int;
            return false;
        }
    }
    private string ReadFile(string fileName)
    {
        if (!File.Exists(fileName))
        {
            throw new FileNotFoundException("Файл не найден: " + fileName);
        }

        StringBuilder sb = new StringBuilder();
        using (StreamReader reader = new StreamReader(fileName))
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                sb.Append(line);
                sb.Append(' ');
            }
        }
        return sb.ToString();
    }
    public void ProcessFile(string fileName)
    {
        string content = ReadFile(fileName);
        MatchCollection matches = DefinitionPattern.Matches(content);

        foreach (Match match in matches)
        {
            string typeName = match.Groups[1].Value;
            string varName = match.Groups[2].Value;
            string varValue = match.Groups[3].Value;

            VarType type;
            if (!TryParseType(typeName, out type))
            {
                errors.Add("Некорректный тип '" + typeName + "': " + match.Value);
                continue;
            }
            if (variables.ContainsKey(varName))
            {
                errors.Add("Переопределение переменной '" + varName + "' в определении: " + match.Value +" (оставлено первое определение)");
                continue;
            }
            VarInfo info = new VarInfo(type, varValue);
            variables.Put(varName, info);
        }
    }
    public void WriteResult(string outputFileName)
    {
        using (StreamWriter writer = new StreamWriter(outputFileName))
        {
            ISet<KeyValuePair<string, VarInfo>> entries = variables.EntrySet();
            List<KeyValuePair<string, VarInfo>> list =
                new List<KeyValuePair<string, VarInfo>>(entries);

            list.Sort(delegate (KeyValuePair<string, VarInfo> a, KeyValuePair<string, VarInfo> b)
            {
                return string.Compare(a.Key, b.Key, StringComparison.Ordinal);
            });

            foreach (KeyValuePair<string, VarInfo> entry in list)
            {
                string line = entry.Value.TypeToString() + " => " + entry.Key +"(" + entry.Value.Value + ")";
                writer.WriteLine(line);
            }
        }
    }

    public void Print()
    {
        if (errors.Count == 0)
        {
            Console.WriteLine("Ошибок и предупреждений нет.");
            return;
        }

        Console.WriteLine("Ошибк");
        foreach (string error in errors)
        {
            Console.WriteLine(error);
        }
    }
    public MyHashMap<string, VarInfo> GetVariables()
    {
        return variables;
    }
    public List<string> GetErrors()
    {
        return errors;
    }
}
public class Program
{
    public static void Main()
    {
        string inFile = "input.txt";
        string outFile = "output.txt";

        if (!File.Exists(inFile))
        {
            Console.WriteLine("Файл " + inFile + " не найден.");
            return;
        }
        string[] lines = File.ReadAllLines(inFile);
        for (int i = 0; i < lines.Length; i++)
        {
            Console.WriteLine(lines[i]);
        }
        Console.WriteLine();
        Variable parser = new Variable();
        parser.ProcessFile(inFile);

        parser.Print();
        Console.WriteLine();
        parser.WriteResult(outFile);
        Console.WriteLine("Результат записан в файл ");
        Console.WriteLine();

        Console.WriteLine("Содержимое файла " + outFile);
        string[] outputLines = File.ReadAllLines(outFile);
        for (int i = 0; i < outputLines.Length; i++)
        {
            Console.WriteLine(outputLines[i]);
        }
        Console.WriteLine();
    }
}
