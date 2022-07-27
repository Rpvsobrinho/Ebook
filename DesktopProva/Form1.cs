using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace DesktopProva
{

    public partial class Form1 : Form
    {
        private MySqlConnection Conexao;
        private string data_source = "datasource=localhost;username=root;password=;database=DesktopProva";
        private int? idSelecionado = null;
        public Form1()
        {
            InitializeComponent();
            carregar();
            listlivro.View = View.Details;
            listlivro.LabelEdit = true;
            listlivro.AllowColumnReorder = true;
            listlivro.FullRowSelect = true;
            listlivro.GridLines = true;


            listlivro.Columns.Add("Id", 60, HorizontalAlignment.Center);
            listlivro.Columns.Add("Titulo", 100, HorizontalAlignment.Center);
            listlivro.Columns.Add("Autor", 100, HorizontalAlignment.Center);
            listlivro.Columns.Add("Categoria", 150, HorizontalAlignment.Center);
        }

        private void button1_Click(object sender, EventArgs e)
        {


            try
            {

                string autor = txtautor.Text;
                string titulo = txttitulo.Text;
                string categoria = txtcategoria.Text;

                if (titulo == "")
                {
                    MessageBox.Show("Favor informar o título", "Título", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else if (autor == "")
                {
                    MessageBox.Show("Favor informar o autor", "Autor", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else if (categoria == "")
                {
                    MessageBox.Show("Favor informar a categoria", "Categoria", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {





                    Conexao = new MySqlConnection(data_source);
                    Conexao.Open();
                    MySqlCommand cmd = new MySqlCommand();
                    cmd.Connection = Conexao;




                    if (idSelecionado == null)
                    {

                        //salvar
                        cmd.Prepare();
                        cmd.Parameters.AddWithValue("titulo", titulo);
                        cmd.Parameters.AddWithValue("autor", autor);
                        cmd.Parameters.AddWithValue("categoria", categoria);



                        cmd.CommandText = "INSERT INTO base (titulo, autor, categoria) " +
                                     " VALUES (@titulo, @autor, @categoria)";

                        cmd.ExecuteNonQuery();

                        MessageBox.Show("Livro inserido com sucesso!", "Inserido", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);

                        txtautor.Clear();
                        txttitulo.Clear();
                        txtcategoria.Clear();
                        carregar();
                    }
                    else
                    {
                        //atualizar
                        cmd.Prepare();
                        cmd.Parameters.AddWithValue("titulo", titulo);
                        cmd.Parameters.AddWithValue("autor", autor);
                        cmd.Parameters.AddWithValue("categoria", categoria);
                        cmd.Parameters.AddWithValue("id", idSelecionado);

                        cmd.CommandText = "UPDATE base SET titulo = @titulo, autor = @autor, categoria = @categoria WHERE id = @id ";

                        cmd.ExecuteNonQuery();

                        MessageBox.Show("Livro editado com sucesso!");
                        idSelecionado = null;
                        txtautor.Clear();
                        txttitulo.Clear();
                        txtcategoria.Clear();
                        carregar();
                    }
                }


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                Conexao.Close();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtbuscar.Text == "")
                {
                    MessageBox.Show("Favor informar o que deseja pesquisar", "Buscar", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                string buscar = txtbuscar.Text;

                Conexao = new MySqlConnection(data_source);
                Conexao.Open();
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = Conexao;
                cmd.Prepare();

                cmd.CommandText = "SELECT * FROM base WHERE titulo LIKE @q OR autor LIKE @q OR categoria LIKE @q";
                cmd.Parameters.AddWithValue("@q", "%" + buscar + "%");




                MySqlDataReader reader = cmd.ExecuteReader();


                listlivro.Items.Clear();

                while (reader.Read())
                {
                    string[] row =
                    {
                        reader.GetString(0),
                        reader.GetString(1),
                        reader.GetString(2),
                        reader.GetString(3),
                    };
                    listlivro.Items.Add(new ListViewItem(row));
                }




            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {

            }
        }

        private void carregar()
        {
            try
            {
                string buscar = txtbuscar.Text;

                Conexao = new MySqlConnection(data_source);
                Conexao.Open();
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = Conexao;
                cmd.CommandText = "SELECT * FROM base ORDER BY id DESC";
                cmd.Prepare();





                MySqlDataReader reader = cmd.ExecuteReader();


                listlivro.Items.Clear();

                while (reader.Read())
                {
                    string[] row =
                    {
                        reader.GetString(0),
                        reader.GetString(1),
                        reader.GetString(2),
                        reader.GetString(3),
                    };
                    listlivro.Items.Add(new ListViewItem(row));
                }




            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {

            }
        }

        private void listlivro_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            ListView.SelectedListViewItemCollection selecionar = listlivro.SelectedItems;

            foreach (ListViewItem seleciona in selecionar)
            {
                idSelecionado = Convert.ToInt32(seleciona.SubItems[0].Text);
                txttitulo.Text = seleciona.SubItems[1].Text;
                txtautor.Text = seleciona.SubItems[2].Text;
                txtcategoria.Text = seleciona.SubItems[3].Text;
            }

        }

        private void Editar_Click(object sender, EventArgs e)
        {
            idSelecionado = null;

            txtautor.Clear();
            txttitulo.Clear();
            txtcategoria.Clear();
            txtbuscar.Clear();
            button3.Visible = false;
            carregar();
        }

        private void button3_Click(object sender, EventArgs e)
        {

            try
            {
                DialogResult conf = MessageBox.Show("Deseja Excluir?", "Excluir", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (conf == DialogResult.Yes)
                {

                    Conexao = new MySqlConnection(data_source);
                    Conexao.Open();
                    MySqlCommand cmd = new MySqlCommand();
                    cmd.Connection = Conexao;

                    cmd.Parameters.AddWithValue("id", idSelecionado);

                    cmd.CommandText = "DELETE FROM base WHERE id = @id ";

                    cmd.ExecuteNonQuery();


                    MessageBox.Show("Livro excluído", "Excluído", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    carregar();
                    button3.Visible = false;
                    txtautor.Clear();
                    txttitulo.Clear();
                    txtcategoria.Clear();
                    idSelecionado = null;
                }
                else
                {

                }

            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                Conexao.Close();
            }


        }

        private void listlivro_SelectedIndexChanged(object sender, EventArgs e)
        {
            button3.Visible = true;
            if (listlivro.SelectedItems.Count < 0)
            {
                string autor = txtautor.Text;
                string titulo = txttitulo.Text;
                string categoria = txtcategoria.Text;

                if (titulo == "")
                {
                    MessageBox.Show("Favor informar o título", "Título", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else if (autor == "")
                {
                    MessageBox.Show("Favor informar o autor", "Autor", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else if (categoria == "")
                {
                    MessageBox.Show("Favor informar a categoria", "Categoria", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {





                    Conexao = new MySqlConnection(data_source);
                    Conexao.Open();
                    MySqlCommand cmd = new MySqlCommand();
                    cmd.Connection = Conexao;

                    //salvar
                    cmd.Prepare();
                    cmd.Parameters.AddWithValue("titulo", titulo);
                    cmd.Parameters.AddWithValue("autor", autor);
                    cmd.Parameters.AddWithValue("categoria", categoria);



                    cmd.CommandText = "INSERT INTO base (titulo, autor, categoria) " +
                                 " VALUES (@titulo, @autor, @categoria)";

                    cmd.ExecuteNonQuery();

                    MessageBox.Show("Livro inserido com sucesso!", "Inserido", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);

                    txtautor.Clear();
                    txttitulo.Clear();
                    txtcategoria.Clear();
                    carregar();
                }
            }
        }

        private void txtbuscar_TextChanged(object sender, EventArgs e)
        {
            if (txtbuscar.Text == "")
            {
                carregar();
            }
        }

        private void txttitulo_TextChanged(object sender, EventArgs e)
        {

        }
    }



}






