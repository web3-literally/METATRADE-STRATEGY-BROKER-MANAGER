using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using Newtonsoft.Json;
using System.Diagnostics;

namespace Terminal_Manager
{
    delegate void LoginReplyEvent(LoginReplyPacket loginReplyPacket);

    class MainController
    {
        private static MainForm _main_gui;
        private static Timer _guiTimer;
        public static List<MTAccount> m_listMTAccount { get; set; } = new List<MTAccount>();
        public static string m_userID;
        public static MTAccount m_curSelectedMTAccount;
        public static MainForm MainGui
        {
            get { return _main_gui; }
            set
            {
                _main_gui = value;
                EventHandler onLogin = (sender, args) =>
                {
                    checkLogin();
                };
                EventHandler onRegister = (sender, args) =>
                {
                    Process.Start("http://127.0.0.1:3330/auth/register");
                };
                EventHandler onRunAccount = (sender, args) =>
                {
                };
                EventHandler onExit = (sender, args) =>
                {
                    Application.Exit();
                };
                EventHandler onMinimum = (sender, args) =>
                {
                    _main_gui.WindowState = FormWindowState.Minimized;
                };
                
                DataGridViewRowStateChangedEventHandler onSelectMTAccount = (sender, args) =>
                {
                    try
                    {
                        string id = args.Row.Cells["id"].FormattedValue.ToString();
                        selectMTAccountToExecute(id);
                    }
                    catch (Exception)
                    {

                    }
                };

                EventHandler onMyAccounts = (sender, args) =>
                {
                    _main_gui.MainTabControl.SelectTab("TabMyAccounts");
                };
                EventHandler onMore = (sender, args) =>
                {
                    Process.Start("http://127.0.0.1:3330/auth/register");
                };
                EventHandler onOpenMT4 = (sender, args) =>
                {
                    string strMT4Path = System.IO.Path.GetDirectoryName(Application.ExecutablePath) + "/MT4/terminal.exe";
                    Process.Start(strMT4Path);
                };
                EventHandler onOpenMT5 = (sender, args) =>
                {
                    string strMT5Path = System.IO.Path.GetDirectoryName(Application.ExecutablePath) + "/MT5/terminal64.exe";
                    Process.Start(strMT5Path);
                };
                EventHandler onBack = (sender, args) =>
                {
                    _main_gui.MainTabControl.SelectTab("MainDashboardControl");
                };

                _main_gui.m_btnLogin.Click += onLogin;
                _main_gui.m_btnRegister.Click += onRegister;
                _main_gui.m_btnBack.Click += onRunAccount;
                _main_gui.m_picExit.Click += onExit;
                _main_gui.m_picClose.Click += onExit;
                _main_gui.m_picMinimum.Click += onMinimum;
                _main_gui.m_picMyAccounts.Click += onMyAccounts;
                _main_gui.m_picMore.Click += onMore;
                _main_gui.m_picOpenMT4.Click += onOpenMT4;
                _main_gui.m_picOpenMT5.Click += onOpenMT5;
                _main_gui.m_btnBack.Click += onBack;

                _main_gui.m_dataGridMTAccounts.RowStateChanged += onSelectMTAccount;




                LoginReplyEvent onLoginReply = (loginReplyPacket) =>
                {
                    if (loginReplyPacket.status == "Failed")
                    {
                        MessageBox.Show(loginReplyPacket.message, "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    else if (loginReplyPacket.status == "Success")
                    {
                        MessageBox.Show(loginReplyPacket.message, "Login Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        m_listMTAccount = loginReplyPacket.accounts;
                        m_userID = loginReplyPacket.id;
                    }
                };
                Socket_Manager.Instance.Initialize();
                Socket_Manager.Instance.LoginReplied += onLoginReply;
                _guiTimer = new Timer { Interval = 200, Enabled = true };
                _guiTimer.Tick += (sender, args) => {
                    if (!string.IsNullOrWhiteSpace(m_userID))
                    {
                        _main_gui.MainTabControl.SelectTab("MainDashboardControl");
                        _guiTimer.Enabled = false;
                        _main_gui.mTAccountBindingSource.DataSource = m_listMTAccount;
                    }
                };
            }
        }

        private static void checkLogin()
        {
            string strEmail = _main_gui.m_txtEmail.Text;
            string strPassword = _main_gui.m_txtPassword.Text;
            Socket_Manager.Instance.SendLoginPacket(strEmail, strPassword);
        }

        private static void selectMTAccountToExecute(string id)
        {
            m_curSelectedMTAccount = m_listMTAccount.Find(x => x.id == id);

            _main_gui.m_lblAccountNumber.Text = m_curSelectedMTAccount.account_number;
            _main_gui.m_lblBrokerName.Text = m_curSelectedMTAccount.broker.server_name;
            _main_gui.m_lblCreateDate.Text = m_curSelectedMTAccount.create_date;
            _main_gui.m_lblAproveState.Text = m_curSelectedMTAccount.status;

            if (_main_gui.m_lblAproveState.Text == "Aproved")
                _main_gui.m_lblAproveState.ForeColor = Color.Lime;
            else
                _main_gui.m_lblAproveState.ForeColor = Color.Red;

            _main_gui.m_lblPlanName.Text = m_curSelectedMTAccount.plan.name;
            _main_gui.m_lblPlanPrice.Text = m_curSelectedMTAccount.plan.price.ToString();
            _main_gui.m_lblMinLot.Text = m_curSelectedMTAccount.plan.min_lot.ToString();
            _main_gui.m_lblMaxLot.Text = m_curSelectedMTAccount.plan.max_lot.ToString();
            _main_gui.m_lblMaxDailyProfit.Text = m_curSelectedMTAccount.plan.max_daily_profit.ToString();
            _main_gui.m_lblMaxDailyLoss.Text = m_curSelectedMTAccount.plan.max_daily_loss.ToString();
            _main_gui.m_lblDailyLossFix.Text = m_curSelectedMTAccount.plan.daily_loss_fix.ToString();
            _main_gui.m_lblMaxTotalProfit.Text = m_curSelectedMTAccount.plan.max_total_profit.ToString();
            _main_gui.m_lblMaxTotalLoss.Text = m_curSelectedMTAccount.plan.max_total_loss.ToString();
            _main_gui.m_lblTotalLossFix.Text = m_curSelectedMTAccount.plan.total_loss_fix.ToString();
            _main_gui.m_lblMaxOrders.Text = m_curSelectedMTAccount.plan.max_orders.ToString();
            _main_gui.m_lblComment.Text = m_curSelectedMTAccount.plan.comment;
            _main_gui.m_txtCurrencyPairs.Text = string.Join(",", m_curSelectedMTAccount.plan.currency_pair);
            _main_gui.m_lblUsePreApproal.Text = m_curSelectedMTAccount.plan.use_pre_approval.ToString();
            _main_gui.m_lblUSDForPreApproal.Text = m_curSelectedMTAccount.plan.usd_for_pre_approval.ToString();
            _main_gui.m_lblValidityPeriod.Text = m_curSelectedMTAccount.plan.validity_period.ToString();
            _main_gui.m_lblOutControlFlag.Text = m_curSelectedMTAccount.plan.outside_control_flag.ToString();
            _main_gui.m_lblApprovalStatus.Text = m_curSelectedMTAccount.plan.approval_status;
        }
    }
}
