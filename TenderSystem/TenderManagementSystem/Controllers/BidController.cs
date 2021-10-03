using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TenderManagementSystem.Models;
using System.Data;
using System.Configuration;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Net.Mail;

namespace TenderManagementSystem.Controllers
{
    public class BidController : Controller
    {

        // View all bid data
        public ActionResult ViewBids()
        {

            var bidList = new List<Bid>();
            string userrole = "";
            string constr = ConfigurationManager.ConnectionStrings["WebProjectDb"].ConnectionString;

            if (Session["Email"] != null)
            {
                using (SqlConnection con2 = new SqlConnection(constr))
                {
                    con2.Open();

                    String sql2 = "SELECT UserRole FROM SystemUser WHERE Email = @Email";
                    SqlCommand cmd2 = new SqlCommand(sql2, con2);
                    cmd2.Parameters.AddWithValue("@Email", Session["Email"]);

                    SqlDataReader sdr2 = cmd2.ExecuteReader();
                    while (sdr2.Read())
                    {
                        userrole = sdr2["UserRole"].ToString().Trim();
                    }

                    con2.Close();
                }
            }


            if (userrole.Contains("Officer") || userrole.Contains("Director"))
            {
                using (SqlConnection con = new SqlConnection(constr))
                {
                    con.Open();

                    String sql = "SELECT Bid.Id, Bid.BidID, Bid.DateSubmitted, Bid.Price, Bid.TenderID, " +
                        "Bid.UserID, Bid.Description, Bid.Status FROM Bid";
                    SqlCommand cmd = new SqlCommand(sql, con);

                    SqlDataReader sdr = cmd.ExecuteReader();
                    while (sdr.Read())
                    {
                        var bid = new Bid();
                        bid.Id = Convert.ToInt32(sdr["Id"].ToString().Trim());
                        bid.BidID = sdr["BidID"].ToString().Trim();
                        bid.DateSubmitted = sdr["DateSubmitted"].ToString().Trim();
                        bid.TenderID = sdr["TenderID"].ToString().Trim();
                        bid.Price = sdr["Price"].ToString().Trim();
                        bid.UserID = sdr["UserID"].ToString().Trim();
                        bid.Status = sdr["Status"].ToString().Trim();
                        bid.Description = sdr["Description"].ToString().Trim();
                        bidList.Add(bid);
                    }

                    con.Close();
                }
                if (Session["Email"] != null)
                {
                    return View(bidList);
                }
                else
                {
                    return RedirectToAction("Login", "Home");
                }
            }
            else
            {
                using (SqlConnection con = new SqlConnection(constr))
                {
                    con.Open();

                    String sql = "SELECT Bid.Id, Bid.BidID, Bid.DateSubmitted, Bid.Price, Bid.TenderID, Bid.UserID," +
                        "Bid.Status, Bid.Description FROM Bid" +
                        " WHERE UserID IN (SELECT UserID FROM SystemUser WHERE Email = @Email)";
                    SqlCommand cmd = new SqlCommand(sql, con);
                    cmd.Parameters.AddWithValue("@Email", Session["Email"]);

                    SqlDataReader sdr = cmd.ExecuteReader();
                    while (sdr.Read())
                    {
                        var bid = new Bid();
                        bid.Id = Convert.ToInt32(sdr["Id"].ToString());
                        bid.BidID = sdr["BidID"].ToString();
                        bid.DateSubmitted = sdr["DateSubmitted"].ToString();
                        bid.TenderID = sdr["TenderID"].ToString();
                        bid.Price = sdr["Price"].ToString();
                        bid.Description = sdr["Description"].ToString();
                        bid.UserID = sdr["UserID"].ToString();
                        bid.Status = sdr["Status"].ToString();
                        bidList.Add(bid);
                    }

                    con.Close();
                }
                if (Session["Email"] != null)
                {
                    return View(bidList);
                }
                else
                {
                    return RedirectToAction("Login", "Home");
                }
            }


        }


