using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace A13_DVDKolekcija_3
{
    public partial class Form1 : Form
    {
        string connstr = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\A13.mdf;Integrated Security=True";
        DataTable dtProd = new DataTable();
        public Form1()
        {
            InitializeComponent();
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedItems.Count > 0)// ako je nesto selektovano
            {
                textBoxSifra.Text =
                    dtProd.Rows[listBox1.SelectedIndex][0].ToString();
                textBoxIme.Text =
                    dtProd.Rows[listBox1.SelectedIndex][1].ToString();
                textBoxEmail.Text =
                    dtProd.Rows[listBox1.SelectedIndex][2].ToString();
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            OsveziListu();
            // indeks najmanjeg ProducentID u dtProd
            int minID = (int)dtProd.Compute("min([ProducentID])", ""); // najmanja vrednost id u koloni nam ne treba, treba nam ideks
            listBox1.SelectedIndex = minID; // ova naredba implicitno poziva listBox1_SelectedIndexChanged
        }
        private void OsveziListu()
        {
            SqlConnection conn = new SqlConnection(connstr);
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandText = "select ProducentID, Ime, Email from Producent";
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            try
            {
                dtProd.Clear(); // praznimo dateTable da ne bi imali duplikate
                da.Fill(dtProd);
                listBox1.Items.Clear();
                foreach (DataRow dr in dtProd.Rows)
                {
                    listBox1.Items.Add(String.Format("{0,-7}{1,-40}{2,-30}", dr[0], dr[1], dr[2]));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Greska: " + ex.Message);
            }
            finally
            {
                conn.Close();
                cmd.Dispose();
                da.Dispose();
            }
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex == -1)
            {
                MessageBox.Show("Niste izabrali producenta kog menjate!");
                return;
            }
            if (textBoxIme.Text == "" || textBoxEmail.Text == "")
            {
                MessageBox.Show("Morate popuniti sva polja!");
                return;
            }
            SqlConnection conn = new SqlConnection(connstr);
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandText = "UPDATE Producent " +
                "SET Ime=@ime, Email=@email " + // ProducentID se ne menja (kolona ima Identity true)
                "WHERE ProducentID=@id";
            cmd.Parameters.AddWithValue("@ime", textBoxIme.Text);
            cmd.Parameters.AddWithValue("@email", textBoxEmail.Text);
            cmd.Parameters.AddWithValue("@id", int.Parse(textBoxSifra.Text));
            int selInd = listBox1.SelectedIndex;
            try
            {
                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
                MessageBox.Show("Uspešna izmena!");
                OsveziListu();
                listBox1.SelectedIndex = selInd;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Greska! " + ex.Message);
            }
            finally
            {
                conn.Close();
                cmd.Dispose();
            }
        }
    }
}
