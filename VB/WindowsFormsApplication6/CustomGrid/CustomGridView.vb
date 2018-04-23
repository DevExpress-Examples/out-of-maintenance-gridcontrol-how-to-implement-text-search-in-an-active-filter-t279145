Imports DevExpress.XtraEditors.Repository
Imports DevExpress.XtraGrid.Columns
Imports DevExpress.XtraGrid.Views.Grid
Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports System.Threading.Tasks

Namespace WindowsFormsApplication6
	Public Class CustomGridView
		Inherits GridView

		Protected Overrides ReadOnly Property ViewName() As String
			Get
				Return "CustomGridView"
			End Get
		End Property
'INSTANT VB NOTE: The variable customControl was renamed since Visual Basic does not allow variables and other class members to have the same name:
		Private customControl_Renamed As FilterPanel
		Public ReadOnly Property CustomControl() As FilterPanel
			Get
				Return customControl_Renamed
			End Get
		End Property
		Private FilterColumns As List(Of GridColumn)
		Public Sub New()
			Me.New(Nothing)
		End Sub
		Public Sub New(ByVal grid As DevExpress.XtraGrid.GridControl)
			MyBase.New(grid)

		End Sub
		Private customChangeFilter As Boolean
		Protected Overrides Sub OnActiveFilterChanged(ByVal sender As Object, ByVal e As EventArgs)
			MyBase.OnActiveFilterChanged(sender, e)
			If CustomControl IsNot Nothing AndAlso (Not customChangeFilter) Then
				CustomControl.OnFilterChanged(CType(Me.ActiveFilterCriteria, DevExpress.Data.Filtering.CriteriaOperator))
			End If
			customChangeFilter = False
		End Sub
		Public Function GetRepositoryItem(ByVal column As GridColumn) As RepositoryItem
			If column Is Nothing Then
				Return Nothing
			End If
			Dim columnEdit As RepositoryItem = column.ColumnEdit
			If (columnEdit IsNot Nothing) AndAlso columnEdit.IsDisposed Then
				columnEdit = Nothing
			End If
			If columnEdit Is Nothing Then
				columnEdit = Me.GetColumnDefaultRepositoryItem(column)
			End If
			columnEdit = Me.GetRepositoryItem(Me.GetColumnFieldNameSortGroup(column), columnEdit)
			Return columnEdit
		End Function
		Public Function GetRepositoryItem(ByVal column As GridColumn, ByVal current As RepositoryItem) As RepositoryItem
			Return MyBase.GetFilterRowRepositoryItem(column, current)
		End Function
		Public Sub SetColumns(ByVal columns As List(Of GridColumn))
			FilterColumns = columns
		End Sub
		Public Sub ChangeFilterValue(ByVal col As GridColumn, ByVal value As Object)
			customChangeFilter = True
			OnFilterRowValueChanging(col, value)

		End Sub
		Public ReadOnly Property IsCustomPanelVisible() As Boolean
			Get
				Return FilterColumns IsNot Nothing AndAlso FilterColumns.Count > 0
			End Get
		End Property
		Public Sub RequestFilterControl()
			If customControl_Renamed IsNot Nothing OrElse FilterColumns Is Nothing OrElse FilterColumns.Count = 0 Then
				Return
			End If
			customControl_Renamed = New FilterPanel(Me, FilterColumns)
			customControl_Renamed.Visible = True
			GridControl.Controls.Add(customControl_Renamed)
		End Sub

		Protected Friend Sub DestroyFilterControl()
			If customControl_Renamed IsNot Nothing Then
				customControl_Renamed.Dispose()
			End If
			Me.customControl_Renamed = Nothing
		End Sub

		Friend Function CalcCustomPanelHeight() As Integer
			Return CustomControl.Size.Height
		End Function

	End Class
End Namespace