        // View own bid data
        public ActionResult ViewMyBids()
        {

            var bidList = new List<Bid>();
            string userrole = "";
            string constr = ConfigurationManager.ConnectionStrings["WebProjectDb"].ConnectionString;

            if (Session["Email"] != null)
            {
                using (SqlConnection con2 = new SqlConnection(constr))
                {
                    con2.Open();

                    String sql2 = "SELECT UserRole FROM SystemUser WHERE Email = @Email";
                    SqlCommand cmd2 = new SqlCommand(sql2, con2);
                    cmd2.Parameters.AddWithValue("@Email", Session["Email"]);

                    SqlDataReader sdr2 = cmd2.ExecuteReader();
                    while (sdr2.Read())
                    {
                        userrole = sdr2["UserRole"].ToString().Trim();
                    }

                    con2.Close();
                }
            }


            if (userrole.Contains("Officer") || userrole.Contains("Director"))
            {
                using (SqlConnection con = new SqlConnection(constr))
                {
                    con.Open();

                    String sql = "SELECT Bid.Id, Bid.BidID, Bid.DateSubmitted, Bid.Price, Bid.TenderID, " +
                        "Bid.UserID, Bid.Description, Bid.Status FROM Bid ORDER BY cast(Bid.Price as decimal) DESC";
                    SqlCommand cmd = new SqlCommand(sql, con);

                    SqlDataReader sdr = cmd.ExecuteReader();
                    while (sdr.Read())
                    {
                        var bid = new Bid();
                        bid.Id = Convert.ToInt32(sdr["Id"].ToString().Trim());
                        bid.BidID = sdr["BidID"].ToString().Trim();
                        bid.DateSubmitted = sdr["DateSubmitted"].ToString().Trim();
                        bid.TenderID = sdr["TenderID"].ToString().Trim();
                        bid.Price = sdr["Price"].ToString().Trim();
                        bid.UserID = sdr["UserID"].ToString().Trim();
                        bid.Status = sdr["Status"].ToString().Trim();
                        bid.Description = sdr["Description"].ToString().Trim();
                        bidList.Add(bid);
                    }

                    con.Close();
                }
                if (Session["Email"] != null)
                {
                    return View(bidList);
                }
                else
                {
                    return RedirectToAction("Login", "Home");
                }
            }
            else
            {
                using (SqlConnection con = new SqlConnection(constr))
                {
                    con.Open();

                    String sql = "SELECT Bid.Id, Bid.BidID, Bid.DateSubmitted, Bid.Price, Bid.TenderID, Bid.UserID," +
                        "Bid.Status, Bid.Description FROM Bid" +
                        " WHERE UserID IN (SELECT UserID FROM SystemUser WHERE Email = @Email) ORDER BY Bid.DateSubmitted DESC";
                    SqlCommand cmd = new SqlCommand(sql, con);
                    cmd.Parameters.AddWithValue("@Email", Session["Email"]);

                    SqlDataReader sdr = cmd.ExecuteReader();
                    while (sdr.Read())
                    {
                        var bid = new Bid();
                        bid.Id = Convert.ToInt32(sdr["Id"].ToString().Trim());
                        bid.BidID = sdr["BidID"].ToString().Trim();
                        bid.DateSubmitted = sdr["DateSubmitted"].ToString().Trim();
                        bid.TenderID = sdr["TenderID"].ToString().Trim();
                        bid.Price = sdr["Price"].ToString().Trim();
                        bid.Description = sdr["Description"].ToString().Trim();
                        bid.UserID = sdr["UserID"].ToString().Trim();
                        bid.Status = sdr["Status"].ToString().Trim();
                        bidList.Add(bid);
                    }

                    con.Close();
                }
                if (Session["Email"] != null)
                {
                    return View(bidList);
                }
                else
                {
                    return RedirectToAction("Login", "Home");
                }
            }
        }


