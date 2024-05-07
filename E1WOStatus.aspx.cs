using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using System.Security.Principal;

namespace etlMon
{
    public partial class E1WOStatus : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Label1.Text = Properties.Settings.Default.csSGIG;
        }

        protected void btnRetrieve_Click(Object obj, EventArgs e)
        {
            try
            {
                String sSQL = "SELECT [WorkOrderNumber],[WorkOrderStatus],[WrittenDate] FROM [ShopFloor].[dbo].[E1WorkOrderStatus] WHERE [WorkOrderNumber] = ";
                String sWONum = txtWorkOrder.Text;
                String sWOStatus = "";
                String sWODate = "";
                Int32 iWONum;
                if (Int32.TryParse(sWONum, out iWONum))
                {
                    sSQL += sWONum;
                    String csSGIG = Properties.Settings.Default.csSGIG;
                    SqlConnection cnSGIG = new SqlConnection(csSGIG);
                    SqlDataReader drWONum = null;
                    SqlCommand cmdWONum = new SqlCommand(sSQL, cnSGIG);
                    cmdWONum.CommandType = CommandType.Text;
                    cnSGIG.Open();

                    drWONum = cmdWONum.ExecuteReader(CommandBehavior.CloseConnection);
                    if (drWONum.Read())
                    {
                        sWOStatus = Convert.ToString(drWONum["WorkOrderStatus"]);
                        sWODate = Convert.ToString(drWONum["WrittenDate"]);
                        txtWorkOrderStatus.Text = sWOStatus;
                        txtWorkOrderDate.Text = sWODate;
                        fnShowDiv("divCurStatus");
                    }
                    else
                    {
                        fnShowDiv("divWONotFound");
                    }
                }
                else
                {
                }
            }
            catch
            {
                lblNoPermissions.Text = "User " + User.Identity.Name + " does not have permission to perform this operation.";
                fnShowDiv("divNoPermissions");
            }
            finally
            {
            }
        }

        protected void btnReset_Click(Object obj, EventArgs e)
        {
            try
            {
                String sWONum = txtWorkOrder.Text;
                String csSGIG = Properties.Settings.Default.csSGIG;
                SqlConnection cnSGIG = new SqlConnection(csSGIG);
                String sQry = "INSERT INTO [ShopFloor].[dbo].[E1WorkOrderStatusLedger] ([WorkOrderNumber],[WorkOrderStatus],[QueueId]) VALUES (" + sWONum + ",40,-1)";
                SqlCommand cmdSGIG = new SqlCommand(sQry, cnSGIG);
                cmdSGIG.CommandType = CommandType.Text;
                cnSGIG.Open();
                cmdSGIG.ExecuteNonQuery();
                cnSGIG.Close();
                txtWorkOrderStatus.Text = "40";
                txtWorkOrderDate.Text = DateTime.Now.ToString();
                fnShowDiv("divNewStatus");
            }
            catch
            {
                lblNoPermissions.Text = "User " + User.Identity.Name + " does not have permission to perform this operation.";
                fnShowDiv("divNoPermissions");
            }
            finally
            {
            }
       }
                
        protected void fnShowDiv(String sDivType)
        {
            divWorkOrder.Visible = false;
            divRetrieve.Visible = false;
            divCurStatus.Visible = false;
            divWONotFound.Visible = false;
            divReset.Visible = false;
            divNoPermissions.Visible = false;
            switch (sDivType)
            {
                case "divWorkOrder":
                    divWorkOrder.Visible = true;
                    break;
                case "divRetrieve":
                    divRetrieve.Visible = true;
                    break;
                case "divCurStatus":
                    divWorkOrder.Visible = true;
                    divCurStatus.Visible = true;
                    divReset.Visible = true;
                    break;
                case "divWONotFound":
                    divWorkOrder.Visible = true;
                    divRetrieve.Visible = true;
                    divWONotFound.Visible = true;
                    break;
                case "divNewStatus":
                    divWorkOrder.Visible = true;
                    divCurStatus.Visible = true;
                    break;
                case "divNoPermissions":
                    divNoPermissions.Visible = true;
                    break;
                case "None":
                    //divNavButtons.Visible = true;
                    break;
            }
        }

    }
}
