
// 
// LTRLib
// 
// Copyright (c) Olof Lagerkvist, LTR Data
// http://ltr-data.se   https://github.com/LTRData
// 

#if NET45_OR_GREATER || NETSTANDARD || NETCOREAPP

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.InteropServices;
using LTRLib.Extensions;

namespace LTRLib.IO;

public abstract partial class CsvReader : MarshalByRefObject, IEnumerable, IEnumerator, IDisposable
{
    protected readonly char[] _Delimiters = [','];

    protected readonly char[] _Textquotes = ['"'];

    public TextReader BaseReader { get; }

    public ReadOnlyCollection<char> Delimiters => new(_Delimiters);

    public ReadOnlyCollection<char> TextQuotes => new(_Textquotes);

    protected abstract object? IEnumerable_Current { get; }

    object? IEnumerator.Current => IEnumerable_Current;

    protected CsvReader(TextReader Reader, char[]? Delimiters, char[]? TextQuotes)
    {
        if (Delimiters is not null)
        {
            _Delimiters = Delimiters.CreateTypedClone();
        }

        if (TextQuotes is not null)
        {
            _Textquotes = TextQuotes.CreateTypedClone();
        }

        BaseReader = Reader;
    }

    public static CsvReader<T> Create<T>(string FilePath) where T : class, new() => new(FilePath);

    public static CsvReader<T> Create<T>(string FilePath, char[] delimiters) where T : class, new() => new(FilePath, delimiters);

    public static CsvReader<T> Create<T>(string FilePath, char[] delimiters, char[] textquotes) where T : class, new() => new(FilePath, delimiters, textquotes);

    public static CsvReader<T> Create<T>(TextReader Reader) where T : class, new() => new(Reader);

    public static CsvReader<T> Create<T>(TextReader Reader, char[] delimiters) where T : class, new() => new(Reader, delimiters);

    public static CsvReader<T> Create<T>(TextReader Reader, char[] delimiters, char[] textquotes) where T : class, new() => new(Reader, delimiters, textquotes);

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
                BaseReader?.Dispose();

            }

            // TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.

            // TODO: set large fields to null.

        }

        disposedValue = true;

    }

    // TODO: override Finalize() only if Dispose( disposing As Boolean) above has code to free unmanaged resources.
    ~CsvReader()
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

    protected virtual IEnumerator GetEnumerator() => this;

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public abstract bool MoveNext();

    protected virtual void Reset() => throw new NotImplementedException();

    void IEnumerator.Reset() => Reset();
    #endregion

}

[ComVisible(false)]
public partial class CsvReader<T> : CsvReader, IEnumerable<T>, IEnumerator<T> where T : class, new()
{
    protected readonly Action<T, string>?[] _Properties;

    private static readonly MethodInfo _EnumParse
        = typeof(Enum).GetMethod("Parse", BindingFlags.Public | BindingFlags.Static, null, [typeof(Type), typeof(string)], null)!;

