
// 
// LTRLib
// 
// Copyright (c) Olof Lagerkvist, LTR Data
// http://ltr-data.se   https://github.com/LTRData
// 

#if NET35_OR_GREATER || NETSTANDARD || NETCOREAPP

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.InteropServices;
using LTRLib.Extensions;

namespace LTRLib.IO;

public abstract partial class CsvWriter : MarshalByRefObject, IDisposable
{

    public TextWriter BaseWriter { get; }

    public string? Delimiter { get; }

    public string? TextQuotes { get; }

    protected CsvWriter(TextWriter Writer, string? Delimiter, string? TextQuotes)
    {
        if (Delimiter is not null)
        {
            this.Delimiter = Delimiter;
        }

        if (TextQuotes is not null)
        {
            this.TextQuotes = TextQuotes;
        }

        BaseWriter = Writer;
    }

    public static CsvWriter<T> Create<T>(string FilePath) => new(FilePath);

    public static CsvWriter<T> Create<T>(string FilePath, string delimiters) => new(FilePath, delimiters);

    public static CsvWriter<T> Create<T>(string FilePath, string delimiters, string textquotes) => new(FilePath, delimiters, textquotes);

    public static CsvWriter<T> Create<T>(TextWriter Writer) => new(Writer);

    public static CsvWriter<T> Create<T>(TextWriter Writer, string delimiters) => new(Writer, delimiters);

    public static CsvWriter<T> Create<T>(TextWriter Writer, string delimiters, string textquotes) => new(Writer, delimiters, textquotes);

    public static void Write<T>(string FilePath, IEnumerable<T> objects)
    {

        using var csvwriter = new CsvWriter<T>(FilePath);

        csvwriter.WriteAll(objects);

    }

    public static void Write<T>(string FilePath, string delimiters, IEnumerable<T> objects)
    {

        using var csvwriter = new CsvWriter<T>(FilePath, delimiters);

        csvwriter.WriteAll(objects);

    }

    public static void Write<T>(string FilePath, string delimiters, string textquotes, IEnumerable<T> objects)
    {

        using var csvwriter = new CsvWriter<T>(FilePath, delimiters, textquotes);

        csvwriter.WriteAll(objects);

    }

    public static string Convert<T>(string delimiter, string textquotes, T obj) => CsvWriter<T>.Convert(delimiter, textquotes, obj);

    public static IEnumerable<string> ConvertAll<T>(string delimiter, string textquotes, IEnumerable<T> objects)
    {

        yield return CsvWriter<T>.GetHeaderLine(delimiter);

        foreach (var obj in objects)
        {
            yield return CsvWriter<T>.Convert(delimiter, textquotes, obj);
        }
    }

    #region IDisposable Support
    private bool disposedValue; // To detect redundant calls

    // IDisposable
    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                // TODO: dispose managed state (managed objects).
                BaseWriter?.Dispose();
            }

            // TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.

            // TODO: set large fields to null.
        }

        disposedValue = true;
    }

    // TODO: override Finalize() only if Dispose( disposing As Boolean) above has code to free unmanaged resources.
    ~CsvWriter()
    {
        // Do not change this code.  Put cleanup code in Dispose( disposing As Boolean) above.
        Dispose(false);
    }

    // This code added by Visual Basic to correctly implement the disposable pattern.
    public void Dispose()
    {
        // Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    #endregion

}

[ComVisible(false)]
public partial class CsvWriter<T> : CsvWriter
{
    protected readonly partial struct PropertyDescriptor
    {
        public Type ReturnType { get; }

        public string PropertyName { get; }

        public Func<T, string> Accessor { get; }

        /// <summary>Generate a specific member setter for a specific reference type</summary>
        /// <param name="prop">The property's getter method in <typeparamref name="T"/></param>
        /// <param name="name"></param>
        public PropertyDescriptor(MethodInfo prop, string name)
        {

            ReturnType = prop.ReturnType;

            PropertyName = name;

            var param_this = Expression.Parameter(typeof(T), "this");

            var member_call = Expression.Call(param_this, prop);   // ' i.e., 'this.member_name'

            Expression value_as_string;

            if (ReferenceEquals(prop.ReturnType, typeof(string)))
            {
                value_as_string = member_call;
            }
            else
            {
                value_as_string = Expression.Call(member_call, _ToStringMethod);
            }

            if (!prop.ReturnType.IsValueType)
            {
                value_as_string = Expression.Condition(Expression.Equal(member_call, Expression.Constant(null)), Expression.Constant(string.Empty), value_as_string);
            }

            var lambda = Expression.Lambda<Func<T, string>>(value_as_string, param_this);

            Accessor = lambda.Compile();

        }


    }

    protected static readonly PropertyDescriptor[] _Properties;

    private static readonly MethodInfo _ToStringMethod = typeof(object).GetMethod("ToString", BindingFlags.Public | BindingFlags.Instance, null, Type.EmptyTypes, null)!;

    public CsvWriter(string FilePath) : this(File.CreateText(FilePath))
    {

    }

    public CsvWriter(string FilePath, string delimiters) : this(File.CreateText(FilePath), delimiters)
    {

    }

    public CsvWriter(string FilePath, string delimiters, string textquotes) : this(File.CreateText(FilePath), delimiters, textquotes)
    {

    }

    public CsvWriter(TextWriter Writer) : this(Writer, null, null)
    {

    }

    public CsvWriter(TextWriter Writer, string delimiters) : this(Writer, delimiters, null)
    {

    }

    static CsvWriter()
    {

        _Properties = (from prop in typeof(T).GetProperties()
                       where prop.CanRead && prop.GetIndexParameters().Length == 0
                       let accessor = prop.GetGetMethod()
                       where accessor is not null
                       select new PropertyDescriptor(accessor, prop.Name)).ToArray();

    }

    public CsvWriter(TextWriter Writer, string? delimiter, string? textquotes) : this(Writer, delimiter, textquotes, writeheader: true)
    {
    }

    public CsvWriter(TextWriter Writer, string? delimiter, string? textquotes, bool writeheader) : base(Writer, delimiter, textquotes)
    {
        if (writeheader)
        {
            WriteHeaderLine();
        }
    }

    public void WriteHeaderLine() => BaseWriter.WriteLine(GetHeaderLine(Delimiter));

    public static string GetHeaderLine(string? delimiter)
    {
        static string field_converter(PropertyDescriptor prop) => prop.PropertyName;

#if NET40_OR_GREATER || NETSTANDARD || NETCOREAPP
        var parsed_fields = _Properties.Select(field_converter);
#else
        var parsed_fields = Array.ConvertAll(_Properties, field_converter);
#endif

        return string.Join(delimiter, parsed_fields);

    }

    public void Write(T obj) => BaseWriter.WriteLine(Convert(Delimiter, TextQuotes, obj));

    public void WriteAll(IEnumerable<T> objects)
    {

        foreach (var obj in objects)
        {
            Write(obj);
        }
    }

    public static string Convert(string? delimiter, string? textquotes, T obj)
    {

        string field_converter(PropertyDescriptor prop)
        {
            var value = prop.Accessor(obj);

            if (!string.IsNullOrEmpty(textquotes) && !prop.ReturnType.IsValueType)
            {

                value = string.Concat(textquotes, value, textquotes);

            }

            return value;
        };

#if NET40_OR_GREATER || NETSTANDARD || NETCOREAPP
        var parsed_fields = _Properties.Select(field_converter);
#else
        var parsed_fields = Array.ConvertAll(_Properties, field_converter);
#endif

        return string.Join(delimiter, parsed_fields);

    }

}
#endif
