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
using System.IO;
using System.Net.Mail;

namespace TenderManagementSystem.Controllers
{
    public class TenderController : Controller
    {
        // View the tenders page
        public ActionResult Tenders()
        {
            var tenderList = new List<Tender>();

            string constr = ConfigurationManager.ConnectionStrings["WebProjectDb"].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                con.Open();

                String sql = "SELECT Id, TenderID, TenderName, TenderDate, Description, Image FROM Tender ORDER BY TenderDate DESC";
                SqlCommand cmd = new SqlCommand(sql, con);

                SqlDataReader sdr = cmd.ExecuteReader();
                while (sdr.Read())
                {
                    var tenderdate = DateTime.Parse((sdr["TenderDate"].ToString().Split()[0]).Replace("-", "/")).Date;
                    DateTime datenow = DateTime.Now.Date;

                    var tender = new Tender();
                    tender.Id = Convert.ToInt32(sdr["Id"].ToString().Trim());
                    tender.TenderID = sdr["TenderID"].ToString().Trim();
                    tender.TenderName = sdr["TenderName"].ToString().Trim();
                    tender.TenderDate = sdr["TenderDate"].ToString().Trim();
                    tender.Description = sdr["Description"].ToString().Trim();
                    tender.ImageURL = "~/Content/Images/" + sdr["Image"].ToString().Trim();
                    if((tenderdate > datenow) || (tenderdate == datenow))
                    {
                        tenderList.Add(tender);
                    }
                    
                }

                con.Close();
            }
            return View(tenderList);

        }

