using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OracleClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Cpanel.Models;
 
using System.Text;
using System.Security.Cryptography;
using System.IO;
using Cpanel.Context;

namespace Cpanel.Context
{
    public class DataSource
    {
        //ConnectionString....
        private string conString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

        //-----------------------GET chq------------------------------------------------------
        //
        public int updatechqsts(int id, string sts)
        {
            using (OracleConnection con = new OracleConnection(conString))
            {
                string query = "update  cheque_reqs set req_status='" + sts + "'  where request_id=" + id;

                OracleCommand cmd = new OracleCommand(query, con);

                con.Open();
                int result = -1;
                result = cmd.ExecuteNonQuery();



                return result;

            }
        }

        public int insert(userInsertModel model)
        {

            using (OracleConnection con = new OracleConnection(conString))
            {
                OracleCommand cmd = new OracleCommand("INSERT INTO security_master (USER_LOG,USER_PASS,USER_NAME,USER_LASTLOGIN,USER_LASTWORK,USER_ID,ROLEID,USER_BRANCH,USER_PAS,USER_STAT)VALUES('" + model.user_name + "','R6K2zyfxJqbwmXqixfkRMw==','" + model.name + "','T',NULL,CP_USERID.nextval,'" + model.roleid + "','" + model.BranchCode + "',NULL,'A')", con);
                if (con.State == ConnectionState.Closed)
                { con.Open(); }

                return cmd.ExecuteNonQuery();
            }
        }
        public int Update(userUpdateModel model)
        {

            using (OracleConnection con = new OracleConnection(conString))
            {
                OracleCommand cmd = new OracleCommand("Update security_master set user_log ='" + model.user_name + "',user_name ='" + model.name + "',roleid='" + model.roleid + "',user_branch='" + model.BranchCode + "' where  user_id='" + model.user_id + "'", con);
                if (con.State == ConnectionState.Closed)
                { con.Open(); }

                return cmd.ExecuteNonQuery();
            }
        }
        public userUpdateModel getuserdata(int id)
        {
            userUpdateModel updatemodel =new  userUpdateModel();

            using (OracleConnection con = new OracleConnection(conString))
            {
                OracleCommand cmd = new OracleCommand("select  user_id, user_log, user_name,roleid,user_branch  from security_master where user_id=" + id, con);
                con.Open();
                OracleDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {

                    updatemodel.user_id = Convert.ToInt32(dr["user_id"].ToString());
                    updatemodel.roleid = dr["roleid"].ToString();
                    updatemodel.BranchCode = dr["user_branch"].ToString();
                    updatemodel.user_name = dr["user_log"].ToString();
                    updatemodel.name = dr["user_name"].ToString();
                }
                else
                {
                    dr.Close();
                }
                dr.Close();
                con.Close();
            }
            return updatemodel;
        }

