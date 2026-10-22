using System;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms.DataVisualization.Charting;
using UP_Denisov_Coffeeshop.Helpers;

namespace UP_Denisov_Coffeeshop.Pages
{
    public partial class DiagrammPage : Page
    {
        private static readonly SeriesChartType[] ChartTypes =
        {
            SeriesChartType.Column, SeriesChartType.Bar, SeriesChartType.Pie, SeriesChartType.Doughnut,
            SeriesChartType.Line, SeriesChartType.Spline, SeriesChartType.Area, SeriesChartType.Point
        };

        public DiagrammPage()
        {
            InitializeComponent();

            ChartPayments.ChartAreas.Add(new ChartArea("Main"));
            ChartPayments.Series.Add(new Series("Продажи") { IsValueShownAsLabel = true });
            ChartPayments.Titles.Add("Продажи по категориям, руб.");

            CbUser.ItemsSource = Entities.GetContext().User.AsNoTracking().OrderBy(u => u.FIO).ToList();
            CbChartType.ItemsSource = ChartTypes;
            CbChartType.SelectedItem = SeriesChartType.Column;
            CbUser.SelectedIndex = 0;
        }

        private void UpdateChart(object sender, SelectionChangedEventArgs e)
        {
            var user = CbUser.SelectedItem as User;
            if (user == null || CbChartType.SelectedItem == null || ChartPayments.Series.Count == 0)
                return;

            var series = ChartPayments.Series[0];
            series.ChartType = (SeriesChartType)CbChartType.SelectedItem;
            series.Name = user.FIO;
            series.Points.Clear();

            var totals = Entities.GetContext().Payment.AsNoTracking()
                .Where(p => p.UserID == user.ID)
                .Select(p => new { Category = p.Category.Name, Sum = p.Num * p.Price })
                .ToList()
                .GroupBy(p => p.Category)
                .Select(g => new { Category = g.Key, Sum = g.Sum(p => p.Sum) })
                .OrderBy(g => g.Category)
                .ToList();

            foreach (var item in totals)
                series.Points.AddXY(item.Category, item.Sum);

            ChartPayments.ChartAreas[0].AxisY.Title = "Сумма, руб.";
            ChartPayments.Titles[0].Text = "Продажи по категориям: " + user.FIO;
        }

        private void BtnWord_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "Документ Word (*.docx)|*.docx",
                FileName = "Отчет_по_продажам",
                DefaultExt = ".docx"
            };
            if (dialog.ShowDialog() != true) return;

            try
            {
                ReportExporter.ExportToWord(dialog.FileName);
                MessageHelper.Info("Отчёт сохранён:\n" + dialog.FileName + "\nPDF-версия лежит рядом.");
            }
            catch (Exception ex)
            {
                MessageHelper.Error("Не удалось выполнить экспорт. Проверьте, что установлен Microsoft Word.\n"
                                    + ex.GetBaseException().Message);
            }
        }

        private void BtnExcel_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "Книга Excel (*.xlsx)|*.xlsx",
                FileName = "Отчет_по_продажам",
                DefaultExt = ".xlsx"
            };
            if (dialog.ShowDialog() != true) return;

            try
            {
                ReportExporter.ExportToExcel(dialog.FileName);
                MessageHelper.Info("Отчёт сохранён:\n" + dialog.FileName);
            }
            catch (Exception ex)
            {
                MessageHelper.Error("Не удалось выполнить экспорт. Проверьте, что установлен Microsoft Excel.\n"
                                    + ex.GetBaseException().Message);
            }
        }
    }
}
