Imports DevExpress.Utils.Drawing
Imports DevExpress.XtraEditors.Filtering
Imports DevExpress.XtraEditors.Repository
Imports DevExpress.XtraGrid
Imports DevExpress.XtraGrid.Columns
Imports DevExpress.XtraGrid.FilterEditor
Imports DevExpress.XtraGrid.Registrator
Imports DevExpress.XtraGrid.Scrolling
Imports DevExpress.XtraGrid.Views.Base
Imports DevExpress.XtraGrid.Views.Grid
Imports DevExpress.XtraGrid.Views.Grid.Handler
Imports DevExpress.XtraGrid.Views.Grid.ViewInfo
Imports System
Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Data
Imports System.Drawing
Imports System.Linq
Imports System.Text
Imports System.Threading.Tasks
Imports System.Windows.Forms

Namespace WindowsFormsApplication6
	Partial Public Class Form1
		Inherits Form

		Public Sub New()
			InitializeComponent()
			Dim customers = GetCustomers()
			Dim source1 As New BindingList(Of Customer)(customers)
			Me.gridControl1.DataSource = source1
			gridView1.SetColumns(New List(Of GridColumn)() From {gridView1.Columns(2), gridView1.Columns(3)})
			For Each col As GridColumn In gridView1.Columns
				col.OptionsFilter.AutoFilterCondition = AutoFilterCondition.Contains
			Next col
			gridView1.OptionsView.ShowAutoFilterRow = True

		End Sub


		Public Function GetCustomers() As IList(Of Customer)
			Dim customers As IList(Of Customer) = New List(Of Customer)()
			customers.Add(New Customer() With {.FirstName = "Charlotte", .LastName = "Cooper", .IsEnabled = True})
			customers.Add(New Customer() With {.FirstName = "Shelley", .LastName = "Burke", .IsEnabled = True})
			customers.Add(New Customer() With {.FirstName = "Regina", .LastName = "Murphy", .IsEnabled = True})
			customers.Add(New Customer() With {.FirstName = "Yoshi", .LastName = "Nagase", .IsEnabled = True})
			customers.Add(New Customer() With {.FirstName = "Mayumi", .LastName = "Ohno", .IsEnabled = True})
			customers.Add(New Customer() With {.FirstName = "Nancy", .LastName = "Davolio", .IsEnabled = True})
			customers.Add(New Customer() With {.FirstName = "Andrew", .LastName = "Fuller", .IsEnabled = True})
			customers.Add(New Customer() With {.FirstName = "Janet", .LastName = "Leverling", .IsEnabled = True})
			customers.Add(New Customer() With {.FirstName = "Steven", .LastName = "Buchanan", .IsEnabled = True})
			customers.Add(New Customer() With {.FirstName = "Michael", .LastName = "Suyama", .IsEnabled = True})
			customers.Add(New Customer() With {.FirstName = "Robert", .LastName = "King", .IsEnabled = True})
			customers.Add(New Customer() With {.FirstName = "Laura", .LastName = "Callahan", .IsEnabled = True})
			customers.Add(New Customer() With {.FirstName = "Anne", .LastName = "Dodsworth", .IsEnabled = True})
			Return customers
		End Function
	End Class
	Public Class Customer
		Private _id As Integer
		Private _firstName As String
		Private _lastName As String
		Private _isEnabled As Boolean
		Public Sub New()
		End Sub
		Public Property Id() As Integer
			Get
				Return _id
			End Get
			Set(ByVal value As Integer)
				If value <> _id Then
					_id = value
				End If
			End Set
		End Property
		Public Property IsEnabled() As Boolean
			Get
				Return _isEnabled
			End Get
			Set(ByVal value As Boolean)
				If value <> _isEnabled Then
					_isEnabled = value
				End If
			End Set
		End Property
		Public Property FirstName() As String
			Get
				Return _firstName
			End Get
			Set(ByVal value As String)
				If value <> _firstName Then
					_firstName = value
				End If
			End Set
		End Property
		Public Property LastName() As String
			Get
				Return _lastName
			End Get
			Set(ByVal value As String)
				If value <> _lastName Then
					_lastName = value
				End If
			End Set
		End Property

	End Class



End Namespace
