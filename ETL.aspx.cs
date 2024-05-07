using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Web.SessionState;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Security.Principal;
using System.Net.Mail;
using System.Net.Mime;
using System.Text.RegularExpressions;
using etlMon.SSISExecutionServiceProxy;
using System.Drawing;

namespace etlMon
{
    public partial class ETL : System.Web.UI.Page
    {
        System.Drawing.Color cAltRowColor = ColorTranslator.FromHtml("#FCCFFCC");
        System.Drawing.Color cHeaderColor = System.Drawing.ColorTranslator.FromHtml("#FFFCCCC");
        Int16 iRowsShown = 100;
                
        protected void Page_Load(object sender, EventArgs e)
        {
            gvHistory.HeaderStyle.BackColor = cHeaderColor;
            gvHistory.AlternatingRowStyle.BackColor = cAltRowColor;
            if (!IsPostBack) /* This is the first load of this page. */
            {
                Int16 iSubscriptionID = 0;
                fnGetSubscriptions(iSubscriptionID);
                //txtFromDate.Text = Convert.ToString(System.DateTime.Today.Date.ToShortDateString());
                //txtToDate.Text = Convert.ToString(System.DateTime.Today.Date.ToShortDateString());
            }
        }

        protected void fnGetSubscriptions(int selectedSubscriptionID)
        {
            String csStaging = Properties.Settings.Default.csStaging;
            SqlConnection cnStaging = new SqlConnection(csStaging);
            SqlDataAdapter cmd = new SqlDataAdapter("[uspGetActiveSubscriptions]", cnStaging);
            DataSet ds = new DataSet();
            cmd.Fill(ds, "Subscriptions");
            selSubscription.DataSource = ds;
            selSubscription.DataTextField = "MappingTableDesc";
            selSubscription.DataValueField = "E1MappingTableID";
            selSubscription.DataBind();
            selSubscription.SelectedIndex = selectedSubscriptionID;
            cnStaging.Close();
        }

        protected Int16 fnGetPlantsBySubscription(int iSubscriptionID, int selectedPlantID)
        {
            String csStaging = Properties.Settings.Default.csStaging;
            SqlConnection cnStaging = new SqlConnection(csStaging);
            SqlDataAdapter cmd = new SqlDataAdapter("[uspGetPlantsBySubscription]", cnStaging);
            cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
            SqlParameter pSubscriptionId = cmd.SelectCommand.Parameters.Add("@SubscriptionId", SqlDbType.Int);
            pSubscriptionId.Direction = ParameterDirection.Input;
            pSubscriptionId.Value = iSubscriptionID;
            SqlParameter pRowCount = cmd.SelectCommand.Parameters.Add("@RowCount", SqlDbType.Int);
            pRowCount.Direction = ParameterDirection.Output;
            DataSet ds = new DataSet();
            cmd.Fill(ds, "Plants");
            selPlant.DataSource = ds;
            selPlant.DataTextField = "Plant";
            selPlant.DataValueField = "ID";
            selPlant.DataBind();
            selPlant.SelectedIndex = selectedPlantID;
            cnStaging.Close();
            Int16 iRowCount = Convert.ToInt16(pRowCount.Value);
            iRowCount -= 1;  /* Subtract one for the 'Select Plant' row. */
            return iRowCount;
        }

        protected void fnSubscriptionChanged(Object sender, EventArgs e)
        {
            divGridHistory.Visible = false;
            txtDataSource.Text = "";
            Int16 iSubscriptionID = Convert.ToInt16(selSubscription.SelectedValue);
            if (iSubscriptionID == 0)
            {
                fnShowDiv("None");
                divSelPlant.Visible = false;
            }
            else
            {
                selSubscription.BackColor = System.Drawing.Color.White;
                fnGetSelections(iSubscriptionID);
                Int16 iPlantsFound = fnGetPlantsBySubscription(iSubscriptionID, 0);
                if ( iPlantsFound == 1 )
                {
                    selPlant.SelectedIndex = 1;
                    //fnPlantChanged(sender, e);
                }
                //fnPlantChanged(sender, e);
            }
        }

