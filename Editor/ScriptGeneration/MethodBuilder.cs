using System;

namespace Baracuda.Bedrock.Editor.ScriptGeneration
{
    public class MethodBuilder
    {
        private readonly ScriptBuilder _scriptBuilder;

        public static MethodBuilder Create(ScriptBuilder scriptBuilder)
        {
            return new MethodBuilder(scriptBuilder);
        }

        internal MethodBuilder(ScriptBuilder scriptBuilder)
        {
            _scriptBuilder = scriptBuilder;
        }

        public MethodBuilder WithName(string name)
        {
            return this;
        }

        public MethodBuilder WithParameter<T>(string name)
        {
            return WithParameter(name, typeof(T));
        }

        public MethodBuilder WithParameter(string name, Type type)
        {
            return this;
        }

        public MethodBuilder WithKeyword()
        {
            return this;
        }

        public MethodBuilder WithReturnType<T>()
        {
            return WithReturnType(typeof(T));
        }

        public MethodBuilder WithReturnType(Type type)
        {
            return this;
        }

        public ScriptBuilder CompleteMethod()
        {
            return _scriptBuilder;
        }

        public string Build(int indent)
        {
            return string.Empty;
        }
    }
}