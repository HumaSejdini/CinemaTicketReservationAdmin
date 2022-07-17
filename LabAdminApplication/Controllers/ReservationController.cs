using ClosedXML.Excel;
using GemBox.Document;
using LabAdminApplication.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;

namespace LabAdminApplication.Controllers
{
    public class ReservationController : Controller
    {

        public ReservationController()
        {
            ComponentInfo.SetLicense("FREE-LIMITED-KEY");
        }
        public IActionResult Index()
        {
            HttpClient client = new HttpClient();

            string URL = "https://localhost:44307/api/admin/GetAllActiveReservations";

            HttpResponseMessage response = client.GetAsync(URL).Result;

            var data = response.Content.ReadAsAsync<List<Reservation>>().Result;

            return View(data);
        }

        public IActionResult GetMovieDetails(int id)
        {
            HttpClient client = new HttpClient();

            string URL = "https://localhost:44307/api/admin/GetReservationDetails";

            var model = new
            {
                Id = id
            };

            HttpContent content = new StringContent(JsonConvert.SerializeObject(model),Encoding.UTF8,"application/json");

            HttpResponseMessage response = client.PostAsync(URL,content).Result;

            var data = response.Content.ReadAsAsync<Reservation>().Result;

            return View(data);
        }
        public FileResult SavePdf(int id)
        {
            HttpClient client = new HttpClient();

            string URL = "https://localhost:44307/api/admin/GetReservationDetails";

            var model = new
            {
                Id = id
            };

            HttpContent content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
            HttpResponseMessage response = client.PostAsync(URL, content).Result;
            
            var result =response.Content.ReadAsAsync<Reservation>().Result;

            var templatePath = Path.Combine(Directory.GetCurrentDirectory(), "Invoice.docx");

            var document = DocumentModel.Load(templatePath);

            document.Content.Replace("{{Reservation.number}}",result.Id.ToString());
            document.Content.Replace("{{UserName}}", result.ReservedBy.Username);

            StringBuilder sb = new StringBuilder();
            double totalPrice = 0.0;
            foreach (var item in result.Movies)
            {
                totalPrice += item.Quantity * item.Movie.MoviePrice;
                sb.AppendLine(item.Movie.MovieTitle + ", quantity: " + item.Quantity+ ",price: $" + item.Movie.MoviePrice); 
            }
            document.Content.Replace("{{MovieList}}", sb.ToString());
            document.Content.Replace("{{TotalPrice}}", "$"+totalPrice.ToString());
            
            var stream = new MemoryStream();

            document.Save(stream, new PdfSaveOptions());

            return File(stream.ToArray(),new PdfSaveOptions().ContentType,"ExportInvoice.pdf");
        }
        [HttpGet]
        public FileContentResult ExportAllReservations()
        {
            string fileName = "Reservations.xlsx";
            string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

            using(var workbook= new XLWorkbook())
            {
                IXLWorksheet worksheet = workbook.Worksheets.Add("All Reservations");

                worksheet.Cell(1, 1).Value = "Reservation Id";
                worksheet.Cell(1, 2).Value = "Costumer Email";


                HttpClient client = new HttpClient();

                string URL = "https://localhost:44307/api/admin/GetAllActiveReservations";

                HttpResponseMessage response = client.GetAsync(URL).Result;

                var data = response.Content.ReadAsAsync<List<Reservation>>().Result;

                for (int i = 1; i <= data.Count(); i++)
                {
                    var item = data[i-1];
                    worksheet.Cell(i+1, 1).Value = item.Id.ToString();
                    worksheet.Cell(i+1, 2).Value = item.ReservedBy.Email;

                    for (int p = 0; p < item.Movies.Count(); p++)
                    {
                        worksheet.Cell(1, p + 3).Value = "Movie-" + (p + 1);
                        worksheet.Cell(i+1, p + 3).Value = item.Movies.ElementAt(p).Movie.MovieTitle;
                    }

                }
                using(var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();

                    return File(content, contentType, fileName);
                }

            }
         
        }
    }
}
