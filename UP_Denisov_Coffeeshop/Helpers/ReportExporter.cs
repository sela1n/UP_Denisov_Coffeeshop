using System;
using System.IO;
using System.Linq;
using Excel = Microsoft.Office.Interop.Excel;
using Word = Microsoft.Office.Interop.Word;

namespace UP_Denisov_Coffeeshop.Helpers
{

    public static class ReportExporter
    {
        private const int ExcelRed = 0x0000C0;
        private const int ExcelGreen = 0x006400;
        private const int ExcelHeader = 0xAF9801;
        private const int ExcelWhite = 0xFFFFFF;


        public static void ExportToWord(string docxPath)
        {
            using (var db = new Entities())
            {
                var users = db.User.OrderBy(u => u.FIO).ToList();
                var categories = db.Category.OrderBy(c => c.Name).ToList();
                var payments = db.Payment.ToList();

                var app = new Word.Application();
                Word.Document doc = null;
                try
                {
                    doc = app.Documents.Add();

                    for (int i = 0; i < users.Count; i++)
                    {
                        var user = users[i];
                        var userPayments = payments.Where(p => p.UserID == user.ID).ToList();

                        AddParagraph(doc, user.FIO, 18, true, Word.WdColor.wdColorAutomatic,
                                     Word.WdParagraphAlignment.wdAlignParagraphCenter);
                        AddParagraph(doc, "Роль: " + user.Role, 12, false, Word.WdColor.wdColorAutomatic,
                                     Word.WdParagraphAlignment.wdAlignParagraphCenter);

                        AddCategoryTable(doc, categories, userPayments);

                        if (userPayments.Count > 0)
                        {
                            var max = userPayments.OrderByDescending(p => p.Total).First();
                            var min = userPayments.OrderBy(p => p.Total).First();

                            AddParagraph(doc, "", 12, false, Word.WdColor.wdColorAutomatic,
                                         Word.WdParagraphAlignment.wdAlignParagraphLeft);
                            AddParagraph(doc, string.Format("Самая крупная продажа: {0} — {1:N2} руб. ({2:dd.MM.yyyy})",
                                         max.Name, max.Total, max.Date), 12, true,
                                         Word.WdColor.wdColorDarkRed, Word.WdParagraphAlignment.wdAlignParagraphLeft);
                            AddParagraph(doc, string.Format("Самая мелкая продажа: {0} — {1:N2} руб. ({2:dd.MM.yyyy})",
                                         min.Name, min.Total, min.Date), 12, true,
                                         Word.WdColor.wdColorDarkGreen, Word.WdParagraphAlignment.wdAlignParagraphLeft);
                        }
                        else
                        {
                            AddParagraph(doc, "Продаж нет.", 12, false, Word.WdColor.wdColorAutomatic,
                                         Word.WdParagraphAlignment.wdAlignParagraphLeft);
                        }

                        if (i < users.Count - 1)
                        {
                            Word.Range end = doc.Content;
                            end.Collapse(Word.WdCollapseDirection.wdCollapseEnd);
                            end.InsertBreak(Word.WdBreakType.wdPageBreak);
                        }
                    }

                    foreach (Word.Section section in doc.Sections)
                    {
                        Word.Range header = section.Headers[Word.WdHeaderFooterIndex.wdHeaderFooterPrimary].Range;
                        header.Text = "Отчёт по продажам сети кофеен от " + DateTime.Now.ToString("dd.MM.yyyy");
                        header.Font.Size = 10;
                        header.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;

                        section.Footers[Word.WdHeaderFooterIndex.wdHeaderFooterPrimary]
                               .PageNumbers.Add(Word.WdPageNumberAlignment.wdAlignPageNumberCenter);
                    }

                    doc.SaveAs2(docxPath);
                    doc.ExportAsFixedFormat(Path.ChangeExtension(docxPath, ".pdf"),
                                            Word.WdExportFormat.wdExportFormatPDF);
                    app.Visible = true;
                }
                catch
                {
                    if (doc != null) ((Word._Document)doc).Close(false);
                    ((Word._Application)app).Quit();
                    throw;
                }
            }
        }

        private static void AddParagraph(Word.Document doc, string text, int size, bool bold,
                                         Word.WdColor color, Word.WdParagraphAlignment align)
        {
            Word.Range range = doc.Content;
            range.Collapse(Word.WdCollapseDirection.wdCollapseEnd);
            range.Text = text;
            range.Font.Name = "Times New Roman";
            range.Font.Size = size;
            range.Font.Bold = bold ? 1 : 0;
            range.Font.Color = color;
            range.ParagraphFormat.Alignment = align;
            range.ParagraphFormat.SpaceAfter = 6;
            range.InsertParagraphAfter();
        }

        private static void AddCategoryTable(Word.Document doc,
            System.Collections.Generic.List<Category> categories,
            System.Collections.Generic.List<Payment> userPayments)
        {
            Word.Range range = doc.Content;
            range.Collapse(Word.WdCollapseDirection.wdCollapseEnd);

            Word.Table table = doc.Tables.Add(range, categories.Count + 2, 2);
            table.Borders.InsideLineStyle = Word.WdLineStyle.wdLineStyleSingle;
            table.Borders.OutsideLineStyle = Word.WdLineStyle.wdLineStyleSingle;
            table.Range.Font.Name = "Times New Roman";
            table.Range.Font.Size = 12;
            table.Range.Font.Bold = 0;
            table.Range.Font.Color = Word.WdColor.wdColorAutomatic;
            table.Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;

            table.Cell(1, 1).Range.Text = "Категория";
            table.Cell(1, 2).Range.Text = "Сумма продаж, руб.";
            table.Rows[1].Range.Font.Bold = 1;
            table.Rows[1].Shading.BackgroundPatternColor = Word.WdColor.wdColorGray15;

            decimal total = 0;
            for (int i = 0; i < categories.Count; i++)
            {
                decimal sum = userPayments.Where(p => p.CategoryID == categories[i].ID).Sum(p => p.Total);
                total += sum;
                table.Cell(i + 2, 1).Range.Text = categories[i].Name;
                table.Cell(i + 2, 2).Range.Text = sum.ToString("N2");
            }

            int last = categories.Count + 2;
            table.Cell(last, 1).Range.Text = "Итого";
            table.Cell(last, 2).Range.Text = total.ToString("N2");
            table.Rows[last].Range.Font.Bold = 1;
        }


