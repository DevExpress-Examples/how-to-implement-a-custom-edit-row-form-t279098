using System;
using System.Collections.Generic;
using Xamarin.Forms;
using DevExpress.Mobile.DataGrid;

namespace CustomEditFormDemo
{	
	public partial class MainPage : ContentPage
	{
        TestOrdersRepository model;
        public MainPage ()
		{
			InitializeComponent ();

			model = new TestOrdersRepository ();
			BindingContext = model;


            DataTemplate editFormContentTemplate = new DataTemplate(GetTemplate);
            grid.EditFormContent = editFormContentTemplate;
			grid.RowEditMode = RowEditMode.Popup;
		}

        object GetTemplate() {
            return new CustomEditFormContent() { ViewModel = model };
        }

        void OnInitNewItemRow(object sender, InitNewRowEventArgs e) {
            e.EditableRowData.SetFieldValue("Quantity", model.MinimumQuantity);
            e.EditableRowData.SetFieldValue("Product.Name", model.Products[0].Name);
        }
    }
}

