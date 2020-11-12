using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Diagnostics;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;

namespace WebScraper_Zenab_o_Madeleine
{
    public partial class Form1 : Form
    {
       

        public Form1()
        {
            InitializeComponent();
            
        }


        private HttpClient client = new HttpClient();
        private Stopwatch restart = new Stopwatch();
        private Dictionary<Task<byte[]>, string> allImageBytes = new Dictionary<Task<byte[]>, string>();
        



        private async void buttonExtract_Click(object sender, EventArgs e)
        {
            listBox1.Items.Clear();

            string pattern = @"(?<=<img.*src="")[^""]+(?="".*)";
            string input = URLInput.Text;

            if (!input.StartsWith("http"))
            {
               
                input = $"http://{input}";
            }


            string website = await client.GetStringAsync(input);
            MatchCollection matches = Regex.Matches(website, pattern, RegexOptions.IgnoreCase);

            foreach (Match match in matches)
            {
                if (!match.Value.StartsWith("http", StringComparison.InvariantCultureIgnoreCase))
                {
                    listBox1.Items.Add(input + match.Value);
                }
                else
                {
                    listBox1.Items.Add(match.Value);
                }
            }

            if (listBox1.Items.Count != 0)
            {
                buttonSave.Enabled = true;
              
                labelNumberOfImages.Text = $"Found {listBox1.Items.Count} images.";
            }
        }


        private async void buttonSave_Click(object sender, EventArgs e)
        {
                       

            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
               string directory = folderBrowserDialog1.SelectedPath;
                Directory.CreateDirectory(directory);

                await download(directory);
            }        

        }

        private int map;

        private async Task download(string directory)
        {
          

            foreach (string same in listBox1.Items)
            {
              
                Task<byte[]> imageTask = client.GetByteArrayAsync(same);

                allImageBytes.Add(imageTask, same);
            }


            while (allImageBytes.Count > 0)
            {
                Task<byte[]> completedTask = null;

                map = 0;
                
                    completedTask = await Task.WhenAny(allImageBytes.Keys);
                
                    Uri fileURL = new Uri(allImageBytes[completedTask]);

                    using (FileStream fileStream = File.Open(Path.Combine(directory, map + Path.GetFileName(fileURL.AbsolutePath)), FileMode.Create))
                    {
                        await fileStream.WriteAsync(completedTask.Result, 0, completedTask.Result.Length);                       
                      
                    }  


                    allImageBytes.Remove(completedTask);
                
                
                    if (completedTask != null)
                    {
                        allImageBytes.Remove(completedTask);
                    }
                
            }
        }
       
       

       
        private void URLInput_TextChanged(object sender, EventArgs e) { }
        private void label1_Click(object sender, EventArgs e) { }

        private void folderBrowserDialog1_HelpRequest(object sender, EventArgs e)
        {

        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void listBox1_SelectedIndexChanged_1(object sender, EventArgs e)
        {

        }
    }

}

