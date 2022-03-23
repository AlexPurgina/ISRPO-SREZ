using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Text.Json;
// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SaleAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SaleController : ControllerBase
    {
        string[] clients;
        string[] telephones;
        List<Client> Clients = new List<Client>();
        List<Telephone> Telephones = new List<Telephone>();
        public class Client
        {
            public string LastName { get; set; }
            public string FirstName { get; set; }
            public string Patronymic { get; set; }
        }
        public class Telephone
        {
            public int Articul { get; set; }
            public string NameTelephone { get; set; }
            public string Category { get; set; }
            public string Cost { get; set; }
            public int Count { get; set; }
            public string Manufacturer { get; set; }
        }
        public class Sale
        {
            public DateTime DateSale { get; set; }
            public Client Client { get; set; }
            public List<Telephone> Telephones { get; set; }
        }
        public SaleController()
        {
            clients = System.IO.File.ReadAllLines("fio.txt");
            foreach (var item in clients)
            {
                var fio = item.Split(' ');
                Clients.Add(new Client { LastName = fio[0], FirstName = fio[1], Patronymic = fio[2] });
            }
            telephones = System.IO.File.ReadAllLines("Telephones.csv");
            foreach (var item in telephones)
            {
                var splitphones = item.Split(';');
                Telephones.Add(new Telephone { Articul = Convert.ToInt32(splitphones[0]), NameTelephone = splitphones[1], Category = splitphones[2], Cost = Convert.ToDecimal(splitphones[3].Replace(",", ".").Replace(" ", "")), Count = Convert.ToInt32(splitphones[4]), Manufacturer = splitphones[5] });
            }

        }
       
        // POST api/<SaleController>
        [HttpGet ("Get")]
        public List<Sale> Get(DateTime dateStart, DateTime dateEnd)
        {
            Random random = new Random();
            List<Sale> sales = new List<Sale>();
            int date = (dateEnd - dateStart).Days;

            for (int i = 0; i <=date; i++)
            {

                int countsale = random.Next(1, 25);
                for (int j = 0; j < countsale; j++)
                {
                    Sale sale = new Sale();
                    sale.DateSale = dateStart.AddDays(i);
                    sale.Client = Clients.ElementAt(random.Next(Clients.Count()));
                    sale.Telephones = new List<Telephone>();
                    int k = random.Next(1, 10);
                    for (int l = 0; l < k; l++)
                    {
                        sale.Telephones.Add(Telephones.ElementAt(random.Next(Telephones.Count())));
                    }
                    sales.Add(sale);
                }
               
            }
            return sales;
        }

    }
}
