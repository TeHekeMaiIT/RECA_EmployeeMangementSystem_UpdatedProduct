using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data;
using loginsec;


namespace RecaPayroll
{
    
    public partial class PayrollReview : Form
    {
        Connection con = new Connection();
        string id, Hours;
        public string Emp_ID;
        private Panel leftBoarderBtn;
        public string ConnectionString = "server=localhost;user id=root;password=Dbms@2022;persistsecurityinfo=True;database=human_resources";

        public PayrollReview()
        {
            InitializeComponent();
        }

        private void datefrom_ValueChanged(object sender, EventArgs e)
        {

        }

        private void dataGridReviewpay_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void PayrollReview_Load(object sender, EventArgs e)
        {
            FillPayrollReviewGrid();
        }

public void FillPayrollReviewGrid()
        {
            double  Earning, Deduction, Payment;
            string Hourly_Base_Salary, Leave_Requested, Work_Hours, Tax_Percentage;
            string sqlCommand = "SELECT distinct concat(human_resources.employee.Emp_FName,' ',human_resources.employee.Emp_LName) as Employee_Name," +
                "human_resources.department.Dep_Name as Dep, " +
                "round(human_resources.department.Base_Salary / (40 * 52)) as Hourly_Base_Salary," +
                "(select Sum(DATEDIFF(LE_To_Date, LE_From_Date)) * 8 from human_resources.Leave where human_resources.Leave.EMP_ID = employee.Emp_ID) as Leave_Requested," +
                " human_resources.weekly_attendence.Total_Att_Hours as Work_Hours, human_resources.department.Tax_Percentage" +
                " FROM human_resources.employee, human_resources.department, " +
                "human_resources.employee_department, human_resources.weekly_attendence Where(employee.Emp_ID = employee_department.Emp_ID) " +
                "and(department.Dep_ID = employee_department.Dep_ID)and(employee.Emp_ID = human_resources.weekly_attendence.Emp_ID); ";
            try
            {
                if (datefrom.Text != "" && dateTo.Text != "")
                {
                    MySqlConnection mySqlConnection = new MySqlConnection(ConnectionString); 
                    MySqlCommand cmd = new MySqlCommand(sqlCommand, mySqlConnection); mySqlConnection.Open(); 
                    DataTable dt = new DataTable(); 
                    dt.Load(cmd.ExecuteReader()); 
                    mySqlConnection.Close();
                    dt.Columns.Add("Earning", typeof(double));
                    dt.Columns.Add("Deduction", typeof(double));
                    dt.Columns.Add("Payment", typeof(double));  
                    foreach (DataRow dr in dt.Rows)          
                    {                                       
                        if (dr["Hourly_Base_Salary"].ToString() == "")
                            Hourly_Base_Salary = "0";
                        else 
                            Hourly_Base_Salary = dr["Hourly_Base_Salary"].ToString();
                        if (dr["Leave_Requested"].ToString() == "")
                            Leave_Requested = "0";
                        else
                            Leave_Requested = dr["Leave_Requested"].ToString();
                        if (dr["Work_Hours"].ToString() == "")
                            Work_Hours = "0";
                        else
                            Work_Hours = dr["Work_Hours"].ToString();
                        if (dr["Tax_Percentage"].ToString() == "")
                            Tax_Percentage = "0";
                        else
                            Tax_Percentage = dr["Tax_Percentage"].ToString();


                        // Earning = (Leave_Requested + Work_Hours) * Hourly_Base_Salary ;
                        Earning = (Convert.ToDouble(Hourly_Base_Salary) + Convert.ToDouble(Leave_Requested)) * Convert.ToDouble(Work_Hours);
                        dr["Earning"] = Convert.ToString(Earning);
                        Deduction = (Earning * Convert.ToDouble(Tax_Percentage))/100;       // Deduction = Earning * (human_resources.employee.Tax_Percentage);
                        dr["Deduction"] = Convert.ToString(Deduction);
                        Payment = Earning - Deduction;          // Payment = Earning - Deduction
                        dr["Payment"] = Convert.ToString(Payment);


                    }
                    foreach (DataColumn td in dt.Columns)
                    {

                       dataGridReviewpay.DataSource = dt; 
                       dataGridReviewpay.Columns[0].Width = 50;// employee Name                        
                       dataGridReviewpay.Columns[1].Width = 100;// The Department                       
                       dataGridReviewpay.Columns[2].Width = 100;// Base salary                        
                       dataGridReviewpay.Columns[4].Width = 300;// Leave request = change to amount instead of days                        
                       dataGridReviewpay.Columns[5].Width = 300;// work hours                       
                       dataGridReviewpay.Columns[6].Width = 300;// Deduction                       
                       dataGridReviewpay.Columns[7].Width = 300;// Payment

                    }
                }

            }
            catch (Exception ex) 
            { 
                MessageBox.Show(ex.Message); 
            }

        }



