using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Columns;
using DevExpress.Utils.Drawing;
using DevExpress.LookAndFeel;
using DevExpress.Utils;
using DevExpress.Skins;
using DevExpress.XtraEditors.Repository;
using System.Linq;
using DevExpress.XtraGrid.Views.Grid;
using System.Collections;
using DevExpress.XtraLayout;
using DevExpress.Data.Filtering;

namespace WindowsFormsApplication6 {
    public partial class FilterPanel : DevExpress.XtraEditors.XtraUserControl {
        CustomGridView view;
        DevExpress.XtraGrid.Registrator.GridEmbeddedLookAndFeel ElementsLookAndFeel { get { return view.GetType().GetProperty("ElementsLookAndFeel", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(view, null) as DevExpress.XtraGrid.Registrator.GridEmbeddedLookAndFeel; } }
        public bool IsVisible { get { return dictionary != null && dictionary.Count > 0; } }
        public FilterPanel() {
            InitializeComponent();
        }
        Dictionary<GridColumn, BaseEdit> dictionary;
        SimpleButton btn;
        public FilterPanel(CustomGridView view, List<GridColumn> columns)
            : this() {
            this.view = view;
            BackColor = GetBackColor();
            UpdateFilterEditors(columns);
        }
        public void SetColumns(List<GridColumn> columns) {
            if(this.dictionary != null && this.dictionary.Count > 0) this.dictionary.Clear();
            UpdateFilterEditors(columns);
        }
        void DisposeEdits() {
            foreach(var item in dictionary) {

                ((BaseEdit)item.Value).EditValueChanged -= FilterControl_EditValueChanged;
            }
            dictionary.Clear();
        }
        public CriteriaOperatorCollection GetCollection(CriteriaOperator oper) {
            Type t = oper.GetType();
            var prop = t.GetProperty("Operands");
            if(prop != null) {
                return prop.GetValue(oper,null) as CriteriaOperatorCollection;
            }
            return new CriteriaOperatorCollection();
        }
        public void OnFilterChanged(DevExpress.Data.Filtering.CriteriaOperator oper) {
            foreach(var item in dictionary) {
                item.Value.EditValue = null;
            }
            if(oper!=null)Recursion(oper);
        }

        void Recursion(CriteriaOperator oper) {
            if(oper is DevExpress.Data.Filtering.FunctionOperator) {
                GetFunctionalOperands(oper);
            } else {
                foreach(var item in GetCollection(oper)) {
                    Recursion(item);
                }
            }
        }

        void GetFunctionalOperands(CriteriaOperator item) {
            var operands = GetCollection(item);
            
            if(operands.Count == 2) {
                var nameOperand = operands.OfType<OperandProperty>().FirstOrDefault();
                var col = view.Columns.ColumnByFieldName(nameOperand.PropertyName);
                var valOperand = operands.OfType<OperandValue>().FirstOrDefault();
                
                if(col != null) {
                    if(dictionary.ContainsKey(col)) {
                        var editor = dictionary[col];
                        editor.EditValue = valOperand.Value;
                    }
                }
            }
        }


        public void UpdateFilterEditors(List<GridColumn> cols) {
            if(dictionary != null) DisposeEdits();
            ClearLayout();
            var items = new List<DevExpress.XtraLayout.BaseLayoutItem>();
            dictionary = new Dictionary<GridColumn, BaseEdit>();
            int i = 0;
            foreach(var col in cols.OrderBy(c => c.VisibleIndex)) {
                var repositoryEditor = view.GetRepositoryItem(col);
                var editor = repositoryEditor.CreateEditor();
                editor.Text = "";
                if(repositoryEditor is RepositoryItemTextEdit) editor.Size = editor.MaximumSize = editor.MinimumSize = new Size(103, 20);
                else if(repositoryEditor is RepositoryItemCheckEdit) {
                    ((CheckEdit)editor).Properties.NullStyle = DevExpress.XtraEditors.Controls.StyleIndeterminate.Inactive;
                    ((CheckEdit)editor).Properties.AllowGrayed = true;
                    editor.EditValue = null;
                    editor.Size = editor.MaximumSize = editor.MinimumSize = editor.CalcBestSize();
                }

                ((BaseEdit)editor).EditValueChanged += FilterControl_EditValueChanged;
                dictionary.Add(col, (BaseEdit)editor);

                var item = layoutControlGroup1.AddItem();
                editor.Name = Guid.NewGuid().ToString();
                item.Control = editor;
                item.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.AutoSize;
                item.Name = col.FieldName;
                if(i != 0) { item.Move(items[i - 1], DevExpress.XtraLayout.Utils.InsertType.Right); }
                items.Add(item);

                i++;
            }
            if(items.Count > 0) {
                var buttonItem = layoutControlGroup1.AddItem();
                if(btn != null) { btn.Dispose(); btn = null; }
                btn = new SimpleButton();
                btn.Name = Guid.NewGuid().ToString();
                btn.Size = btn.MaximumSize = btn.MinimumSize = new Size(60, 22);

                btn.Text = DevExpress.XtraGrid.Localization.GridLocalizer.Active.GetLocalizedString(DevExpress.XtraGrid.Localization.GridStringId.FilterPanelCustomizeButton);
                buttonItem.Control = btn;
                buttonItem.TextVisible = false;
                buttonItem.Move(items.Last(), DevExpress.XtraLayout.Utils.InsertType.Right);
                btn.Click += btn_Click;
                var emptySpaceItem = new EmptySpaceItem();
                emptySpaceItem.Name = "emptySpaceItemNew";
                layoutControl1.Root.AddItem(emptySpaceItem);
                emptySpaceItem.Move(buttonItem, DevExpress.XtraLayout.Utils.InsertType.Right);
                layoutControlGroup1.BestFit();
            }

        }

        void FilterControl_EditValueChanged(object sender, EventArgs e) {
            UpdateFilter(sender);
        }

        void UpdateFilter(object sender) {
            var pair = dictionary.FirstOrDefault(c => c.Value == sender);
            object editingValue = null;
            BaseEdit activeEditor = sender as BaseEdit;
            if(activeEditor != null) {
                editingValue = activeEditor.EditValue;
            } else editingValue = ((Control)sender).Text;
            view.ChangeFilterValue(pair.Key, editingValue);
        }
        void btn_Click(object sender, EventArgs e) {
            view.ShowFilterEditor(view.FocusedColumn);
        }
        public void ClearLayout() {
            layoutControlGroup1.Clear();
        }
        Color GetBackColor() {
            if(ElementsLookAndFeel.ActiveStyle == ActiveLookAndFeelStyle.Skin) {
                SkinElement element = GridSkins.GetSkin(ElementsLookAndFeel)[GridSkins.SkinGridGroupPanel];
                if(element.Color.BackColor2 != Color.Empty)
                    return element.Color.BackColor2;
            }
            return LookAndFeelHelper.GetSystemColor(ElementsLookAndFeel, SystemColors.Control);
        }

    }
}
