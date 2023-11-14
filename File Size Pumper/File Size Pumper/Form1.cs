using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace File_Size_Pumper
{
    public partial class Form1 : Form
    {
        private string selectedFileName;

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }

        private void guna2Button2_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Title = "Open File";
                openFileDialog.Filter = "All Files (*.*)|*.*";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    selectedFileName = openFileDialog.FileName;
                    textBox1.Text = selectedFileName;
                }
            }
        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(selectedFileName))
            {
                MessageBox.Show("Please select a file first.");
                return;
            }

            if (!double.TryParse(guna2TextBox1.Text, out double megabytesToAdd))
            {
                MessageBox.Show("Invalid input for megabytes.");
                return;
            }

            try
            {
                // Convert megabytes to bytes
                long bytesToAdd = (long)(megabytesToAdd * 1024 * 1024);

                // Open the file in append mode
                using (FileStream fileStream = new FileStream(selectedFileName, FileMode.Append))
                {
                    // Write the data to the end of the file
                    fileStream.Seek(0, SeekOrigin.End);
                    fileStream.Write(new byte[bytesToAdd], 0, (int)bytesToAdd);
                }

                MessageBox.Show("Bytes added to the file successfully.");

                if (guna2ToggleSwitch1.Checked)
                {
                    ChangeIcon();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private void guna2Button3_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Title = "Open File";
                openFileDialog.Filter = "Icon (*.ico)|*.ico";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    selectedFileName = openFileDialog.FileName;

                    if (Path.GetExtension(selectedFileName).Equals(".ico", StringComparison.OrdinalIgnoreCase))
                    {
                        // Valid icon file selected
                    }
                    else
                    {
                        MessageBox.Show("Please select a valid ICO file.", "Invalid File", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        selectedFileName = string.Empty; // Reset the file name
                    }
                }
            }
        }

        private void ChangeIcon()
        {
            try
            {
                // Load the new icon from the specified file
                Icon newIcon = new Icon(selectedFileName);

                // Change the application's icon
                ChangeIcon(selectedFileName, newIcon);

                MessageBox.Show("Icon changed successfully.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private void ChangeIcon(string selectedFileName, Icon newIcon)
        {
            string tempIconPath = Path.Combine(Path.GetTempPath(), "tempIcon.ico");

            try
            {
                // Save the new icon to a temporary file
                using (FileStream fs = new FileStream(tempIconPath, FileMode.Create))
                {
                    newIcon.Save(fs);
                }

                // Wait for a while to make sure the file is released by the Icon.Save operation
                System.Threading.Thread.Sleep(1000);

                // Update the application's icon
                using (var iconUpdater = new BinaryWriter(new FileStream(selectedFileName, FileMode.Open, FileAccess.Write)))
                {
                    const int iconOffset = 22; // The offset where the icon data starts

                    using (var iconReader = new BinaryReader(File.OpenRead(tempIconPath)))
                    {
                        iconReader.BaseStream.Seek(iconOffset, SeekOrigin.Begin);
                        iconUpdater.BaseStream.Seek(iconOffset, SeekOrigin.Begin);

                        byte[] iconData = iconReader.ReadBytes(256); // assuming the icon data size is 256 bytes
                        iconUpdater.Write(iconData);
                    }
                }

                MessageBox.Show("Icon changed successfully.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
            finally
            {
                // Clean up the temporary icon file
                File.Delete(tempIconPath);
            }
        }

        private void guna2ToggleSwitch1_CheckedChanged(object sender, EventArgs e)
        {
            if (guna2ToggleSwitch1.Checked == true)
            {
                guna2Button3.Enabled = true;
            }
            if (guna2ToggleSwitch1.Checked == false)
            {
                guna2Button3.Enabled = false;
            }
        }
    }
}
