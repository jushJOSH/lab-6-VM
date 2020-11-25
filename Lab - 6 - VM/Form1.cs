using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lab___6___VM
{
    public partial class Form1 : Form
    {
        protected double X0, 
                         Y0, 
                         Xn, 
                         Step, 
                         Precision;

        public Form1()
        {
            InitializeComponent();
        }

        // Начальная функция
        private static double F(double x, double y)
        {
            return 3 - y - x;
        }

        // Точное решение
        private static double f(double x)
        {
            return 4 - x - 4 * Math.Exp(-x);
        }

        // Отрисовка точного решения
        public void DrawOriginalFunc()
        {
            chart1.Series[4].Name = "Точное решение";
            chart1.Series[4].Points.Clear();
            chart1.Series[4].IsVisibleInLegend = true;

            double x = X0;

            while (x <= Xn)
            {
                chart1.Series[4].Points.AddXY(x, f(x));
                x += Step;
            }
        }

        // Отрисовка Эйлера
        public void DrawEuler()
        {
            chart1.Series[0].Name = "Эйлер";
            chart1.Series[0].Points.Clear();
            chart1.Series[0].IsVisibleInLegend = true;

            double x = X0, 
                   y = Y0;

            while (x <= Xn)
            {
                chart1.Series[0].Points.AddXY(x, y);
                y = y + Step * F(x, y);
                x += Step;
            }
        }

        // Отрисовка исправленного Эйлера
        public void DrawFixedEuler()
        {
            chart1.Series[2].Name = "Исправленный Эйлер";
            chart1.Series[2].IsVisibleInLegend = true;
            chart1.Series[2].Points.Clear();

            double x = X0, y = Y0;

            while (x <= Xn)
            {
                chart1.Series[2].Points.AddXY(x, y);
                double inCalc = Step * (F(x + Step, y + Step * F(x, y)) + F(x, y));
                inCalc /= 2;
                y += inCalc;
                x += Step;
            }
        }

        // Отрисовка Адамса
        public void DrawAdams(int rang = 5)
        {
            chart1.Series[3].Name = "Адамс 5-го порядка";
            chart1.Series[3].IsVisibleInLegend = true;
            chart1.Series[3].Points.Clear();

            int[,] coeffs = { {3, -1, 0, 0, 0 },
                              {23,-16, 5, 0, 0 },
                              {55, -59,37,-9, 0 },
                              {1901, -2774, 2616, -1274, 251 } };

            double dev = 0;
            for (int i = 0; i < 5; i++)
            {
                dev += coeffs[rang - 2, i];
            }
            double[] Fs = new double[rang];

            double x = X0, y = Y0;
            for (int i = 0; i < rang; i++)
            {
                chart1.Series[3].Points.AddXY(x, y);
                Fs[rang - 1 - i] = F(x, y);
                y += Step * F(x, y);
                x += Step;
            }
            while (x <= Xn)
            {
                chart1.Series[3].Points.AddXY(x, y);
                for (int i = rang - 2; i >= 0; i--)
                {
                    Fs[i + 1] = Fs[i];
                }
                Fs[0] = F(x, y);
                double inCalc = 0;
                for (int i = 0; i < rang; i++)
                {
                    inCalc += (Fs[i] * coeffs[rang - 2, i]);
                }
                inCalc *= (Step / dev);
                y += inCalc;
                x += Step;
            }
        }

        // Отрисовка Рунге
        public void DrawRunge()
        {
            chart1.Series[1].Name = "Метод Рунге–Кутты, Мерсона";
            chart1.Series[1].IsVisibleInLegend = true;
            chart1.Series[1].Points.Clear();

            double[] k = new double[5];
            double x = X0, y = Y0, prec = Precision;

            while (x <= Xn)
            {
                chart1.Series[1].Points.AddXY(x, y);
                double p = 0;
                while (true)
                {
                    k[0] = Step * F(x, y);
                    k[1] = Step * F(x + Step / 3, y + k[0] / 3);
                    k[2] = Step * F(x + Step / 3, y + k[0] / 6 + k[1] / 6);
                    k[3] = Step * F(x + Step / 2, y + k[0] / 8 + 3 * k[2] / 8);
                    k[4] = Step * F(x + Step, y + k[0] / 2 - 3 * k[2] / 2 + 2 * k[3]);
                    p = (2 * k[0] - 9 * k[2] + 8 * k[3] - k[4]) / 30;

                    if (Math.Abs(Math.Round(p, 8)) > prec * Math.Abs(y)) 
                        Step /= 2;
                    else 
                        break;
                }

                if (Math.Abs(Math.Round(p, 8)) <= (prec * Math.Abs(y) / 32)) 
                    Step *= 2;
                
                y += k[0] / 6 + 2 * k[3] / 3 + k[4] / 6;
                x += Step;
            }
        }

        // Запуск
        private void button1_Click(object sender, EventArgs e)
        {
            if (!Double.TryParse(start_x.Text, out X0))
            {
                MessageBox.Show("Ошибка. Введённый X не является числом. Повторите ввод", "Ошибка ввода!");
                start_x.Clear();
                return;
            }

            if (!Double.TryParse(start_y.Text, out Y0))
            {
                MessageBox.Show("Ошибка. Введённый Y не является числом. Повторите ввод", "Ошибка ввода!");
                start_x.Clear();
                return;
            }

            if (!Double.TryParse(step.Text, out Step))
            {
                MessageBox.Show("Ошибка. Введённый шаг не является числом. Повторите ввод", "Ошибка ввода!");
                start_x.Clear();
                return;
            }

            if (!Double.TryParse(precision.Text, out Precision))
            {
                MessageBox.Show("Ошибка. Введённая точность не является числом. Повторите ввод", "Ошибка ввода!");
                start_x.Clear();
                return;
            }

            if (!Double.TryParse(start_xn.Text, out Xn))
            {
                MessageBox.Show("Ошибка. Введённый Xn не является числом. Повторите ввод", "Ошибка ввода!");
                start_x.Clear();
                return;
            }

            DrawAdams();
            DrawEuler();
            DrawFixedEuler();
            DrawRunge();
            DrawOriginalFunc();
        }

        // Загрузка формы
        private void Form1_Load(object sender, EventArgs e)
        {
            // Положение окна
            Top = 20;
            Left = 20;
        }
    }
}
