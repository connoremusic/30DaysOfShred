using System.Data;
using Microsoft.Data.SqlClient;

namespace _30DaysOfShred.Models
{
    public class GuitarTab
    {
        public Guid uniqueId { get; set; }
        public string Title { get; set; }
        [System.ComponentModel.DisplayName("Tags")]
        public List<string> TabCategories { get; set; }

        public GuitarTab(Guid uniqueId, string title, List<string> tabCategories)
        {
            this.uniqueId = uniqueId;
            Title = title;
            TabCategories = tabCategories;
        }

        public GuitarTab()
        {
            this.uniqueId = Guid.Empty;
            this.Title = "";
            this.TabCategories = new List<string>();
        }

        //public static string GetDownloadLink()
        //{
        //    SqlConnection con = new SqlConnection("Data Source=DEEPTHOUGHT-2\\SQLEXPRESS;Initial Catalog=guitar_tabs_database;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");
        //    con.Open();

        //    SqlCommand cmd = new SqlCommand("SELECT file_stream.GetFileNamespacePath(1, 2) FROM guitar_tabs_files WHERE stream_id = @idParam", con);
        //    SqlParameter idParam = new SqlParameter("@idParam", SqlDbType.UniqueIdentifier);
        //    idParam.Value = new Guid("8E91CDB4-602E-ED11-8D84-04ED33527636");
        //    cmd.Parameters.Add(idParam);

        //    string? filePath = null;
        //    Object pathObject = cmd.ExecuteScalar();
        //    if (DBNull.Value != pathObject)
        //    {
        //        filePath = (string)pathObject;
        //    }
        //    else
        //    {
        //        throw new System.Exception("problem");
        //    }

        //    return filePath;
        //}

        public static byte[] GetGuitarTabData(string stream_id)
        {
            SqlConnection con = new SqlConnection("Data Source=DEEPTHOUGHT-2\\SQLEXPRESS;Initial Catalog=guitar_tabs_database;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");
            con.Open();
            byte[] GuitarTabData;

            SqlCommand cmd = new SqlCommand("SELECT file_stream FROM guitar_tabs_files WHERE stream_id = @idParam", con);
            SqlParameter idParam = new SqlParameter("@idParam", SqlDbType.UniqueIdentifier);
            idParam.Value = new Guid(stream_id);
            cmd.Parameters.Add(idParam);
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                reader.Read();
                GuitarTabData = (byte[])reader["file_stream"];
            }
            con.Close();
            return GuitarTabData;
        }

        public static string GetGuitarTabFileName(string stream_id)
        {
            SqlConnection con = new SqlConnection("Data Source=DEEPTHOUGHT-2\\SQLEXPRESS;Initial Catalog=guitar_tabs_database;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");
            con.Open();
            string GuitarTabFileName = "download";

            SqlCommand cmd = new SqlCommand("SELECT name FROM guitar_tabs_files WHERE stream_id = @idParam", con);
            SqlParameter idParam = new SqlParameter("@idParam", SqlDbType.UniqueIdentifier);
            idParam.Value = new Guid(stream_id);
            cmd.Parameters.Add(idParam);

            SqlDataReader sqlDataReader = cmd.ExecuteReader();
            while (sqlDataReader.Read()) GuitarTabFileName = sqlDataReader["name"].ToString();

            con.Close();
            if (GuitarTabFileName == null) return "download";
            return GuitarTabFileName;
        }
    }
}
