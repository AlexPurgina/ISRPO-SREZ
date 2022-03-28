using Library.Classes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Word = Microsoft.Office.Interop.Word;
using Excel = Microsoft.Office.Interop.Excel;
using Microsoft.Win32;

namespace Library.Views.Peges
{
    /// <summary>
    /// Логика взаимодействия для PageReader.xaml
    /// </summary>
    public partial class PageReader : Page
    {
        public PageReader()
        {
            InitializeComponent();
        }
        public Reader selectedClient { get; set; }
        List<RecordBook> booksByReaders = new List<RecordBook>();
        LibraryCard libraryCard;
        string token { get; set; }
        public PageReader(Reader selectedClient, string token, LibraryCard libraryCard)
        {
            InitializeComponent();
            this.selectedClient = selectedClient;
            this.DataContext = this;
            this.token = token;
            this.libraryCard = libraryCard;
            DgReaderBook.ItemsSource = libraryCard.Records;

        }
        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            using (HttpClient httpClient = new HttpClient { BaseAddress = new Uri(Properties.Settings.Default.BaseAddress) })
            {
                var content = new StringContent("", Encoding.UTF8, "application/json");
            }
            List<string> genres = new List<string>();
            genres.Add("Отобразить всё");
            genres.AddRange((DgReaderBook.ItemsSource as List<RecordBook>).Select(b => b.book.genre).Distinct().ToList());
            cbGenre.ItemsSource = genres;

            List<string> authors = new List<string>();
            authors.Add("Отобразить всё");
            authors.AddRange((DgReaderBook.ItemsSource as List<RecordBook>).Select(b => b.book.author).Distinct().ToList());
            cbAuthor.ItemsSource = authors;

            List<string> publishers = new List<string>();
            publishers.Add("Отобразить всё");
            publishers.AddRange((DgReaderBook.ItemsSource as List<RecordBook>).Select(b => b.book.publisherEx).Distinct().ToList());
            cbPublisher.ItemsSource = publishers;

            cbGenre.SelectedIndex = 0;
            cbAuthor.SelectedIndex = 0;
            cbPublisher.SelectedIndex = 0;

            cbGenre.SelectionChanged += cbGenre_SelectionChanged;
            cbAuthor.SelectionChanged += cbAuthor_SelectionChanged;
            cbPublisher.SelectionChanged += cbPublisher_SelectionChanged;
            dpStart.SelectedDateChanged += dpStart_SelectedDateChanged;
            dpEnd.SelectedDateChanged += dpStart_SelectedDateChanged;
            CountSt(cbGenre.Text, cbAuthor.Text, cbPublisher.Text,dpStart.SelectedDate != null, dpStart.SelectedDate != null);
        }
        public void CountSt(string sortGen = "Отобразить всё", string sortAut = "Отобразить всё", string sortPub = "Отобразить всё", bool sortDateSt = false, bool sortDateEnd = false)
        {
            var list = libraryCard.Records;
            if (sortGen != "Отобразить всё")
            {
                list = list.Where(x => x.book.genre == sortGen).ToList();
            }

            if (sortAut != "Отобразить всё")
            {
                list = list.Where(x => x.book.author == sortAut).ToList();
            }
            if (sortPub != "Отобразить всё")
            {
                list = list.Where(x => x.book.publisherEx == sortPub).ToList();
            }
            if (sortDateSt != false)
            {
                list = list.OrderBy(x => x.dateStart).ToList();
                dpEnd.SelectedDate = null;
            }
            if (sortDateEnd != false)
            {
                list = list.OrderBy(x => x.dateEnd).ToList();
                dpEnd.SelectedDate = null;
            }
            DgReaderBook.ItemsSource = list;
        }
        private void back_MouseDown(object sender, MouseButtonEventArgs e)
        {
            NavigationService.Navigate(new ReadersList(token));
        }

