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

        public ActionResult ShowSearchResults(string searchPhrase)
        {
            List<GuitarTab> guitarTabsList = new List<GuitarTab>();
            SqlConnection con = new SqlConnection("Data Source=DEEPTHOUGHT-2\\SQLEXPRESS;Initial Catalog=guitar_tabs_database;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");
            {
                con.Open();

                SqlCommand cmd = new SqlCommand("SELECT guitar_tabs_files.name FROM guitar_tabs_files " +
                    "JOIN tab_tags ON guitar_tabs_files.stream_id = tab_tags.Tab_ID " +
                    "JOIN tags ON tags.Tag_ID = tab_tags.Tag_ID WHERE tags.Tag =  @nameParam", con);
                SqlParameter nameParam = new SqlParameter("@nameParam", SqlDbType.VarChar);
                nameParam.Value = searchPhrase;
                cmd.Parameters.Add(nameParam);

                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    var guitarTab = new GuitarTab();

                    guitarTab.Title = rdr["name"].ToString();
                    guitarTabsList.Add(guitarTab);
                }
            }
            return View(guitarTabsList);
        }
    }
}
