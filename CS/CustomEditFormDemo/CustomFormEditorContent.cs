using System;
using System.Collections.Generic;
using Xamarin.Forms;
using DevExpress.Mobile.DataGrid;
using DevExpress.Mobile.DataGrid.Theme;
using System.Globalization;
using DevExpress.Mobile.DataGrid.Localization;

namespace CustomEditFormDemo {

	class EditFormLocalizer : GridLocalizer {
		public void SetEditItemFormCaption() {
			AddString(GridStringId.EditingForm_LabelCaption, "Edit Order");
		}
		public void SetNewItemFormCaption() {
			AddString(GridStringId.EditingForm_LabelCaption, "Add New Order");
		}
	}


	public class CustomEditFormContent : ContentView {
		public TestOrdersRepository ViewModel { get; set; }

		public CustomEditFormContent() {
			HorizontalOptions = LayoutOptions.FillAndExpand;
			VerticalOptions = LayoutOptions.FillAndExpand; 
			GridLocalizer.Active = new EditFormLocalizer();
		}

		protected override void OnBindingContextChanged () {
			base.OnBindingContextChanged ();
			EditFormLocalizer localizer = GridLocalizer.Active as EditFormLocalizer;

			if ((BindingContext as EditValuesContainer).IsNewRow) {
				if (localizer != null) {
					localizer.SetNewItemFormCaption();
				}
				Content = CreateNewItemFormContent();
			}
			else {
				if (localizer != null) {
					localizer.SetEditItemFormCaption();
				}
				Content = CreateEditItemFormContent ();
			}
		}

		View CreateNewItemFormContent() {
			Grid layoutGrid = CreateGrid(4);
			FillFormGrid(layoutGrid, true);
			return layoutGrid;
		}

		View CreateEditItemFormContent() {
			Grid layoutGrid = CreateGrid(5);
			FillFormGrid(layoutGrid, false);
			return layoutGrid;
		}

		Grid CreateGrid(int rowsCount) {
			Grid layoutGrid = new Grid();

			layoutGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
			layoutGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(200, GridUnitType.Star) });

			for (int i = 0; i < rowsCount; i++) {
				layoutGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
			}

			layoutGrid.RowSpacing = 20;
			layoutGrid.ColumnSpacing = 20;

