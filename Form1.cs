
using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using WinFormsClass;

namespace WinFormsApp1
{

    public partial class Form1 : Form
    {
        GMapOverlay markerOverlay = new GMapOverlay("markerLayer");
        GMarkerGoogle marker1 = null;
        GMarkerGoogle marker2 = null;

        public Form1()
        {
            InitializeComponent();
            HaritayiBaslat();
        }



        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Length == 0 || textBox2.Text.Length == 0 || textBox3.Text.Length == 0 || textBox4.Text.Length == 0 || textBox5.Text.Length == 0
                || (comboBox1.Text != "Öðrenci" && comboBox1.Text != "Genel" && comboBox1.Text != "Yaþlý") ||
                (comboBox2.Text != "Nakit" && comboBox2.Text != "Kredi Kartý" && comboBox2.Text != "Kentkart"))
            {

                if (textBox5.Text.Length == 0)
                {
                    MessageBox.Show("Bakiye eksik tanýmlandý !");
                }
                else
                {
                    MessageBox.Show("Boþ Býrakýlamaz ! ");
                }
            }
            else
            {
                textBox1.Text = ConvertTextToDouble(textBox1.Text);
                double start_lat = double.Parse(textBox1.Text);
                textBox2.Text = ConvertTextToDouble(textBox2.Text);
                double start_lon = double.Parse(textBox2.Text);
                textBox3.Text = ConvertTextToDouble(textBox3.Text);
                double route_lat = double.Parse(textBox3.Text);
                textBox4.Text = ConvertTextToDouble(textBox4.Text);
                double route_lon = double.Parse(textBox4.Text);
                double bakiye = double.Parse(textBox5.Text);
                string yolcu = comboBox1.Text;
                string odeme = comboBox2.Text;

                Form2 form2 = new Form2(start_lat, start_lon, route_lat, route_lon, bakiye, yolcu, odeme);
                form2.Show();
                this.Hide();
            }

        }
        private void HaritayiBaslat()
        {
            gMapControl1.DragButton = MouseButtons.Left;
            gMapControl1.CanDragMap = true;
            gMapControl1.MapProvider = GMapProviders.OpenStreetMap;
            gMapControl1.Position = new PointLatLng(40.76524, 29.92461);
            gMapControl1.MinZoom = 1;
            gMapControl1.MaxZoom = 20;
            gMapControl1.Zoom = 13;
            gMapControl1.AutoScroll = true;
        }
        private string ConvertTextToDouble(string input)
        {

            if (input.Contains('.'))
            {
                input = input.Replace('.', ',');
            }
            return input;
        }
        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != '.' && e.KeyChar != ',' && e.KeyChar != (char)Keys.Back)
            {
                e.Handled = true;
            }

            if ((e.KeyChar == '.' || e.KeyChar == ',') && (textBox1.Text.Contains(".") || textBox1.Text.Contains(",")))
            {
                e.Handled = true;
            }
        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != '.' && e.KeyChar != ',' && e.KeyChar != (char)Keys.Back)
            {
                e.Handled = true;
            }

            if ((e.KeyChar == '.' || e.KeyChar == ',') && (textBox2.Text.Contains(".") || textBox2.Text.Contains(",")))
            {
                e.Handled = true;
            }
        }
        private void textBox3_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != '.' && e.KeyChar != ',' && e.KeyChar != (char)Keys.Back)
            {
                e.Handled = true;
            }

            if ((e.KeyChar == '.' || e.KeyChar == ',') && (textBox3.Text.Contains(".") || textBox3.Text.Contains(",")))
            {
                e.Handled = true;
            }
        }

        private void textBox4_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != '.' && e.KeyChar != ',' && e.KeyChar != (char)Keys.Back)
            {
                e.Handled = true; 
            }


            if ((e.KeyChar == '.' || e.KeyChar == ',') && (textBox4.Text.Contains(".") || textBox4.Text.Contains(",")))
            {
                e.Handled = true;
            }
        }

        private void textBox5_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != '.' && e.KeyChar != ',' && e.KeyChar != (char)Keys.Back)
            {
                e.Handled = true; 
            }


            if ((e.KeyChar == '.' || e.KeyChar == ',') && (textBox5.Text.Contains(".") || textBox5.Text.Contains(",")))
            {
                e.Handled = true;
            }
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.Control && e.KeyCode == Keys.C))
            {
                return;
            }
        }

        private void textBox2_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.Control && e.KeyCode == Keys.C))
            {
                return;
            }
        }

        private void textBox3_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.Control && e.KeyCode == Keys.C))
            {
                return;
            }
        }

        private void textBox4_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.Control && e.KeyCode == Keys.C))
            {
                return;
            }
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked)
            {
                checkBox1.Checked = false;
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                checkBox2.Checked = false;
            }
        }

        private void gMapControl1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && checkBox1.Checked)
            {
                var point = gMapControl1.FromLocalToLatLng(e.X, e.Y);
                double lat = point.Lat;
                double lng = point.Lng;
                textBox1.Text = lat.ToString();
                textBox2.Text = lng.ToString();

                if (marker1 != null)
                    markerOverlay.Markers.Remove(marker1);

                marker1 = new GMarkerGoogle(point, GMarkerGoogleType.purple_dot);
                markerOverlay.Markers.Add(marker1);

                gMapControl1.Refresh();

            }
            else if (e.Button == MouseButtons.Left && checkBox2.Checked)
            {
                var point = gMapControl1.FromLocalToLatLng(e.X, e.Y);
                double lat = point.Lat;
                double lng = point.Lng;
                textBox3.Text = lat.ToString();
                textBox4.Text = lng.ToString();

                if (marker2 != null)
                    markerOverlay.Markers.Remove(marker2);

                marker2 = new GMarkerGoogle(point, GMarkerGoogleType.yellow_dot);
                markerOverlay.Markers.Add(marker2);

                gMapControl1.Refresh();

            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            gMapControl1.Overlays.Add(markerOverlay);
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }
    }
}