        //protected void fnPlantChanged(Object sender, EventArgs e)
        //{
        //    Int16 iPlantID = Convert.ToInt16(selPlant.SelectedValue);
        //    selPlant.BackColor = System.Drawing.Color.White;
        //}

        protected void fnGetSelections(Int16 iSubscriptionID)
        {
            fnShowDiv("None");
            fnShowDiv("Plant");
            fnShowDiv("Dates");
            switch (iSubscriptionID)
            {
                case 30: /* RawTags */
                    fnShowDiv("SerialNumber");
                    fnShowDiv("Item");
                    fnShowDiv("RackID");
                    break;
                case 33: /* CG HVQue */
                case 34: /* MTO IncomingOrders */
                    fnShowDiv("Schedule");
                    break;
                case 36: /* FG IncomingOrders */
                    fnShowDiv("Schedule");
                    break;
                case 56: /* FG FinishedGoods */
                    fnShowDiv("Schedule");
                    break;
            }
            fnShowDiv("Go");
        }

        protected void fnShowDiv(String sDivType)
        {
            switch (sDivType)
            {
                case "None":
                    divSelRackID.Visible = false;
                    divSelItem.Visible = false;
                    divSelSerialNumber.Visible = false;
                    divSelSchedule.Visible = false;
                    divSelDates.Visible = false;
                    divSelPlant.Visible = false;
                    divGo.Visible = false;
                    break;
                case "WorkOrder":
                    divSelWorkOrder.Visible = true;
                    break;
                case "SerialNumber":
                    divSelSerialNumber.Visible = true;
                    break;
                case "Item":
                    divSelItem.Visible = true;
                    break;
                case "RackID":
                    divSelRackID.Visible = true;
                    break;
                case "Schedule":
                    divSelSchedule.Visible = true;
                    break;
                case "Dates":
                    divSelDates.Visible = true;
                    break;
                case "Plant":
                    divSelPlant.Visible = true;
                    break;
                case "Go":
                    divGo.Visible = true;
                    break;
            }
        }
        
        protected void btnGetHistory_Click(Object obj, EventArgs e)
        {
            Int16 iSubscriptionID = Convert.ToInt16(selSubscription.SelectedValue);
            Int16 iPlantID = Convert.ToInt16(selPlant.SelectedValue);
            String sFromDate = Convert.ToString(txtFromDate.Text).Trim();
            String sToDate = Convert.ToString(txtToDate.Text).Trim();
            String sSerialNumber = Convert.ToString(txtSerialNumber.Text).Trim();
            String sSchedule = Convert.ToString(txtSchedule.Text).Trim();
            String sWorkOrder = Convert.ToString(txtWorkOrder.Text).Trim();
            if (fnValidate(iSubscriptionID, iPlantID, sFromDate, sToDate, sSerialNumber, sSchedule, sWorkOrder)) 
            {
                fnGetHistory(iSubscriptionID, iPlantID, sFromDate, sToDate, sSerialNumber, sSchedule, sWorkOrder);
                divGridHistory.Visible=true;
                //divGo.Visible = false;
            }
        }

        protected Boolean fnValidate(Int16 iSubscriptionID, Int16 iPlantID, String sFromDate, String sToDate, String sSerialNumber, String sSchedule, String sWorkOrder)
        {
            Boolean bIsValid = false;
            DateTime dFromDate = System.DateTime.Today;
            DateTime dToDate = System.DateTime.Today.AddDays(1);
            lblDatesError.Text = "";
            divDatesError.Visible = false;
            txtFromDate.BackColor = System.Drawing.Color.White;
            txtToDate.BackColor = System.Drawing.Color.White;
            txtSerialNumber.BackColor = System.Drawing.Color.White;
            txtSchedule.BackColor = System.Drawing.Color.White;
            txtWorkOrder.BackColor = System.Drawing.Color.White;
            if (sFromDate.Length != 0)
            {
                dFromDate = Convert.ToDateTime(sFromDate);
            }
            if (sToDate.Length != 0)
            {
                dToDate = Convert.ToDateTime(sToDate);
            }
            if (dFromDate > dToDate)
            {
                lblDatesError.Text = "From date must be before to date.";
                lblDatesError.Visible = true;
                txtFromDate.BackColor = System.Drawing.Color.Pink;
                return false;
            }
            bIsValid = true;
            return bIsValid;
        }