        public ActionResult AddBidDetails(int? id)
        {
            int TenderID = Convert.ToInt32(id);
            var bid = new Bid();
            List<SelectListItem> showroomList = new List<SelectListItem>();

            if (Session["Email"] != null)
            {
                string constr = ConfigurationManager.ConnectionStrings["WebProjectDb"].ConnectionString;
                using (SqlConnection con = new SqlConnection(constr))
                {
                    con.Open();

                    String sql = "SELECT Id, TenderID, TenderName,TenderDate, Image FROM Tender WHERE Id = @Id";
                    SqlCommand cmd = new SqlCommand(sql, con);
                    cmd.Parameters.AddWithValue("@Id", TenderID);

                    SqlDataReader sdr = cmd.ExecuteReader();
                    while (sdr.Read())
                    {
                        bid.Id = Convert.ToInt32(sdr["Id"].ToString().Trim());
                        bid.TenderID = sdr["TenderID"].ToString().Trim();
                        bid.TenderName = sdr["TenderName"].ToString().Trim();
                        bid.TenderDate = sdr["TenderDate"].ToString().Trim();
                        bid.ImageURL = "~/Content/Images/" + sdr["Image"].ToString().Trim();
                    }

                    con.Close();
                }

                using (SqlConnection con3 = new SqlConnection(constr))
                {
                    con3.Open();

                    String sql3 = "SELECT UserID FROM SystemUser WHERE Email = @Email";
                    SqlCommand cmd3 = new SqlCommand(sql3, con3);
                    cmd3.Parameters.AddWithValue("@Email", Session["Email"]);

                    SqlDataReader sdr3 = cmd3.ExecuteReader();
                    while (sdr3.Read())
                    {
                        bid.UserID = sdr3["UserID"].ToString().Trim();
                    }

                    con3.Close();
                    Guid guid = Guid.NewGuid();
                    bid.BidID = guid.ToString();

                }
                return View("AddBidDetails", bid);
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }


        // Get the bid model and insert new bid details to the bid table
        [HttpPost]
        public ActionResult AddBidDetails(Bid bid)
        {
            foreach (var modelValue in ModelState.Values)
            {
                modelValue.Errors.Clear();
            }


            Guid guid = Guid.NewGuid();
            bid.BidID = guid.ToString();
            Random random = new Random();
            bid.Id = random.Next();
            bid.Price = Convert.ToDecimal(bid.Price).ToString();

            if (string.IsNullOrEmpty(bid.TenderID))
            {
                ModelState.AddModelError("TenderID", "This field cannot be empty ");
            }
            if (string.IsNullOrEmpty(bid.UserID))
            {
                ModelState.AddModelError("UserID", "This field cannot be empty ");
            }
            if (string.IsNullOrEmpty(bid.Price))
            {
                ModelState.AddModelError("Price", "This field cannot be empty ");
            }
            if (string.IsNullOrEmpty(bid.Description))
            {
                ModelState.AddModelError("Description", "This field cannot be empty ");
            }

            if (ModelState.IsValid)
            {
                string constr = ConfigurationManager.ConnectionStrings["WebProjectDb"].ConnectionString;

                using (SqlConnection con = new SqlConnection(constr))
                {
                    con.Open();

                    SqlCommand com = new SqlCommand("INSERT INTO Bid(Id, BidID, DateSubmitted, TenderID, Price, UserID, Status, Description) " +
                        "VALUES(@Id, @BidID, @DateSubmitted, @TenderID, @Price, @UserID, @Status, @Description)");
                    com.CommandType = CommandType.Text;
                    com.Connection = con;

                    com.Parameters.AddWithValue("@Id", bid.Id);
                    com.Parameters.AddWithValue("@BidID", bid.BidID);
                    com.Parameters.AddWithValue("@DateSubmitted", DateTime.Now.ToString("yyyy-MM-dd"));
                    com.Parameters.AddWithValue("@TenderID", bid.TenderID);
                    com.Parameters.AddWithValue("@Price", bid.Price);
                    com.Parameters.AddWithValue("@UserID", bid.UserID);
                    com.Parameters.AddWithValue("@Status", "Pending");
                    com.Parameters.AddWithValue("@Description", bid.Description);

                    com.ExecuteReader();
                    con.Close();
                }

                //sending email
                System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage();
                mail.To.Add(Session["Email"].ToString());
                mail.From = new MailAddress("sihaninidu3@gmail.com", "Tender Management System", System.Text.Encoding.UTF8);
                mail.Subject = "Submitted Bid";
                mail.SubjectEncoding = System.Text.Encoding.UTF8;
                mail.Body = "Bid Id: " + bid.BidID + "<br>" +
                    "Date Submitted: " + bid.DateSubmitted + "<br>" +
                    "Tender ID: " + bid.TenderID + "<br>" +
                    "Price: " + bid.Price + "<br>" +
                    "User ID: " + bid.UserID + "<br>" +
                    "Status: " + "Pending" + "<br>";
                mail.BodyEncoding = System.Text.Encoding.UTF8;
                mail.IsBodyHtml = true;
                mail.Priority = MailPriority.High;
                SmtpClient client = new SmtpClient();
                client.Credentials = new System.Net.NetworkCredential("sihaninidu3@gmail.com", "itsMINE-1023##");
                client.Port = 587;
                client.Host = "smtp.gmail.com";
                client.EnableSsl = true;
                client.Send(mail);

                return RedirectToAction("Tenders", "Tender");
            }
            else
            {
                return View("AddBidDetails", bid);
            }


        }


        public ActionResult ViewBidDetails(int? id)
        {
            int BidID = Convert.ToInt32(id);
            var bid = new Bid();

            string constr = ConfigurationManager.ConnectionStrings["WebProjectDb"].ConnectionString;

            using (SqlConnection con = new SqlConnection(constr))
            {
                con.Open();

                String sql = "SELECT Bid.Id, Bid.BidID, Bid.DateSubmitted, Bid.Price, Bid.TenderID, Bid.UserID, " +
                        "Bid.Description, Bid.Status FROM Bid WHERE Bid.Id = @BidID";
                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@BidID", BidID);

                SqlDataReader sdr = cmd.ExecuteReader();
                while (sdr.Read())
                {
                    bid.Id = Convert.ToInt32(sdr["Id"].ToString().Trim());
                    bid.BidID = sdr["BidID"].ToString().Trim();
                    bid.DateSubmitted = sdr["DateSubmitted"].ToString().Trim();
                    bid.TenderID = sdr["TenderID"].ToString().Trim();
                    bid.Price = sdr["Price"].ToString().Trim();
                    bid.UserID = sdr["UserID"].ToString().Trim();
                    bid.Description = sdr["Description"].ToString().Trim();
                    bid.Status = sdr["Status"].ToString().Trim();
                }
                con.Close();
            }
            using (SqlConnection con = new SqlConnection(constr))
            {
                con.Open();

                String sql = "SELECT Id, TenderID, TenderName,TenderDate, Image FROM Tender WHERE TenderID = @Id";
                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@Id", bid.TenderID);

                SqlDataReader sdr = cmd.ExecuteReader();
                while (sdr.Read())
                {
                    bid.TenderName = sdr["TenderName"].ToString().Trim();
                }

                con.Close();
            }
            if (Session["Email"] != null)
            {
                return View(bid);
            }
            else
            {
                return RedirectToAction("Home/Login");
            }

        }



        // Get the bid id and delete the tender from the bid table
        [HttpPost]
        public ActionResult ViewBidDetails(Bid bid)
        {
            int bidId = Convert.ToInt32(bid.Id);
            string constr = ConfigurationManager.ConnectionStrings["WebProjectDb"].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                con.Open();

                SqlCommand com = new SqlCommand("DELETE FROM Bid WHERE Id = @Id");
                com.CommandType = CommandType.Text;
                com.Connection = con;

                com.Parameters.AddWithValue("@Id", bidId);

                com.ExecuteReader();
                con.Close();
            }
            if (Session["Email"] != null)
            {
                string userrole = "";

                using (SqlConnection con2 = new SqlConnection(constr))
                {
                    con2.Open();

                    String sql2 = "SELECT UserRole FROM SystemUser WHERE Email = @Email";
                    SqlCommand cmd2 = new SqlCommand(sql2, con2);
                    cmd2.Parameters.AddWithValue("@Email", Session["Email"]);

                    SqlDataReader sdr2 = cmd2.ExecuteReader();
                    while (sdr2.Read())
                    {
                        userrole = sdr2["UserRole"].ToString().Trim();
                    }

                    con2.Close();
                }
                if (userrole.Contains("Officer") || userrole.Contains("Director"))
                {
                    return RedirectToAction("ViewBids");
                }
                else
                {
                    return RedirectToAction("ViewMyBids");
                }
                
            }
            else
            {
                return RedirectToAction("Login");
            }

        }


