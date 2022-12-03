'
' LTRLib
' 
' Copyright (c) Olof Lagerkvist, LTR Data
' http://ltr-data.se   https://github.com/LTRData
'


#Disable Warning IDE0079 ' Remove unnecessary suppression
#Disable Warning SYSLIB0003 ' Type or member is obsolete

Imports System.Runtime.InteropServices
Imports System.Security
Imports System.Security.Permissions

Namespace LTRGeneric

    ''' <summary>
    ''' Provides IDisposable semantics to a COM object.
    ''' </summary>
    <SecurityCritical>
    <SecurityPermission(SecurityAction.Demand, Flags:=SecurityPermissionFlag.AllFlags)>
    Public MustInherit Class DisposableComWrapper
        Inherits MarshalByRefObject

        ''' <summary>
        ''' Creates a new DisposableComWrapper(Of T) object around an existing type T COM object.
        ''' </summary>
        Public Shared Function Create(Of T As Class)(target As T) As DisposableComWrapper(Of T)

            Return New DisposableComWrapper(Of T)(target)

        End Function

    End Class

    ''' <summary>
    ''' Provides IDisposable semantics to a COM object.
    ''' </summary>
    <ComVisible(False)>
    <SecurityCritical>
    <SecurityPermission(SecurityAction.Demand, Flags:=SecurityPermissionFlag.AllFlags)>
    Public Class DisposableComWrapper(Of T As Class)
        Inherits DisposableComWrapper
        Implements IDisposable

        Private _target As T

        ''' <summary>
        ''' Creates a new instance without having Target property set to any initial
        ''' object reference.
        ''' </summary>
        Public Sub New()

        End Sub

        ''' <summary>
        ''' Creates a new instance with Target property set to an existing object, if
        ''' object is of type T. If conversion fails, an exception is thrown.
        ''' </summary>
        Public Sub New(Target As Object)
            _target = DirectCast(Target, T)
        End Sub

        ''' <summary>
        ''' Creates a new instance with Target property set to an existing object of
        ''' type T.
        ''' </summary>
        Public Sub New(Target As T)
            _target = Target
        End Sub

        ''' <summary>
        ''' Gets or sets object that is encapsulated by this instance. If this property is
        ''' set it will release existing object. Set property to Nothing/null to release
        ''' object without setting a new object. This is also automatically done by
        ''' IDisposable.Dispose implementation, or by finalizer.
        ''' </summary>
        ''' <value>New object to control.</value>
        Public Property Target As T
            Get
                Return _target
            End Get
            <SecurityCritical>
            <SecurityPermission(SecurityAction.Demand, Flags:=SecurityPermissionFlag.AllFlags)>
            Set(value As T)

                If value Is _target Then
                    Return
                End If

                If _target IsNot Nothing Then
                    If TypeOf _target Is IDisposable Then
                        DirectCast(_target, IDisposable).Dispose()
                    ElseIf Marshal.IsComObject(_target) Then
                        Marshal.ReleaseComObject(_target)
                    End If
                End If

                _target = value

            End Set
        End Property

        Public Shared Widening Operator CType(value As DisposableComWrapper(Of T)) As T
            Return value.Target
        End Operator

        <SecuritySafeCritical>
        Protected Overrides Sub Finalize()
            Dispose(False)
            MyBase.Finalize()
        End Sub

        <SecurityCritical>
        Protected Overridable Sub Dispose(disposing As Boolean)

            Try
                Target = Nothing

            Catch ex As Exception
                Debug.WriteLine(ex.ToString())

            End Try

        End Sub

        ''' <summary>
        ''' Sets Target property to Nothing/null and thereby releasing existing
        ''' object, if any. This method does this within a try/catch block so that
        ''' any exceptions are ignored.
        ''' </summary>
        <SecuritySafeCritical>
        Public Sub Dispose() Implements IDisposable.Dispose
            Dispose(True)
            GC.SuppressFinalize(Me)
        End Sub

    End Class

End Namespace