        public static void ExportToExcel(string xlsxPath)
        {
            using (var db = new Entities())
            {
                var users = db.User.OrderBy(u => u.FIO).ToList();
                var categories = db.Category.ToDictionary(c => c.ID, c => c.Name);
                var payments = db.Payment.ToList();

                var app = new Excel.Application();
                Excel.Workbook book = null;
                try
                {
                    book = app.Workbooks.Add();

                    for (int i = 0; i < users.Count; i++)
                    {
                        var user = users[i];
                        var userPayments = payments.Where(p => p.UserID == user.ID)
                                                   .OrderBy(p => p.Date).ThenBy(p => p.ID).ToList();

                        Excel.Worksheet sheet = (i == 0)
                            ? (Excel.Worksheet)book.Worksheets[1]
                            : (Excel.Worksheet)book.Worksheets.Add(Type.Missing, book.Worksheets[book.Worksheets.Count],
                                                                    Type.Missing, Type.Missing);
                        sheet.Name = SheetName(user.FIO, i + 1);
                        FillSheet(sheet, user, userPayments, categories);
                    }

                    app.DisplayAlerts = false;
                    book.SaveAs(xlsxPath, Excel.XlFileFormat.xlOpenXMLWorkbook);
                    app.DisplayAlerts = true;
                    app.Visible = true;
                }
                catch
                {
                    if (book != null) book.Close(false);
                    app.Quit();
                    throw;
                }
            }
        }

        private static void FillSheet(Excel.Worksheet sheet, User user,
            System.Collections.Generic.List<Payment> userPayments,
            System.Collections.Generic.Dictionary<int, string> categories)
        {
            sheet.Cells[1, 1] = user.FIO;
            sheet.Range["A1"].Font.Bold = true;
            sheet.Range["A1"].Font.Size = 14;

            string[] headers = { "Дата", "Категория", "Позиция", "Кол-во", "Цена, руб.", "Сумма, руб." };
            for (int c = 0; c < headers.Length; c++)
                sheet.Cells[3, c + 1] = headers[c];
            sheet.Range["A3:F3"].Font.Bold = true;
            sheet.Range["A3:F3"].Font.Color = ExcelWhite;
            sheet.Range["A3:F3"].Interior.Color = ExcelHeader;

            int row = 4;
            foreach (var p in userPayments)
            {
                sheet.Cells[row, 1] = p.Date;
                sheet.Cells[row, 2] = categories.ContainsKey(p.CategoryID) ? categories[p.CategoryID] : "";
                sheet.Cells[row, 3] = p.Name;
                sheet.Cells[row, 4] = p.Num;
                sheet.Cells[row, 5] = p.Price;
                sheet.Cells[row, 6] = p.Total;
                row++;
            }

            if (userPayments.Count > 0)
            {
                int first = 4, last = row - 1;
                sheet.Range["A" + first + ":A" + last].NumberFormat = "dd.MM.yyyy";
                sheet.Range["E" + first + ":F" + (last + 1)].NumberFormat = "# ##0,00";

                sheet.Cells[row, 5] = "Итого:";
                sheet.Range["F" + row].Formula = "=SUM(F" + first + ":F" + last + ")";
                sheet.Range["E" + row + ":F" + row].Font.Bold = true;

                var max = userPayments.OrderByDescending(p => p.Total).First();
                var min = userPayments.OrderBy(p => p.Total).First();
                sheet.Range["A" + (4 + userPayments.IndexOf(max)) + ":F" + (4 + userPayments.IndexOf(max))]
                     .Font.Color = ExcelRed;
                sheet.Range["A" + (4 + userPayments.IndexOf(min)) + ":F" + (4 + userPayments.IndexOf(min))]
                     .Font.Color = ExcelGreen;

                sheet.Cells[row + 2, 1] = string.Format("Самая крупная продажа: {0} — {1:N2} руб.", max.Name, max.Total);
                sheet.Range["A" + (row + 2)].Font.Color = ExcelRed;
                sheet.Range["A" + (row + 2)].Font.Bold = true;
                sheet.Cells[row + 3, 1] = string.Format("Самая мелкая продажа: {0} — {1:N2} руб.", min.Name, min.Total);
                sheet.Range["A" + (row + 3)].Font.Color = ExcelGreen;
                sheet.Range["A" + (row + 3)].Font.Bold = true;
            }
            else
            {
                sheet.Cells[4, 1] = "Продаж нет.";
            }

            sheet.Columns.AutoFit();

            try
            {
                sheet.PageSetup.CenterHeader = "Отчёт по продажам сети кофеен от " + DateTime.Now.ToString("dd.MM.yyyy");
                sheet.PageSetup.CenterFooter = "Страница &P из &N";
            }
            catch { }
        }

        private static string SheetName(string fio, int index)
        {
            char[] bad = { ':', '\\', '/', '?', '*', '[', ']' };
            string name = new string(fio.Where(c => !bad.Contains(c)).ToArray());
            string prefix = index + ". ";
            if (prefix.Length + name.Length > 31)
                name = name.Substring(0, 31 - prefix.Length);
            return prefix + name;
        }
    }
}
