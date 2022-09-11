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
        public string DownloadLink()
        {
            SqlConnection con = new SqlConnection("Data Source=DEEPTHOUGHT-2\\SQLEXPRESS;Initial Catalog=guitar_tabs_database;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");
            con.Open();

            SqlCommand cmd = new SqlCommand("SELECT file_stream.GetFileNamespacePath(1, 2) FROM guitar_tabs_files WHERE stream_id = @idParam", con);
            SqlParameter idParam = new SqlParameter("@idParam", SqlDbType.UniqueIdentifier);
            idParam.Value = new Guid("8E91CDB4-602E-ED11-8D84-04ED33527636");
            cmd.Parameters.Add(idParam);

            string? filePath = null;
            Object pathObject = cmd.ExecuteScalar();
            if (DBNull.Value != pathObject)
            {
                filePath = (string)pathObject;
            }
            else
            {
                throw new System.Exception("problem");
            }

            return filePath;
        }

        [Authorize]
        [HttpGet("download/{streamId}")]
        //[Route("downloadfile/{streamId}")]
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

        [Authorize]
        public void Download(/*string guitarTabId*/)
        {
            SqlConnection con = new SqlConnection("Data Source=DEEPTHOUGHT-2\\SQLEXPRESS;Initial Catalog=guitar_tabs_database;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");
            con.Open();

            SqlCommand cmd = new SqlCommand("SELECT file_stream.GetFileNamespacePath(1, 2) FROM guitar_tabs_files WHERE stream_id = @idParam", con);
            SqlParameter idParam = new SqlParameter("@idParam", SqlDbType.UniqueIdentifier);
            idParam.Value = new Guid("8E91CDB4-602E-ED11-8D84-04ED33527636");
            cmd.Parameters.Add(idParam);

            string? filePath = null;
            string? fileName = null;
            Object pathObject = cmd.ExecuteScalar();
            if (DBNull.Value != pathObject)
            {
                filePath = (string)pathObject;
            }
            else
            {
                throw new System.Exception("problem");
            }

            SqlCommand cmd2 = new SqlCommand("SELECT name FROM guitar_tabs_files WHERE stream_id = @idParam2", con);
            SqlParameter idParam2 = new SqlParameter("@idParam2", SqlDbType.UniqueIdentifier);
            idParam2.Value = new Guid("8E91CDB4-602E-ED11-8D84-04ED33527636");
            cmd2.Parameters.Add(idParam2);

            SqlDataReader sqlDataReader = cmd2.ExecuteReader();
            while (sqlDataReader.Read()) fileName = sqlDataReader["name"].ToString();

            //HttpClient client = new HttpClient();
            //try
            //{
            //    HttpResponseMessage response = await client.GetAsync(filePath);
            //    response.EnsureSuccessStatusCode();
            //    string responseBody = await response.Content.ReadAsStringAsync();
            //    Console.WriteLine(responseBody);
            //}
            //catch (HttpRequestException e)
            //{
            //    Console.WriteLine("HttpRequestException caught");
            //    Console.WriteLine(e.Message);
            //}

            using (var client = new System.Net.WebClient())
            {
                client.DownloadFile(filePath, fileName);
                Console.WriteLine("file downloading");
            }
        }

        public ActionResult ShowSearchResults(string searchPhrase)
        {
            if (string.IsNullOrEmpty(searchPhrase))
            {
                return View();
            }
            List<GuitarTab> guitarTabsList = new List<GuitarTab>();
            SqlConnection con = new SqlConnection("Data Source=DEEPTHOUGHT-2\\SQLEXPRESS;Initial Catalog=guitar_tabs_database;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");
            
            // Retrieves the guitar tabs from the database based on the tag searched
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
