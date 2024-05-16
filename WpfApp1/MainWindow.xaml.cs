using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Windows;

namespace WpfApp1
{
    public partial class MainWindow : Window
    {
        SqlConnection sqlConnection;

        public MainWindow()
        {
            InitializeComponent();
            string connectionString = ConfigurationManager.ConnectionStrings["WpfApp1.Properties.Settings.elkholyjrConnectionString"].ConnectionString;
            sqlConnection = new SqlConnection(connectionString);
            DisplayStores();
            DisplayAllProducts();
        }

        private void DisplayStores()
        {
            try
            {
                string query = "SELECT * FROM Store";
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(query, sqlConnection);
                using (sqlDataAdapter)
                {
                    DataTable storeTable = new DataTable();
                    sqlDataAdapter.Fill(storeTable);
                    storeList.DisplayMemberPath = "Name";
                    storeList.SelectedValuePath = "Id";
                    storeList.ItemsSource = storeTable.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void DisplayStoreInventory()
        {
            try
            {
                if (storeList.SelectedValue == null)
                {
                    return;
                }

                string query = "SELECT * FROM Product p INNER JOIN StoreInventory si ON p.Id = si.ProductId WHERE si.StoreId = @StoreId";
                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                sqlCommand.Parameters.AddWithValue("@StoreId", storeList.SelectedValue);

                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);
                using (sqlDataAdapter)
                {
                    DataTable inventoryTable = new DataTable();
                    sqlDataAdapter.Fill(inventoryTable);
                    storeInventory.DisplayMemberPath = "Brand";
                    storeInventory.SelectedValuePath = "Id";
                    storeInventory.ItemsSource = inventoryTable.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void storeList_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            DisplayStoreInventory();
        }

        private void DisplayAllProducts()
        {
            try
            {
                string query = "SELECT * FROM Product";
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(query, sqlConnection);
                using (sqlDataAdapter)
                {
                    DataTable productTable = new DataTable();
                    sqlDataAdapter.Fill(productTable);
                    productList.DisplayMemberPath = "Brand";
                    productList.SelectedValuePath = "Id";
                    productList.ItemsSource = productTable.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void AddStoreClick(object sender, RoutedEventArgs e)
        {
            try
            {
                List<SqlParameter> parameters = new List<SqlParameter>
                {
                    new SqlParameter("@Name", SqlDbType.NVarChar) { Value = storeName.Text },
                    new SqlParameter("@Street", SqlDbType.NVarChar) { Value = storeStreet.Text },
                    new SqlParameter("@City", SqlDbType.NVarChar) { Value = storeCity.Text },
                    new SqlParameter("@State", SqlDbType.NChar) { Value = storeState.Text },
                    new SqlParameter("@Zipcode", SqlDbType.Int) { Value = storeZip.Text }
                };
                string query = "INSERT INTO Store (Name, Street, City, State, Zip) VALUES (@Name, @Street, @City, @State, @Zipcode)";
                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                sqlCommand.Parameters.AddRange(parameters.ToArray());
                sqlConnection.Open();
                sqlCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                sqlConnection.Close();
                DisplayStores();
            }
        }

        private void AddInvClick(object sender, RoutedEventArgs e)
        {
            try
            {
                if (storeList.SelectedValue == null || productList.SelectedValue == null)
                {
                    MessageBox.Show("Please select both a store and a product.");
                    return;
                }

                string query = "INSERT INTO StoreInventory (StoreId, ProductId) VALUES (@StoreId, @ProductId)";
                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                sqlCommand.Parameters.AddWithValue("@StoreId", storeList.SelectedValue);
                sqlCommand.Parameters.AddWithValue("@ProductId", productList.SelectedValue);
                sqlConnection.Open();
                sqlCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                sqlConnection.Close();
                DisplayStoreInventory();
            }
        }

        private void AddProductClick(object sender, RoutedEventArgs e)
        {
            try
            {
                List<SqlParameter> parameters = new List<SqlParameter>
                {
                    new SqlParameter("@Manufacturer", SqlDbType.NVarChar) { Value = prodManu.Text },
                    new SqlParameter("@Brand", SqlDbType.NVarChar) { Value = prodBrand.Text }
                };
                string query = "INSERT INTO Product (Manufacturer, Brand) VALUES (@Manufacturer, @Brand)";
                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                sqlCommand.Parameters.AddRange(parameters.ToArray());
                sqlConnection.Open();
                sqlCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                sqlConnection.Close();
                DisplayAllProducts();
            }
        }

        private void RemoveStoreClick(object sender, RoutedEventArgs e)
        {
            try
            {
                if (storeList.SelectedValue == null)
                {
                    MessageBox.Show("Please select a store.");
                    return;
                }

                string query = "DELETE FROM Store WHERE Id = @StoreId";
                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                sqlCommand.Parameters.AddWithValue("@StoreId", storeList.SelectedValue);
                sqlConnection.Open();
                sqlCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                sqlConnection.Close();
                DisplayStores();
            }
        }

        private void RemoveInvClick(object sender, RoutedEventArgs e)
        {
            try
            {
                if (storeInventory.SelectedValue == null)
                {
                    MessageBox.Show("Please select an inventory item.");
                    return;
                }

                string query = "DELETE FROM StoreInventory WHERE ProductId = @ProductId";
                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                sqlCommand.Parameters.AddWithValue("@ProductId", storeInventory.SelectedValue);
                sqlConnection.Open();
                sqlCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                sqlConnection.Close();
                DisplayStoreInventory();
            }
        }

        private void RemoveProductClick(object sender, RoutedEventArgs e)
        {
            try
            {
                if (productList.SelectedValue == null)
                {
                    MessageBox.Show("Please select a product.");
                    return;
                }

                string query = "DELETE FROM Product WHERE Id = @ProductId";
                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                sqlCommand.Parameters.AddWithValue("@ProductId", productList.SelectedValue);
                sqlConnection.Open();
                sqlCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                sqlConnection.Close();
                DisplayAllProducts();
            }
        }
    }
}