        private void button1_Click(object sender, EventArgs e)
        {
            int ID;
            string sqlCommand = "select max(Pay_ID) from human_resources.Payroll";
            MySqlConnection mySqlConnection = new MySqlConnection(ConnectionString);
            MySqlCommand cmd = new MySqlCommand(sqlCommand, mySqlConnection);
            mySqlConnection.Open();
            int Pay_ID = (int)cmd.ExecuteScalar();
            mySqlConnection.Close();
            if (Pay_ID == null)
            {
                ID = 0;
            }
            else
            {
                ID = Pay_ID;
                ID++;
            }



            try
            {
                using (MySqlConnection connection = new MySqlConnection(ConnectionString))
                {
                    using (MySqlCommand command = new MySqlCommand())
                    {
                        command.Connection = connection;
                        command.CommandText = "INSERT INTO human_resources.Payroll (Pay_ID, Emp_ID, Att_ID, LE_ID, Issue_Date, Tax_Amount, Gross_Amount, Total_Amount) " +
                            "VALUES (?Pay_ID, ?Emp_ID, ?Att_ID, ?LE_ID, ?Issue_Date, ?Tax_Amount, ?Gross_Amount, ?Total_Amount)";

                        MySqlParameter blobPay_ID = new MySqlParameter("?Pay_ID", MySqlDbType.Int32);
                        MySqlParameter blobEmp_ID = new MySqlParameter("?Emp_ID", MySqlDbType.Int32);
                        MySqlParameter blobAtt_ID = new MySqlParameter("?Att_ID", MySqlDbType.Int32);
                        MySqlParameter blobLE_ID = new MySqlParameter("?LE_ID", MySqlDbType.Int32);
                        MySqlParameter blobLEIssueDate = new MySqlParameter("?Issue_Date", MySqlDbType.Date);
                        MySqlParameter blobTax_Amount = new MySqlParameter("?Gross_Amount", MySqlDbType.Double);
                        MySqlParameter blobGross_Amount = new MySqlParameter("?Gross_Amount", MySqlDbType.Double);
                        MySqlParameter blobTotal_Amount = new MySqlParameter("?Total_Amount", MySqlDbType.Double);
                        
                        

                        blobPay_ID.Value = ID;
                        blobEmp_ID.Value = Convert.ToInt32(Emp_ID);
                        blobAtt_ID.Value = //Attendence_View_Fill_lbl.Text;
                        blobLE_ID.Value = //Leave_View_Fill_lbl.Text;
                        blobLEIssueDate.Value = Convert.ToDateTime(Paycycle.Text);
                        blobGross_Amount.Value = //how to add the temporary collumns from the query?
                        blobTax_Amount.Value = //how to add the temporary collumns from the query?
                        blobTotal_Amount.Value = //how to add the temporary collumns from the query?





                        command.Parameters.Add(blobPay_ID);
                        command.Parameters.Add(blobEmp_ID);
                        command.Parameters.Add(blobAtt_ID);
                        command.Parameters.Add(blobLE_ID);
                        command.Parameters.Add(blobLEIssueDate);
                        command.Parameters.Add(blobGross_Amount);
                        command.Parameters.Add(blobTax_Amount);
                        command.Parameters.Add(blobTotal_Amount);
                        

                        connection.Open();

                        command.ExecuteNonQuery();
                        connection.Close();

                    }

                }
                MessageBox.Show("The paycicle is generated: " + Convert.ToString(ID), "Payroll");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ops.");
            }
        }

        //private class 
        private void View_btn_Click(object sender, EventArgs e)
        {
            
            
        }
    }
}
