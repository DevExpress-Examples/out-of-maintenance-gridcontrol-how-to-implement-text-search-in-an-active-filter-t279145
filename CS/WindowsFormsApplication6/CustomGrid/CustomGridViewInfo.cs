using DevExpress.Utils.Drawing;
using DevExpress.XtraGrid.Scrolling;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraTreeList.ViewInfo;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApplication6 {
    public class CustomGridViewInfo : GridViewInfo {
        public new CustomGridView View { get { return base.View as CustomGridView; } }
        public ObjectPainter CustomPanelPainter { get { return Painter.ElementsPainter.FilterPanel; } }
        public CustomGridViewInfo(CustomGridView gridView)
            : base(gridView) {
        }
        protected void UpdateCustomControlVisibility() {
            if(View.CustomControl != null) {
                if(CustomPanelBounds.Height < 1)
                    View.CustomControl.Visible = false;
                else
                    View.CustomControl.Bounds = CustomPanelBounds;
            }
        }
        public override void Calc(Graphics g, Rectangle bounds) {
            base.Calc(g, bounds);
            UpdateCustomControlVisibility();
        }
        private Rectangle customPanelBounds = Rectangle.Empty;
        protected Rectangle CustomPanelBounds { get { return customPanelBounds; } }
        public ScrollInfo ScrlInfo { get { return View.GetType().GetProperty("ScrollInfo").GetValue(View, null) as ScrollInfo; } }
        public override void CalcRects(Rectangle bounds, bool partital) {
            Rectangle r = Rectangle.Empty;
            ViewRects.Bounds = bounds;
            ViewRects.Scroll = CalcScrollRect();
            ViewRects.Client = CalcClientRect();
            FilterPanel.Bounds = Rectangle.Empty;

            if(!partital) {
                CalcRectsConstants();
            }

            if(View.OptionsView.ShowIndicator) {
                ViewRects.IndicatorWidth = Math.Max(ScaleHorizontal(View.IndicatorWidth), ViewRects.MinIndicatorWidth);
            }
            int minTop = ViewRects.Client.Top;
            int maxBottom = ViewRects.Client.Bottom;
            if(View.OptionsView.ShowViewCaption) {
                r = ViewRects.Scroll;
                r.Y = minTop;
                r.Height = CalcViewCaptionHeight(ViewRects.Client);
                ViewRects.ViewCaption = r;
                minTop = ViewRects.ViewCaption.Bottom;
            }
            minTop = UpdateFindControlVisibility(new Rectangle(ViewRects.Scroll.X, minTop, ViewRects.Scroll.Width, maxBottom - minTop), false).Y;
            minTop = UpdateCustomControlVisibility(new Rectangle(ViewRects.Scroll.X, minTop, ViewRects.Scroll.Width, maxBottom - minTop), false).Y;
            if(View.OptionsView.ShowGroupPanel) {
                r = ViewRects.Scroll;
                r.Y = minTop;
                r.Height = CalcGroupPanelHeight();
                ViewRects.GroupPanel = r;
                minTop = ViewRects.GroupPanel.Bottom;
            }



            minTop = CalcRectsColumnPanel(minTop);
            ViewRects.VScrollLocation = minTop;

            if(View.IsShowFilterPanel) {
                r = ViewRects.Scroll;
                int fPanel = GetFilterPanelHeight();
                r.Y = maxBottom - fPanel;
                r.Height = fPanel;
                FilterPanel.Bounds = r;
                maxBottom = r.Top;
            }
            ViewRects.HScrollLocation = maxBottom;
            if(HScrollBarPresence == ScrollBarPresence.Visible) {
                if(!ScrlInfo.IsOverlapHScrollBar) maxBottom -= HScrollSize;
            }

            if(View.OptionsView.ShowFooter) {
                r = ViewRects.Scroll;
                r.Height = GetFooterPanelHeight();
                r.Y = maxBottom - r.Height;
                ViewRects.Footer = r;
                maxBottom = r.Top;
            }
            r = ViewRects.Client;
            r.Y = minTop;
            r.Height = maxBottom - minTop;
            ViewRects.Rows = r;

        }

        private int GetCustomPanelHeight() {
            return View.CalcCustomPanelHeight();
        }
        protected Rectangle UpdateCustomControlVisibility(Rectangle client, bool setPosition) {
            if(View.IsCustomPanelVisible) {
                View.RequestFilterControl();
            } else {
                View.DestroyFilterControl();
            }
            if(View.CustomControl != null) {
                bool prevVisible = View.CustomControl.Visible;
                Rectangle bounds = client;
                bounds.Height = View.CustomControl.Height;
                if(bounds.Height > client.Height / 2) {
                    View.CustomControl.Visible = false;
                    this.customPanelBounds = Rectangle.Empty;
                } else {
                    this.customPanelBounds = bounds;
                    if(setPosition) View.CustomControl.Bounds = bounds;
                    View.CustomControl.Visible = true;
                    bounds.Y = bounds.Bottom;
                    bounds.Height = (client.Bottom - bounds.Y);
                    client = bounds;
                }
            }
            return client;
        }
    }
}