        // Get the bid id and delete the bid from the bid table
        public ActionResult DeleteBid(int? id)
        {
            return RedirectToAction("ViewBidDetails", id);
        }



        // Get the bid id and view bid detials to edit
        public ActionResult EditBid(int? id)
        {
            int bidId = Convert.ToInt32(id);
            var bid = new Bid();

            string constr = ConfigurationManager.ConnectionStrings["WebProjectDb"].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                con.Open();

                String sql = "SELECT Bid.Id, Bid.BidID, Bid.DateSubmitted, Bid.Price, Bid.TenderID, Bid.UserID, " +
                        "Bid.Description, Bid.Status FROM Bid WHERE Bid.Id = @Id";
                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@Id", bidId);

                SqlDataReader sdr = cmd.ExecuteReader();
                while (sdr.Read())
                {
                    bid.Id = Convert.ToInt32(sdr["Id"].ToString().Trim());
                    bid.BidID = sdr["BidID"].ToString().Trim();
                    bid.DateSubmitted = sdr["DateSubmitted"].ToString().Trim();
                    bid.TenderID = sdr["TenderID"].ToString().Trim();
                    bid.Price = sdr["Price"].ToString().Trim();
                    bid.UserID = sdr["UserID"].ToString();
                    bid.Description = sdr["Description"].ToString().Trim();
                    bid.Status = sdr["Status"].ToString().Trim();
                }

                con.Close();
            }
            //sending email
            System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage();
            mail.To.Add(Session["Email"].ToString());
            mail.From = new MailAddress("sihaninidu3@gmail.com", "Tender Management System", System.Text.Encoding.UTF8);
            mail.Subject = "Submitted Bid";
            mail.SubjectEncoding = System.Text.Encoding.UTF8;
            mail.Body = "Bid Id: " + bid.BidID + "<br>" +
                "Date Submitted: " + bid.DateSubmitted + "<br>" +
                "Tender ID: " + bid.TenderID + "<br>" +
                "Price: " + bid.Price + "<br>" +
                "User ID: " + bid.UserID + "<br>" +
                "Status: " + "Pending" + "<br>";
            mail.BodyEncoding = System.Text.Encoding.UTF8;
            mail.IsBodyHtml = true;
            mail.Priority = MailPriority.High;
            SmtpClient client = new SmtpClient();
            client.Credentials = new System.Net.NetworkCredential("sihaninidu3@gmail.com", "itsMINE-1023##");
            client.Port = 587;
            client.Host = "smtp.gmail.com";
            client.EnableSsl = true;
            client.Send(mail);