        // View all the tender data
        public ActionResult ViewTenders()
        {

            var tenderList = new List<Tender>();
            var bidList = new List<Bid>();

            string constr = ConfigurationManager.ConnectionStrings["WebProjectDb"].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                con.Open();

                String sql = "SELECT Id, TenderID, TenderName, TenderDate, Description, Image FROM Tender ORDER BY TenderDate DESC";
                SqlCommand cmd = new SqlCommand(sql, con);

                SqlDataReader sdr = cmd.ExecuteReader();
                while (sdr.Read())
                {

                    var tender = new Tender();
                    tender.Id = Convert.ToInt32(sdr["Id"].ToString().Trim());
                    tender.TenderID = sdr["TenderID"].ToString().Trim();
                    tender.TenderName = sdr["TenderName"].ToString().Trim();
                    tender.TenderDate = sdr["TenderDate"].ToString().Trim();
                    tender.Description = sdr["Description"].ToString().Trim();
                    tender.ImageURL = "~/Content/Images/" + sdr["Image"].ToString().Trim();

                    
                    tenderList.Add(tender);
                }

                con.Close();
            }
            foreach (Tender temptender in tenderList)
            {

                using (SqlConnection con = new SqlConnection(constr))
                {
                    con.Open();

                    String sql = "SELECT Bid.Id, Bid.BidID, Bid.DateSubmitted, Bid.Price, Bid.TenderID, Bid.UserID, " +
                        "Bid.Description, Bid.Status FROM Bid WHERE Bid.TenderID = @TenderID";
                    SqlCommand cmd = new SqlCommand(sql, con);
                    cmd.Parameters.AddWithValue("@TenderID", temptender.TenderID);

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
                        bid.Description = sdr["Description"].ToString().Trim();
                        bid.Status = sdr["Status"].ToString().Trim();
                        bidList.Add(bid);
                    }

                    temptender.BidsList = bidList;
                    con.Close();
                }
            }
            if (Session["Email"] != null)
            {
                return View(tenderList);
            }
            else
            {
                return RedirectToAction("Home/Login");
            }

        }

        // View add tender page
        public ActionResult AddTender()
        {
            return View();
        }

        // Get the tender model and insert new tender details to the tender table
        [HttpPost]
        public ActionResult AddTender(Tender tender)
        {

            foreach (var modelValue in ModelState.Values)
            {
                modelValue.Errors.Clear();
            }


            Guid guid = Guid.NewGuid();
            tender.TenderID = guid.ToString();
            Random random = new Random();
            tender.Id = random.Next();

            if (string.IsNullOrEmpty(tender.TenderName))
            {
                ModelState.AddModelError("TenderName", "This field cannot be empty ");
            }
            if (string.IsNullOrEmpty(tender.Description))
            {
                ModelState.AddModelError("Description", "This field cannot be empty ");
            }

            if (ModelState.IsValid)
            {
                //byte[] image = new byte[tender.Image.ContentLength];
                //tender.Image.InputStream.Read(image, 0, image.Length);

                string newfilename;
                if (tender.Image != null)
                {
                    var filename = Path.GetFileName(tender.Image.FileName);
                    var path = Path.Combine(Server.MapPath("~/Content/Images/"), filename);
                    if(System.IO.File.Exists(path))
                    {
                        System.IO.File.Delete(path);
                    }
                    newfilename = tender.Id + System.IO.Path.GetExtension(tender.Image.FileName);
                    tender.Image.SaveAs(System.IO.Path.Combine(Server.MapPath("~/Content/Images/"), newfilename));
                }
                else
                {
                    newfilename = "noimage.jpg";
                }


                string constr = ConfigurationManager.ConnectionStrings["WebProjectDb"].ConnectionString;

                using (SqlConnection con = new SqlConnection(constr))
                {
                    con.Open();

                    SqlCommand com = new SqlCommand("INSERT INTO Tender(Id, TenderID, TenderName, TenderDate, Description, Image) " +
                        "VALUES(@Id, @TenderID, @TenderName, @TenderDate, @Image)");
                    com.CommandType = CommandType.Text;
                    com.Connection = con;

                    com.Parameters.AddWithValue("@Id", tender.Id);
                    com.Parameters.AddWithValue("@TenderID", tender.TenderID);
                    com.Parameters.AddWithValue("@TenderName", tender.TenderName);
                    com.Parameters.AddWithValue("@TenderDate", tender.TenderDate);
                    com.Parameters.AddWithValue("@Description", tender.Description);
                    com.Parameters.AddWithValue("@Image", newfilename);

                    com.ExecuteReader();
                    con.Close();
                }

                return RedirectToAction("ViewTenders");
            }
            else
            {
                return View("AddTender", tender);
            }


        }


        public ActionResult ViewTenderDetails(int? id)
        {
            int TenderID = Convert.ToInt32(id);
            var tender = new Tender();
            var bidList = new List<Bid>();

            string constr = ConfigurationManager.ConnectionStrings["WebProjectDb"].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                con.Open();

                String sql = "SELECT Id, TenderID, TenderName, TenderDate, Description, Image FROM Tender WHERE Id = @Id";
                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@Id", TenderID);

                SqlDataReader sdr = cmd.ExecuteReader();
                while (sdr.Read())
                {
                    tender.Id = Convert.ToInt32(sdr["Id"].ToString().Trim());
                    tender.TenderID = sdr["TenderID"].ToString().Trim();
                    tender.TenderName = sdr["TenderName"].ToString().Trim();
                    tender.TenderDate = sdr["TenderDate"].ToString().Trim();
                    tender.Description = sdr["Description"].ToString().Trim();
                    tender.ImageURL = "~/Content/Images/" + sdr["Image"].ToString().Trim();
                }

                con.Close();
            }

            using (SqlConnection con = new SqlConnection(constr))
            {
                con.Open();

                String sql = "SELECT Bid.Id, Bid.BidID, Bid.DateSubmitted, Bid.Price, Bid.TenderID, Bid.UserID, " +
                        "Bid.Description, Bid.Status FROM Bid WHERE Bid.TenderID = @TenderID ORDER BY cast(Bid.Price as decimal) DESC";
                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@TenderID", tender.TenderID);

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
                        bid.Description = sdr["Description"].ToString().Trim();
                        bid.Status = sdr["Status"].ToString().Trim();
                        bidList.Add(bid);
                    }

                tender.BidsList = bidList;
                con.Close();
            }

            using (SqlConnection con2 = new SqlConnection(constr))
            {
                con2.Open();

                String sql2 = "SELECT UserRole FROM SystemUser WHERE Email = @Email";
                SqlCommand cmd2 = new SqlCommand(sql2, con2);
                cmd2.Parameters.AddWithValue("@Email", Session["Email"]);

                SqlDataReader sdr2 = cmd2.ExecuteReader();
                while (sdr2.Read())
                {
                    tender.UserRole = sdr2["UserRole"].ToString().Trim();
                }

                con2.Close();
            }


            if (Session["Email"] != null)
            {
                return View(tender);
            }
            else
            {
                return RedirectToAction("Home/Login");
            }

        }

        // Get the tender id and delete the tender from the tender table
        public ActionResult DeleteTender(int? id)
        {
            return RedirectToAction("ViewTenderDetails", id);
        }


        // Get the tender id and delete the tender from the tender table
        [HttpPost]
        public ActionResult ViewTenderDetails(Tender tender)
        {
            int tenderId = Convert.ToInt32(tender.Id);
            string constr = ConfigurationManager.ConnectionStrings["WebProjectDb"].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                con.Open();

                SqlCommand com = new SqlCommand("DELETE FROM Tender WHERE Id = @Id");
                com.CommandType = CommandType.Text;
                com.Connection = con;

                com.Parameters.AddWithValue("@Id", tenderId);

                com.ExecuteReader();
                con.Close();
                if (System.IO.File.Exists(Server.MapPath(tender.ImageURL)))
                {
                    System.IO.File.Delete(Server.MapPath(tender.ImageURL));
                }
            }

            using (SqlConnection con = new SqlConnection(constr))
            {
                con.Open();

                SqlCommand com = new SqlCommand("DELETE FROM Bid WHERE TenderID = @Id");
                com.CommandType = CommandType.Text;
                com.Connection = con;

                com.Parameters.AddWithValue("@Id", tender.TenderID);

                com.ExecuteReader();
                con.Close();
            }

            if (Session["Email"] != null)
            {
                return RedirectToAction("ViewTenders");
            }
            else
            {
                return RedirectToAction("Login");
            }

        }

        // Get the tender id and view tender detials to edit
        public ActionResult EditTender(int? id)
        {
            int tenderId = Convert.ToInt32(id);
            var tender = new Tender();

            string constr = ConfigurationManager.ConnectionStrings["WebProjectDb"].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                con.Open();

                String sql = "SELECT Id, TenderID, TenderName, TenderDate, Description FROM tender WHERE Id = @Id";
                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@Id", tenderId);

                SqlDataReader sdr = cmd.ExecuteReader();
                while (sdr.Read())
                {
                    tender.Id = Convert.ToInt32(sdr["Id"].ToString().Trim());
                    tender.TenderID = sdr["TenderID"].ToString().Trim();
                    tender.TenderName = sdr["TenderName"].ToString().Trim();
                    tender.Description = sdr["Description"].ToString().Trim();
                    tender.TenderDate = sdr["TenderDate"].ToString().Trim();
                }

                con.Close();
            }
            if (Session["Email"] != null)
            {
                return View(tender);
            }
            else
            {
                return RedirectToAction("Login");
            }

        }

        // Get the tender model and update the tender details
        [HttpPost]
        public ActionResult EditTender(Tender tender)
        {
            if (string.IsNullOrEmpty(tender.TenderName))
            {
                ModelState.AddModelError("TenderName", "This field cannot be empty ");
            }
            if (string.IsNullOrEmpty(tender.Description))
            {
                ModelState.AddModelError("Description", "This field cannot be empty ");
            }

            if (ModelState.IsValid)
            {
                int tenderId = Convert.ToInt32(tender.Id);
                string constr = ConfigurationManager.ConnectionStrings["WebProjectDb"].ConnectionString;
                using (SqlConnection con = new SqlConnection(constr))
                {
                    con.Open();

                    SqlCommand cmd = new SqlCommand("UPDATE tender SET TenderID = @TenderID, TenderName = @TenderName, " +
                        "TenderDate = @TenderDate, Description = @Description WHERE Id = @Id");
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = con;

                    cmd.Parameters.AddWithValue("@Id", tenderId);
                    cmd.Parameters.AddWithValue("@TenderID", tender.TenderID);
                    cmd.Parameters.AddWithValue("@TenderName", tender.TenderName);
                    cmd.Parameters.AddWithValue("@Description", tender.Description);
                    cmd.Parameters.AddWithValue("@TenderDate", tender.TenderDate);

                    cmd.ExecuteReader();
                    con.Close();
                }
                if (Session["Email"] != null)
                {
                    return RedirectToAction("ViewTenders");
                }
                else
                {
                    return RedirectToAction("Login");
                }
            }
            else
            {
                return View("EditTender", tender);
            }

        }

        public ActionResult AcceptBid(Bid bid)
        {
            int bidId = Convert.ToInt32(bid.Id);

            string constr = ConfigurationManager.ConnectionStrings["WebProjectDb"].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                con.Open();

                SqlCommand cmd = new SqlCommand("UPDATE Bid SET Status = @Status WHERE Id = @Id");
                cmd.CommandType = CommandType.Text;
                cmd.Connection = con;

                cmd.Parameters.AddWithValue("@Id", bidId);
                cmd.Parameters.AddWithValue("@Status", "Accepted");

                cmd.ExecuteReader();
                con.Close();
            }
            return RedirectToAction("ViewTenders");
        }


        public ActionResult EvaluateBid(Bid bid)
        {
            int bidId = Convert.ToInt32(bid.Id);

            string constr = ConfigurationManager.ConnectionStrings["WebProjectDb"].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                con.Open();

                SqlCommand cmd = new SqlCommand("UPDATE Bid SET Status = @Status WHERE Id = @Id");
                cmd.CommandType = CommandType.Text;
                cmd.Connection = con;

                cmd.Parameters.AddWithValue("@Id", bidId);
                cmd.Parameters.AddWithValue("@Status", "Evaluated");

                cmd.ExecuteReader();
                con.Close();
            }
            return RedirectToAction("ViewTenders");
        }


        public ActionResult RejectBid(Bid bid)
        {
            int bidId = Convert.ToInt32(bid.Id);

            string constr = ConfigurationManager.ConnectionStrings["WebProjectDb"].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                con.Open();

                SqlCommand cmd = new SqlCommand("UPDATE Bid SET Status = @Status WHERE Id = @Id");
                cmd.CommandType = CommandType.Text;
                cmd.Connection = con;

                cmd.Parameters.AddWithValue("@Id", bidId);
                cmd.Parameters.AddWithValue("@Status", "Rejected");

                cmd.ExecuteReader();
                con.Close();
            }
            return RedirectToAction("ViewTenders");
        }


        public ActionResult EmailBidders(int? id)
        {
            int TenderID = Convert.ToInt32(id);
            var tender = new Tender();
            var bidList = new List<Bid>();
            string emailbody = "";

            string constr = ConfigurationManager.ConnectionStrings["WebProjectDb"].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                con.Open();

                String sql = "SELECT TenderID,TenderName FROM Tender WHERE Id = @Id";
                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@Id", TenderID);

                SqlDataReader sdr = cmd.ExecuteReader();
                while (sdr.Read())
                {
                    tender.TenderID = sdr["TenderID"].ToString().Trim();
                    tender.TenderName = sdr["TenderName"].ToString().Trim();
                }

                con.Close();
            }

            using (SqlConnection con = new SqlConnection(constr))
            {
                con.Open();

                String sql = "SELECT Bid.UserID,Bid.Price,Bid.Status FROM Bid WHERE Bid.TenderID = @TenderID";
                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@TenderID", tender.TenderID);

                SqlDataReader sdr = cmd.ExecuteReader();
                while (sdr.Read())
                {
                    var bid = new Bid();
                    bid.UserID = sdr["UserID"].ToString().Trim();
                    bid.Price = sdr["Price"].ToString().Trim();
                    bid.Status = sdr["Status"].ToString().Trim();
                    bidList.Add(bid);
                }
                con.Close();
            }

            foreach (Bid tempbid in bidList)
            {
                using (SqlConnection con2 = new SqlConnection(constr))
                {
                    con2.Open();

                    String sql2 = "SELECT Email FROM SystemUser WHERE UserID = @UserID";
                    SqlCommand cmd2 = new SqlCommand(sql2, con2);
                    cmd2.Parameters.AddWithValue("@UserID", tempbid.UserID);

                    SqlDataReader sdr2 = cmd2.ExecuteReader();
                    while (sdr2.Read())
                    {
                        tempbid.Email = sdr2["Email"].ToString().Trim();
                    }

                    con2.Close();
                }
                if(tempbid.Status == "Accepted")
                {
                    emailbody = "Accepted bid price for the tender " + tender.TenderName + " is" + tempbid.Price;
                }
            }

            foreach (Bid tempbid in bidList)
            {
                //sending email
                System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage();
                mail.To.Add(tempbid.Email.ToString());
                mail.From = new MailAddress("sihaninidu3@gmail.com", "Tender Management System", System.Text.Encoding.UTF8);
                mail.Subject = "Submitted Bid";
                mail.SubjectEncoding = System.Text.Encoding.UTF8;
                mail.Body = emailbody;
                mail.BodyEncoding = System.Text.Encoding.UTF8;
                mail.IsBodyHtml = true;
                mail.Priority = MailPriority.High;
                SmtpClient client = new SmtpClient();
                client.Credentials = new System.Net.NetworkCredential("sihaninidu3@gmail.com", "itsMINE-1023##");
                client.Port = 587;
                client.Host = "smtp.gmail.com";
                client.EnableSsl = true;
                client.Send(mail);
            }

            return RedirectToAction("ViewTenders");

        }
    }
}