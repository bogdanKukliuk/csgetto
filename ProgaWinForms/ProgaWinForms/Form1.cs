using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace ProgaWinForms
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            for (int i = 1, k = 0; i < Problem.N-1; k++, i++)
            {
         //       chart1.Series["0"].Points.AddXY(k * Problem.h, Problem.C[i, 3]);
           //     chart2.Series["0"].Points.AddXY(k * Problem.h, Problem.CV2[i, 3]);
                chart1.Series["1"].Points.AddXY(k * Problem.h, Problem.V[i, 3]);
                chart2.Series["1"].Points.AddXY(k * Problem.h, Problem.V2[i, 3]);
                chart1.Series["0"].Points.AddXY(k * Problem.h, 3.5 * (Problem.H2 - Problem.H1) / Problem.L);
                chart2.Series["0"].Points.AddXY(k * Problem.h, 0.05 * (Problem.H2 - Problem.H1) / Problem.L);



            }

        }

        private void GridViewMass(int x, int t, double[,] C, double[,] CV2)
        {
            dataGridView1.RowCount = t;
            dataGridView1.ColumnCount = x;
            dataGridView2.RowCount = t;
            dataGridView2.ColumnCount = x;
            for (int i = 0; i < x; i++)
            {
                for (int j = 0; j < t; j++)
                {
                    dataGridView1.Rows[j].Cells[i].Value = C[i, j];
                    dataGridView2.Rows[j].Cells[i].Value = CV2[i, j];
                }
            }
        }

        private void DrawChart(int x, int t, int h, double[,] C)
        {
            chart1.ChartAreas.Add(new ChartArea("Default"));
            chart1.ChartAreas["Default"].AxisX.MajorGrid.LineDashStyle = ChartDashStyle.Dash;
            chart1.ChartAreas["Default"].AxisY.MajorGrid.LineDashStyle = ChartDashStyle.Dash;
            chart1.ChartAreas["Default"].AxisX.MajorGrid.Enabled = true;
            chart1.ChartAreas["Default"].AxisY.MajorGrid.Enabled = true;
            chart1.ChartAreas["Default"].AxisY.MajorGrid.LineColor = System.Drawing.Color.Gray;
            chart1.ChartAreas["Default"].AxisX.MajorGrid.LineColor = System.Drawing.Color.Gray;
            Random rnd = new Random();

            for (int j = 0; j < t; j++)
            {
                chart1.Series.Add($"K={j}");
                chart1.Series[$"K={j}"].ChartArea = "Default";
                chart1.Series[$"K={j}"].BorderWidth = 2;
                chart1.Series[$"K={j}"].ChartType = SeriesChartType.Spline;


                for (int i = 0, k = 0; i < x; k++, i++)
                {
                    chart1.Series[$"K={j}"].Points.AddXY(k * h, C[i, j]);
                }
            }
            chart1.Series[$"K={0}"].Color = System.Drawing.Color.Cyan;
            chart1.Series[$"K={1}"].Color = System.Drawing.Color.Red;
            chart1.Series[$"K={2}"].Color = System.Drawing.Color.Green;
            chart1.Series[$"K={3}"].Color = System.Drawing.Color.Magenta;

        }

        private void button1_Click(object sender, EventArgs e)
        {
            GridViewMass(Problem.N, Problem.K, Problem.V, Problem.V2);
            //        DrawChart(Problem.N, Problem.K,Problem.h, Problem.C);
        }
    }
    static class Problem
    {
        static Problem()
        {
            Start();
            CalculateC();
            CalculateC2();
            CalculateCK1();
            CalculateCK2();

        }
        public static int L = 100;
        public static double H1 = 0.5;
        public static double H2 = 2;
        public static double C1 = 5;
        public static double C2 = 50;
        public static double Cm = 100;
        public static int h = 10;
        public static int N = 10;
        public static int K = 4;
        public static double[,] C;
        public static double[,] CV2;
        public static double[,] CK1;
        public static double[,] CK2;

        public static double[,] H;
        public static double[,] V;
        public static double[,] V2;

        public static void Start()
        {
            CV2 = new double[N, K];
            CK1 = new double[N, K];
            CK2 = new double[N, K];

            C = new double[N, K];
            H = new double[N, K];
            V = new double[N, K];
            V2 = new double[N, K];

            for (int j = 0; j < K; j++)
            {
                C[0, j] = C1;
                C[N - 1, j] = C2;
                CV2[0, j] = C1;
                CV2[N - 1, j] = C2;

                CK1[0, j] = C1;
                CK1[N - 1, j] = C2;
                CK2[0, j] = C1;
                CK2[N - 1, j] = C2;


                H[0, j] = H1;
                H[N - 1, j] = H2;
            }

            for (int i = 1; i < N - 1; i++)
            {
                C[i, 0] = (C2 - C1) / (L * L) * ((h * i) * (h * i)) + C1;
                CV2[i, 0] = (C2 - C1) / (L * L) * ((h * i) * (h * i)) + C1;
                CK1[i, 0] = (C2 - C1) / (L * L) * ((h * i) * (h * i)) + C1;
                CK2[i, 0] = (C2 - C1) / (L * L) * ((h * i) * (h * i)) + C1;

            }

            for (int j = 0; j < K; j++)
            {
                for (int i = 1; i < N - 1; i++)
                {
                    H[i, j] = (H2 - H1) / (L * L) * ((h * i) * (h * i)) + H1;
                }
            }
        }

        public static void CalculateC()
        {
            double D = 0.02;
            double Sigma = 0.9;
            double Gamma = 0.0065;

            double rMinus = -0.0228;
            double rPlus = 0;
            double tau = 30;

            double[] Alfa = new double[N];
            double[] Beta = new double[N];
            double[] F = new double[N];

            Alfa[0] = 0;
            Beta[0] = C2;

            double a6 = 2 * Math.Pow(10, -4);
            double a5 = -0.88 * Math.Pow(10, -2);
            double a4 = 0.162;
            double a3 = -1.3194;
            double a2 = 3.9229;
            double a1 = 0.0223;
            double a0 = 18.187;



            for (int j = 1; j < K; j++)
            {
                for (int i = 1; i < N - 1; i++)
                {
                    var c = C[i, j - 1] / Cm;
                    V[i, j] = Math.Abs(((a6 * Math.Pow(c, 6)) + (a5 * Math.Pow(c, 5)) + (a4 * Math.Pow(c, 4)) + (a3 * Math.Pow(c, 3)) + (a2 * Math.Pow(c, 2)) + (a1 * c) + a0) * (H[i, j] - H[i - 1, j]) / (h));
                }

                for (int i = 1; i < N; i++)
                {

                    double mu = 1 / (1 + h * V[i, j] / (2 * D));
                    double a = ((mu / (h * h)) - (rMinus / h));
                    double b = ((mu / (h * h)) + (rPlus / h));
                    double cc = (((2 * mu) / (h * h)) + rPlus / h - rMinus / h + Gamma / D + Sigma / (D * tau));

                    F[i] = (Gamma * D) / Cm + (Sigma / (D * tau)) * C[i, j - 1];
                    Alfa[i] = b / (cc - Alfa[i - 1] * a);
                    Beta[i] = (a * Beta[i - 1] + F[i]) / (cc - Alfa[i - 1] * a);
                }

                for (int i = N - 2; i > 0; i--)
                {
                    C[i, j] = Alfa[i + 1] * C[i + 1, j] + Beta[i + 1];
                }


            }
        }

        public static void CalculateC2()
        {
            double D = 0.02;
            double Sigma = 0.9;
            double Gamma = 0.0065;

            double rMinus = -0.0228;
            double rPlus = 0;
            double tau = 30;

            double[] Alfa = new double[N];
            double[] Beta = new double[N];
            double[] F = new double[N];

            Alfa[0] = 0;
            Beta[0] = C2;


            double A5 = 5.9404 * Math.Pow(10, -2);
            double A4 = -1.6703 * Math.Pow(10, -1);
            double A3 = 1.7051 * Math.Pow(10, -1);
            double A2 = -7.4311 * Math.Pow(10, -2);
            double A1 = 1.0563 * Math.Pow(10, -2);
            double A0 = 1.0054 * Math.Pow(10, -3);


            for (int j = 1; j < K; j++)
            {
                for (int i = 1; i < N - 1; i++)
                {
                    var c = CV2[i, j - 1] / Cm;
                    V2[i, j] = Math.Abs(((A5 * Math.Pow(c, 5)) + (A4 * Math.Pow(c, 4)) + (A3 * Math.Pow(c, 3)) + (A2 * Math.Pow(c, 2)) + (A1 * c) + A0) * (H[i, j] - H[i - 1, j]) / (h));
                }


                for (int i = 1; i < N; i++)
                {

                    double mu = 1 / (1 + h * V2[i, j] / (2 * D));
                    double a = ((mu / (h * h)) - (rMinus / h));
                    double b = ((mu / (h * h)) + (rPlus / h));
                    double cc = (((2 * mu) / (h * h)) + rPlus / h - rMinus / h + Gamma / D + Sigma / (D * tau));

                    F[i] = (Gamma * D) / Cm + (Sigma / (D * tau)) * CV2[i, j - 1];
                    Alfa[i] = b / (cc - Alfa[i - 1] * a);
                    Beta[i] = (a * Beta[i - 1] + F[i]) / (cc - Alfa[i - 1] * a);
                }

                for (int i = N - 2; i > 0; i--)
                {
                    CV2[i, j] = Alfa[i + 1] * CV2[i + 1, j] + Beta[i + 1];
                }


            }
        }

        public static void CalculateCK1()
        {
            double k = 0.05;//глина
            double D = 0.02;
            double Sigma = 0.9;
            double Gamma = 0.0065;

            double rMinus = -0.0228;
            double rPlus = 0;
            double tau = 30;

            double[] Alfa = new double[N];
            double[] Beta = new double[N];
            double[] F = new double[N];

            Alfa[0] = 0;
            Beta[0] = C2;

            double V = k * (H2 - H1) / L;


            for (int j = 1; j < K; j++)
            {
            
                for (int i = 1; i < N; i++)
                {
                    double mu = 1 / (1 + h * V / (2 * D));
                    double a = ((mu / (h * h)) - (rMinus / h));
                    double b = ((mu / (h * h)) + (rPlus / h));
                    double cc = (((2 * mu) / (h * h)) + rPlus / h - rMinus / h + Gamma / D + Sigma / (D * tau));

                    F[i] = (Gamma * D) / Cm + (Sigma / (D * tau)) * CK1[i, j - 1];
                    Alfa[i] = b / (cc - Alfa[i - 1] * a);
                    Beta[i] = (a * Beta[i - 1] + F[i]) / (cc - Alfa[i - 1] * a);
                }

                for (int i = N - 2; i > 0; i--)
                {
                    CK1[i, j] = Alfa[i + 1] * CK1[i + 1, j] + Beta[i + 1];
                }


            }

        }

        public static void CalculateCK2()
        {
            double k = 3.5;//пісок дрібнозернистий
            double D = 0.02;
            double Sigma = 0.9;
            double Gamma = 0.0065;

            double rMinus = -0.0228;
            double rPlus = 0;
            double tau = 30;

            double[] Alfa = new double[N];
            double[] Beta = new double[N];
            double[] F = new double[N];

            Alfa[0] = 0;
            Beta[0] = C2;

            double V = k * (H2 - H1) / L;


            for (int j = 1; j < K; j++)
            {

                for (int i = 1; i < N; i++)
                {
                    double mu = 1 / (1 + h * V / (2 * D));
                    double a = ((mu / (h * h)) - (rMinus / h));
                    double b = ((mu / (h * h)) + (rPlus / h));
                    double cc = (((2 * mu) / (h * h)) + rPlus / h - rMinus / h + Gamma / D + Sigma / (D * tau));

                    F[i] = (Gamma * D) / Cm + (Sigma / (D * tau)) * CK2[i, j - 1];
                    Alfa[i] = b / (cc - Alfa[i - 1] * a);
                    Beta[i] = (a * Beta[i - 1] + F[i]) / (cc - Alfa[i - 1] * a);
                }

                for (int i = N - 2; i > 0; i--)
                {
                    CK2[i, j] = Alfa[i + 1] * CK2[i + 1, j] + Beta[i + 1];
                }


            }

        }
    }
}