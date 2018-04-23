using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApplication6 {
    public class CustomGridView : GridView {
        protected override string ViewName { get { return "CustomGridView"; } }
        private FilterPanel customControl;
        public FilterPanel CustomControl { get { return customControl; } }
        private List<GridColumn> FilterColumns;
        public CustomGridView() : this(null) { }
        public CustomGridView(DevExpress.XtraGrid.GridControl grid)
            : base(grid) {

        }
        bool customChangeFilter;
        protected override void OnActiveFilterChanged(object sender, EventArgs e) {
            base.OnActiveFilterChanged(sender, e);
            if(CustomControl != null && !customChangeFilter) CustomControl.OnFilterChanged((DevExpress.Data.Filtering.CriteriaOperator)this.ActiveFilterCriteria);
            customChangeFilter = false;
        }
        public RepositoryItem GetRepositoryItem(GridColumn column) {
            if(column == null) {
                return null;
            }
            RepositoryItem columnEdit = column.ColumnEdit;
            if((columnEdit != null) && columnEdit.IsDisposed) {
                columnEdit = null;
            }
            if(columnEdit == null) {
                columnEdit = this.GetColumnDefaultRepositoryItem(column);
            }
            columnEdit = this.GetRepositoryItem(this.GetColumnFieldNameSortGroup(column), columnEdit);
            return columnEdit;
        }
        public RepositoryItem GetRepositoryItem(GridColumn column, RepositoryItem current) {
            return base.GetFilterRowRepositoryItem(column, current);
        }
        public void SetColumns(List<GridColumn> columns) {
            FilterColumns = columns;
        }
        public void ChangeFilterValue(GridColumn col, object value) {
            customChangeFilter = true;
            OnFilterRowValueChanging(col, value);

        }
        public bool IsCustomPanelVisible { get { return FilterColumns != null && FilterColumns.Count > 0; } }
        public void RequestFilterControl() {
            if(customControl != null || FilterColumns == null || FilterColumns.Count == 0) return;
            customControl = new FilterPanel(this, FilterColumns);
            customControl.Visible = true;
            GridControl.Controls.Add(customControl);
        }

        protected internal void DestroyFilterControl() {
            if(customControl != null) {
                customControl.Dispose();
            }
            this.customControl = null;
        }

        internal int CalcCustomPanelHeight() {
            return CustomControl.Size.Height;
        }

    }
}
