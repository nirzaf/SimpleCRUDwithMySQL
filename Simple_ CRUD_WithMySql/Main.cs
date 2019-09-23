using PORTAL.DAL;
using PORTAL.MODEL;
using System;
using System.Data;
using System.Windows.Forms;

namespace Simple__CRUD_WithMySql
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
        }

        private void loadTable() //load values to table
        {
            //clear data grid view
            dgvItems.Rows.Clear();

                DataSet ds = new DataSet();
                MySQLconnection con = new MySQLconnection(); //create connection

                //create sql query for get data
                string sqlQuery = "SELECT * FROM items";

                //Executing
                ds = con.LoadData(sqlQuery);

            //check if data data is available
            if (ds.Tables[0].Rows.Count == 0)
            {
                return;
            }

            for (int n = 0; n < ds.Tables[0].Rows.Count; n++) //loopping through add data to dgv
            {
                itemBindingSource.Add(new item
                {
                    ID = ds.Tables[0].Rows[n]["ID"].ToString(),
                    ProductName = ds.Tables[0].Rows[n]["productName"].ToString(),
                    Price = Convert.ToDouble(ds.Tables[0].Rows[n]["productPrice"].ToString()),
                    isActive = Convert.ToBoolean(Convert.ToInt32(ds.Tables[0].Rows[n]["isActive"].ToString()))
                });
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtbPrice.Text != string.Empty && txtProductName.Text != string.Empty) //check if all fields are filled
                {
                    string isActive = chkActive.Checked ? "1" : "0";
                    string sqlQuery = "INSERT INTO items (productName, productPrice, isActive ) VALUES ('" + txtProductName.Text + "','" + txtbPrice.Text + "', '" + isActive + "'); ";

                    //insert data
                    MySQLconnection con = new MySQLconnection();
                    con.Executing(sqlQuery);

                    loadTable();
                }
                else
                {
                    MessageBox.Show("Please fill all the fields", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void txtbPrice_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
            {
                e.Handled = true;
            }

            // only allow one decimal point
            if ((e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1))
            {
                e.Handled = true;
            }
        }

        private void Main_Load(object sender, EventArgs e)
        {
            try
            {
                loadTable();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void redirectToUpdateForm(int row)
        {
            //create item object
            item Item = new item
            {
                //set data
                ID = dgvItems.Rows[row].Cells[0].Value.ToString(),
                ProductName = dgvItems.Rows[row].Cells[1].Value.ToString(),
                Price = Convert.ToDouble(dgvItems.Rows[row].Cells[2].Value.ToString()),
                isActive = (bool)dgvItems.Rows[row].Cells[3].Value
            };

            //show form
            using (Form frm = new FrmUpdateItem(Item))
            {
                frm.ShowDialog();
            }

            //load gridview
            loadTable();
        }

        private void deleteData(int row)
        {
            string ID = dgvItems.Rows[row].Cells[0].Value.ToString();
            string sqlQuery = "DELETE FROM items WHERE ID = '" + ID + "';";
            MySQLconnection con = new MySQLconnection();
            con.Executing(sqlQuery);
            //Load Table
            loadTable();
        }

        private void dgvItems_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                switch (e.ColumnIndex)
                {
                    case 4:
                        redirectToUpdateForm(e.RowIndex);
                        break;
                    case 5:
                        deleteData(e.RowIndex);
                        break;
                            
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
    }
}
