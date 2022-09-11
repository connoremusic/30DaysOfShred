using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Net.Http.Headers;
using System.Configuration;
using Microsoft.Data.SqlClient;
using System.Data;
using _30DaysOfShred.Models;
using Microsoft.Data.SqlTypes;
using System.Text;
using Microsoft.AspNetCore.StaticFiles;

namespace _30DaysOfShred.Controllers
{
    public class TabsController : Controller
    {
        [Authorize]
        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        public IActionResult Search()
        {
            return View();
        }

        [Authorize]
        [HttpGet("download/{streamId}")]
        public IActionResult DownloadFile(string streamId)
        {
            Console.WriteLine(streamId);
            var data = GuitarTab.GetGuitarTabData(streamId);
            var filename = GuitarTab.GetGuitarTabFileName(streamId);
            var result = new HttpResponseMessage(System.Net.HttpStatusCode.OK);
            var stream = new MemoryStream(data);
            result.Content = new StreamContent(stream);
            result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
            result.Content.Headers.ContentDisposition.FileName = filename;
            result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            return File(data, "application/octet-stream", filename);
        }

        public ActionResult ShowSearchResults(string searchPhrase)
        {
            if (string.IsNullOrEmpty(searchPhrase))
            {
                return View();
            }
            List<GuitarTab> guitarTabsList = new List<GuitarTab>();
            SqlConnection con = new SqlConnection("Data Source=DEEPTHOUGHT-2\\SQLEXPRESS;Initial Catalog=guitar_tabs_database;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");
            
            // Retrieves the guitar tab metadata from the database based on the tag searched
            {
                con.Open();

                SqlCommand cmd = new SqlCommand("SELECT guitar_tabs_files.stream_id, guitar_tabs_files.name FROM guitar_tabs_files " +
                    "JOIN tab_tags ON guitar_tabs_files.stream_id = tab_tags.Tab_ID " +
                    "JOIN tags ON tags.Tag_ID = tab_tags.Tag_ID WHERE tags.Tag =  @nameParam", con);
                SqlParameter nameParam = new SqlParameter("@nameParam", SqlDbType.VarChar);
                nameParam.Value = searchPhrase;
                
                cmd.Parameters.Add(nameParam);

                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    var guitarTab = new GuitarTab();

                    guitarTab.uniqueId = (Guid)rdr["stream_id"];
                    guitarTab.Title = rdr["name"].ToString();
                    guitarTabsList.Add(guitarTab);

                }
                rdr.Close();
            }

            //Populates the found guitar tabs with the correct tags
            foreach (var guitarTab in guitarTabsList)
            {
                SqlCommand cmd = new SqlCommand("SELECT tags.Tag FROM tags JOIN tab_tags ON tags.Tag_ID = tab_tags.Tag_ID " +
                    "JOIN guitar_tabs_files ON guitar_tabs_files.stream_id = tab_tags.Tab_ID WHERE guitar_tabs_files.stream_id = @idParam", con);
                SqlParameter idParam = new SqlParameter("@idParam", SqlDbType.UniqueIdentifier);
                idParam.Value = guitarTab.uniqueId;
                cmd.Parameters.Add(idParam);

                SqlDataReader rdr2 = cmd.ExecuteReader();
                while(rdr2.Read())
                {
                    guitarTab.TabCategories.Add(rdr2["Tag"].ToString());
                }
                rdr2.Close();
            }
            con.Close();
            return View(guitarTabsList);
        }
    }
}