        protected void fnGetHistory(Int16 iSubscriptionID, Int16 iPlantID, String sFromDate, String sToDate, String sSerialNumber, String sSchedule, String sWorkOrder)
        {
            //WebMsgBox.Show(sPackageName); 
            String sSQLCount = "";
            String sSQL = "";
            String sSelectCount = "SELECT COUNT(*)";
            String sSelect = "SELECT TOP " + Convert.ToString(iRowsShown) + " *";
            String sFrom = "";
            String sWhere = "";
            String sOrderBy = "";
            switch (iSubscriptionID)
            {
                case 30: /* RawTags */
                    sFrom = "FROM [vw_F580003A_Archive_History]";
                    if (sSerialNumber.Length > 0)
                    {
                        sWhere = "WHERE [SerialNum] = '" + sSerialNumber + "'";
                    }
                    sOrderBy = "ORDER BY [Plant], [ItemNum], [Seq]";
                    break;
                case 33: /* CG HVQue */
                    sFrom = "FROM vw_HVQue";
                    if (sSchedule.Length > 0)
                    {
                        sWhere = "WHERE [S7Z58SCN] = '" + sSchedule + "'";
                    }
                    sOrderBy = "ORDER BY [S7MCU], [S7Z58SCSN]";
                    break;
                case 34: /* MTO IncomingOrders */
                    sFrom = "FROM [vw_IncomingOrder]";
                    if (sSchedule.Length > 0)
                    {
                        sWhere = "WHERE [S7Z58SCN] = '" + sSchedule + "'";
                    }
                    sOrderBy = "ORDER BY [S7MCU], [S7Z58SCSN]";
                    txtDataSource.Text = "vw_IncomingOrder";
                    break;
                case 36: /* FG IncomingOrders */
                    sFrom = "FROM [vw_FGIncomingOrders]";
                    if (sSchedule.Length > 0)
                    {
                        sWhere = "WHERE [S7Z58SCN] = '" + sSchedule + "'";
                    }
                    sOrderBy = "ORDER BY [S7MCU], [S7Z58SCSN]";
                    break;
                case 56: /* FG FinishedGoods */
                    sFrom = "FROM [tblFinishedGoods]";
                    if (sSchedule.Length > 0)
                    {
                        sWhere = "WHERE [schedule] = '" + sSchedule + "'";
                    }
                    sOrderBy = "ORDER BY [plant], [ticket]";
                    break;
            }
            sSQLCount = sSelectCount + " " + sFrom + " " + sWhere;
            sSQL = sSelect + " " + sFrom + " " + sWhere + " " + sOrderBy;
            txtDataSource.Text = sSQL;
            String csStaging = Properties.Settings.Default.csStaging;
            SqlConnection cnStaging = new SqlConnection(csStaging);
            SqlCommand cmdCount = new SqlCommand(sSQLCount, cnStaging);
            SqlCommand cmdHistory = new SqlCommand(sSQL, cnStaging);
            SqlDataReader drHistory = null;
            cnStaging.Open();
            Int16 iRowCount = Convert.ToInt16(cmdCount.ExecuteScalar());
            drHistory = cmdHistory.ExecuteReader();
            if (drHistory.HasRows)
            {
                lblJobResults.Text = Convert.ToString(iRowCount) + " rows found.";
                if (iRowCount > iRowsShown)
                {
                    lblJobResults.Text += " " + Convert.ToString(iRowsShown) + " rows shown.";
                }
            }
            else
            {
                lblJobResults.Text = "No data found.";
                lblJobResults.CssClass = "disabled";
            }
            gvHistory.DataSource = drHistory;
            gvHistory.DataBind();
            drHistory.Close();
            cnStaging.Close();
        }

    }
}
