'
' LTRLib
' 
' Copyright (c) Olof Lagerkvist, LTR Data
' http://ltr-data.se   https://github.com/LTRData
'

Imports System.Windows.Forms

Public Class GridDialog

    Public ReadOnly Property BindingSource As BindingSource
        Get
            Return mBindingSource
        End Get
    End Property

    Public ReadOnly Property DataGridView As DataGridView
        Get
            Return mDataGridView
        End Get
    End Property

    Public Sub SetDataSource(Source As Object)
        mDataGridView.AutoGenerateColumns = True
        mBindingSource.DataSource = Source
    End Sub

    Public Sub SetDataSource(Source As Object, BoundMember As String)
        mDataGridView.AutoGenerateColumns = True
        mDataGridView.DataMember = BoundMember
        mBindingSource.DataSource = Source
    End Sub

    Public Property DataMember As String
        Get
            Return mBindingSource.DataMember
        End Get
        Set
            mBindingSource.DataMember = Value
        End Set
    End Property

    Public ReadOnly Property SelectedItem As Object
        Get
            Return mBindingSource.Current
        End Get
    End Property

#Disable Warning IDE1006 ' Naming Styles

    Private Sub btnOK_Click(sender As Object, e As EventArgs) Handles btnOK.Click
        DialogResult = DialogResult.OK
    End Sub

End Class