        private void cbGenre_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CountSt(cbGenre.SelectedItem.ToString(), cbAuthor.Text, cbPublisher.Text, dpStart.SelectedDate != null, dpEnd.SelectedDate != null);
        }

        private void cbPublisher_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CountSt(cbGenre.Text, cbAuthor.Text, cbPublisher.SelectedItem.ToString(), dpStart.SelectedDate != null, dpEnd.SelectedDate != null);
        }

        private void cbAuthor_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CountSt(cbGenre.Text, cbAuthor.SelectedItem.ToString(), cbPublisher.Text, dpStart.SelectedDate != null, dpEnd.SelectedDate != null);
        }
    

        public static void KillProcess(string processName)
        {
            foreach (var process in Process.GetProcesses().Where(p => p.ProcessName == processName))
            {
                process.Kill();
            }
        }
        public static void GenerateExcelReport(LibraryCard libraryCard)
        {
            try
            {
                SaveFileDialog savedialog = new SaveFileDialog();
                savedialog.Title = "Сохранить файл как...";
                savedialog.OverwritePrompt = true;
                savedialog.CheckPathExists = true;
                savedialog.Filter = $"Список книг (*.xlsx)|*.xlsx";
                if (savedialog.ShowDialog() == true)

                    KillProcess("EXCEL");
                Excel.Application app = new Excel.Application();
                Excel.Workbook workbook = app.Workbooks.Add(Type.Missing);
                Excel.Worksheet sheet = (Excel.Worksheet)app.Worksheets[1];

                // Шапка таблицы
                sheet.Cells[1, 1] = "№ п/п";
                sheet.Cells[1, 2] = "Срок возвращения";
                sheet.Cells[1, 3] = "Автор и название";
                sheet.Cells[1, 4] = "Издательство";
                sheet.Cells[1, 5] = "Отметка о сдаче";
                int row = 2;

                foreach (RecordBook record in libraryCard.Records)
                {
                    sheet.Cells[row, 1] = (row - 1).ToString();
                    sheet.Cells[row, 2] = record.retDay.ToString();
                    sheet.Cells[row, 3] = record.book.authorTitle;
                    sheet.Cells[row, 4] = record.book.publisherEx;
                    string mark = "";
                    if (record.retDay > 7)
                    {
                        mark = "сдана не вовремя";
                    }
                    else
                    {
                        mark = "сдана вовремя";
                    }
                    sheet.Cells[row, 5] = mark;
                    row++;
                }

                app.Application.ActiveWorkbook.SaveAs(savedialog.FileName);
                app.Application.Quit();
                MessageBox.Show("Отчет сформирован");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        async System.Threading.Tasks.Task Print(LibraryCard libraryСard, FileType fileType)
        {
            SaveFileDialog savedialog = new SaveFileDialog();
            savedialog.Title = "Сохранить файл как...";
            savedialog.OverwritePrompt = true;
            savedialog.CheckPathExists = true;
            savedialog.Filter = $"Список книг (*.{fileType.ToString()})|*.{fileType.ToString()}";
            if (savedialog.ShowDialog() == true)
            {
                await System.Threading.Tasks.Task.Run(() =>
                {
                    var word = new Word.Application();
                    var document = word.Documents.Open(Environment.CurrentDirectory + @"\список_книг_template.docx");
                    DateTime date = DateTime.Now;
                    try
                    {
                        var table1 = document.Tables[1];
                        int row = 1;
                        foreach (var item in libraryCard.Records)
                        {

                            row++;
                            table1.Rows.Add();
                            table1.Cell(row, 1).Range.Text = (row - 1).ToString();
                            table1.Cell(row, 2).Range.Text = item.retDay.ToString();
                            table1.Cell(row, 3).Range.Text = item.book.authorTitle;
                            table1.Cell(row, 4).Range.Text = item.book.publisherEx;
                            string mark = "";
                            if (item.retDay > 7)
                            {
                                mark = "сдана не вовремя";
                            }
                            else
                            {
                                mark = "сдана вовремя";
                            }
                            table1.Cell(row, 5).Range.Text = mark;
                        }

                        int T = table1.Rows.Count;
                        table1.Rows[T].Delete();
                        if (fileType == FileType.doc)
                        {
                            document.SaveAs2(savedialog.FileName, Word.WdSaveFormat.wdFormatDocument, Word.WdSaveOptions.wdDoNotSaveChanges);
                        }
                        else if (fileType == FileType.pdf)
                        {
                            document.SaveAs2(savedialog.FileName, Word.WdSaveFormat.wdFormatPDF, Word.WdSaveOptions.wdDoNotSaveChanges);
                        }
                        else
                        {
                            MessageBox.Show("Данный формат не поддерживается");
                        }
                        document.Close(Word.WdSaveOptions.wdDoNotSaveChanges);
                        word.Quit(Word.WdSaveOptions.wdDoNotSaveChanges);
                        MessageBox.Show("Отчёт успешно создан!");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        document.Close(Word.WdSaveOptions.wdDoNotSaveChanges);
                        word.Quit(Word.WdSaveOptions.wdDoNotSaveChanges);
                    }
                });


            }
        }

        private void word_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Print(libraryCard, FileType.doc);
        }

        private void pdf_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Print(libraryCard, FileType.pdf);
        }

        private void xl_MouseDown(object sender, MouseButtonEventArgs e)
        {
            GenerateExcelReport(libraryCard);
        }

        private void dpStart_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dpStart.SelectedDate > dpEnd.SelectedDate && dpEnd.SelectedDate > DateTime.Now && dpStart.SelectedDate > DateTime.Now)
            {
                MessageBox.Show("Не правильная дата");
            }
            CountSt(cbGenre.Text, cbAuthor.Text, cbPublisher.Text, dpStart.SelectedDate != null, dpEnd.SelectedDate != null);
        }

     
    }
    enum FileType
    {
        pdf,
        doc
    }

}
