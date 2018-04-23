Imports System
Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Drawing
Imports System.Data
Imports System.Text
Imports System.Windows.Forms
Imports DevExpress.XtraEditors
Imports DevExpress.XtraGrid.Views.Base
Imports DevExpress.XtraGrid.Columns
Imports DevExpress.Utils.Drawing
Imports DevExpress.LookAndFeel
Imports DevExpress.Utils
Imports DevExpress.Skins
Imports DevExpress.XtraEditors.Repository
Imports System.Linq
Imports DevExpress.XtraGrid.Views.Grid
Imports System.Collections
Imports DevExpress.XtraLayout
Imports DevExpress.Data.Filtering

Namespace WindowsFormsApplication6
	Partial Public Class FilterPanel
		Inherits DevExpress.XtraEditors.XtraUserControl

		Private view As CustomGridView
		Private ReadOnly Property ElementsLookAndFeel() As DevExpress.XtraGrid.Registrator.GridEmbeddedLookAndFeel
			Get
				Return TryCast(view.GetType().GetProperty("ElementsLookAndFeel", System.Reflection.BindingFlags.NonPublic Or System.Reflection.BindingFlags.Instance).GetValue(view, Nothing), DevExpress.XtraGrid.Registrator.GridEmbeddedLookAndFeel)
			End Get
		End Property
		Public ReadOnly Property IsVisible() As Boolean
			Get
				Return dictionary IsNot Nothing AndAlso dictionary.Count > 0
			End Get
		End Property
		Public Sub New()
			InitializeComponent()
		End Sub
		Private dictionary As Dictionary(Of GridColumn, BaseEdit)
		Private btn As SimpleButton
		Public Sub New(ByVal view As CustomGridView, ByVal columns As List(Of GridColumn))
			Me.New()
			Me.view = view
			BackColor = GetBackColor()
			UpdateFilterEditors(columns)
		End Sub
		Public Sub SetColumns(ByVal columns As List(Of GridColumn))
			If Me.dictionary IsNot Nothing AndAlso Me.dictionary.Count > 0 Then
				Me.dictionary.Clear()
			End If
			UpdateFilterEditors(columns)
		End Sub
		Private Sub DisposeEdits()
			For Each item In dictionary

				RemoveHandler CType(item.Value, BaseEdit).EditValueChanged, AddressOf FilterControl_EditValueChanged
			Next item
			dictionary.Clear()
		End Sub
		Public Function GetCollection(ByVal oper As CriteriaOperator) As CriteriaOperatorCollection
			Dim t As Type = oper.GetType()
			Dim prop = t.GetProperty("Operands")
			If prop IsNot Nothing Then
				Return TryCast(prop.GetValue(oper,Nothing), CriteriaOperatorCollection)
			End If
			Return New CriteriaOperatorCollection()
		End Function
		Public Sub OnFilterChanged(ByVal oper As DevExpress.Data.Filtering.CriteriaOperator)
			For Each item In dictionary
				item.Value.EditValue = Nothing
			Next item
			If oper IsNot Nothing Then
				Recursion(oper)
			End If
		End Sub

		Private Sub Recursion(ByVal oper As CriteriaOperator)
			If TypeOf oper Is DevExpress.Data.Filtering.FunctionOperator Then
				GetFunctionalOperands(oper)
			Else
				For Each item In GetCollection(oper)
					Recursion(item)
				Next item
			End If
		End Sub

		Private Sub GetFunctionalOperands(ByVal item As CriteriaOperator)
			Dim operands = GetCollection(item)

			If operands.Count = 2 Then
				Dim nameOperand = operands.OfType(Of OperandProperty)().FirstOrDefault()
				Dim col = view.Columns.ColumnByFieldName(nameOperand.PropertyName)
				Dim valOperand = operands.OfType(Of OperandValue)().FirstOrDefault()

				If col IsNot Nothing Then
					If dictionary.ContainsKey(col) Then
						Dim editor = dictionary(col)
						editor.EditValue = valOperand.Value
					End If
				End If
			End If
		End Sub


		Public Sub UpdateFilterEditors(ByVal cols As List(Of GridColumn))
			If dictionary IsNot Nothing Then
				DisposeEdits()
			End If
			ClearLayout()
			Dim items = New List(Of DevExpress.XtraLayout.BaseLayoutItem)()
			dictionary = New Dictionary(Of GridColumn, BaseEdit)()
			Dim i As Integer = 0
			For Each col In cols.OrderBy(Function(c) c.VisibleIndex)
				Dim repositoryEditor = view.GetRepositoryItem(col)
				Dim editor = repositoryEditor.CreateEditor()
				editor.Text = ""
				If TypeOf repositoryEditor Is RepositoryItemTextEdit Then
					editor.MinimumSize = New Size(103, 20)
					editor.MaximumSize = editor.MinimumSize
					editor.Size = editor.MaximumSize
				ElseIf TypeOf repositoryEditor Is RepositoryItemCheckEdit Then
					CType(editor, CheckEdit).Properties.NullStyle = DevExpress.XtraEditors.Controls.StyleIndeterminate.Inactive
					CType(editor, CheckEdit).Properties.AllowGrayed = True
					editor.EditValue = Nothing
					editor.MinimumSize = editor.CalcBestSize()
					editor.MaximumSize = editor.MinimumSize
					editor.Size = editor.MaximumSize
				End If

				AddHandler CType(editor, BaseEdit).EditValueChanged, AddressOf FilterControl_EditValueChanged
				dictionary.Add(col, CType(editor, BaseEdit))

				Dim item = layoutControlGroup1.AddItem()
				editor.Name = Guid.NewGuid().ToString()
				item.Control = editor
				item.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.AutoSize
				item.Name = col.FieldName
				If i <> 0 Then
					item.Move(items(i - 1), DevExpress.XtraLayout.Utils.InsertType.Right)
				End If
				items.Add(item)

				i += 1
			Next col
			If items.Count > 0 Then
				Dim buttonItem = layoutControlGroup1.AddItem()
				If btn IsNot Nothing Then
					btn.Dispose()
					btn = Nothing
				End If
				btn = New SimpleButton()
				btn.Name = Guid.NewGuid().ToString()
				btn.MinimumSize = New Size(60, 22)
				btn.MaximumSize = btn.MinimumSize
				btn.Size = btn.MaximumSize

				btn.Text = DevExpress.XtraGrid.Localization.GridLocalizer.Active.GetLocalizedString(DevExpress.XtraGrid.Localization.GridStringId.FilterPanelCustomizeButton)
				buttonItem.Control = btn
				buttonItem.TextVisible = False
				buttonItem.Move(items.Last(), DevExpress.XtraLayout.Utils.InsertType.Right)
				AddHandler btn.Click, AddressOf btn_Click
				Dim emptySpaceItem = New EmptySpaceItem()
				emptySpaceItem.Name = "emptySpaceItemNew"
				layoutControl1.Root.AddItem(emptySpaceItem)
				emptySpaceItem.Move(buttonItem, DevExpress.XtraLayout.Utils.InsertType.Right)
				layoutControlGroup1.BestFit()
			End If

		End Sub

		Private Sub FilterControl_EditValueChanged(ByVal sender As Object, ByVal e As EventArgs)
			UpdateFilter(sender)
		End Sub

		Private Sub UpdateFilter(ByVal sender As Object)
			Dim pair = dictionary.FirstOrDefault(Function(c) c.Value Is sender)
			Dim editingValue As Object = Nothing
			Dim activeEditor As BaseEdit = TryCast(sender, BaseEdit)
			If activeEditor IsNot Nothing Then
				editingValue = activeEditor.EditValue
			Else
				editingValue = DirectCast(sender, Control).Text
			End If
			view.ChangeFilterValue(pair.Key, editingValue)
		End Sub
		Private Sub btn_Click(ByVal sender As Object, ByVal e As EventArgs)
			view.ShowFilterEditor(view.FocusedColumn)
		End Sub
		Public Sub ClearLayout()
			layoutControlGroup1.Clear()
		End Sub
		Private Function GetBackColor() As Color
			If ElementsLookAndFeel.ActiveStyle = ActiveLookAndFeelStyle.Skin Then
				Dim element As SkinElement = GridSkins.GetSkin(ElementsLookAndFeel)(GridSkins.SkinGridGroupPanel)
				If element.Color.BackColor2 <> Color.Empty Then
					Return element.Color.BackColor2
				End If
			End If
			Return LookAndFeelHelper.GetSystemColor(ElementsLookAndFeel, SystemColors.Control)
		End Function

	End Class
End Namespace
