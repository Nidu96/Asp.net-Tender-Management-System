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

namespace TenderManagementSystem.Controllers
{
    public class HomeController : Controller
    {
        // View the index page
        public ActionResult Index()
        {
            return View();
        }

        // View the about page
        public ActionResult About()
        {
            return View();
        }

        // View the contact us page
        public ActionResult ContactUs()
        {
            return View();
        }

        // View customer login page
        public ActionResult Login()
        {
            Session.Clear();
            return View();
        }

        // Get the customer model and insert new customer details to the customer table
        [HttpPost]
        public ActionResult Login(SystemUser user)
        {
            Session.Clear();
            foreach (var modelValue in ModelState.Values)
            {
                modelValue.Errors.Clear();
            }

            if (string.IsNullOrEmpty(user.Email))
            {
                ModelState.AddModelError("Email", "This field cannot be empty ");
            }

            if (string.IsNullOrEmpty(user.Password))
            {
                ModelState.AddModelError("Password", "This field cannot be empty ");
            }
            if (ModelState.IsValid)
            {
                string constr = ConfigurationManager.ConnectionStrings["WebProjectDb"].ConnectionString;
                using (SqlConnection con = new SqlConnection(constr))
                {
                    con.Open();

                    String sql = "SELECT COUNT(*) AS [Count] FROM SystemUser WHERE Email = @Email AND Password = @Password";
                    SqlCommand cmd = new SqlCommand(sql, con);
                    cmd.Parameters.AddWithValue("@Email", user.Email);
                    cmd.Parameters.AddWithValue("@Password", user.Password);

                    SqlDataReader sdr = cmd.ExecuteReader();
                    while (sdr.Read())
                    {
                        if (Convert.ToInt32(sdr["Count"]) == 1)
                        {
                            Session["Email"] = user.Email;

                        }
                    }
                    con.Close();
                    //return View("Login", user);
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
                            if (userrole.Contains("Officer") || userrole.Contains("Director"))
                            {
                                return RedirectToAction("ViewTenders", "Tender");
                            }
                            else
                            {
                                return RedirectToAction("Tenders", "Tender");
                            }
                        }

                        con2.Close();
                    }
                    if (Session.Count == 0)
                    {
                        ModelState.AddModelError("Error", "Unauthorized");
                        return View("Login", user);
                    }
                }
                else
                {
                    ModelState.AddModelError("Error", "Invalid Credentials");
                    return View("Login", user);
                }
            }
            else
            {
                return View("Login", user);
            }
            return View("Login", user);
        }

        // View all the user data
        public ActionResult ViewCustomers()
        {

            var customerList = new List<SystemUser>();

            string constr = ConfigurationManager.ConnectionStrings["WebProjectDb"].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                con.Open();

                String sql = "SELECT Id, UserID, FullName, Phone,AddressLine_1,AddressLine_2,AddressLine_3, Email, Password FROM SystemUser";
                SqlCommand cmd = new SqlCommand(sql, con);

                SqlDataReader sdr = cmd.ExecuteReader();
                while (sdr.Read())
                {
                    var customer = new SystemUser();
                    customer.Id = Convert.ToInt32(sdr["Id"].ToString().Trim());
                    customer.UserID = sdr["UserID"].ToString().Trim();
                    customer.FullName = sdr["FullName"].ToString().Trim();
                    customer.Phone = sdr["Phone"].ToString().Trim();
                    customer.Email = sdr["Email"].ToString().Trim();
                    customer.Password = sdr["Password"].ToString().Trim();
                    customer.AddressLine_1 = sdr["AddressLine_1"].ToString().Trim();
                    customer.AddressLine_2 = sdr["AddressLine_2"].ToString().Trim();
                    customer.AddressLine_3 = sdr["AddressLine_3"].ToString().Trim();
                    customerList.Add(customer);
                }

                con.Close();
            }
            if (Session["Email"] != null)
            {
                return View(customerList);
            }
            else
            {
                return RedirectToAction("Login");
            }

        }

        // View customer registration page
        public ActionResult Register()
        {
            return View();
        }

        // Get the customer model and insert new customer details to the customer table
        [HttpPost]
        public ActionResult Register(SystemUser customer)
        {

            foreach (var modelValue in ModelState.Values)
            {
                modelValue.Errors.Clear();
            }


            Guid guid = Guid.NewGuid();
            customer.UserID = guid.ToString();
            Random random = new Random();
            customer.Id = random.Next();
            customer.UserRole = "Bidder";
            if (string.IsNullOrEmpty(customer.FullName))
            {
                ModelState.AddModelError("FullName", "This field cannot be empty ");
            }
            if (string.IsNullOrEmpty(customer.Phone))
            {
                ModelState.AddModelError("Phone", "This field cannot be empty ");
            }
            if (string.IsNullOrEmpty(customer.AddressLine_1))
            {
                ModelState.AddModelError("AddressLine_1", "This field cannot be empty ");
            }
            if (string.IsNullOrEmpty(customer.AddressLine_2))
            {
                ModelState.AddModelError("AddressLine_2", "This field cannot be empty ");
            }
            if (string.IsNullOrEmpty(customer.AddressLine_3))
            {
                ModelState.AddModelError("AddressLine_3", "This field cannot be empty ");
            }
            if (string.IsNullOrEmpty(customer.UserRole))
            {
                ModelState.AddModelError("UserRole", "This field cannot be empty ");
            }
            if (string.IsNullOrEmpty(customer.Email))
            {
                ModelState.AddModelError("Email", "This field cannot be empty ");
            }
            if (string.IsNullOrEmpty(customer.ConfirmEmail))
            {
                ModelState.AddModelError("ConfirmEmail", "This field cannot be empty ");
            }
            if (string.IsNullOrEmpty(customer.Password))
            {
                ModelState.AddModelError("Password", "This field cannot be empty ");
            }
            if (string.IsNullOrEmpty(customer.ConfirmPassword))
            {
                ModelState.AddModelError("ConfirmPassword", "This field cannot be empty ");
            }


            if (ModelState.IsValid)
            {
                string constr = ConfigurationManager.ConnectionStrings["WebProjectDb"].ConnectionString;
                string userrole = customer.UserRole;
                if (customer.UserRole == null || customer.UserRole == "")
                {
                    userrole = "Bidder";
                }

                using (SqlConnection con = new SqlConnection(constr))
                {
                    con.Open();

                    SqlCommand com = new SqlCommand("INSERT INTO SystemUser (Id, UserID, FullName, Phone, AddressLine_1, AddressLine_2, AddressLine_3, UserRole, Email, Password) VALUES(@Id, @UserID, @FullName, @Phone, @AddressLine_1, @AddressLine_2, @AddressLine_3, @UserRole, @Email, @Password)");
                    com.CommandType = CommandType.Text;
                    com.Connection = con;

                    com.Parameters.AddWithValue("@Id", customer.Id);
                    com.Parameters.AddWithValue("@UserID", customer.UserID);
                    com.Parameters.AddWithValue("@FullName", customer.FullName);
                    com.Parameters.AddWithValue("@Phone", customer.Phone);
                    com.Parameters.AddWithValue("@AddressLine_1", customer.AddressLine_1);
                    com.Parameters.AddWithValue("@AddressLine_2", customer.AddressLine_2);
                    com.Parameters.AddWithValue("@AddressLine_3", customer.AddressLine_3);
                    com.Parameters.AddWithValue("@UserRole", userrole);
                    com.Parameters.AddWithValue("@Email", customer.Email);
                    com.Parameters.AddWithValue("@Password", customer.Password);

                    com.ExecuteReader();
                    con.Close();
                }

                return RedirectToAction("Login");
            }
            else
            {
                return View("Register", customer);
            }


        }


        public ActionResult ViewDetails(int? id)
        {
            int UserID = Convert.ToInt32(id);
            var customer = new SystemUser();

            string constr = ConfigurationManager.ConnectionStrings["WebProjectDb"].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                con.Open();

                String sql = "SELECT Id, UserID, UserRole, FullName, Phone, AddressLine_1, AddressLine_2, AddressLine_3, Email, Password FROM SystemUser WHERE Id = @Id";
                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@Id", UserID);

                SqlDataReader sdr = cmd.ExecuteReader();
                while (sdr.Read())
                {
                    customer.Id = Convert.ToInt32(sdr["Id"].ToString().Trim());
                    customer.UserID = sdr["UserID"].ToString().Trim();
                    customer.UserRole = sdr["UserRole"].ToString().Trim();
                    customer.FullName = sdr["FullName"].ToString().Trim();
                    customer.Phone = sdr["Phone"].ToString().Trim();
                    customer.AddressLine_1 = sdr["AddressLine_1"].ToString().Trim();
                    customer.AddressLine_2 = sdr["AddressLine_2"].ToString().Trim();
                    customer.AddressLine_3 = sdr["AddressLine_3"].ToString().Trim();
                    customer.Email = sdr["Email"].ToString().Trim();
                    customer.Password = sdr["Password"].ToString().Trim();
                }

                con.Close();
            }
            if (Session["Email"] != null)
            {
                return View(customer);
            }
            else
            {
                return RedirectToAction("Login");
            }

        }

        // Get the customer id and delete the customer from the customer table
        public ActionResult DeleteCustomer(int? id)
        {
            return RedirectToAction("DeleteCustomer", id);
        }


        // Get the customer id and delete the customer from the customer table
        [HttpPost]
        public ActionResult ViewDetails(SystemUser customer)
        {
            int customerId = Convert.ToInt32(customer.Id);
            string constr = ConfigurationManager.ConnectionStrings["WebProjectDb"].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                con.Open();

                SqlCommand com = new SqlCommand("DELETE FROM SystemUser WHERE Id = @Id");
                com.CommandType = CommandType.Text;
                com.Connection = con;

                com.Parameters.AddWithValue("@Id", customerId);

                com.ExecuteReader();
                con.Close();
            }
            if (Session["Email"] != null)
            {
                return RedirectToAction("ViewCustomers");
            }
            else
            {
                return RedirectToAction("Login");
            }

        }

        // Get the customer id and view customer detials to edit
        public ActionResult EditCustomer(int? id)
        {
            int customerId = Convert.ToInt32(id);
            var customer = new SystemUser();

            string constr = ConfigurationManager.ConnectionStrings["WebProjectDb"].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                con.Open();

                String sql = "SELECT Id, UserID, UserRole, FullName,Phone, AddressLine_1,AddressLine_2, AddressLine_3, Email, Password FROM SystemUser WHERE Id = @Id";
                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@Id", customerId);

                SqlDataReader sdr = cmd.ExecuteReader();
                while (sdr.Read())
                {
                    customer.Id = Convert.ToInt32(sdr["Id"].ToString().Trim());
                    customer.UserID = sdr["UserID"].ToString().Trim();
                    customer.UserRole = sdr["UserRole"].ToString().Trim();
                    customer.FullName = sdr["FullName"].ToString().Trim();
                    customer.Phone = sdr["Phone"].ToString().Trim();
                    customer.AddressLine_1 = sdr["AddressLine_1"].ToString().Trim();
                    customer.AddressLine_2 = sdr["AddressLine_2"].ToString().Trim();
                    customer.AddressLine_3 = sdr["AddressLine_3"].ToString().Trim();
                    customer.Email = sdr["Email"].ToString().Trim();
                    customer.Password = sdr["Password"].ToString().Trim();
                }

                con.Close();
            }
            if (Session["Email"] != null)
            {
                return View(customer);
            }
            else
            {
                return RedirectToAction("Login");
            }

        }

        // Get the customer model and update the customer details
        [HttpPost]
        public ActionResult EditCustomer(SystemUser customer)
        {
            int customerId = Convert.ToInt32(customer.Id);
            string constr = ConfigurationManager.ConnectionStrings["WebProjectDb"].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                con.Open();

                SqlCommand cmd = new SqlCommand("UPDATE SystemUser SET UserID = @UserID, UserRole = @UserRole, FullName = @FullName, Phone = @Phone, Email = @Email, Password = @Password,AddressLine_1 = @AddressLine_1, AddressLine_2 = @AddressLine_2, AddressLine_3 = @AddressLine_3 WHERE Id = @Id");
                cmd.CommandType = CommandType.Text;
                cmd.Connection = con;

                cmd.Parameters.AddWithValue("@Id", customer.Id);
                cmd.Parameters.AddWithValue("@UserID", customer.UserID);
                cmd.Parameters.AddWithValue("@UserRole", customer.UserRole);
                cmd.Parameters.AddWithValue("@FullName", customer.FullName);
                cmd.Parameters.AddWithValue("@Phone", customer.Phone);
                cmd.Parameters.AddWithValue("@AddressLine_1", customer.AddressLine_1);
                cmd.Parameters.AddWithValue("@AddressLine_2", customer.AddressLine_2);
                cmd.Parameters.AddWithValue("@AddressLine_3", customer.AddressLine_3);
                cmd.Parameters.AddWithValue("@Email", customer.Email);
                cmd.Parameters.AddWithValue("@Password", customer.Password);

                cmd.ExecuteReader();
                con.Close();
            }
            if (Session["Email"] != null)
            {
                return RedirectToAction("ViewCustomers");
            }
            else
            {
                return RedirectToAction("Login");
            }

        }


        // Get the customer id and view customer detials to edit
        public ActionResult EditProfile()
        {
            var customer = new SystemUser();

            string constr = ConfigurationManager.ConnectionStrings["WebProjectDb"].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                con.Open();

                String sql = "SELECT Id, UserID, UserRole, FullName,Phone, AddressLine_1,AddressLine_2, AddressLine_3, " +
                    "Password FROM SystemUser WHERE Email = @Email";
                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@Email", Session["Email"].ToString());

                SqlDataReader sdr = cmd.ExecuteReader();
                while (sdr.Read())
                {
                    customer.Id = Convert.ToInt32(sdr["Id"].ToString().Trim());
                    customer.UserID = sdr["UserID"].ToString().Trim();
                    customer.FullName = sdr["FullName"].ToString().Trim();
                    customer.Phone = sdr["Phone"].ToString().Trim();
                    customer.AddressLine_1 = sdr["AddressLine_1"].ToString().Trim();
                    customer.AddressLine_2 = sdr["AddressLine_2"].ToString().Trim();
                    customer.AddressLine_3 = sdr["AddressLine_3"].ToString().Trim();
                    customer.Email = Session["Email"].ToString().Trim();
                    customer.Password = sdr["Password"].ToString().Trim();
                }

                con.Close();
            }
            if (Session["Email"] != null)
            {
                return View(customer);
            }
            else
            {
                return RedirectToAction("Login");
            }

        }

        // Get the user model and update the user details
        [HttpPost]
        public ActionResult EditProfile(SystemUser customer)
        {
            string constr = ConfigurationManager.ConnectionStrings["WebProjectDb"].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                con.Open();

                SqlCommand cmd = new SqlCommand("UPDATE SystemUser SET " +
                    "FullName = @FullName, Phone = @Phone, Password = @Password," +
                    "AddressLine_1 = @AddressLine_1, AddressLine_2 = @AddressLine_2, AddressLine_3 = @AddressLine_3 " +
                    "WHERE Email = @Email");
                cmd.CommandType = CommandType.Text;
                cmd.Connection = con;

                cmd.Parameters.AddWithValue("@Id", customer.Id);
                cmd.Parameters.AddWithValue("@FullName", customer.FullName);
                cmd.Parameters.AddWithValue("@Phone", customer.Phone);
                cmd.Parameters.AddWithValue("@AddressLine_1", customer.AddressLine_1);
                cmd.Parameters.AddWithValue("@AddressLine_2", customer.AddressLine_2);
                cmd.Parameters.AddWithValue("@AddressLine_3", customer.AddressLine_3);
                cmd.Parameters.AddWithValue("@Email", Session["Email"].ToString());
                cmd.Parameters.AddWithValue("@Password", customer.Password);

                cmd.ExecuteReader();
                con.Close();
            }
            if (Session["Email"] != null)
            {
                return View(customer);
            }
            else
            {
                return RedirectToAction("Login");
            }

        }
    }
}