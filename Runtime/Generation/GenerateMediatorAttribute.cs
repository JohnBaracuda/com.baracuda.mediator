using System;
using System.Runtime.CompilerServices;

namespace Baracuda.Bedrock.Generation
{
    [AttributeUsage(
        AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Struct | AttributeTargets.Enum,
        AllowMultiple = true)]
    public class GenerateMediatorAttribute : Attribute
    {
        public MediatorTypes MediatorTypes { get; }

        public string FilePath { get; }

        /// <summary>
        ///     Optional namespace override for the generated mediator file.
        /// </summary>
        public string NameSpace { get; set; }

        /// <summary>
        ///     Optional subfolder.
        /// </summary>
        public string Subfolder { get; set; } = "Mediator";

        public GenerateMediatorAttribute(MediatorTypes mediatorTypes, [CallerFilePath] string filePath = default)
        {
            MediatorTypes = mediatorTypes;
            FilePath = filePath;
        }

        public GenerateMediatorAttribute([CallerFilePath] string filePath = default)
        {
            MediatorTypes = MediatorTypes.Everything;
            FilePath = filePath;
        }
    }
}