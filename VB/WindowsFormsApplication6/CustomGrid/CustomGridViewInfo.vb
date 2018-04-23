Imports DevExpress.Utils.Drawing
Imports DevExpress.XtraGrid.Scrolling
Imports DevExpress.XtraGrid.Views.Grid.ViewInfo
Imports DevExpress.XtraTreeList.ViewInfo
Imports System
Imports System.Collections.Generic
Imports System.Drawing
Imports System.Linq
Imports System.Text
Imports System.Threading.Tasks

Namespace WindowsFormsApplication6
	Public Class CustomGridViewInfo
		Inherits GridViewInfo

		Public Shadows ReadOnly Property View() As CustomGridView
			Get
				Return TryCast(MyBase.View, CustomGridView)
			End Get
		End Property
		Public ReadOnly Property CustomPanelPainter() As ObjectPainter
			Get
				Return Painter.ElementsPainter.FilterPanel
			End Get
		End Property
		Public Sub New(ByVal gridView As CustomGridView)
			MyBase.New(gridView)
		End Sub
		Protected Sub UpdateCustomControlVisibility()
			If View.CustomControl IsNot Nothing Then
				If CustomPanelBounds.Height < 1 Then
					View.CustomControl.Visible = False
				Else
					View.CustomControl.Bounds = CustomPanelBounds
				End If
			End If
		End Sub
		Public Overrides Sub Calc(ByVal g As Graphics, ByVal bounds As Rectangle)
			MyBase.Calc(g, bounds)
			UpdateCustomControlVisibility()
		End Sub
'INSTANT VB NOTE: The variable customPanelBounds was renamed since Visual Basic does not allow variables and other class members to have the same name:
		Private customPanelBounds_Renamed As Rectangle = Rectangle.Empty
		Protected ReadOnly Property CustomPanelBounds() As Rectangle
			Get
				Return customPanelBounds_Renamed
			End Get
		End Property
		Public ReadOnly Property ScrlInfo() As ScrollInfo
			Get
				Return TryCast(View.GetType().GetProperty("ScrollInfo").GetValue(View, Nothing), ScrollInfo)
			End Get
		End Property
		Public Overrides Sub CalcRects(ByVal bounds As Rectangle, ByVal partital As Boolean)
			Dim r As Rectangle = Rectangle.Empty
			ViewRects.Bounds = bounds
			ViewRects.Scroll = CalcScrollRect()
			ViewRects.Client = CalcClientRect()
			FilterPanel.Bounds = Rectangle.Empty

			If Not partital Then
				CalcRectsConstants()
			End If

			If View.OptionsView.ShowIndicator Then
				ViewRects.IndicatorWidth = Math.Max(ScaleHorizontal(View.IndicatorWidth), ViewRects.MinIndicatorWidth)
			End If
			Dim minTop As Integer = ViewRects.Client.Top
			Dim maxBottom As Integer = ViewRects.Client.Bottom
			If View.OptionsView.ShowViewCaption Then
				r = ViewRects.Scroll
				r.Y = minTop
				r.Height = CalcViewCaptionHeight(ViewRects.Client)
				ViewRects.ViewCaption = r
				minTop = ViewRects.ViewCaption.Bottom
			End If
			minTop = UpdateFindControlVisibility(New Rectangle(ViewRects.Scroll.X, minTop, ViewRects.Scroll.Width, maxBottom - minTop), False).Y
			minTop = UpdateCustomControlVisibility(New Rectangle(ViewRects.Scroll.X, minTop, ViewRects.Scroll.Width, maxBottom - minTop), False).Y
			If View.OptionsView.ShowGroupPanel Then
				r = ViewRects.Scroll
				r.Y = minTop
				r.Height = CalcGroupPanelHeight()
				ViewRects.GroupPanel = r
				minTop = ViewRects.GroupPanel.Bottom
			End If



			minTop = CalcRectsColumnPanel(minTop)
			ViewRects.VScrollLocation = minTop

			If View.IsShowFilterPanel Then
				r = ViewRects.Scroll
				Dim fPanel As Integer = GetFilterPanelHeight()
				r.Y = maxBottom - fPanel
				r.Height = fPanel
				FilterPanel.Bounds = r
				maxBottom = r.Top
			End If
			ViewRects.HScrollLocation = maxBottom
			If HScrollBarPresence = ScrollBarPresence.Visible Then
				If Not ScrlInfo.IsOverlapHScrollBar Then
					maxBottom -= HScrollSize
				End If
			End If

			If View.OptionsView.ShowFooter Then
				r = ViewRects.Scroll
				r.Height = GetFooterPanelHeight()
				r.Y = maxBottom - r.Height
				ViewRects.Footer = r
				maxBottom = r.Top
			End If
			r = ViewRects.Client
			r.Y = minTop
			r.Height = maxBottom - minTop
			ViewRects.Rows = r

		End Sub

		Private Function GetCustomPanelHeight() As Integer
			Return View.CalcCustomPanelHeight()
		End Function
		Protected Function UpdateCustomControlVisibility(ByVal client As Rectangle, ByVal setPosition As Boolean) As Rectangle
			If View.IsCustomPanelVisible Then
				View.RequestFilterControl()
			Else
				View.DestroyFilterControl()
			End If
			If View.CustomControl IsNot Nothing Then
				Dim prevVisible As Boolean = View.CustomControl.Visible
				Dim bounds As Rectangle = client
				bounds.Height = View.CustomControl.Height
				If bounds.Height > client.Height \ 2 Then
					View.CustomControl.Visible = False
					Me.customPanelBounds_Renamed = Rectangle.Empty
				Else
					Me.customPanelBounds_Renamed = bounds
					If setPosition Then
						View.CustomControl.Bounds = bounds
					End If
					View.CustomControl.Visible = True
					bounds.Y = bounds.Bottom
					bounds.Height = (client.Bottom - bounds.Y)
					client = bounds
				End If
			End If
			Return client
		End Function
	End Class
End Namespace
