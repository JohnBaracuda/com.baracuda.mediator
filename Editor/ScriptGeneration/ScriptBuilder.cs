using Baracuda.Utilities;
using Baracuda.Utilities.Pools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Baracuda.Bedrock.Editor.ScriptGeneration
{
    public class ScriptBuilder
    {
        #region Properties

        private string NameSpace { get; set; }
        private string Name { get; set; }
        private string BaseClass { get; set; } = typeof(object).FullName;
        private ClassKeywords ClassKeywords { get; set; }
        private List<string> Dependencies { get; } = new();
        private AccessibilityModifiers Accessibility { get; set; } = AccessibilityModifiers.Public;
        private List<MethodBuilder> Methods { get; } = new();

        #endregion


        #region Builder Methods

        public static ScriptBuilder CreateClass()
        {
            return new ScriptBuilder();
        }

        public ScriptBuilder WithNameSpace(string nameSpace)
        {
            Dependencies.Remove(nameSpace);
            var previousNameSpace = NameSpace;
            NameSpace = nameSpace;
            if (previousNameSpace.IsNotNullOrWhitespace())
            {
                Dependencies.Add(previousNameSpace);
            }
            return this;
        }

        public ScriptBuilder WithName(string className)
        {
            Name = className;
            return this;
        }

        public ScriptBuilder WithKeywords(ClassKeywords keywords)
        {
            ClassKeywords = keywords;
            return this;
        }

        public ScriptBuilder WithBaseClass(Type baseClass)
        {
            BaseClass = GetGenericTypeName(baseClass);
            return WithDependency(baseClass);
        }

        public ScriptBuilder WithAccessibility(AccessibilityModifiers accessibility)
        {
            Accessibility = accessibility;
            return this;
        }

        public ScriptBuilder WithDependency(Type type)
        {
            if (NameSpace == type.Namespace)
            {
                return this;
            }
            Dependencies.AddUnique(type.Namespace);
            return this;
        }

        public ScriptBuilder WithDependency(Type[] types)
        {
            foreach (var type in types)
            {
                WithDependency(type);
            }
            return this;
        }

        public MethodBuilder AddMethod()
        {
            var methodBuilder = new MethodBuilder(this);
            Methods.Add(methodBuilder);
            return methodBuilder;
        }

        #endregion


        #region Building

        public string Build()
        {
            var stringBuilder = new StringBuilder();

            // Dependencies
            foreach (var dependency in Dependencies)
            {
                stringBuilder.Append("using ");
                stringBuilder.Append(dependency);
                stringBuilder.Append(';');
                stringBuilder.Append(Environment.NewLine);
            }
            stringBuilder.AppendIf(Environment.NewLine, Dependencies.Any());

            // NameSpace
            var hasNameSpace = NameSpace.IsNotNullOrWhitespace();
            if (hasNameSpace)
            {
                stringBuilder.Append("namespace ");
                stringBuilder.Append(NameSpace);
                stringBuilder.Append(Environment.NewLine);
                stringBuilder.Append('{');
                stringBuilder.Append(Environment.NewLine);
            }

            var indent = hasNameSpace ? new string(' ', 4) : string.Empty;

            // Type Definition
            stringBuilder.Append(indent);
            stringBuilder.Append(Accessibility.AsStringSanitizeForType());
            stringBuilder.Append(' ');
            foreach (var classKeywords in EnumUtility<ClassKeywords>.GetFlagsValueArray(ClassKeywords))
            {
                stringBuilder.Append(classKeywords.ToString().ToLower());
                stringBuilder.Append(' ');
            }
            stringBuilder.Append("class");
            stringBuilder.Append(' ');
            stringBuilder.Append(Name);
            stringBuilder.Append(' ');
            stringBuilder.Append(':');
            stringBuilder.Append(' ');
            stringBuilder.Append(BaseClass);
            stringBuilder.Append(Environment.NewLine);
            stringBuilder.Append(indent);
            stringBuilder.Append('{');
            stringBuilder.Append(Environment.NewLine);

            // Methods
            foreach (var methodBuilder in Methods)
            {
                var method = methodBuilder.Build(hasNameSpace ? 8 : 4);
                stringBuilder.Append(method);
                stringBuilder.Append(Environment.NewLine);
            }

            stringBuilder.Append(indent);
            stringBuilder.Append('}');
            stringBuilder.Append(Environment.NewLine);

            if (hasNameSpace)
            {
                indent = string.Empty;
                stringBuilder.Append(indent);
                stringBuilder.Append('}');
            }

            return stringBuilder.ToString();
        }

        #endregion


        #region Helper

        private string GetGenericTypeName(Type type)
        {
            WithDependency(type);

            if (type.IsGenericType)
            {
                var builder = StringBuilderPool.Get();
                var argBuilder = StringBuilderPool.Get();

                var arguments = type.GetGenericArguments();

                foreach (var typeArg in arguments)
                {
                    WithDependency(typeArg);
                    var typeArgumentName = GetGenericTypeName(typeArg);
                    typeArgumentName = ToKeyword(typeArgumentName);

                    if (argBuilder.Length > 0)
                    {
                        argBuilder.AppendFormat(", {0}", typeArgumentName);
                    }
                    else
                    {
                        argBuilder.Append(typeArgumentName);
                    }
                }

                if (argBuilder.Length > 0)
                {
                    builder.AppendFormat("{0}<{1}>", type.FullName!.Split('`')[0], argBuilder);
                }

                var retType = builder.ToString();

                StringBuilderPool.Release(builder);
                StringBuilderPool.Release(argBuilder);
                return retType.Replace('+', '.');
            }

            var returnValue = type.FullName!.Replace('+', '.');
            return returnValue;
        }

        private static string ToKeyword(string typeName)
        {
            return typeName switch
            {
                "Int32" => "int",
                "Int16" => "short",
                "Int64" => "long",
                "Byte" => "byte",
                "SByte" => "sbyte",
                "Single" => "float",
                "Double" => "double",
                "Decimal" => "decimal",
                "Boolean" => "bool",
                "Char" => "char",
                "String" => "string",
                var _ => typeName
            };
        }

        #endregion
    }
}