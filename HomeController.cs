using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Data.SqlClient;
using bankingprocess.Models;
using System.Data;
using System.Data.SqlClient;

namespace bankingprocess.Controllers
{
    public class HomeController : Controller
    {


        public string connectionstring = "Data source=Web-Server\\SQLEXPRESS;initial catalog=Freshers; uid=fresher;pwd=fresher;";
        SqlConnection con = null;
        SqlCommand cmd = null;
        SqlDataReader dr;


        public ActionResult Index()
        {
            ViewBag.Message = "Modify this template to kick-start your ASP.NET MVC application.";

            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your quintessential app description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your quintessential contact page.";

            return View();
        }
        public ActionResult login()
        {
            return View();
        }
        public ActionResult admin()
        {
            return View();

        }
        public ActionResult register()
        {
            return View();
        }
        public ActionResult transaction()
        {
            return View();
        }

        public string save(string name, string password, string address, Int64 phoneno, string city, Int64 amount)
        {
            user s = new user();

            s.username = name;
            s.password = password;
            s.address = address;
            s.phoneno = phoneno;
            s.city = city;
            s.amount = amount;
            //Service1 se = new Service1();

            return save(s);


        }


        public string save(user s)
        {

            using (con = new SqlConnection(connectionstring))
            {
                con.Open();
                con.CreateCommand();

                using (cmd = new SqlCommand("sp_savebanking", con))
                {

                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@username", s.username));
                    cmd.Parameters.Add(new SqlParameter("@password", s.password));
                    cmd.Parameters.Add(new SqlParameter("@address", s.address));
                    cmd.Parameters.Add(new SqlParameter("@phoneno", s.phoneno));
                    cmd.Parameters.Add(new SqlParameter("@city", s.city));
                    cmd.Parameters.Add(new SqlParameter("@amount", s.amount));
                    cmd.ExecuteNonQuery();
                }
                con.Close();
            }
            return "";
        }


        public ActionResult bind()
        {
            jQueryDataTableParamModel dm = new jQueryDataTableParamModel();
            //Service1 ss = new Service1();

            var list = main();
            var result = from a in list
                         select new[] { a.username.ToString(), a.password.ToString(), a.address.ToString(), a.amount.ToString(), a.city.ToString(), a.amount.ToString() };

            return Json(new { sEcho = dm.sEcho, iTotalLength = result, iTotalDisplayRecords = result, aaData = result }, JsonRequestBehavior.AllowGet);

        }

        public List<user> main()
        {
            List<user> abc = new List<user>();
            using (con = new SqlConnection(connectionstring))
            {
                con.Open();
                con.CreateCommand();
                using (cmd = new SqlCommand("sp_bindbanking", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        user c = new user();
                        c.username = Convert.ToString(dr["username"]);
                        c.password = Convert.ToString(dr["password"]);
                        c.address = Convert.ToString(dr["address"]);
                        c.phoneno = Convert.ToInt32(dr["phoneno"]);
                        c.city = Convert.ToString(dr["city"]);
                        c.amount = Convert.ToInt32(dr["amount"]);
                        abc.Add(c);
                    }
                    dr.Close();
                }
                return abc;
            }

        }

        [HttpPost]

        public ActionResult login(string username, string password)
        {
            List<user> list = new List<user>();

            if (username == "admin" && password == "admin")
            {
                return Json(new { success = 1 });

            }
            else
            {
                using (con = new SqlConnection(connectionstring))
                {
                    con.Open();
                    con.CreateCommand();
                    using (cmd = new SqlCommand("sp_loginbanking", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@username", username));
                        cmd.Parameters.Add(new SqlParameter("@password", password));
                        dr = cmd.ExecuteReader();

                        while (dr.Read())
                        {
                            user s = new user();
                            s.username = Convert.ToString(dr["username"]);
                            s.password = Convert.ToString(dr["password"]);

                            list.Add(s);

                        }
                    }

                }

                Session["username"] = username;
                foreach (var s in list)
                {
                    if
                           (s.username == "admin" && s.password == "admin")
                    {
                        return Json(new { success = 1 });

                    }

                    else if (s.username == username && s.password == password)
                    {
                        return Json(new { success = 2 });

                    }

                }
                return Json(list);
            }
        }

        public string save2(string username, Int32 uamount, Int32 utransactionamount, Int32 ubalanceamount, string uname)
        {
            user u = new user();
            u.username = username;
            u.amount = uamount;
            u.transcationamount = utransactionamount;
            u.balance = ubalanceamount;
            u.name = uname;

            Service1 si = new Service1();
            return si.save2(u);
        }


        public ActionResult binds(string Name)
        {
            var bin = bindfn(Name);
            return this.Json(bin);

        }
        public List<user> bindfn(string username)
        {
            List<user> acclist = new List<user>();
            user bind = null;
            SqlConnection conn = null;
            SqlCommand cmd = null;

            IDataReader reader;
            using (conn = new SqlConnection(connectionstring))
            {
                conn.Open();
                cmd = conn.CreateCommand();
                using (cmd = new SqlCommand("sp_bindbankinggg", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;


                    cmd.Parameters.Add(new SqlParameter("username", username));
                    reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        bind = new user();
                        bind.username = Convert.ToString(reader["username"]);
                        bind.amount = Convert.ToInt64(reader["Totalamount"]);
                        acclist.Add(bind);

                    }
                }
                return acclist;
            }
        }

        public ActionResult autocomp(string searchtext)
        {
            var autoboxpopulate = from c in autocomplet(searchtext) select new { label = c.username };
            return this.Json(autoboxpopulate, JsonRequestBehavior.AllowGet);
        }

        public List<user> autocomplet(string searchtext)
        {
            user obj = null;
            List<user> list = new List<user>();
            SqlConnection con = new SqlConnection("Data source=Web-Server\\SQLEXPRESS;initial catalog=Freshers; uid=fresher;pwd=fresher;Integrated Security=False");
            con.Open();
            SqlCommand cmd = new SqlCommand("sp_autobank", con);
            cmd.CommandType = CommandType.StoredProcedure;
            SqlDataReader dr = null;
            cmd.Parameters.Add(new SqlParameter("@username", searchtext));
            dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                obj = new user();
                obj.username = Convert.ToString(dr["username"]);
                list.Add(obj);
            }
            dr.Close();
            return list;
        }
    }
}














