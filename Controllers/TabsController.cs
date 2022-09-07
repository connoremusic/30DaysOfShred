using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Configuration;
using Microsoft.Data.SqlClient;
using System.Data;
using _30DaysOfShred.Models;

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

        public ActionResult ShowSearchResults()
        {
            List<GuitarTab> guitarTabsList = new List<GuitarTab>();
            string CS = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            using (SqlConnection con = new SqlConnection(CS))
            {
                SqlCommand cmd = new SqlCommand("SELECT name FROM guitar_tabs_files", con);
                cmd.CommandType = CommandType.Text;
                con.Open();

                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    var guitarTab = new GuitarTab();

                    //guitarTab.Id = Convert.ToInt32(rdr["stream_id"]);
                    guitarTab.Title = rdr["Name"].ToString();
                    guitarTabsList.Add(guitarTab);
                }
            }
            return View(guitarTabsList);
        }

        private static void GetGuitarTabByName(string connectionString)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(null, connection);

                // Create and prepare an SQL statement.
                command.CommandText = "SELECT name FROM guitar_tabs WHERE name = @name";

                SqlParameter nameParam = new SqlParameter("@name", SqlDbType.VarChar);
                //SqlParameter descParam = new SqlParameter("@desc", SqlDbType.Text, 100);
                //idParam.Value = 20;
                //descParam.Value = "First Region";
                command.Parameters.Add(nameParam);
                //command.Parameters.Add(descParam);

                // Call Prepare after setting the Commandtext and Parameters.
                command.Prepare();
                command.ExecuteNonQuery();

                // Change parameter values and call ExecuteNonQuery.
                //command.Parameters[0].Value = 21;
                //command.Parameters[1].Value = "Second Region";
                //command.ExecuteNonQuery();
            }
        }

        // GET: Jokes/ShowSearchResults
        public async Task<IActionResult> ShowSearchResults(string SearchPhrase)
        {
            return View("SearchResults", await _context.GuitarTab.Where(j => j.Title.Contains(SearchPhrase)).ToListAsync());
        }
    }
}
