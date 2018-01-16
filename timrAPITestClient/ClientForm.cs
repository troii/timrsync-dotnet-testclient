using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TimrSyncTest.TimrSyncWebService;

namespace TimrSyncTest
{
    public partial class ClientForm : Form
    {
        private TimrSyncService timr = null;

        public ClientForm()
        {
            InitializeComponent();

            DateTime now = DateTime.Now;
            txtChanged.Text = new DateTime(now.Year, now.Month, 1).ToShortDateString();
        }

        private TimrSyncService Timr
        {
            get
            {
                if (timr == null)
                {
                    if (String.IsNullOrEmpty(txtIdentifier.Text) || String.IsNullOrEmpty(txtToken.Text))
                        throw new Exception("Identifier or authentication token are missing (could be generated in you timr account settings)");

                    timr = new TimrSyncService();
                    timr.Url = @"https://timrsync.timr.com/timr/timrsync";
                    timr.Credentials = new System.Net.NetworkCredential(txtIdentifier.Text, txtToken.Text);
                }
                return timr;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                result.Items.Clear();
                foreach (Car car in Timr.GetCars(new Object()))
                {
                    result.Items.Add(car.name);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void txtChanged_Validating(object sender, CancelEventArgs e)
        {
            DateTime result;
            e.Cancel = !DateTime.TryParse(txtChanged.Text, out result);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                result.Items.Clear();
                WorkTimeQuery query = new WorkTimeQuery();

                DateTime? date = GetLastModified();
                if (date.HasValue)
                    query.lastModified = date.Value;
                query.lastModifiedSpecified = date.HasValue;

                foreach (WorkTime workTime in Timr.GetWorkTimes(query))
                {
                    result.Items.Add(workTime.lastModifiedTime + ", " + workTime.description);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private DateTime? GetLastModified()
        {
            if (!String.IsNullOrEmpty(txtChanged.Text))
            {
                DateTime lastModified = DateTime.Parse(txtChanged.Text);
                DateTime lastModifiedUTC = new DateTime(lastModified.Year, lastModified.Month, lastModified.Day, lastModified.Hour, lastModified.Minute, lastModified.Second, DateTimeKind.Unspecified);

                return lastModifiedUTC;
            }
            return null;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                result.Items.Clear();
                foreach (User user in Timr.GetUsers(new Object()))
                {
                    result.Items.Add(user.firstname + " " + user.lastname);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void txtIdentifier_TextChanged(object sender, EventArgs e)
        {
            timr = null;
        }

        private void txtToken_TextChanged(object sender, EventArgs e)
        {
            timr = null;
        }
    }
}