        //-----------------Get User with Branch, Category and Status
        public List<Custreport> GetBranchUsers(string branch, string category, string status)
        {
            String sqlbranch = "", sqlstatus="", sqlcategory = "";
            List<Custreport> users = new List<Custreport>();
            using (OracleConnection con = new OracleConnection(conString))
            {
                if (branch != "0")
                    sqlbranch = "  and   substr(account,3,3)='" + branch + "'";
                
                 if (category != "0")
                    sqlcategory = "  and catogry = '" + category +  "'";

                 if (status != "0")
                     sqlstatus = "  and user_status = '" + status + "'";
                string query = "select user_name,b.branch_name||'-'||t.act_name||'-'||c.curr_name||'-'||SUBSTR(def_acc,14),status_name from  users,branchs b ,act_types t , currency c,customerstatus where      SUBSTR(def_acc,3,3)=b.branch_code and   SUBSTR(def_acc,6,5)=t.act_type_code and SUBSTR(def_acc,11,3)=c.CURR_CODE  and user_status=status_code   "+
            " " + sqlbranch + sqlcategory + sqlstatus;

                OracleCommand cmd = new OracleCommand(query, con);
                if (con.State == ConnectionState.Closed)
                { con.Open(); }


                OracleDataReader dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {

                        users.Add(new Custreport
                        {
                            CustomerName = dr[0].ToString(),
                            AccountNumber = dr[1].ToString(),
                            //user_id = Convert.ToInt32(dr[1].ToString()),
                            CustStatus = dr[2].ToString(),


                        });
                    }
                }


            }
            return users;
        }
      
        
        /// <summary>
        //__________________________________Get Customers + Branch____________________________
        public List<CustomerBranshReportModel> GetUsersInBranch(string branch, string category, string status, string FromDate, string ToDate)
        {
            String sqlbranch = "", sqlstatus = "", sqlcategory = "", sqlfromdate = "", sqltodate = "";
            List<CustomerBranshReportModel> users = new List<CustomerBranshReportModel>();
            using (OracleConnection con = new OracleConnection(conString))
            {
                if (branch != "0")
                    sqlbranch = "  and   substr(account,3,3)='" + branch + "'";

                if (category != "0")
                    sqlcategory = "  and catogry = '" + category + "'";

                if (status != "0")
                    sqlstatus = "  and user_status = '" + status + "'";

                if (FromDate != "")
                    sqlfromdate = "  and reg_date >= TO_DATE('" + FromDate + "')";

                if (ToDate != "")
                    sqltodate = "  and reg_date <= TO_DATE('" + ToDate + "')";

                string query = "select user_name,b.branch_name||'-'||t.act_name||'-'||c.curr_name||'-'||SUBSTR(def_acc,14),status_name, REG_DATE from  users,branchs b ,act_types t , currency c,customerstatus where      SUBSTR(def_acc,3,3)=b.branch_code and   SUBSTR(def_acc,6,5)=t.act_type_code and SUBSTR(def_acc,11,3)=c.CURR_CODE  and user_status=status_code  " +
            " " + sqlbranch + sqlcategory + sqlstatus + sqlfromdate + sqltodate;

                OracleCommand cmd = new OracleCommand(query, con);
                if (con.State == ConnectionState.Closed)
                { con.Open(); }


                OracleDataReader dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {

                        users.Add(new CustomerBranshReportModel
                        {
                            CustomerName = dr[0].ToString(),
                            AccountNumber = dr[1].ToString(),
                            //user_id = Convert.ToInt32(dr[1].ToString()),
                            CustStatus = dr[2].ToString(),
                            RegDate = dr[3].ToString(),



                        });
                    }
                }


            }
            return users;
        }
        
        
        /// </summary>
        /// <returns></returns>

        public List<userlist> GetAllusers()
        {
            List<userlist> users=new List<userlist>();
            using (OracleConnection con = new OracleConnection(conString))
            {
                OracleCommand cmd = new OracleCommand("SELECT user_name, user_id,r.name,b.branch_name   FROM security_master u , branchs b ,cpanel_rolemaster r where u.roleid=r.roleid and u.user_branch=b.branch_code ", con);
                if (con.State == ConnectionState.Closed)
                { con.Open(); }
                

             OracleDataReader   dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        

                        users.Add(new userlist
                        { name=dr[0].ToString(),
                        user_id=Convert.ToInt32(  dr[1].ToString()),
                        user_branch=dr[3].ToString(),
                        rolename=dr[2].ToString()

                        });
                    }
                }


            }
            return users;
        }

        public int resetpassworduser(int user_id)
        {
            using (OracleConnection con = new OracleConnection(conString))
            {
                OracleCommand cmd = new OracleCommand("Update security_master set  USER_LASTLOGIN='F',USER_PAS='admin123',USER_STAT ='A', USER_PASS='R6K2zyfxJqbwmXqixfkRMw==' where  user_id='" + user_id + "'", con);
                if (con.State == ConnectionState.Closed)
                { con.Open(); }

                return cmd.ExecuteNonQuery();
            }
        }
        public int deleteuser(int user_id)
        {
            using (OracleConnection con = new OracleConnection(conString))
            {
                OracleCommand cmd = new OracleCommand("Update security_master set USER_STAT ='D' where  user_id='" + user_id + "'", con);
                if (con.State == ConnectionState.Closed)
                { con.Open(); }

                return cmd.ExecuteNonQuery();
            }
        }
        public List<ChqRequest> Chqrequest(String bracode)
        {
             OracleCommand cmd;
            OracleDataReader dr;

            int requestid;
            String name, act,date, booksize,reqdate;
            String query1, result;
            List<ChqRequest> customer = new List<ChqRequest>();
            if (!bracode.Equals("000"))
            {
                query1 = "select c.request_id,branch_name||'-'||curr_name||'-'||act_name||'-'|| SUBSTR(c.account_no,14) account_no,c.requested_size,c.req_date,u.user_name from cheque_reqs c,users u,branchs, currency,act_types where req_status='process' and u.user_id=c.user_id and   SUBSTR(c.account_no,3,3)='" + bracode + "' and branchs.branch_code=SUBSTR(c.account_no,3,3) and act_types.act_type_code=SUBSTR(c.account_no,6,5)and currency.curr_code=SUBSTR(c.account_no,11,3) order by c.request_id";
            }
            else
            {
                query1 = "select c.request_id,branch_name||'-'||curr_name||'-'||act_name||'-'|| SUBSTR(c.account_no,14) account_no,c.requested_size,c.req_date,u.user_name from cheque_reqs c,users u,branchs, currency,act_types where req_status='process' and u.user_id=c.user_id  and branchs.branch_code=SUBSTR(c.account_no,3,3) and act_types.act_type_code=SUBSTR(c.account_no,6,5)and currency.curr_code=SUBSTR(c.account_no,11,3) order by c.request_id";
            }

            using (OracleConnection con = new OracleConnection(conString))
            {
                cmd = new OracleCommand(query1, con);

                con.Open();

                dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        requestid = Convert.ToInt32(dr[0].ToString());
                       
                        act = dr[1].ToString();
                        booksize = dr[2].ToString();
                        date = dr[3].ToString();

                        name = dr[4].ToString();
                      

                        customer.Add(new ChqRequest
                        {
                            request_id = requestid,
                            accountmap = act,
                            booksize = booksize,
                            name = name,
                            date=date
                        });
                    }
                }


            }


            return customer;
        
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="user_id"></param>
        /// <returns></returns>
        public String getaccount(string user_id)
        {
            String Accounts = "";
            using (OracleConnection con = new OracleConnection(conString))
            {
                string query = " SELECT acc_id,acc_no from user_acc_link where  user_id =" + user_id;

                OracleCommand cmd = new OracleCommand(query, con);

                con.Open();


                using (IDataReader dataReader = cmd.ExecuteReader())
                {
                    while (dataReader.Read())
                    {

                        if (dataReader["acc_id"] != DBNull.Value)
                        {

                            if (dataReader["acc_no"] != DBNull.Value)
                            {
                                Accounts = Accounts + "-" + (string)dataReader["acc_no"];
                                //Accounts = Accounts.Substring(2);
                            }

                        }
                    }
                    Accounts = Accounts.Substring(1);
                    return Accounts;

                }

            }

        }
        //---------------------------------------------------------get act --------------------------------//
        public String getspfaccount(string user_id, string act)
        {
            String Accounts = "";
            using (OracleConnection con = new OracleConnection(conString))
            {
                string query = " SELECT acc_id,acc_no from user_acc_link where user_id =" + user_id + " and substr(acc_no,14)=" + act;

                OracleCommand cmd = new OracleCommand(query, con);

                con.Open();


                using (IDataReader dataReader = cmd.ExecuteReader())
                {
                    while (dataReader.Read())
                    {

                        if (dataReader["acc_id"] != DBNull.Value)
                        {

                            if (dataReader["acc_no"] != DBNull.Value)
                            {
                                Accounts = (string)dataReader["acc_no"];
                                //Accounts = Accounts.Substring(2);
                            }

                        }
                    }

                    return Accounts;

                }

            }

        }

        //-----------------------DropDownGET Branchs------------------------------------------------------
        //
        public List<CustomerRegBankinfo> GetBranchs()
        {
            string branchs = "";
            using (OracleConnection con = new OracleConnection(conString))
            {
                string query = " select branch_code,branch_name from branchs where branch_sts = '1'";

                OracleCommand cmd = new OracleCommand(query, con);

                con.Open();

                List<CustomerRegBankinfo> list = new List<CustomerRegBankinfo>();
                using (IDataReader dataReader = cmd.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        CustomerRegBankinfo obj = new CustomerRegBankinfo();

                        if (dataReader["branch_code"] != DBNull.Value)
                        {
                            obj.BranchCode = (string)dataReader["branch_code"];

                            if (dataReader["branch_name"] != DBNull.Value)
                            {
                                obj.Branch = (string)dataReader["branch_name"];

                            }

                            list.Add(obj);

                        }
                    }
                    //branchs = branchs.Substring(1);
                    return list;
                }
            }
        }

        //----------------------DropDownGet Account Type---------------------------------
        public List<CustomerRegBankinfo> GetAccountType()
        {
            string branchs = "";
            using (OracleConnection con = new OracleConnection(conString))
            {
                string query = "select act_type_code,act_name from Act_types";

                OracleCommand cmd = new OracleCommand(query, con);

                con.Open();

                List<CustomerRegBankinfo> list = new List<CustomerRegBankinfo>();
                using (IDataReader dataReader = cmd.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        CustomerRegBankinfo obj = new CustomerRegBankinfo();

                        if (dataReader["act_type_code"] != DBNull.Value)
                        {
                            obj.AccountTypecode = (string)dataReader["act_type_code"];

                            if (dataReader["act_name"] != DBNull.Value)
                            {
                                obj.AccountType = (string)dataReader["act_name"];

                            }

                            list.Add(obj);

                        }
                    }
                    //branchs = branchs.Substring(1);
                    return list;
                }
            }
        }

        //------------------DropDown Get Currency---------------------------
        public List<CustomerRegBankinfo> GetCurrency()
        {
            string branchs = "";
            using (OracleConnection con = new OracleConnection(conString))
            {
                string query = "select curr_code,curr_name from currency";

                OracleCommand cmd = new OracleCommand(query, con);

                con.Open();

                List<CustomerRegBankinfo> list = new List<CustomerRegBankinfo>();
                using (IDataReader dataReader = cmd.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        CustomerRegBankinfo obj = new CustomerRegBankinfo();

                        if (dataReader["curr_code"] != DBNull.Value)
                        {
                            obj.CurrencyCode = (string)dataReader["curr_code"];

                            if (dataReader["curr_name"] != DBNull.Value)
                            {
                                obj.Currency = (string)dataReader["curr_name"];

                            }

                            list.Add(obj);

                        }
                    }
                    //branchs = branchs.Substring(1);
                    return list;
                }
            }
        }


        //-----------------------GET AccountTypes------------------------------------------------------
        //
        public String getaccounttype(string acctype)
        {
            String acctypename = "NULL";
            using (OracleConnection con = new OracleConnection(conString))
            {
                string query = " select act_name from act_types where act_type_code=" + acctype;
                string query2 = "select act_name from invact_types where act_type_code=" + acctype;



                OracleCommand cmd = new OracleCommand(query, con);
                OracleCommand cmd2 = new OracleCommand(query2, con);
                con.Open();


                OracleDataReader dataReader = cmd.ExecuteReader();

                if (dataReader.HasRows)
                {
                    while (dataReader.Read())
                    {
                        if (dataReader["act_name"] != DBNull.Value)
                        {
                            acctypename = (string)dataReader["act_name"];
                        }

                    }
                }
                else
                {
                    dataReader = cmd2.ExecuteReader();
                    if (dataReader.HasRows)
                    {
                        while (dataReader.Read())
                        {
                            if (dataReader["act_name"] != DBNull.Value)
                            {
                                acctypename = (string)dataReader["act_name"];
                            }

                        }
                    }
                    else
                    { acctypename = "Account Type Not Found"; }
                }



                return acctypename;

            }

        }


        //-----------------------GET BRANCH NAME English------------------------------------------------------
        //
        public String getbranchnameenglish(string brcode)
        {
            String brname = "NULL";
            using (OracleConnection con = new OracleConnection(conString))
            {
                string query = " select  branch_name from branchs where branch_sts='1' and branch_code=" + brcode;

                OracleCommand cmd = new OracleCommand(query, con);

                con.Open();


                using (IDataReader dataReader = cmd.ExecuteReader())
                {
                    while (dataReader.Read())
                    {



                        if (dataReader["branch_name"] != DBNull.Value)
                        {
                            brname = (string)dataReader["branch_name"];
                        }

                    }
                }

                return brname;

            }

        }

        public String getcurrencyname(string currcode)
        {
            String curr_name = "NULL";
            using (OracleConnection con = new OracleConnection(conString))
            {
                string query = " select  curr_name from currency where curr_code=" + currcode;

                OracleCommand cmd = new OracleCommand(query, con);

                con.Open();


                using (IDataReader dataReader = cmd.ExecuteReader())
                {
                    while (dataReader.Read())
                    {



                        if (dataReader["curr_name"] != DBNull.Value)
                        {
                            curr_name = (string)dataReader["curr_name"];
                        }

                    }
                }

                return curr_name;

            }

        }



        //-------------------------------------DropClient for ChequeStatus Controller DropDownList--------------------------------
        //
        public List<CustomerRegBankinfo> DropClient(string user_id)
        {
            using (OracleConnection con = new OracleConnection(conString))
            {
                string query = " SELECT acc_id,acc_no from user_acc_link where user_id =" + user_id;

                OracleCommand cmd = new OracleCommand(query, con);

                con.Open();

                List<CustomerRegBankinfo> list = new List<CustomerRegBankinfo>();
                using (IDataReader dataReader = cmd.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        CustomerRegBankinfo obj = new CustomerRegBankinfo();
                        if (dataReader["acc_id"] != DBNull.Value)
                        {
                            if (dataReader["acc_id"] != DBNull.Value)
                            {
                                //obj.AccountID = (int)dataReader["acc_id"];
                                obj.CustomerID = dataReader["acc_id"].ToString();
                            }
                            if (dataReader["acc_no"] != DBNull.Value)
                            {
                                obj.AccountNumber = (string)dataReader["acc_no"];
                            }
                            list.Add(obj);
                        }
                    }

                    return list;

                }

            }

        }


        public List<CustomerRegBankinfo> checkaccount(string act)
        {
            using (OracleConnection con = new OracleConnection(conString))
            {
                string query = "select user_id,user_name from users where DEF_ACC='" + act + "'";

                OracleCommand cmd = new OracleCommand(query, con);

                con.Open();

                List<CustomerRegBankinfo> list = new List<CustomerRegBankinfo>();
                using (IDataReader dataReader = cmd.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        CustomerRegBankinfo obj = new CustomerRegBankinfo();
                        if (dataReader["user_id"] != DBNull.Value)
                        {
                            if (dataReader["user_id"] != DBNull.Value)
                            {
                                //obj.AccountID = (int)dataReader["acc_id"];
                                obj.CustomerID = dataReader["user_id"].ToString();
                            }
                            if (dataReader["user_name"] != DBNull.Value)
                            {
                                obj.CustomerName = (string)dataReader["user_name"];
                            }
                            list.Add(obj);
                        }
                    }

                    return list;

                }

            }

        }


        //---------------------test pr--------------------------------------------------------------//
        /// <summary>
        /// 
        /// </summary>
        /// <param name="user_id"></param>
        /// <param name="FILE_NAME"></param>
        /// <returns></returns>
        public int insertfilesalary(string user_id, string FILE_NAME)
        {
            using (OracleConnection con = new OracleConnection(conString))
            {
                string query = "INSERT INTO salary_files (FILE_ID,FILE_NAME,NO_OF_ROWS,STATUS,NO_OF_PROCESS_ROWS,FILE_DATE,USER_ID,FILE_TOTAL) " +
                               "VALUES(salaryfile.nextval,'" + FILE_NAME + "','0','P','0',sysdate," + user_id + ",'0')";

                OracleCommand cmd = new OracleCommand(query, con);

                con.Open();
                int result = -1;
                result = cmd.ExecuteNonQuery();



                return result;

            }



        }

        //----------------------- INSERT FiLE SALARY ITEMS----------------------------------------------
        //
        public int insertfilesalaryitems(string user_id, string FILE_NAME, string acc, string amount, string acccomp)
        {
            using (OracleConnection con = new OracleConnection(conString))
            {
                string query = "INSERT INTO salary_temp (SALARY_ID,SALARY_USER_ID,SALARY_ACCOUNT_NO,SALARY_AMOUNT,SALARY_STATUS,SALARY_FILE_NAME,SALARY_COMP_ACT,SALARY_PROCESS_DATE,SALARY_REQ_DATE)" +
                               "VALUES(salarytemp.nextval," + user_id + ",'" + acc + "','" + amount + "','P','" + FILE_NAME + "','" + acccomp + "',sysdate,sysdate)";

                OracleCommand cmd = new OracleCommand(query, con);

                con.Open();
                int result = -1;
                result = cmd.ExecuteNonQuery();



                return result;

            }



        }

        //----------------------- update FiLE SALARY ITEMS----------------------------------------------
        //
        public int updatesalaryitems(string user_id, string FILE_NAME, string acc, string sts)
        {
            using (OracleConnection con = new OracleConnection(conString))
            {
                string query = "update salary_temp  set SALARY_STATUS ='" + sts + "', SALARY_PROCESS_DATE=sysdate where SALARY_USER_ID=" + user_id + " and SALARY_ACCOUNT_NO='" + acc + "' and SALARY_FILE_NAME='" + FILE_NAME + "'";

                OracleCommand cmd = new OracleCommand(query, con);

                con.Open();
                int result = -1;
                result = cmd.ExecuteNonQuery();



                return result;

            }



        }


        /// updates file sallary
        /// items in a table
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="fileName"></param>
        /// <param name="countrow"></param>
        /// <param name="totalamount"></param>
        /// <param name="modelAccountNumber"></param>
        /// <returns></returns>
        /// //-----------------------------------updatefilesalaryitems---------------------
        public int updatefilesalaryitems(string userId, string fileName, int countrow, double totalamount, string modelAccountNumber)
        {
            using (OracleConnection con = new OracleConnection(conString))
            {
                string query = "update  salary_files  set  NO_OF_ROWS=" + countrow + ",STATUS='RWS',FILE_TOTAL=" + totalamount +
                               " where FILE_NAME='" + fileName + "' and user_id=" + userId;

                OracleCommand cmd = new OracleCommand(query, con);

                con.Open();
                int result = -1;
                result = cmd.ExecuteNonQuery();



                return result;

            }
        }


        //-----------------------------------InsertTranLog---------------------
        /// <summary>
        /// Insert into Log 
        /// all the info about each transaction
        /// </summary>
        /// <param name="user_id"></param>
        /// <param name="tranName"></param>
        /// <param name="req"></param>
        /// <param name="resp"></param>
        /// <param name="status"></param>
        /// <param name="respResult"></param>
        /// <returns></returns>
        public int InsertTranLog(string user_id, string tranName, string req, string resp, string status, string respResult)
        {
            using (OracleConnection con = new OracleConnection(conString))
            {
                string query = "INSERT INTO trans_log (TRAN_ID,TRAN_REQ,TRAN_RESP,TRAN_REQ_DATE,TRAN_RESP_DATE,TRAN_STATUS,TRAN_RESP_RESULT,USER_ID,TRAN_NAME)" +
                               "VALUES(tranlog.nextval,'" + req + "','" + resp + "',sysdate,sysdate,'" + status + "','" + respResult + "','" + user_id + "','" + tranName + "')";
                //string query = "INSERT INTO trans_log (TRAN_ID,TRAN_REQ,TRAN_RESP,TRAN_REQ_DATE,TRAN_RESP_DATE)" +
                //               "VALUES(tranlog.nextval,'" + req + "','"+ resp +"',sysdate,sysdate,sysdate)";

                OracleCommand cmd = new OracleCommand(query, con);

                con.Open();
                int result = -1;

                result = cmd.ExecuteNonQuery();



                return result;

            }
        }


        public int InsertChequeReq(string user_id, string accountNo, string size)
        {
            using (OracleConnection con = new OracleConnection(conString))
            {
                string query = "INSERT INTO cheque_reqs (REQUEST_ID,ACCOUNT_NO,USER_ID,REQUESTED_SIZE,REQ_DATE,REQ_STATUS,REQ_REASON) " +
                               "VALUES(cheque_req_seq.nextval,'" + accountNo + "','" + user_id + "','" + size + "',sysdate,'process', '')";


                OracleCommand cmd = new OracleCommand(query, con);

                con.Open();
                int result = -1;

                result = cmd.ExecuteNonQuery();



                return result;

            }
        }



        public String custregcheck(String branchcode, String acttype, String acc_no, String acc_curr , String category )
        {
            Boolean FLAG;
            String lblconfirm;
            OracleCommand cmd;
            OracleDataReader dr;
            int counter;

            String query1 = "select count(*) from users  where DEF_ACC='99" + branchcode + acttype + acc_curr + acc_no + "'   and catogry ='" + category + "' and user_status not in('R','DE')";

            String query2 = "select count(*) from user_acc_link where acc_no='99" + branchcode + acttype + acc_curr + acc_no + "' and catogry ='" + category + "' and ACC_STATUS not in('R','DE')";
            using (OracleConnection con = new OracleConnection(conString))
            {
                cmd = new OracleCommand(query1, con);

                con.Open();

                dr = cmd.ExecuteReader();
                dr.Read();

                counter = Convert.ToInt32(dr[0].ToString());
                dr.Close();
                con.Close();
                if ((counter == 0))
                {
                    cmd = new OracleCommand(query2, con);

                    con.Open();

                    dr = cmd.ExecuteReader();
                    dr.Read();

                    counter = Convert.ToInt32(dr[0].ToString());
                    dr.Close();
                    con.Close();
                    if ((counter != 0))
                    {
                        lblconfirm = "This Account is linked with another user";

                        return lblconfirm;
                    }
                    else
                    {
                        lblconfirm = "This Account is available";
                    }
                }
                else
                {
                    lblconfirm = "This Account is Already exist";
                }

            }
            return lblconfirm;
        }


        public String custregcheck2(String Account,String category)
        {
            Boolean FLAG;
            String lblconfirm;
            OracleCommand cmd;
            OracleDataReader dr;
            int counter;
            String query1 = "select count(*) from users  where DEF_ACC= '" + Account + "' and catogry='" + category + "' and user_status  in('A')";

            using (OracleConnection con = new OracleConnection(conString))
            {
                cmd = new OracleCommand(query1, con);

                con.Open();

                dr = cmd.ExecuteReader();
                dr.Read();

                counter = Convert.ToInt32(dr[0].ToString());
                dr.Close();
                con.Close();
                if ((counter == 0))
                {

                    lblconfirm = "This Account is available";

                }
                else
                {
                    lblconfirm = "This Account is Already exist";
                }

            }
            return lblconfirm;
        }


        ////////////populate List//////////////////////////////////////////////////////

        public List<SelectListItem> PopulateBranchs()
        {
            List<SelectListItem> items = new List<SelectListItem>();
            //string constr = ConfigurationManager.ConnectionStrings["Constring"].ConnectionString;
            using (OracleConnection con = new OracleConnection(conString))
            {
                string query = " select branch_code,branch_name from branchs where branch_sts = '1'";
                using (OracleCommand cmd = new OracleCommand(query))
                {
                    cmd.Connection = con;
                    con.Open();
                    using (OracleDataReader sdr = cmd.ExecuteReader())
                    {
                        if (sdr.HasRows)
                        {

                            items.Add(new SelectListItem
                            {
                                Text = "-- Select Branch --",
                                Value = "0",
                            });
                            while (sdr.Read())
                            {
                                items.Add(new SelectListItem
                                {
                                    Text = sdr["branch_name"].ToString(),
                                    Value = sdr["branch_code"].ToString()
                                });
                            }
                        }
                    }
                    con.Close();
                }
            }

            return items;
        }


        public List<SelectListItem> PopulateCurrencies()
        {
            List<SelectListItem> items = new List<SelectListItem>();
            //string constr = ConfigurationManager.ConnectionStrings["Constring"].ConnectionString;
            using (OracleConnection con = new OracleConnection(conString))
            {
                string query = "select curr_code,curr_name from currency";
                using (OracleCommand cmd = new OracleCommand(query))
                {
                    cmd.Connection = con;
                    con.Open();
                    using (OracleDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            items.Add(new SelectListItem
                            {
                                Text = sdr["curr_name"].ToString(),
                                Value = sdr["curr_code"].ToString()
                            });
                        }
                    }
                    con.Close();
                }
            }

            return items;
        }

        public List<SelectListItem> PopulateAccountTypes()
        {
            List<SelectListItem> items = new List<SelectListItem>();
            //string constr = ConfigurationManager.ConnectionStrings["Constring"].ConnectionString;
            using (OracleConnection con = new OracleConnection(conString))
            {
                string query = "select act_type_code,act_name from Act_types";
                using (OracleCommand cmd = new OracleCommand(query))
                {
                    cmd.Connection = con;
                    con.Open();
                    using (OracleDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            items.Add(new SelectListItem
                            {
                                Text = sdr["act_name"].ToString(),
                                Value = sdr["act_type_code"].ToString()
                            });
                        }
                    }
                    con.Close();
                }
            }

            return items;
        }







        internal List<SelectListItem> PopulateProfiles()
        {
            List<SelectListItem> items = new List<SelectListItem>();
            //string constr = ConfigurationManager.ConnectionStrings["Constring"].ConnectionString;
            using (OracleConnection con = new OracleConnection(conString))
            {
                string query = "select roleid,name  from TBL_ROLEMASTER where active='1'";
                using (OracleCommand cmd = new OracleCommand(query))
                {
                    cmd.Connection = con;
                    con.Open();
                    using (OracleDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            items.Add(new SelectListItem
                            {
                                Text = sdr["name"].ToString(),
                                Value = sdr["roleid"].ToString()
                            });
                        }
                    }
                    con.Close();
                }
            }

            return items;
        }

        internal List<SelectListItem> PopulatecpanelProfiles()
        {
            List<SelectListItem> items = new List<SelectListItem>();
            //string constr = ConfigurationManager.ConnectionStrings["Constring"].ConnectionString;
            using (OracleConnection con = new OracleConnection(conString))
            {
                string query = "select roleid,name  from cpanel_rolemaster where active='1'";
                using (OracleCommand cmd = new OracleCommand(query))
                {
                    cmd.Connection = con;
                    con.Open();
                    using (OracleDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            items.Add(new SelectListItem
                            {
                                Text = sdr["name"].ToString(),
                                Value = sdr["roleid"].ToString()
                            });
                        }
                    }
                    con.Close();
                }
            }

            return items;
        }

        public int custreg(string CustomerID, string CustomerName, string account, string username, string address, string CustomerPhone, string email, string customerprofile, string customercatgory, string CUSTOMERSERVICE)
        {
            int result = -1;

            using (OracleConnection con = new OracleConnection(conString))
            {
                String re = CreatePassword(8);

                String enc_pwd = Encrypt(re);

//                string query = "INSERT INTO users (USER_ID,USER_NAME,USER_LOG,USER_PWD,USER_EMAIL,USER_MOBILE,USER_FAX,USER_ADRS,USER_STATUS,DEF_ACC,LAST_LOGIN,LAST_LOG_IP,FAILD_LOGINS,USER_CUSTID,FIRST_LOGIN,CATOGRY,USER_PAS,USER_TRANSFER,ROLEID,ACCOUNT,ACTIVE)" +
//"VALUES((select max( to_number(user_id))+1 from users),'" + CustomerName + "','" + username + "','" + enc_pwd + "','" + email + "','" + CustomerPhone + "','" + CustomerPhone + "','al-khaleejbank','P','" + account + "',sysdate,'127.0.0.1',0,'"+CustomerID+"','T','"+customercatgory+"','" + re + "','True','" + customerprofile + "','" + account + "','1')";

//                OracleCommand cmd = new OracleCommand(query, con);

//                con.Open();

//                result = cmd.ExecuteNonQuery();
                OracleCommand cmd = new OracleCommand();
                cmd.CommandText = "insertnewcustomer";
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Connection = con;
                con.Open();

                cmd.Parameters.Add("CustomerName", OracleType.VarChar).Value = CustomerName;
                cmd.Parameters.Add("username", OracleType.VarChar).Value = username;
                cmd.Parameters.Add("enc_pwd", OracleType.VarChar).Value = enc_pwd;
                cmd.Parameters.Add("email", OracleType.VarChar).Value = email;
                cmd.Parameters.Add("CustomerPhone", OracleType.VarChar).Value = CustomerPhone;
                cmd.Parameters.Add("useraccount", OracleType.VarChar).Value = account;
                cmd.Parameters.Add("CustomerID", OracleType.VarChar).Value = CustomerID;
                //cmd.Parameters.Add("CustomerPhone", OracleType.VarChar).Value = CustomerPhone;
                //cmd.Parameters.Add("useraccount", OracleType.VarChar).Value = account;
                cmd.Parameters.Add("customercatgory", OracleType.VarChar).Value = customercatgory;
                cmd.Parameters.Add("re", OracleType.VarChar).Value = re;
                cmd.Parameters.Add("customerprofile", OracleType.VarChar).Value = customerprofile;
                cmd.Parameters.Add("CUSTOMERSERVICE", OracleType.VarChar).Value = CUSTOMERSERVICE;
                cmd.Parameters.Add("res", OracleType.Int32).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("errcode", OracleType.VarChar, 4000).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("errmsg", OracleType.VarChar, 4000).Direction = ParameterDirection.Output;
                cmd.ExecuteNonQuery();
                String res = cmd.Parameters["res"].Value.ToString();
                String errormsg = cmd.Parameters["errmsg"].Value.ToString();
                String errorcode = cmd.Parameters["errcode"].Value.ToString();
                result =Int32.Parse( res);
            }

            return result;


        }



        public List<CustomerAuthorization> PendingCustomer(String bracode)
        {
            OracleCommand cmd;
            OracleDataReader dr;

            String userid, username, useract;
            String query1, result;
            List<CustomerAuthorization> customer = new List<CustomerAuthorization>();
            if (!bracode.Equals("000"))
            {
                query1 = "select user_id,user_name,b.branch_name||'-'||t.act_name||'-'||c.curr_name||'-'||SUBSTR(def_acc,14) from  users,branchs b ,act_types t , currency c where    user_status = 'P'   and SUBSTR(def_acc,3,3)=b.branch_code and   SUBSTR(def_acc,6,5)=t.act_type_code and SUBSTR(def_acc,11,3)=c.CURR_CODE  and substr(def_acc,3,3)='" + bracode + "'";
            }
            else
            {
                query1 = "select user_id,user_name,b.branch_name||'-'||t.act_name||'-'||c.curr_name||'-'||SUBSTR(def_acc,14) from  users,branchs b ,act_types t , currency c where    user_status = 'P'   and SUBSTR(def_acc,3,3)=b.branch_code and   SUBSTR(def_acc,6,5)=t.act_type_code and SUBSTR(def_acc,11,3)=c.CURR_CODE";
            }

            using (OracleConnection con = new OracleConnection(conString))
            {
                cmd = new OracleCommand(query1, con);

                con.Open();

                dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        userid = dr[0].ToString();
                        username = dr[1].ToString();
                        useract = dr[2].ToString();


                        customer.Add(new CustomerAuthorization
                        {
                            CustomerID = userid,
                            Customername = username,
                            Customeraccount = useract
                        });
                    }
                }


            }


            return customer;
        }


        public List<CustomerAuthorizationinfo> CustomerAuthorizationinfo(String userid)
        {
            OracleCommand cmd;
            OracleDataReader dr;

            String username, useract;
            String query1, result;
            List<CustomerAuthorizationinfo> customer = new List<CustomerAuthorizationinfo>();
            OracleDataReader dr3;
            OracleCommand cmd3;
            string sqstr;
            string msg = "";
            string br, Sessioncurr = "";
            String acc = "";
            String acc_type = "";
            String acc_no;
            String curr;
            String curr_name = "";
            String lang;
            String brname = "";
            String acctype = "";
            String roleid = "", profilename = "";
            query1 = "select *  from users where user_id='" + userid + "'";
            using (OracleConnection con = new OracleConnection(conString))
            {
                cmd3 = new OracleCommand(query1, con);

                con.Open();


                dr3 = cmd3.ExecuteReader();
                if (dr3.Read())
                {
                    // 'lb_cust_name.Text = dr3(1)
                    OracleDataReader dr2;
                    OracleDataReader dr4;
                    OracleDataReader dr5;
                    OracleDataReader dr6;
                    OracleCommand cmd2;
                    OracleCommand cmd4;
                    OracleCommand cmd5;
                    OracleCommand cmd6;


                    acc = dr3[9].ToString();
                    roleid = dr3[18].ToString();
                    br = acc.Substring(2, 3);
                    acc_type = acc.Substring(5, 5);
                    Sessioncurr = acc.Substring(10, 3);
                    acc_no = acc.Substring(13);

                    cmd4 = new OracleCommand(("select BRANCH_NAME from BRANCHS where BRANCH_CODE_NO='" + br + "'"), con);
                    dr4 = cmd4.ExecuteReader();
                    if (dr4.Read())
                    {
                        brname = dr4[0].ToString();

                    }

                    dr4.Close();
                    //cmd5 = new OracleCommand(("select act_name from act_types where act_type_code ='" + (acc_type + "'")), con);
                    //dr5 = cmd5.ExecuteReader();
                    //if (dr5.Read())
                    //{
                    //    acctype = dr5[0].ToString();

                    //}

                    //dr5.Close();
                    cmd5 = new OracleCommand(("select act_name from act_types where act_type_code ='" + (acc_type + "'")), con);
                    dr5 = cmd5.ExecuteReader();
                    if (dr5.HasRows)
                    {
                        dr5.Read();
                        acctype = dr5[0].ToString();

                    }
                    else
                    {
                        cmd5 = new OracleCommand(("select act_name from invact_types where act_type_code='" + (acc_type + "'")), con);
                        dr5 = cmd5.ExecuteReader();
                        if (dr5.HasRows)
                        {
                            dr5.Read();
                            acctype = dr5[0].ToString();

                        }
                        else
                            acctype = "Account Type Not Found";

                    }

                    dr5.Close();
                    cmd2 = new OracleCommand(("select name  from tbl_rolemaster where roleid='" + roleid + "'  "), con);
                    dr2 = cmd2.ExecuteReader();
                    if (dr2.Read())
                    {
                        profilename = dr2[0].ToString();

                    }


                    dr2.Close();
                    cmd6 = new OracleCommand(("select CURR_NAME from CURRENCY where CURR_CODE = '" + Sessioncurr + "'"), con);
                    dr6 = cmd6.ExecuteReader();
                    if (dr6.Read())
                    {
                        curr_name = dr6[0].ToString();

                    }

                    dr6.Close();
                }
                customer.Add(new CustomerAuthorizationinfo
               {
                   userid = dr3[0].ToString(),
                   Branch = brname,
                   AccountType = acctype,
                   Customername = dr3[1].ToString(),
                   Currency = curr_name,
                   Customeraccount = acc.Substring(13),
                   UserName = dr3[2].ToString(),
                   Address = dr3[7].ToString(),
                   CustomerPhone = dr3[5].ToString(),
                   Email = dr3[4].ToString(),
                   Profile = profilename,
               });

                dr3.Close();
            }
            return customer;

        }



        public int updatecustomer(String userid, String status)
        {
            OracleCommand cmd;
            int result = -1;


            String query1;
            query1 = "update users set USER_STATUS='" + status + "' where user_id='" + userid + "'";
            using (OracleConnection con = new OracleConnection(conString))
            {
                cmd = new OracleCommand(query1, con);

                con.Open();
                result = cmd.ExecuteNonQuery();
            }
            return result;
        }
        public int updatecustomerusingact(String account, String status)
        {
            OracleCommand cmd;
            int result = -1;


            String query1;
            query1 = "update USERS set USER_STATUS ='" + status + "',FAILD_LOGINS=0 where DEF_ACC ='" + account + "'";
            using (OracleConnection con = new OracleConnection(conString))
            {
                cmd = new OracleCommand(query1, con);

                con.Open();
                result = cmd.ExecuteNonQuery();
            }
            return result;
        }


        public Loginmodelresult checkuserlogin(String usrname, String password, String UserHostAddress)
        {
            Loginmodelresult model = new Loginmodelresult();
            string encpass;

            encpass = Encrypt(password);
            OracleCommand cmd;
            OracleDataReader dr;
            string Sqlstr;
            Sqlstr = "Select user_id,user_name,user_branch, USER_LASTLOGIN,roleid from security_master where user_LOG= '"
                        + usrname + "' and user_pass= '" + encpass + "'";
            model.Login = false;
            using (OracleConnection con = new OracleConnection(conString))
            {
                cmd = new OracleCommand(Sqlstr, con);
                try
                {
                    con.Open();
                    dr = cmd.ExecuteReader();
                    OracleCommand cmd2;
                    OracleCommand cmd3;

                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {

                            cmd2 = new OracleCommand(("insert into Users_login values('"
                                            + (UserHostAddress + ("','"
                                            + (DateTime.Today.ToString() + ("','"
                                            + (usrname + "','-', 'S')")))))), con);
                            cmd2.ExecuteNonQuery();

                            model.UserId = dr[0].ToString();
                            model.user_name = dr[1].ToString();
                            model.user_branch = dr[2].ToString();
                            model.USER_LASTLOGIN = dr[3].ToString();
                            model.user_roleid = dr[4].ToString();

                            model.user_log = usrname;
                            model.Login = true;


                            if ((model.USER_LASTLOGIN == "F"))
                            {
                                cmd3 = new OracleCommand(("update SECURITY_MASTER set  user_pas='',user_lastlogin='T' where user_id='"
                                                + (model.UserId + "' ")), con);
                                cmd3.ExecuteNonQuery();
                                model.lblconfirm = "change_pass";
                            }
                            else
                            {
                                model.lblconfirm = "home";
                            }
                        }
                    }
                    else
                    {
                        model.lblconfirm = "There is wrong into username or password";
                        cmd2 = new OracleCommand(("insert into Users_login values('"
                                        + (UserHostAddress + ("','"
                                        + (DateTime.Today.ToString() + ("','"
                                        + (usrname + "','-', 'F')")))))), con);
                        cmd2.ExecuteNonQuery();
                        model.lblconfirm = "There is wrong into username or password";
                    }

                }
                catch (Exception ex)
                {
                    model.lblconfirm = "System Error";
                }
            }
            return model;
        }

        protected string GetIPAddress()
        {
            System.Web.HttpContext context = System.Web.HttpContext.Current;
            string ipAddress = context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

            if (!string.IsNullOrEmpty(ipAddress))
            {
                string[] addresses = ipAddress.Split(',');
                if (addresses.Length != 0)
                {
                    return addresses[0];
                }
            }

            return context.Request.ServerVariables["REMOTE_ADDR"];
        }


        protected string Encrypt(string clearText)
        {
            //string EncryptionKey = "IBAZ2TWTQS77898";
            //byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
            //using (Aes encryptor = Aes.Create())
            //{
            //    Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
            //    encryptor.Key = pdb.GetBytes(32);
            //    encryptor.IV = pdb.GetBytes(16);
            //    using (MemoryStream ms = new MemoryStream())
            //    {
            //        using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
            //        {
            //            cs.Write(clearBytes, 0, clearBytes.Length);
            //            cs.Close();
            //        }
            //        clearText = Convert.ToBase64String(ms.ToArray());
            //    }
            //}
            CryptLib _crypt = new CryptLib();

            String key = "b16920894899c7780b5fc7161560a412";//CryptLib.SHA256("my secret key", 32); //32 bytes = 256 bit

            String iv = "e77886746a9b416d";
            //String iv = CryptLib.GenerateRandomIV(16); //16 bytes = 128 bits
            //string key = CryptLib.getHashSha256("my secret key", 31); //32 bytes = 256 bits
            String cypherText = _crypt.encrypt(clearText, key, iv);

            //Console.WriteLine("Plain text =" + _crypt.decrypt(cypherText, key, iv));
            return cypherText;
        }

        protected string Decrypt(string cipherText)
        {
            string EncryptionKey = "IBAZ2TWTQS77898";
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }
                    cipherText = Encoding.Unicode.GetString(ms.ToArray());
                }
            }
            return cipherText;
        }

        public String changepass(String usrname, String oldpass, String newpass)
        {
            String encpass;
            String new_encpass;
            String lblconfirm = "System Error";
            encpass = Encrypt(oldpass);
            OracleCommand cmd;
            OracleDataReader dr;
            String Sqlstr;

            Sqlstr = "Select * from security_master where user_LOG= '"
                       + usrname + "' and user_pass= '"
                       + encpass + "'";
            using (OracleConnection con = new OracleConnection(conString))
            {
                cmd = new OracleCommand(Sqlstr, con);
                try
                {
                    con.Open();
                    dr = cmd.ExecuteReader();
                    OracleCommand cmd2;
                    OracleDataReader dr2;
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            new_encpass = Encrypt(newpass);
                            cmd2 = new OracleCommand("update security_master set user_pass='"
                                            + new_encpass + "' where user_log= '"
                                            + usrname + "'", con);
                            cmd2.ExecuteNonQuery();
                            lblconfirm = "Your Password was Changed Successfully";
                        }
                    }
                    else
                    {
                        lblconfirm = "Your Password was Not Changed successfully";
                    }

                }
                catch (Exception ex)
                {
                    // lblconfirm.Text = ex.Message
                    lblconfirm = "System Error";
                }
            }
            return lblconfirm;
        }


        public List<addaccount> Populatecustacts()
        {
            int i = 0; ;
            List<addaccount> items = new List<addaccount>();
            using (OracleConnection con = new OracleConnection(conString))
            {
                string query = " select  acc_no,branch_name,act_name,curr_name from USER_ACC_LINK acc,branchs br ,CURRENCY cur ,act_types cty" +
                    " where substr(acc.acc_no,3,3)= br.branch_code and substr(acc.acc_no,6,5)=cty.ACT_TYPE_CODE" +
                    " and substr(acc.acc_no,11,3)=cur.curr_code ";
                using (OracleCommand cmd = new OracleCommand(query))
                {
                    cmd.Connection = con;
                    con.Open();
                    using (OracleDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            items.Add(new addaccount
                            {
                                AccountID = i + 1,
                                AccountNumber = sdr["acc_no"].ToString().Substring(13),
                                AccountNumbercomplete = sdr["acc_no"].ToString(),
                                Branch = sdr["branch_name"].ToString(),
                                AccountType = sdr["act_name"].ToString(),
                                Currency = sdr["curr_name"].ToString(),
                                IsSelected = false,
                            });
                        }
                    }
                    con.Close();
                }
            }

            return items;
        }
        public String addnewacount(String act, String account,String category)
        {
            String lblconfirm = "System Error", user_id = null;
            bool FLAG;
            OracleCommand cmd;
            OracleDataReader dr;
            using (OracleConnection con = new OracleConnection(conString))
            {
                try
                {
                    cmd = new OracleCommand("select acc_no from user_acc_link  where acc_status  not in('R','DE') and  acc_no='" + account + "' and CATOGRY='" + category + "'", con);
                    OracleCommand cmd2;
                    OracleCommand cmd_acc_lnk;
                    con.Open();
                    dr = cmd.ExecuteReader();
                    if (dr.Read())
                    {
                        FLAG = false;
                        lblconfirm = "These Account Already exist";
                        con.Close();
                        return lblconfirm;
                    }
                    else
                    {
                        FLAG = true;
                    }

                    if (FLAG == true)
                    {

                        string query = "select user_id,user_name from users where DEF_ACC='" + act + "'and CATOGRY='" + category + "' and user_status  in('A')";

                        OracleCommand cmd3 = new OracleCommand(query, con);
                        OracleDataReader drr = cmd3.ExecuteReader();
                        if (drr.Read())
                        {
                            user_id = drr[0].ToString();
                        }
                        cmd = new OracleCommand("select count(*) from user_acc_link where acc_no='" + account + "' and user_id='" + user_id + "'", con);
                        dr = cmd.ExecuteReader();
                        dr.Read();
                        int counter;
                        counter = Convert.ToInt32(dr[0].ToString());
                        dr.Close();
                        cmd.Dispose();
                        if (counter == 0)
                        {
                            String dp_branch, dp_acc_tybe, dp_acc_curr;
                            dp_acc_tybe = account.Substring(5, 5);
                            dp_branch = account.Substring(2, 3);
                            dp_acc_curr = account.Substring(10, 3);
                            String sql2 = "select  nvl(max (acc_id),0) from user_acc_link where user_id=" + user_id;
                            cmd2 = new OracleCommand(sql2, con);
                            dr = cmd2.ExecuteReader();
                            dr.Read();
                            int ACC_ID;
                            ACC_ID = Convert.ToInt32(dr[0].ToString());
                            dr.Close();
                            cmd2.Dispose();
                            ACC_ID = ACC_ID + 1;
                            cmd_acc_lnk = new OracleCommand("INSERT INTO user_acc_link (BRANCH_CODE,ACT_TYPE,USER_ID,ACC_NO,ACC_STS,ACC_CURR,ACC_LANG,ACC_STATUS,ACC_ID,CATOGRY) values ('"
                                             + dp_branch + "','" + dp_acc_tybe + "','" + user_id + "','" + account + "','P','" + dp_acc_curr + "','AR','P','" + ACC_ID + "',  '" + category + "')", con);
                            cmd_acc_lnk.ExecuteNonQuery();
                            lblconfirm = "Account Added Successfully";
                        }
                        else
                        {
                            lblconfirm = "These Account Already exist";
                        }
                        con.Close();
                    }


                }
                catch (Exception ex)
                {
                    lblconfirm = "System Error";
                }
            }
            return lblconfirm;
        }
        public List<pendingacts> Pendingacounts(String bracode)
        {
            OracleCommand cmd;
            OracleDataReader dr;

            String userid, username, useract, newuseract;
            String query1, result;
            List<pendingacts> customer = new List<pendingacts>();
            if (!bracode.Equals("000"))
            {
                query1 = "select user_acc_link.user_id,user_name,def_acc,ACC_NO from users , user_acc_link where ACC_STATUS='P' and user_acc_link.user_id=users.user_id and substr(def_acc,3,3)='" + bracode + "'";
            }
            else
            {
                query1 = "select user_acc_link.user_id,user_name,def_acc,ACC_NO from users , user_acc_link where ACC_STATUS='P' and user_acc_link.user_id=users.user_id";
            }

            using (OracleConnection con = new OracleConnection(conString))
            {
                cmd = new OracleCommand(query1, con);

                con.Open();

                dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        userid = dr[0].ToString();
                        username = dr[1].ToString();
                        useract = dr[2].ToString();
                        newuseract = dr[3].ToString();

                        customer.Add(new pendingacts
                        {
                            USER_ID = userid,
                            USER_NAME = username,
                            DEF_ACC = useract,
                            ACC_NO = newuseract
                        });
                    }
                }


            }


            return customer;
        }



        public List<actAuthorizationinfo> newactAuthorizationinfo(string userid, string act)
        {

            OracleCommand cmd;
            OracleDataReader dr;

            String username, useract;
            String query1, result;
            List<actAuthorizationinfo> customer = new List<actAuthorizationinfo>();
            OracleDataReader dr3;
            OracleCommand cmd3;
            string sqstr;
            string msg = "";
            string br, Sessioncurr = "";
            String acc = "";
            String acc_type = "";
            String acc_no;
            String curr;
            String curr_name = "";
            String lang;
            String brname = "";
            String acctype = "";
            String roleid = "", profilename = "";
            query1 = "select user_acc_link.user_id,user_name,def_acc,ACC_NO from users , user_acc_link where user_acc_link.user_id='" + userid + "' and users.user_id='" + userid + "' and ACC_NO='" + act + "' and user_acc_link.acc_status='P'";
            using (OracleConnection con = new OracleConnection(conString))
            {
                cmd3 = new OracleCommand(query1, con);

                con.Open();


                dr3 = cmd3.ExecuteReader();
                if (dr3.HasRows)
                {
                    while (dr3.Read())
                    {
                        // 'lb_cust_name.Text = dr3(1)
                        OracleDataReader dr2;
                        OracleDataReader dr4;
                        OracleDataReader dr5;
                        OracleDataReader dr6;
                        OracleCommand cmd2;
                        OracleCommand cmd4;
                        OracleCommand cmd5;
                        OracleCommand cmd6;


                        acc = dr3[3].ToString();
                        br = acc.Substring(2, 3);
                        acc_type = acc.Substring(5, 5);
                        Sessioncurr = acc.Substring(10, 3);
                        acc_no = acc.Substring(13);

                        cmd4 = new OracleCommand(("select BRANCH_NAME from BRANCHS where BRANCH_CODE_NO='" + br + "'"), con);
                        dr4 = cmd4.ExecuteReader();
                        if (dr4.Read())
                        {
                            brname = dr4[0].ToString();

                        }

                        dr4.Close();
                        cmd5 = new OracleCommand(("select act_name from act_types where act_type_code ='" + (acc_type + "'")), con);
                        dr5 = cmd5.ExecuteReader();
                        if (dr5.HasRows)
                        {
                            dr5.Read();
                            acctype = dr5[0].ToString();

                        }
                        else
                        {
                            cmd5 = new OracleCommand(("select act_name from invact_types where act_type_code='" + (acc_type + "'")), con);
                            dr5 = cmd5.ExecuteReader();
                            if (dr5.HasRows)
                            {
                                dr5.Read();
                                acctype = dr5[0].ToString();

                            }
                            else
                                acctype = "Account Type Not Found";

                        }

                        dr5.Close();

                        cmd6 = new OracleCommand(("select CURR_NAME from CURRENCY where CURR_CODE = '" + Sessioncurr + "'"), con);
                        dr6 = cmd6.ExecuteReader();
                        if (dr6.Read())
                        {
                            curr_name = dr6[0].ToString();

                        }

                        dr6.Close();

                        customer.Add(new actAuthorizationinfo
                        {
                            userid = dr3[0].ToString(),
                            Branch = brname,
                            AccountType = acctype,
                            Customername = dr3[1].ToString(),
                            Currency = curr_name,
                            Customeraccount = acc.Substring(13),
                            completeact = acc,
                        });
                    }
                    dr3.Close();
                }
            }
            return customer;
        }

        public int updateAccount(String userid, String account, String status)
        {
            OracleCommand cmd;
            int result = -1;


            String query1;
            query1 = "update user_acc_link set ACC_STATUS='" + status + "', ACC_STS='" + status + "' where  ACC_no='" + account + "' and  user_id ='" + userid + "'";
            using (OracleConnection con = new OracleConnection(conString))
            {
                cmd = new OracleCommand(query1, con);

                con.Open();
                result = cmd.ExecuteNonQuery();
            }
            return result;
        }
        public List<GETpassword> getpassword(String account)
        {
            OracleCommand cmd;
            OracleDataReader dr;
            String acttypename="",acttype;
            List<GETpassword> list = new List<GETpassword>();
            string enc_pwd = "", br, branchname = "", lblconfirm = "System Error", pass, name = "";
            using (OracleConnection con = new OracleConnection(conString))
            {
                OracleCommand cmd4, cmd5;

                OracleDataReader dr4, dr5;

                try
                {

                    br = account.Substring(2, 3);
                     acttype = account.Substring(5, 5);
                    

                    cmd4 = new OracleCommand("select BRANCH_NAME from BRANCHS where BRANCH_CODE_NO='" + br + "'", con);
                    con.Open();
                    dr4 = cmd4.ExecuteReader();
                    if (dr4.Read())
                    {
                        branchname = dr4[0].ToString();
                    }

                    cmd5 = new OracleCommand("select act_name from act_types  where act_type_code='" + acttype + "'", con);                
                    dr5 = cmd5.ExecuteReader();
                    if (dr5.Read())
                    {
                        acttypename= dr5[0].ToString();
                    }


                    cmd = new OracleCommand("select USER_PAS,DEF_ACC ,USER_NAME from users where DEF_ACC='" + account + "'", con);
                    dr = cmd.ExecuteReader();
                    if (dr.Read())
                    {
                        enc_pwd = dr[0].ToString();
                        pass = enc_pwd;
                        account = branchname + "-" + acttypename + "-" + dr[1].ToString().Substring(13);
                        name = dr[2].ToString();
                        lblconfirm = "Successfully";
                    }
                    else
                    {
                        lblconfirm = "Wrong Customer Account";
                    }

                }
                catch (Exception ex)
                {
                    lblconfirm = "System Error";
                }
                list.Add(new GETpassword
                {
                    pass = enc_pwd,
                    name = name,
                    account = account,
                    lblconfirm = lblconfirm,
                    branchname = branchname,
                });
                return list;
            }
        }


        // changed by zainab and lubna in 22-11-2022 from  resetpassword(String account, String userLog) to  resetpassword(String userLog)
        // Reasons: on corporate all users of the corporate have the same def_acc, then since the unique feature is the user_log it has been changed to it.
        public List<resetpass> resetpassword(String account, String userLog)
        {


            List<resetpass> list = new List<resetpass>();
            string enc_pwd = "", br, branchname = "", lblconfirm = "System Error", pass, name = "";
            OracleCommand cmd, cmd1;
            OracleDataReader dr, dr1;
            String Sqlstr, sqlstr1;
            String re = "", enc_pwd2, str;
            using (OracleConnection con = new OracleConnection(conString))
            {

                try
                {
                    re = CreatePassword(8);

                    enc_pwd = Encrypt(re);
                    enc_pwd2 = enc_pwd;
                    br = account.Substring(2, 3);

                    OracleCommand cmd4;

                    OracleDataReader dr4;

                    cmd4 = new OracleCommand("select BRANCH_NAME from BRANCHS where BRANCH_CODE_NO='" + br + "'", con);
                    con.Open();
                    dr4 = cmd4.ExecuteReader();
                    if (dr4.Read())
                    {
                        branchname = dr4[0].ToString();
                    }


                    sqlstr1 = "select DEF_ACC ,USER_NAME from users where def_acc='" + account + "'";
                    cmd1 = new OracleCommand(sqlstr1, con);
                    dr1 = cmd1.ExecuteReader();
                    if (dr1.Read())
                    {

                        account = dr1[0].ToString();
                        name = dr1[1].ToString();
                        cmd = new OracleCommand("update users set user_pwd='" + enc_pwd.ToString() + "' , first_login='T',USER_PAS='" + re.ToString() + "' where  user_log='" + userLog + "'", con); // where  def_acc='" + account + "'", con);
                        cmd.ExecuteNonQuery();
                        lblconfirm = "Successfully";


                    }
                    else
                    {
                        lblconfirm = "Pleace Check Your Account";
                    }
                }
                catch (Exception ex)
                {
                    lblconfirm = "System Error";
                }
                list.Add(new resetpass
                {
                    pass = re,
                    name = name,
                    account = account,
                    lblconfirm = lblconfirm,
                    branchname = branchname,
                });
                return list;
            }
        }



        public string CreatePassword(int length)
        {
            const string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            StringBuilder res = new StringBuilder();
            Random rnd = new Random();
            while (0 < length--)
            {
                res.Append(valid[rnd.Next(valid.Length)]);
            }
            return res.ToString();
        }


        public custinfo getcustinfo(String branchcode, String acttype, String acc_no, String acc_curr, String category, String User_Log)
        {
            Boolean FLAG;
            String lblconfirm = "";
            OracleCommand cmd;
            OracleDataReader dr;
            int counter;
            custinfo model = new custinfo();
            String query1 = "select  u.user_id, u.user_name,u.user_log,u.user_pwd,u.user_email,u.user_mobile,u.user_adrs,m.name,u.user_status" +
             " from users u, tbl_rolemaster m  where u.roleid=m.roleid and u.DEF_ACC='99" + branchcode + acttype + acc_curr + acc_no + "' and  catogry ='"+category+"' and user_log = '"+User_Log+"'";
            
            using (OracleConnection con = new OracleConnection(conString))
            {
                cmd = new OracleCommand(query1, con);

                con.Open();

                dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read()) {

                        model.user_id = dr[0].ToString();
                        model.user_name = dr[1].ToString();
                        model.user_log = dr[2].ToString();
                        model.user_pwd = dr[3].ToString();
                        model.user_email = dr[4].ToString();
                        model.user_adrs = dr[6].ToString();
                        model.user_mobile = dr[5].ToString();
                        model.name = dr[7].ToString();
                        model.status = dr[8].ToString();
                        model.lblconfirm = "This Account is Already exist";
                      
                                                            }

                                                  }

                                                                                
                else
                {
                        model.lblconfirm  = "This Account is available";
                }




            }
            return model;
        }


        public int Updatecustomer(String sts,custinfo model)
        {
            int result=-1;

            if(sts.Equals("U"))
            {
                OracleCommand cmd;
                OracleDataReader dr;
                string Sqlstr;
                string re;
                try
                {

                    Sqlstr = "update USERS set USER_NAME ='"+model.user_name + "',USER_EMAIL ='"
                                + model.user_email + "',user_log ='"
                                + model.user_log + "',USER_ADRS ='"
                                + model.user_adrs + " ', ROLEID='"
                                + model.profileCode + "' where USER_ID ='" + model.user_id+"'";
                    OracleCommand cmd1;
                    OracleDataReader dr1;
                    using (OracleConnection con = new OracleConnection(conString))
                    {
                        cmd = new OracleCommand(Sqlstr, con);

                        con.Open();


                        result = cmd.ExecuteNonQuery();
                        con.Close();
                    }
                }
                catch (Exception ex)
                {
                    result = -2;
                }  
            }
            else
            {
                try
                {

                    String Sqlstr = "update users set USER_STATUS='DE'  where USER_ID ='" + model.user_id + "'";
                 using (OracleConnection con = new OracleConnection(conString))
                    {
                      
                    OracleCommand  cmd = new OracleCommand(Sqlstr, con);
                  con.Open();
                  result=  cmd.ExecuteNonQuery();
                  Sqlstr = "update USER_ACC_LINK set ACC_STS='DE'  where USER_ID ='" + model.user_id + "'";
                    cmd = new OracleCommand(Sqlstr, con);
                    cmd.ExecuteNonQuery();
                     
                     result=-3 ;
                    }
                }
                catch (Exception ex)
                {
                     result= -2;
                }
            }
            return result;
        }
        public List<pageparameter> PopulateProfilemangement(String categoryid)
        {
            int i = 0; ;
            List<pageparameter> items = new List<pageparameter>();
            using (OracleConnection con = new OracleConnection(conString))
            {
                string query = "select  t.menuid ,t.menuname,t.menu_ar_name , tm.menuname  parnet_name,tm.menu_ar_name parnet_name_ar,tm.menuid  parnet_id from (select  menu_ar_name,  menuname,menuid  from tbl_menumaster where MENUPARENTId=0  ) tm ,tbl_menumaster t    where t.MENUPARENTID<>0 and t.menuparentid=tm.menuid  and menu_category in ('" + categoryid + "','0')  order by menuid ,menuparentid";
                using (OracleCommand cmd = new OracleCommand(query))
                {
                    cmd.Connection = con;
                    con.Open();
                    using (OracleDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            items.Add(new pageparameter
                            {
                                menuid=sdr[0].ToString() ,
                                menuname =sdr[1].ToString() ,
                                menuname_ar = sdr[2].ToString(),
                                Parent_menuname = sdr[3].ToString(),
                                Parent_menuname_ar = sdr[4].ToString(),
                                 
                                menuparentid =sdr[5].ToString() ,
                                IsSelected = false,
                            });
                        }
                    }
                    con.Close();
                }
            }

            return items;
        }
     
       
        public List<SelectListItem> GetGatgories()
        
        {
            List<SelectListItem> items = new List<SelectListItem>();

           
            
            int i = 0;
   
            using (OracleConnection con = new OracleConnection(conString))
            {
                string query = " select  cat_id,cat_name from category";
                using (OracleCommand cmd = new OracleCommand(query))
                {
                    cmd.Connection = con;
                    con.Open();
                    using (OracleDataReader sdr = cmd.ExecuteReader())
                    {
                        if (sdr.HasRows)
                        {

                            items.Add(new SelectListItem
                            {
                                Text = "-- Select Customer category --",
                                Value = "0",
                            });
                            while (sdr.Read())
                            {

                                items.Add(new SelectListItem
                                {
                                    Text = sdr[1].ToString(),
                                    Value = sdr[0].ToString()
                                });
                            }
                        }
                    }
                    con.Close();
                }
            }
 
             return items;

        }

        public List<profilesparameter> GetProfiles()
     {
         int i = 0; ;
         List<profilesparameter> items = new List<profilesparameter>();
         using (OracleConnection con = new OracleConnection(conString))
         {
             string query = "select t.roleid,name,DECODE (t.active,'1','Active','DeActive') status  from tbl_rolemaster t where t.name!='Admin' order by t.roleid";
             using (OracleCommand cmd = new OracleCommand(query))
             {
                 cmd.Connection = con;
                 con.Open();
                 using (OracleDataReader sdr = cmd.ExecuteReader())
                 {
                     while (sdr.Read())
                     {
                         items.Add(new profilesparameter
                         {
                             profielid = sdr[0].ToString(),
                             profilename = sdr[1].ToString(),
                             profilestatus = sdr[2].ToString(),
                            
                             IsSelected = false,
                         });
                     }
                 }
                 con.Close();
             }
         }

         return items;
        }
        public List<SelectListItem> PopulateCustStatus()
        {
            List<SelectListItem> items = new List<SelectListItem>();



            int i = 0;

            using (OracleConnection con = new OracleConnection(conString))
            {
                string query = " select  STATUS_CODE,STATUS_NAME from CUSTOMERSTATUS where ACTIVE='1'";
                using (OracleCommand cmd = new OracleCommand(query))
                {
                    cmd.Connection = con;
                    con.Open();
                    using (OracleDataReader sdr = cmd.ExecuteReader())
                    {
                        if (sdr.HasRows)
                        {

                            items.Add(new SelectListItem
                            {
                                Text = "-- Select Customer Status --",
                                Value = "0",
                            });
                            while (sdr.Read())
                            {
                                items.Add(new SelectListItem
                                {
                                    Text = sdr[1].ToString(),
                                    Value = sdr[0].ToString()
                                });
                            }
                        }
                    }
                    con.Close();
                }
            }

            return items;

        }


        internal string addnewprofile(string profilename, string menuid, string parnetid)
        {
            String lblconfirm = "System Error", profileid = null;
            bool FLAG;
            OracleCommand cmd;
            OracleDataReader dr;
            using (OracleConnection con = new OracleConnection(conString))
            {
                try
                {
                    cmd = new OracleCommand("select * from tbl_rolemaster where name='" + profilename + "'", con);
                    OracleCommand cmd2;
                    OracleCommand cmd_acc_lnk;
                   
                    con.Open();
                    dr = cmd.ExecuteReader();
                    if (dr.Read())
                    {
                        profileid = dr[0].ToString();
                        FLAG = true;
                        lblconfirm = "These Account Already exist";
                        
                    }
                    else
                    {

                        cmd = new OracleCommand("select max(to_number(nvl(roleid,0)))+1 from tbl_rolemaster", con);
          
                       profileid  = cmd.ExecuteOracleScalar().ToString();
                         
                        cmd_acc_lnk = new OracleCommand(" INSERT INTO tbl_rolemaster (ROLEID,NAME,ACTIVE) VALUES ('"
                                            +profileid + "','" +profilename + "','1' )", con);
                        cmd_acc_lnk.ExecuteNonQuery();
                        lblconfirm = "Account Added Successfully";
                        FLAG = true;
                    }

                    if (FLAG == true)
                    {

                        cmd = new OracleCommand("select   max(to_number(nvl(id,0)))+ 1 maxid from tbl_rolemenumapping", con);

                        String id = cmd.ExecuteOracleScalar().ToString();
                        cmd = new OracleCommand("select id,roleid,menuid,active from tbl_rolemenumapping where  menuid='" + parnetid + "' and roleid='" + profileid + "'", con);
                         dr = cmd.ExecuteReader();
                         if (dr.Read())
                         {
                             cmd_acc_lnk = new OracleCommand("INSERT INTO tbl_rolemenumapping (ID,ROLEID,MENUID,ACTIVE)VALUES('"
                                + id + "','" + profileid + "','" + menuid + "','1')", con);
                             cmd_acc_lnk.ExecuteNonQuery();
                             lblconfirm = "Account Added Successfully";

                         }
                         else
                         {
                             cmd_acc_lnk = new OracleCommand("INSERT INTO tbl_rolemenumapping (ID,ROLEID,MENUID,ACTIVE)VALUES('"
                                 + id + "','" + profileid + "','" + parnetid + "','1')", con);
                             cmd_acc_lnk.ExecuteNonQuery();
                             cmd_acc_lnk = new OracleCommand("INSERT INTO tbl_rolemenumapping (ID,ROLEID,MENUID,ACTIVE)VALUES('"
                                + Convert.ToInt32(id)+1 + "','" + profileid + "','" + menuid + "','1')", con);
                             cmd_acc_lnk.ExecuteNonQuery();
                             lblconfirm = "Account Added Successfully";
                         }
                        }
                        else
                        {
                            lblconfirm = "These Account Already exist";
                        }
                       con.Close();
                    


                }
                catch (Exception ex)
                {
                    lblconfirm = "System Error";
                }
            }
            return lblconfirm;
  
        }

        public List<channel> Channels()
        {
            List<channel> AvailableItems = new List<channel>();
            using (OracleConnection con = new OracleConnection(conString))
            {
                
                using (OracleCommand cmd = new OracleCommand())
                {
                    cmd.CommandText = "getchannel";
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Connection = con;
                    con.Open();

                    cmd.Parameters.Add("channel_Cursor", OracleType.Cursor).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("res", OracleType.Int32).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("errcode", OracleType.VarChar, 4000).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("errmsg", OracleType.VarChar, 4000).Direction = ParameterDirection.Output;
                    cmd.ExecuteNonQuery();
                    String res = cmd.Parameters["res"].Value.ToString();
                    String errormsg = cmd.Parameters["errmsg"].Value.ToString();
                    String errorcode = cmd.Parameters["errcode"].Value.ToString();

                    using (OracleDataReader sdr = (OracleDataReader)cmd.Parameters["channel_Cursor"].Value)
                    {
                        while (sdr.Read())
                        {
                            AvailableItems.Add(new channel()
                            {
                                ID = sdr[0].ToString(),
                                Name = sdr[1].ToString()
                            });
                        }
                    }
                    con.Close();
                }
            }
 
            return AvailableItems;
        }
        public List<UsersMangementViewModel> GetCustomerLog(String UserName, String loginType)
        {
            List<UsersMangementViewModel> userLog = new List<UsersMangementViewModel>();
            using (OracleConnection con = new OracleConnection(conString))
            {
                string query = "";

                if (loginType.Equals("1"))
                {
                    query = "select ipaddress,login_time,user_login,user_pass,decode(login_status,'F','Failed','S','Succesful','unknown'), user_id from admin_login where user_login = '" + UserName + "'";

                }
                else if (loginType.Equals("2"))
                {
                    query = "select ipaddress,login_time,user_login,user_pass,decode(login_status,'F','Failed','S','Succesful','unknown'), user_id from admin_login where user_login = '" + UserName + "' and login_status = 'S'";

                }
                else
                {
                    query = "select ipaddress,login_time,user_login,user_pass,decode(login_status,'F','Failed','S','Succesful','unknown'), user_id from admin_login where user_login = '" + UserName + "' and login_status = 'F'";

                }

                if (con.State == ConnectionState.Closed)
                { con.Open(); }

                OracleCommand cmd = new OracleCommand(query, con);

                OracleDataReader dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {


                        userLog.Add(new UsersMangementViewModel
                        {
                            IpAddress = dr[0].ToString(),
                            LoginTime = dr[1].ToString(),
                            //user_id = Convert.ToInt32(dr[1].ToString()),
                            UserLogin = dr[2].ToString(),
                            UserPass = dr[3].ToString(),
                            LoginStatus = dr[4].ToString(),
                            UserID = dr[5].ToString(),

                        });
                    }
                }


            }
            return userLog;
        }
        internal int insertuser(string p1, string p2, string p3, string p4)
        {
            throw new NotImplementedException();
        }
        //--------------------------GET UserLog------------
        public List<UsersMangementViewModel> GetUserLog(String UserName, String loginType)
        {
            List<UsersMangementViewModel> userLog = new List<UsersMangementViewModel>();
            using (OracleConnection con = new OracleConnection(conString))
            {
                string query = "";

                if (loginType.Equals("1"))
                {
                    query = "select ipaddress,login_time,user_login,user_pass,decode(login_status,'F','Failed','S','Succesful','unknown') from users_login where user_login = '" + UserName + "'";

                }
                else if (loginType.Equals("2"))
                {
                    query = "select ipaddress,login_time,user_login,user_pass,decode(login_status,'F','Failed','S','Succesful','unknown') from users_login where user_login = '" + UserName + "' and login_status = 'S'";

                }
                else
                {
                    query = "select ipaddress,login_time,user_login,user_pass,decode(login_status,'F','Failed','S','Succesful','unknown') from users_login where user_login = '" + UserName + "' and login_status = 'F'";

                }

                if (con.State == ConnectionState.Closed)
                { con.Open(); }

                OracleCommand cmd = new OracleCommand(query, con);

                OracleDataReader dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {


                        userLog.Add(new UsersMangementViewModel
                        {
                            IpAddress = dr[0].ToString(),
                            LoginTime = dr[1].ToString(),
                            //user_id = Convert.ToInt32(dr[1].ToString()),
                            UserLogin = dr[2].ToString(),
                            UserPass = dr[3].ToString(),
                            LoginStatus = dr[4].ToString(),

                        });
                    }
                }


            }
            return userLog;
        }

        //---------------------------------GetCustomerIDFromAccountNumber------------------------------------------
        /// <summary>
        /// /It Gets Customer Full Account Number
        /// and Returns the ID
        /// </summary>
        /// <param name="AccountNumber"></param>
        /// <returns>CustID</returns>
        public String getCustIDFromAcc(string AccountNumber)
        {
            int CustID = 0;
            using (OracleConnection con = new OracleConnection(conString))
            {
                string query = "select user_id from users where account = " + AccountNumber;

                OracleCommand cmd = new OracleCommand(query, con);

                con.Open();


                using (IDataReader dataReader = cmd.ExecuteReader())
                {
                    while (dataReader.Read())
                    {

                        if (dataReader["user_id"] != DBNull.Value)
                        {

                            CustID = Convert.ToInt32(dataReader["user_id"]);

                        }
                    }
                    //Accounts = Accounts.Substring(1);
                    return CustID.ToString();

                }

            }

        }

        //----------------------------------------------GetTransferReport---------------------------------------------------------
        /// <summary>
        /// GetTransferReport
        /// </summary>
        /// <param name="custId"></param>
        /// <returns>List of Requests and response</returns>
        public List<CustomerTransferReportViewModel> GetTransferReport(string custId)
        {
            List<CustomerTransferReportViewModel> items = new List<CustomerTransferReportViewModel>();
            using (OracleConnection con = new OracleConnection(conString))
            {

                string query = " SELECT tran_req_date,tran_req,tran_resp,tran_resp_result,TRAN_STATUS from trans_log WHERE" +
                               " tran_name in('Own Transfer','To Bank Customer Transfer','To Counter Transfer') AND user_id = " + custId;

                using (OracleCommand cmd = new OracleCommand(query))
                {
                    cmd.Connection = con;
                    con.Open();
                    using (OracleDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            string[] reqString = null;
                            items.Add(new CustomerTransferReportViewModel
                            {
                                TranDate = dr["tran_req_date"].ToString(),
                                TranFullReq = dr["tran_req"].ToString(),
                                TranFullResp = dr["tran_resp"].ToString(),
                                TranResult = dr["tran_resp_result"].ToString(),
                                TranStatus = dr["TRAN_STATUS"].ToString(),

                            });
                        }
                        con.Close();
                    }
                }
            }

            return items;
        }

     
    }//class
    public class Encryptor
    {

        public static string v;

        private static TripleDESCryptoServiceProvider des = new TripleDESCryptoServiceProvider();

        private static char[] ekey = "jaaaoiuyndfghjytewsdaaaa".ToCharArray();

        private static byte[] Key
        {
            get
            {
                return Encoding.Default.GetBytes(ekey);
                // Return Encoding.Default.GetBytes(WindowsIdentity.GetCurrent.Name.PadRight(24, Chr(0)))
            }
        }

        public static byte[] Vector
        {
            get
            {
                return Encoding.Default.GetBytes("fjhksjf9iufjsoifhihfsgdsgsg");
            }
        }

        public static string Encrypt(string Text)
        {
            return Encryptor.Transform(Text, des.CreateEncryptor(Key, Vector));
        }

        public static string Decrypt(string encryptedText)
        {
            return Encryptor.Transform(encryptedText, des.CreateDecryptor(Key, Vector));
        }

        private static string Transform(string Text, ICryptoTransform CryptoTransform)
        {
            MemoryStream stream = new MemoryStream();
            CryptoStream cryptoStream = new CryptoStream(stream, CryptoTransform, CryptoStreamMode.Write);
            byte[] Input = Encoding.Default.GetBytes(Text);
            cryptoStream.Write(Input, 0, Input.Length);
            cryptoStream.FlushFinalBlock();
            return Encoding.Default.GetString(stream.ToArray());
        }
    }
}
