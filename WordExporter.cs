using Microsoft.Office.Interop.Word;
using System;
using Weather.Data;


namespace Weather
{
    public class WordExporter
    {
        public bool Export(MonthStatistic statistic, out string ReportPath, out Exception? error)
        {
            ReportPath = "";
            error = null;
            try
            {
                string? CurrDirectory = AppConfiguration.Configuration["CurrentDirectory"];
                string? TemplateFileName = AppConfiguration.Configuration["WordReportNames:TemplateFileName"];
                string? ReportFileName = AppConfiguration.Configuration["WordReportNames:ReportFileName"];
                if (CurrDirectory == null)
                {
                    throw new ArgumentNullException(nameof(CurrDirectory));
                }
                if (TemplateFileName == null)
                {
                    throw new ArgumentNullException(nameof(TemplateFileName));
                }
                if (ReportFileName == null)
                {
                    throw new ArgumentNullException(nameof(ReportFileName));
                }

                string TemplatePath = System.IO.Path.Combine(CurrDirectory, TemplateFileName);
                ReportPath = System.IO.Path.Combine(CurrDirectory, DateTime.Now.ToString("yyyyMMdd_HHmmss") + ReportFileName);
                Microsoft.Office.Interop.Word.Application wordApp = new Microsoft.Office.Interop.Word.Application();
                Document doc = wordApp.Documents.Add(CurrDirectory + "/" + TemplateFileName);

                doc.Content.Find.Execute("<t_mean>", ReplaceWith: statistic.TemperatureMean.ToString());
                doc.Content.Find.Execute("<p_mean>", ReplaceWith: statistic.PressureMean.ToString());
                int maxDatesPerLine = 10; // Number of dates in each line
                int lineCount = (int)Math.Ceiling((double)statistic.Dates.Count / maxDatesPerLine); // Number of lines
                int remainingDates = statistic.Dates.Count % maxDatesPerLine; // Number of remaining dates


                for (int i = 0; i < lineCount; i++)
                {
                    int startIndex = i * maxDatesPerLine;
                    int endIndex = Math.Min(startIndex + maxDatesPerLine, statistic.Dates.Count);
                    var datesSubset = statistic.Dates.GetRange(startIndex, endIndex - startIndex);
                    string datesLine = string.Join(", ", datesSubset);
                    if (i == 0)
                    {
                        doc.Content.Find.Execute("<data_list>", ReplaceWith: datesLine);
                    }
                    else
                    {
                    doc.Content.InsertAfter(datesLine);
                    }

                    if (i < lineCount - 1)
                    {
                        doc.Content.InsertAfter("\n");
                    }
                }


                doc.SaveAs2(ReportPath);
                doc.Close();

                wordApp.Quit();
                wordApp = null;
                return true;
            }
            catch (Exception ex)
            {
                error = ex;
                return false;
            }
        }

    }
}
