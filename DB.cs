using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kursach
{
    internal class DB
    {
        //Подключение к базе данных
        MySqlConnection _connection = new MySqlConnection("server=localhost;port=3306;username=root;password=root;database=kursach");
        //Подключение и отключение базы данных
        public void openConnection()
        {
            if(_connection.State == System.Data.ConnectionState.Closed) 
                _connection.Open();
        }
        public void closeConnection()
        {
            if (_connection.State == System.Data.ConnectionState.Open)
                _connection.Close();
        }
        public MySqlConnection GetConnection()
        {
            return _connection;
        }
    }
}
