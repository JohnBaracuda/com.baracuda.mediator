using System;

namespace Baracuda.Bedrock.Statistics
{
    [Serializable]
    public class StatData
    {
        public string guid;
        public Modification type;
        public string name;
        public string description;
    }

    [Serializable]
    public class StatData<T> : StatData
    {
        public T value;

        public StatData(string guid, string name, string description, T value, Modification type)
        {
            this.guid = guid;
            this.name = name;
            this.description = description;
            this.value = value;
            this.type = type;
        }

        public override string ToString()
        {
            return string.Concat(name, " ", value.ToString());
        }

        private StatData()
        {
        }

        public static readonly StatData<T> Empty = new();
    }
}