using System;
using System.Runtime.CompilerServices;

namespace Baracuda.Bedrock.Generation
{
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public class GenerateMediatorForAttribute : Attribute
    {
        public MediatorTypes MediatorTypes { get; set; } = MediatorTypes.Everything;
        public string NameSpace { get; set; } = null;
        public string Subfolder { get; set; }
        public Type[] Types { get; }
        public string FilePath { get; }

        public GenerateMediatorForAttribute(Type type, [CallerFilePath] string filePath = default)
        {
            Types = new[] {type};
            FilePath = filePath;
        }

        public GenerateMediatorForAttribute(Type type1, Type type2, [CallerFilePath] string filePath = default)
        {
            Types = new[] {type1, type2};
            MediatorTypes = MediatorTypes.Everything;
            FilePath = filePath;
        }

        public GenerateMediatorForAttribute(Type type1, Type type2, Type type3,
            [CallerFilePath] string filePath = default)
        {
            Types = new[] {type1, type2, type3};
            MediatorTypes = MediatorTypes.Everything;
            FilePath = filePath;
        }

        public GenerateMediatorForAttribute(Type type1, Type type2, Type type3, Type type4,
            [CallerFilePath] string filePath = default)
        {
            Types = new[] {type1, type2, type3, type4};
            MediatorTypes = MediatorTypes.Everything;
            FilePath = filePath;
        }

        public GenerateMediatorForAttribute(Type type1, Type type2, Type type3, Type type4, Type type5,
            [CallerFilePath] string filePath = default)
        {
            Types = new[] {type1, type2, type3, type4, type5};
            MediatorTypes = MediatorTypes.Everything;
            FilePath = filePath;
        }
    }
}