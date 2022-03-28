using System;
using System.Collections.Generic;
using System.Linq;
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
using Library.Views.Peges;
using Library.Classes;
using System.Net.Http;
using Newtonsoft.Json;
using Word = Microsoft.Office.Interop.Word;
using System.IO;
using System.Web.Script.Serialization;
using System.Net;
using Microsoft.Win32;

namespace Library.Views.Peges
{
    /// <summary>
    /// Логика взаимодействия для ReadersList.xaml
    /// </summary>
    public partial class ReadersList : Page
    {
        public ReadersList()
        {
            InitializeComponent();
        }

        string token { get; set; }
        List<Reader> readers = null;
        public ReadersList(string token)
        {
            this.token = token;
            InitializeComponent();
        }
        public void Data()
        {
            try
            {
                using (HttpClient httpClient = new HttpClient { BaseAddress = new Uri(Properties.Settings.Default.BaseAddress) })
                {
                    var reader = httpClient.GetStringAsync($"/GetReaders?token={token}");

                    readers = System.Text.Json.JsonSerializer.Deserialize<List<Reader>>(reader.Result);


                    LvReader.ItemsSource = readers;

                }
            }
            catch (Exception)
            {

                MessageBox.Show("error");
                return;
            }

        }
        private void tbSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (tbSearch.Text != "" && readers != null)
            {
                List<Reader> reader1 = readers.Where(x => x.FIO.ToLower().Contains(tbSearch.Text.ToLower())).ToList();

                if (reader1.Count() == 0)
                {
                    MessageBox.Show("Результаты не найдены");
                    return;
                }
                LvReader.ItemsSource = reader1;
            }
            else if (string.IsNullOrWhiteSpace(tbSearch.Text))
            {
                LvReader.ItemsSource = readers;
            }
        }
        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            App.Current.Shutdown();

        }

        private void Image_MouseLeftButtonDown_1(object sender, MouseButtonEventArgs e)
        {
            try
            {
                string file = "";
                file += $"ФИО\n";
                byte[] photos = new byte[readers.Count()];

                foreach (var reader in this.readers)
                {

                    photos = reader.photo;
                    File.WriteAllBytes($@"C:\Users\Пользователь\Desktop\Library\bin\Debug\readerphoto\{reader.lastName} {reader.firstName}.jpg", photos);
                    file += $"{reader.fullName}" + ";" + $@"C:\Users\Пользователь\Desktop\Library\bin\Debug\readerphoto\{reader.lastName} {reader.firstName}.jpg" + "\n";

                }

                File.WriteAllText($@"{Directory.GetCurrentDirectory()}\readersphoto.csv", file, Encoding.Default);


                MessageBox.Show($@"Файл сохранён по пути {Directory.GetCurrentDirectory()}\readers.csv");
            }
            catch (Exception ex)
            {
                MessageBox.Show("error");
                return;
            }



        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Data();

        }

        private void tbSearch_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            ((TextBox)sender).Text = "";
        }

        private void word_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Reader reader = LvReader.SelectedItem as Reader;
            Dictionary<string, string> data = new Dictionary<string, string>();
            var photo = reader.photo;
            reader.photo = null;
            data.Add("reader", JsonConvert.SerializeObject(reader, Newtonsoft.Json.Formatting.Indented));
            data.Add("token", token);


            string requestData = GetBooksPost("GetBooksByReader?token=" + token, reader);
            LibraryCard libraryСards = JsonConvert.DeserializeObject<LibraryCard>(requestData);

            reader.photo = photo;
            Print(reader, libraryСards);
        }

        public static string GetBooksPost(string method, Reader reader)
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(App.Address + method);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {

                string json = new JavaScriptSerializer().Serialize(reader);
                streamWriter.Write(json);
                streamWriter.Flush();
                streamWriter.Close();
            }

            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();
                return result;
            }
        }
        async System.Threading.Tasks.Task Print(Reader data, LibraryCard libraryСard)
        {
            SaveFileDialog savedialog = new SaveFileDialog();
            savedialog.Title = "Сохранить файл как...";
            savedialog.OverwritePrompt = true;
            savedialog.CheckPathExists = true;
            savedialog.Filter = "Word документ (*.doc)|*.doc";
            if (savedialog.ShowDialog() == true)
            {
                await System.Threading.Tasks.Task.Run(() =>
                {
                    var word = new Word.Application();
                    var document = word.Documents.Open(Environment.CurrentDirectory + @"\читательский_билет_template.docx");
                    DateTime date = DateTime.Now;
                    try
                    {
                        string imagePath = $@"{Directory.GetCurrentDirectory()}\image.jpg";
                        File.WriteAllBytes(imagePath, data.photo);

                        string bookData = String.Empty;
                        foreach (RecordBook record in libraryСard.Records)
                        {
                            bookData += $"{record.dateStart.ToString("dd.MM.yyyy hh:mm")} {record.dateEnd.ToString("dd.MM.yyyy hh:mm")} {record.book.title} {record.book.author} {record.book.publisher}\n";
                        }

                        document.Bookmarks["Фото"].Range.InlineShapes.AddPicture(imagePath, Type.Missing, true);
                        document.Bookmarks["Номер"].Range.Text = $"{data.lastName.Length}{data.middleName.Length}{data.firstName.Length}";
                        document.Bookmarks["Фамилия"].Range.Text = data.lastName;
                        document.Bookmarks["Имя"].Range.Text = data.firstName;
                        document.Bookmarks["Отчество"].Range.Text = data.middleName;
                        document.Bookmarks["Дата"].Range.Text = DateTime.Now.ToString("dd.MM.yyyy");
                        document.Bookmarks["Книги"].Range.Text = bookData;

                        document.SaveAs2(savedialog.FileName, Word.WdSaveFormat.wdFormatDocument, Word.WdSaveOptions.wdDoNotSaveChanges);
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

        private void ContentControl_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Reader reader = LvReader.SelectedItem as Reader;
            Dictionary<string, string> data = new Dictionary<string, string>();
            var photo = reader.photo;
            reader.photo = null;
            data.Add("reader", JsonConvert.SerializeObject(reader, Newtonsoft.Json.Formatting.Indented));
            data.Add("token", token);


            string requestData = GetBooksPost("GetBooksByReader?token=" + token, reader);
            LibraryCard libraryСards = JsonConvert.DeserializeObject<LibraryCard>(requestData);

            reader.photo = photo;
            NavigationService.Navigate(new PageReader(reader, token, libraryСards));

        }
    }
}