    /// <summary>Generate a specific member setter for a specific reference type</summary>
    /// <param name="member_name">The member's name as defined in <typeparamref name="T"/></param>
    /// <returns>A compiled lambda which can access (set> the member</returns>
    private static Action<T, string>? GenerateReferenceTypeMemberSetter(string member_name)
    {
        var param_this = Expression.Parameter(typeof(T), "this");

        var param_value = Expression.Parameter(typeof(string), "value");             // ' the member's new value

        var member_info = typeof(T).GetMember(member_name, BindingFlags.FlattenHierarchy | BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).FirstOrDefault(m => m is PropertyInfo || m is FieldInfo);

        Expression member;

        if (member_info is FieldInfo field_info)
        {
            if (field_info.IsInitOnly || field_info.IsLiteral)
            {
                return null;
            }

            member = Expression.Field(param_this, field_info);
        }
        else if (member_info is PropertyInfo property_info)
        {
            if (!property_info.CanWrite)
            {
                return null;
            }

            member = Expression.Property(param_this, property_info);
        }
        else
        {
            return null;
        }

        Expression assign_value;
        if (ReferenceEquals(member.Type, typeof(string)))
        {
            assign_value = param_value;
        }
        else if (member.Type.IsEnum)
        {
            assign_value = Expression.Convert(Expression.Call(_EnumParse, Expression.Constant(member.Type), param_value), member.Type);
        }
        else
        {
            var method = member.Type.GetMethod("Parse", BindingFlags.Public | BindingFlags.Static, null, [typeof(string)], null);

            if (method is not null)
            {
                assign_value = Expression.Call(method, param_value);
            }
            else
            {
                var can_convert_from_string = TypeDescriptor.GetConverter(member.Type)?.CanConvertFrom(typeof(string));
                if (!can_convert_from_string.GetValueOrDefault())
                {
                    return null;
                }

                assign_value = Expression.Convert(param_value, member.Type);
            }
        }

        assign_value = Expression.Condition(Expression.ReferenceEqual(param_value, Expression.Constant(null)), Expression.Default(member.Type), assign_value);

        var assign = Expression.Assign(member, assign_value);                // ' i.e., 'this.member_name = value'

        var lambda = Expression.Lambda<Action<T, string>>(assign, param_this, param_value);

        return lambda.Compile();
    }

    public CsvReader(string FilePath) : this(File.OpenText(FilePath))
    {
    }

    public CsvReader(string FilePath, char[] delimiters) : this(File.OpenText(FilePath), delimiters)
    {
    }

    public CsvReader(string FilePath, char[] delimiters, char[] textquotes) : this(File.OpenText(FilePath), delimiters, textquotes)
    {
    }

    public CsvReader(TextReader Reader) : this(Reader, null, null)
    {
    }

    public CsvReader(TextReader Reader, char[] delimiters) : this(Reader, delimiters, null)
    {
    }

    public CsvReader(TextReader Reader, char[]? delimiters, char[]? textquotes) : base(Reader, delimiters, textquotes)
    {
        var line = BaseReader.ReadLine()
            ?? throw new InvalidOperationException("Empty input file");

        var field_names = line.Split(_Delimiters, StringSplitOptions.None);

        _Properties = Array.ConvertAll(field_names, GenerateReferenceTypeMemberSetter);
    }

    protected override object? IEnumerable_Current => Current;

    public T Current { get; private set; } = null!;

    public override bool MoveNext()
    {
        var line = BaseReader.ReadLine();

        if (line is null)
        {
            Current = null!;
            return false;
        }

        if (line.Length == 0)
        {
            Current = null!;
            return true;
        }

        var fields = new List<string>(_Properties.Length);
        var startIdx = 0;

        while (startIdx < line.Length)
        {

            var scanIdx = startIdx;
            if (Array.IndexOf(_Textquotes, line[scanIdx]) >= 0 && scanIdx + 1 < line.Length)
            {

                scanIdx = line.IndexOfAny(_Textquotes, scanIdx + 1);

                if (scanIdx < 0)
                {
                    scanIdx = startIdx;
                }

            }

            var i = line.IndexOfAny(_Delimiters, scanIdx);

            if (i < 0)
            {
                fields.Add(line.AsSpan(startIdx).Trim(_Textquotes).ToString());
                break;
            }

            fields.Add(line.AsSpan(startIdx, i - startIdx).Trim(_Textquotes).ToString());

            startIdx = i + 1;
        }

        var obj = new T();

        for (int i = 0, loopTo = Math.Min(fields.Count - 1, _Properties.GetUpperBound(0)); i <= loopTo; i++)
        {
            _Properties[i]?.Invoke(obj, fields[i]);
        }

        Current = obj;
        
        return true;
    }

    public new IEnumerator<T> GetEnumerator() => this;

}
#endif