			return layoutGrid;
		}

		void FillFormGrid(Grid layoutGrid, bool isNewItemForm) {
			FillLabels(layoutGrid, isNewItemForm);
			FillEditors(layoutGrid, isNewItemForm);
		}

		void FillLabels(Grid layoutGrid, bool isNewItemForm) {
			Label productName = new Label() { Text = "Product name:", 
											  FontSize = 18, VerticalOptions = LayoutOptions.Center };
			Label productUnitPrice = new Label() { Text = "Price per product unit:", 
												   FontSize = 18, VerticalOptions = LayoutOptions.Center };
			Label quantity = new Label() { Text = "The number of ordered units:", 
										   FontSize = 18, VerticalOptions = LayoutOptions.Start };
			Label total = new Label() { Text = "Total cost:", 
										FontSize = 18, VerticalOptions = LayoutOptions.Center };
			Label isShipped = null;

			if (!isNewItemForm) {
				isShipped = new Label() { Text = "Has this order been shipped?", 
										  FontSize = 18, VerticalOptions = LayoutOptions.Center };
			}

			layoutGrid.Children.Add(productName, 0, 0);
			layoutGrid.Children.Add(productUnitPrice, 0, 1);
			layoutGrid.Children.Add(quantity, 0, 2);
			layoutGrid.Children.Add(total, 0, 3);

			if (!isNewItemForm) {
				layoutGrid.Children.Add(isShipped, 0, 4);
			}
		}

		void FillEditors(Grid layoutGrid, bool isNewItemForm) {
			View productName;

            // Display a product name in a label in the Edit Item form 
            // or change a product name by using a picker in the New Item form.
			if (isNewItemForm) {
				productName = new Picker() { HorizontalOptions = LayoutOptions.FillAndExpand, 
											 VerticalOptions = LayoutOptions.Center};
				InitProductPickerValues(productName as Picker);
			}
			else {
				productName = new Label() { HorizontalOptions = LayoutOptions.FillAndExpand, 
											VerticalOptions = LayoutOptions.Center, 
											FontAttributes = FontAttributes.Bold, FontSize = 18 };
				productName.SetBinding(Label.TextProperty, 
										new Binding(EditValuesContainer.GetBindingPath("Product.Name")));
			}

            // Display a product unit price in a form.
			Int32ToPriceStringConverter priceConverter = new Int32ToPriceStringConverter();
			Label priceLabel = new Label () { HorizontalOptions = LayoutOptions.FillAndExpand, 
											  VerticalOptions = LayoutOptions.Center, FontSize = 18 };
			priceLabel.SetBinding (Label.TextProperty, EditValuesContainer.GetBindingPath ("Product.UnitPrice"), 
									BindingMode.Default, priceConverter);

            // Modify the number of ordered units via a slider.
			StackLayout quantityStack = new StackLayout () { Orientation = StackOrientation.Vertical, 
															 VerticalOptions = LayoutOptions.CenterAndExpand };
			Label quantityLabel = new Label () { XAlign = TextAlignment.Start, 
												 VerticalOptions = LayoutOptions.Center, FontSize = 18};
			Slider quantitySlider = new Slider () { Maximum = 99, Minimum = 10 };
			quantitySlider.SetBinding (Slider.ValueProperty, EditValuesContainer.GetBindingPath ("Quantity"), 
									   BindingMode.TwoWay, new Int32ToDoubleConverter());
			quantityLabel.BindingContext = quantitySlider;
			quantityLabel.SetBinding(Label.TextProperty, "Value", BindingMode.OneWay, new DoubleToInt32Converter());
			quantityStack.Children.Add(quantityLabel);
			quantityStack.Children.Add(quantitySlider);

            // Display the order total value calculated automatically.
			Label totalLabel = new Label () { XAlign = TextAlignment.Start, 
											  VerticalOptions = LayoutOptions.Center, FontSize = 18 };
			totalLabel.PropertyChanged += OnTotalPropertyChanged;
			totalLabel.SetBinding(Label.TextProperty, EditValuesContainer.GetBindingPath("Total"), 
								  BindingMode.OneWay, priceConverter);

			if (!isNewItemForm) {
				priceLabel.FontAttributes = FontAttributes.Bold;
				quantityLabel.FontAttributes = FontAttributes.Bold;
				totalLabel.FontAttributes = FontAttributes.Bold;
			}

            // Add created elements to a form.
			layoutGrid.Children.Add(productName, 1, 0);
			layoutGrid.Children.Add(priceLabel, 1, 1);
			layoutGrid.Children.Add(quantityStack, 1, 2);
			layoutGrid.Children.Add(totalLabel, 1, 3);

            // Show a switch in the Edit Item form allowing end-users to set whether or not an order is shipped.
			Switch isShipped = new Switch() { VerticalOptions = LayoutOptions.Center, 
											  HorizontalOptions = LayoutOptions.Start };
			isShipped.SetBinding(Switch.IsToggledProperty, EditValuesContainer.GetBindingPath("Shipped"));
			if (!isNewItemForm) {
				layoutGrid.Children.Add(isShipped, 1, 4);
			}
		}

		void InitProductPickerValues(Picker picker) {
			EditValuesContainer valuesContainer = BindingContext as EditValuesContainer;
			String productName = valuesContainer.Values["Product.Name"].ToString();
			int selectedItemIndex = -1;

			foreach (Product product in ViewModel.Products) {
				if (productName == product.Name) {
					selectedItemIndex = picker.Items.Count;
				}

				picker.Items.Add(product.Name);
			}

			picker.SelectedIndexChanged += OnProductNameSelectedIndexChanged;

			if (selectedItemIndex >= 0) {
				picker.SelectedIndex = selectedItemIndex;
			}
		}

		void OnProductNameSelectedIndexChanged(object sender, EventArgs e) {
			EditValuesContainer valuesContainer = BindingContext as EditValuesContainer;
			Picker picker = sender as Picker;

			if (picker.SelectedIndex < 0) {
				return;
			}

			String productName = picker.Items[picker.SelectedIndex];

			valuesContainer.Values["Product.Name"] = productName;

			Product product = ViewModel.Products.Find(p => p.Name == productName);

			valuesContainer.Values["Product.UnitPrice"] = product.UnitPrice;
		}

		void OnTotalPropertyChanged (object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			Label label = (sender as Label);
			if (e.PropertyName == Label.TextProperty.PropertyName && !String.IsNullOrWhiteSpace (label.Text)) {
				Int32 value = (Int32)((label.BindingContext as EditValuesContainer).Values ["Total"]);
				if (value > 2000) {
					label.TextColor = Color.Green; } 
				else {
					if (value < 500) {
						label.TextColor = Color.Red; } 
					else {
						label.TextColor = ThemeManager.Theme.CellCustomizer.Font.Color;}
				}
			}
		}
	}
}