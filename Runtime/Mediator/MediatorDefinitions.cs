using Baracuda.Bedrock.Generation;
using Baracuda.Utilities.Types;
using UnityEngine;
using static Baracuda.Bedrock.Mediator.MediatorDefinitions;

[assembly: GenerateMediatorFor(typeof(bool), NameSpace = NameSpace, Subfolder = Subfolder)]
[assembly: GenerateMediatorFor(typeof(string), NameSpace = NameSpace, Subfolder = Subfolder)]
[assembly: GenerateMediatorFor(typeof(string), typeof(string), NameSpace = NameSpace, Subfolder = Subfolder)]
[assembly: GenerateMediatorFor(typeof(byte), NameSpace = NameSpace, Subfolder = Subfolder)]
[assembly: GenerateMediatorFor(typeof(int), NameSpace = NameSpace, Subfolder = Subfolder)]
[assembly: GenerateMediatorFor(typeof(int), typeof(int), NameSpace = NameSpace, Subfolder = Subfolder)]
[assembly: GenerateMediatorFor(typeof(short), NameSpace = NameSpace, Subfolder = Subfolder)]
[assembly: GenerateMediatorFor(typeof(long), NameSpace = NameSpace, Subfolder = Subfolder)]
[assembly: GenerateMediatorFor(typeof(double), NameSpace = NameSpace, Subfolder = Subfolder)]
[assembly: GenerateMediatorFor(typeof(float), NameSpace = NameSpace, Subfolder = Subfolder)]
[assembly: GenerateMediatorFor(typeof(Color), NameSpace = NameSpace, Subfolder = Subfolder)]
[assembly: GenerateMediatorFor(typeof(Color32), NameSpace = NameSpace, Subfolder = Subfolder)]
[assembly: GenerateMediatorFor(typeof(Vector2), NameSpace = NameSpace, Subfolder = Subfolder)]
[assembly: GenerateMediatorFor(typeof(Vector3), NameSpace = NameSpace, Subfolder = Subfolder)]
[assembly: GenerateMediatorFor(typeof(Vector4), NameSpace = NameSpace, Subfolder = Subfolder)]
[assembly: GenerateMediatorFor(typeof(Quaternion), NameSpace = NameSpace, Subfolder = Subfolder)]
[assembly: GenerateMediatorFor(typeof(Vector2Int), NameSpace = NameSpace, Subfolder = Subfolder)]
[assembly: GenerateMediatorFor(typeof(Vector3Int), NameSpace = NameSpace, Subfolder = Subfolder)]
[assembly: GenerateMediatorFor(typeof(LayerMask), NameSpace = NameSpace, Subfolder = Subfolder)]
[assembly: GenerateMediatorFor(typeof(Timer), NameSpace = NameSpace, Subfolder = Subfolder)]
[assembly: GenerateMediatorFor(typeof(RuntimeGUID), NameSpace = NameSpace, Subfolder = Subfolder)]
[assembly: GenerateMediatorFor(typeof(Optional<int>), NameSpace = NameSpace, Subfolder = Subfolder)]
[assembly: GenerateMediatorFor(typeof(Optional<bool>), NameSpace = NameSpace, Subfolder = Subfolder)]
[assembly: GenerateMediatorFor(typeof(Optional<float>), NameSpace = NameSpace, Subfolder = Subfolder)]
[assembly: GenerateMediatorFor(typeof(Optional<string>), NameSpace = NameSpace, Subfolder = Subfolder)]
[assembly: GenerateMediatorFor(typeof(ScriptableObject), NameSpace = NameSpace, Subfolder = Subfolder)]
[assembly: GenerateMediatorFor(typeof(Object), NameSpace = NameSpace, Subfolder = Subfolder)]
[assembly: GenerateMediatorFor(typeof(FullScreenMode), NameSpace = NameSpace, Subfolder = Subfolder)]

namespace Baracuda.Bedrock.Mediator
{
    internal static class MediatorDefinitions
    {
        public const string Subfolder = "Generated";
        public const string NameSpace = "Baracuda.Bedrock.Mediator";
    }
}