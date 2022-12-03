// 
// LTRLib
// 
// Copyright (c) Olof Lagerkvist, LTR Data
// http://ltr-data.se   https://github.com/LTRData
// 

#if NET40_OR_GREATER || NETSTANDARD2_1_OR_GREATER || NETCOREAPP

using System;
using System.Reflection;
using System.Reflection.Emit;

namespace LTRLib.Compiler;

public class TypeBuilder
{
    private static ModuleBuilder? Mdl;

    private static readonly object SyncObj = new();

    public string? TypeName;
    public MethodAttributes DefaultConstructorAttributes = MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName;
    public CustomAttributeInfo[]? CustomAttributes;
    public FieldInfo[]? Fields;

    public record CustomAttributeInfo(Type CustomAttributeType, object[] CustomAttributeParameters)
    {
        public ConstructorInfo CreateConstructorInfo()
            => CustomAttributeType.GetConstructor(Array.ConvertAll(CustomAttributeParameters, param => param.GetType()))
            ?? throw new InvalidOperationException("Failed to generate constructor");
    }

    public record FieldInfo(CustomAttributeInfo[] CustomAttributes, string FieldName, Type FieldType, FieldAttributes FieldAttributes);

    public Type CreateType()
    {
        lock (SyncObj)
        {
            if (Mdl is null)
            {
#if NETFRAMEWORK
                Mdl = AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName("DynamicTypes"), AssemblyBuilderAccess.Run).DefineDynamicModule("DynamicTypes");
#else
                Mdl = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName("DynamicTypes"), AssemblyBuilderAccess.Run).DefineDynamicModule("DynamicTypes");
#endif
            }
        }

        {
            var withBlock = Mdl.DefineType(TypeName
                ?? throw new InvalidOperationException("Type name required"));

            if (CustomAttributes is not null)
            {
                foreach (var CustomAttribute in CustomAttributes)
                {
                    withBlock.SetCustomAttribute(new CustomAttributeBuilder(CustomAttribute.CreateConstructorInfo(), CustomAttribute.CustomAttributeParameters));
                }
            }

            withBlock.DefineDefaultConstructor(MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName);

            if (Fields is not null)
            {
                foreach (var Field in Fields)
                {
                    {
                        var withBlock1 = withBlock.DefineField(Field.FieldName, Field.FieldType, Field.FieldAttributes);
                        foreach (var CustomAttribute in Field.CustomAttributes)
                        {
                            withBlock1.SetCustomAttribute(new CustomAttributeBuilder(CustomAttribute.CreateConstructorInfo(), CustomAttribute.CustomAttributeParameters));
                        }
                    }
                }
            }

            return withBlock.CreateType()
                ?? throw new InvalidOperationException("Failed to build dynamic class");
        }
    }
}

#endif
