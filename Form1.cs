using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.IO.Ports;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;






namespace Kursach
{
    public partial class Form1 : Form


    {
        public Form1()
        {
            InitializeComponent();

           
            serialPort1.PortName = "COM6"; //Даём имя COM-порту
            serialPort1.BaudRate = 115200; //Задаем скорость
            serialPort1.DtrEnable = true; //Включяем терминал для получения значений
            serialPort1.Open(); //Собственно открываем порт
            serialPort1.DataReceived += serialPort1_DataReceived; //записываем данные
        }
        //****** поток ком порта
        private void serialPort1_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            string vlag = serialPort1.ReadLine(); //Записываем данные с порта в переменную
            this.BeginInvoke(new LineReceivedEvent(LineReceived), vlag); //Задаем асинхронность
        }

        private delegate void LineReceivedEvent(string vlag);
        private void LineReceived(string vlag)
        {
            textBox1.Text = vlag; //выводим на экран в интерфесе значение напряжения
            string path = "Данные с COM порта.txt"; //Подготавливаем текстовый файл
            string date = DateTime.Now.ToString(); //Записываем дату когда были получены данные
            // Создание файла
            using (StreamWriter sw = File.CreateText(path))
            {
                sw.WriteLine(vlag); //Запись данных в текстовый файл
            }
        }



        private int _countSeaconds = 0; //Переменная для работы с таймером

        private void Form1_Load(object sender, EventArgs e)
        {
            Timer.Enabled = true; //Включаем таймер

            Chart.ChartAreas[0].AxisY.Maximum = 3.5; //Максимальное значение по оси Y
            Chart.ChartAreas[0].AxisY.Minimum = 0; //Минимальное значение по оси Y
            Chart.ChartAreas[0].AxisY.Interval = 0.1; //Интервал между значениями

            Chart.ChartAreas[0].AxisX.LabelStyle.Format = "H:mm:ss"; //Формат времени для вывода на график
            Chart.Series[0].XValueType = ChartValueType.DateTime; //Берем значение времени системы

            Chart.ChartAreas[0].AxisX.Minimum = DateTime.Now.ToOADate(); //Минимальное значение по оси X
            Chart.ChartAreas[0].AxisX.Maximum = DateTime.Now.AddSeconds(30).ToOADate(); //Максимальное значение по оси X

            Chart.ChartAreas[0].AxisX.IntervalType = DateTimeIntervalType.Seconds; //Настраиваем интервал
            Chart.ChartAreas[0].AxisX.Interval = 1; //Значение интервал
        }



        private void Timer_Tick_1(object sender, EventArgs e)
        {
            DateTime timeNow = DateTime.Now;
            //Берём значение из текстового файла
            string text = File.ReadAllText(@"C:\Users\user\source\repos\Kursach\Kursach\bin\Release\Данные с COM порта.txt");
            //Обьявляем переменную 
            string volt = "";
            //Проверка подключения
            DB db = new DB();
            DataTable table = new DataTable();
            MySqlDataAdapter adapter = new MySqlDataAdapter();

            //Отправляем запрос на добавление данных в таблицу базы данных
            MySqlCommand command = new MySqlCommand("INSERT INTO `series0` (`id`, `Voltage`, `Date`) VALUES (NULL, @volt, CURRENT_TIME()) ", db.GetConnection());
            command.Parameters.Add("@volt", MySqlDbType.VarChar).Value = text;
            //Отправляем запрос на получение данных из таблицы базы данных
            MySqlCommand commands = new MySqlCommand("SELECT volatage FROM series0 ORDER BY Voltage DESC LIMIT 0,1;", db.GetConnection());
            commands.Parameters.Add("volatge", MySqlDbType.VarChar).Value = volt;
            //Открытие соединения с базой данных
            db.openConnection();
            //Проверка успешности отправки запроса
            if (command.ExecuteNonQuery() == 1)
            {
                
            }
            else
            {
                
            }           
            //Закрытие соединения с базой данных
            db.closeConnection();



            //Объявление переменных
            string value = text;
            double ser2= 3.3;
            double ser3 = 1.65;
            double ser4 = 2.2;


            //Настройка самих графиков
            Chart.Series[0].Points.AddXY(timeNow, value);

            Chart.Series[1].Points.AddXY(timeNow, ser2);

            Chart.Series[2].Points.AddXY(timeNow, ser3);

            Chart.Series[3].Points.AddXY(timeNow, ser4);
           
            _countSeaconds++;
            //Цикличность работы (реализация работы в реальном времени)
            if (_countSeaconds == 30)
            {
                _countSeaconds = 0;

                TimeSpan period = TimeSpan.FromSeconds(10);

                DateTime min = DateTime.FromOADate(Chart.ChartAreas[0].AxisX.Minimum);
                DateTime max = DateTime.FromOADate(Chart.ChartAreas[0].AxisX.Maximum);

                Chart.ChartAreas[0].AxisX.Minimum = min.Add(period).ToOADate();
                Chart.ChartAreas[0].AxisX.Maximum = max.Add(period).ToOADate();

                Chart.ChartAreas[0].AxisX.IntervalType = DateTimeIntervalType.Seconds;
                Chart.ChartAreas[0].AxisX.Interval = 1;
            }
        }
    }
}

