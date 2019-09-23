using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;

namespace PORTAL.DAL
{
    public class MySQLconnection
    {
        public MySqlConnection connection;

        public MySqlConnection CreateConnection() //create mysql connection
        {
            try
            {
                string connectionString = ConfigurationManager.AppSettings["AppConnection"];
                connection = new MySqlConnection(connectionString);
                return connection;
            } catch (MySqlException ex)
            {
                throw ex;
            }
        }

        private bool OpenConnection() //connect open
        {
            try
            {
                connection = CreateConnection();
                if(connection.State == ConnectionState.Closed) connection.Open();
                return true;
            }
            catch (MySqlException ex)
            {
                throw ex;
            }
        }

        private bool CloseConnection() //conne ction close
        {
            try
            {
                if (connection.State == ConnectionState.Open) connection.Close();
                return true;
            }
            catch (MySqlException ex)
            {
                throw ex;
            }
        }

        public void Executing(string query) //execute sql query
        {
            try
            {
                if (OpenConnection())
                {
                    //create command and assign the query and connection from the constructor
                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                CloseConnection();
            }
        }

        public DataSet LoadData(string Query) //get data using sql query
        {
            try
            {
                DataSet ds = new DataSet();
                if (OpenConnection())
                {
                    using (MySqlCommand cmd = new MySqlCommand(Query, connection))
                    {
                        cmd.CommandType = CommandType.Text;

                        using (MySqlDataAdapter sda = new MySqlDataAdapter(cmd))
                        {
                            sda.Fill(ds);
                        }
                    }
                }
                return ds;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                CloseConnection();
            }
        }

        public DataSet ExucteSP(Dictionary<string, string> Params, string spName) // execute sql stored procedure 
        {
            try
            {
                DataSet ds = new DataSet();
                if (OpenConnection()) //open connection
                {
                    using (MySqlCommand cmd = new MySqlCommand(spName, connection))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        foreach (KeyValuePair<string, string> item in Params)
                        {
                            cmd.Parameters.AddWithValue("@" + item.Key, item.Value);
                        }
                        using (MySqlDataAdapter sda = new MySqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            sda.Fill(ds);
                        }
                    }
                }
                return ds;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                CloseConnection();
            }
        }
    }
}