            if (Session["Email"] != null)
            {
                return View(bid);
            }
            else
            {
                return RedirectToAction("Login");
            }

        }



        // Get the bid model and update the bid details
        [HttpPost]
        public ActionResult EditBid(Bid bid)
        {
            if (string.IsNullOrEmpty(bid.Price))
            {
                ModelState.AddModelError("Price", "This field cannot be empty ");
            }

            if (ModelState.IsValid)
            {
                int bidId = Convert.ToInt32(bid.Id);
                string constr = ConfigurationManager.ConnectionStrings["WebProjectDb"].ConnectionString;
                using (SqlConnection con = new SqlConnection(constr))
                {
                    con.Open();

                    SqlCommand cmd = new SqlCommand("UPDATE Bid SET Price = @Price, Description = @Description WHERE Id = @Id");
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = con;

                    cmd.Parameters.AddWithValue("@Id", bidId);
                    cmd.Parameters.AddWithValue("@Price", bid.Price);
                    cmd.Parameters.AddWithValue("@Description", bid.Description);

                    cmd.ExecuteReader();
                    con.Close();
                }
                if (Session["Email"] != null)
                {
                    return RedirectToAction("ViewBids");
                }
                else
                {
                    return RedirectToAction("Login");
                }
            }
            else
            {
                return View("EditBid", bid);
            }

        }
    }
}