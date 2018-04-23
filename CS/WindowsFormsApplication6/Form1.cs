using DevExpress.Utils.Drawing;
using DevExpress.XtraEditors.Filtering;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.FilterEditor;
using DevExpress.XtraGrid.Registrator;
using DevExpress.XtraGrid.Scrolling;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.Handler;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplication6 {
    public partial class Form1 : Form {
        public Form1() {
            InitializeComponent();
            var customers = GetCustomers();
            BindingList<Customer> source1 = new BindingList<Customer>(customers);
            this.gridControl1.DataSource = source1;
            gridView1.SetColumns(new List<GridColumn>() { gridView1.Columns[2], gridView1.Columns[3] });
            foreach(GridColumn col in gridView1.Columns) {
                col.OptionsFilter.AutoFilterCondition = AutoFilterCondition.Contains;
            }
            gridView1.OptionsView.ShowAutoFilterRow = true;
            
        }


        public IList<Customer> GetCustomers() {
            IList<Customer> customers = new List<Customer>();
            customers.Add(new Customer() { FirstName = "Charlotte", LastName = "Cooper", IsEnabled = true });
            customers.Add(new Customer() { FirstName = "Shelley", LastName = "Burke", IsEnabled = true });
            customers.Add(new Customer() { FirstName = "Regina", LastName = "Murphy", IsEnabled = true });
            customers.Add(new Customer() { FirstName = "Yoshi", LastName = "Nagase", IsEnabled = true });
            customers.Add(new Customer() { FirstName = "Mayumi", LastName = "Ohno", IsEnabled = true });
            customers.Add(new Customer() { FirstName = "Nancy", LastName = "Davolio", IsEnabled = true });
            customers.Add(new Customer() { FirstName = "Andrew", LastName = "Fuller", IsEnabled = true });
            customers.Add(new Customer() { FirstName = "Janet", LastName = "Leverling", IsEnabled = true });
            customers.Add(new Customer() { FirstName = "Steven", LastName = "Buchanan", IsEnabled = true });
            customers.Add(new Customer() { FirstName = "Michael", LastName = "Suyama", IsEnabled = true });
            customers.Add(new Customer() { FirstName = "Robert", LastName = "King", IsEnabled = true });
            customers.Add(new Customer() { FirstName = "Laura", LastName = "Callahan", IsEnabled = true });
            customers.Add(new Customer() { FirstName = "Anne", LastName = "Dodsworth", IsEnabled = true });
            return customers;
        }
    }
    public class Customer {
        int _id;
        string _firstName;
        string _lastName;
        bool _isEnabled;
        public Customer() {
        }
        public int Id {
            get { return _id; }
            set {
                if(value != _id) {
                    _id = value;
                }
            }
        }
        public bool IsEnabled {
            get { return _isEnabled; }
            set {
                if(value != _isEnabled) {
                    _isEnabled = value;
                }
            }
        }
        public string FirstName {
            get { return _firstName; }
            set {
                if(value != _firstName) {
                    _firstName = value;
                }
            }
        }
        public string LastName {
            get { return _lastName; }
            set {
                if(value != _lastName) {
                    _lastName = value;
                }
            }
        }

    }


    
}
