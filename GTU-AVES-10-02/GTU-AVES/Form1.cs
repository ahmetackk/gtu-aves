using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Drawing;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GTU_AVES
{
    public partial class MainForm : Form
    {
        private GMapOverlay markers = new GMapOverlay("markers");
        private GMapOverlay polygons = new GMapOverlay("polygons");

        private List<List<PointLatLng>> points = new List<List<PointLatLng>>();
        private List<List<List<Button>>> buttons = new List<List<List<Button>>>();
        private List<List<List<TextBox>>> txtboxes = new List<List<List<TextBox>>>();

        private int AreaCounter = 0;
        private int CurrentArea = 0;

        GMarkerGoogle clickedMarker = null;
        int clickedMarkerIndex = -1;

        double toleranceLat = 0.0005;
        double toleranceLng = 0.0005;

        public MainForm()
        {
            InitializeComponent();
            points.Add(new List<PointLatLng>());
            buttons.Add(new List<List<Button>>());
            txtboxes.Add(new List<List<TextBox>>());
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            //GMaps.Instance.Mode = AccessMode.ServerAndCache;
            //map.CacheLocation = @"C:\aves_cache";
            map.DragButton = MouseButtons.Right;
            map.MapProvider = GMapProviders.GoogleSatelliteMap;
            map.Position = new PointLatLng(40.812000, 29.360000);
            map.MinZoom = 1;
            map.MaxZoom = 20;
            map.Zoom = 17;
            map.Overlays.Add(markers);
            map.Overlays.Add(polygons);
        }

        private void updatePlanScreen()
        {
            polygons.Polygons.Clear();
            markers.Markers.Clear();

            for (int i = 0; i < points[CurrentArea].Count(); i++)
            {
                if (i == 0)
                {
                    AddLandMarker(points[CurrentArea][0]);
                }
                else
                {
                    AddNumberMarker(points[CurrentArea][i], i);
                }
            }

            List<PointLatLng> polygonlist = points[CurrentArea].Where((point, index) => index != 0).ToList();
            var polygon = new GMapPolygon(polygonlist, "Area")
            {
                Stroke = new Pen(Color.Green, 2),
                Fill = new SolidBrush(Color.FromArgb(100, Color.Yellow))
            };
            polygons.Polygons.Add(polygon);

        }

        private void AddNumberMarker(PointLatLng point, int number)
        {
            //GMapMarker marker = new GMarkerGoogle(point, new Bitmap("C:\\Users\\Ahmet Acıkök\\source\\repos\\GTU-AVES\\GTU-AVES\\Resources\\miniarrow1.png"));
            
            Bitmap markerBitmap = new Bitmap(30, 30); // Genişlik ve yükseklik belirleyin
            using (Graphics g = Graphics.FromImage(markerBitmap))
            {
                g.FillEllipse(Brushes.Green, 0, 0, markerBitmap.Width, markerBitmap.Height); // İşaretçi şekli ve rengini belirleyin
                g.DrawString(number.ToString(), new Font("Arial", 10), Brushes.White, new PointF(10, 8)); // İşaretçi üzerine yazı ekleyin
            }

            GMarkerGoogle marker = new GMarkerGoogle(new PointLatLng(point.Lat, point.Lng), markerBitmap);
            markers.Markers.Add(marker);

        }

        private void AddLandMarker(PointLatLng point)
        {
            Bitmap markerBitmap = new Bitmap(30, 30); // Genişlik ve yükseklik belirleyin
            using (Graphics g = Graphics.FromImage(markerBitmap))
            {
                g.FillEllipse(Brushes.Red, 0, 0, markerBitmap.Width, markerBitmap.Height); // İşaretçi şekli ve rengini belirleyin
                g.DrawString("L", new Font("Arial", 10), Brushes.White, new PointF(10, 8)); // İşaretçi üzerine yazı ekleyin
            }

            GMarkerGoogle marker = new GMarkerGoogle(new PointLatLng(point.Lat, point.Lng), markerBitmap);
            markers.Markers.Add(marker);
        }

        private void updateAreaPlan()
        {
            this.pnlAreaPlan.Controls.Clear();

            for (int i = 0; i < points[CurrentArea].Count(); i++)
            {
                this.pnlAreaPlan.Controls.Add(txtboxes[CurrentArea][i][0]);
                this.pnlAreaPlan.Controls.Add(txtboxes[CurrentArea][i][1]);
                this.pnlAreaPlan.Controls.Add(txtboxes[CurrentArea][i][2]);
                this.pnlAreaPlan.Controls.Add(buttons[CurrentArea][i][0]);
                this.pnlAreaPlan.Controls.Add(buttons[CurrentArea][i][1]);
                this.pnlAreaPlan.Controls.Add(buttons[CurrentArea][i][2]);
            }
            updatePlanScreen();
        }

        private void updateAreaPlanTexts()
        {
            for (int i = 0; i < points[CurrentArea].Count(); i++)
            {
                txtboxes[CurrentArea][i][1].Text = points[CurrentArea][i].Lat.ToString();
                txtboxes[CurrentArea][i][2].Text = points[CurrentArea][i].Lng.ToString();
            }
        }

        private void upIndex(int index)
        {
            string lat1 = txtboxes[CurrentArea][index][1].Text;
            string lng1 = txtboxes[CurrentArea][index][2].Text;

            txtboxes[CurrentArea][index][1].Text = txtboxes[CurrentArea][index - 1][1].Text;
            txtboxes[CurrentArea][index][2].Text = txtboxes[CurrentArea][index - 1][2].Text;

            txtboxes[CurrentArea][index - 1][1].Text = lat1;
            txtboxes[CurrentArea][index - 1][2].Text = lng1;

            PointLatLng tmppoint = points[CurrentArea][index];
            points[CurrentArea][index] = points[CurrentArea][index - 1];
            points[CurrentArea][index - 1] = tmppoint;

            updateAreaPlan();
        }

        private void downIndex(int index)
        {
            string lat1 = txtboxes[CurrentArea][index][1].Text;
            string lng1 = txtboxes[CurrentArea][index][2].Text;

            txtboxes[CurrentArea][index][1].Text = txtboxes[CurrentArea][index + 1][1].Text;
            txtboxes[CurrentArea][index][2].Text = txtboxes[CurrentArea][index + 1][2].Text;

            txtboxes[CurrentArea][index + 1][1].Text = lat1;
            txtboxes[CurrentArea][index + 1][2].Text = lng1;

            PointLatLng tmppoint = points[CurrentArea][index];
            points[CurrentArea][index] = points[CurrentArea][index + 1];
            points[CurrentArea][index + 1] = tmppoint;

            updateAreaPlan();
        }

        private void deleteIndex(int index)
        {
            buttons[CurrentArea].RemoveAt(index);
            txtboxes[CurrentArea].RemoveAt(index);
            points[CurrentArea].RemoveAt(index);

            for (int i = index; i < points[CurrentArea].Count(); i++)
            {
                buttons[CurrentArea][i][0].Name = "up_" + (i + 1).ToString();
                buttons[CurrentArea][i][1].Name = "down_" + (i + 1).ToString();
                buttons[CurrentArea][i][2].Name = "delete_" + (i + 1).ToString();

                txtboxes[CurrentArea][i][0].Name = "q_" + (i + 1).ToString();
                txtboxes[CurrentArea][i][0].Text = (i+1).ToString();
                txtboxes[CurrentArea][i][1].Name = "lat_" + (i + 1).ToString();
                txtboxes[CurrentArea][i][2].Name = "lon_" + (i + 1).ToString();
            }

            updateAreaPlan();
            updatePlanScreen();
        }

        private void UpButtonClick(object sender, EventArgs e)
        {
            Button clickedButton = sender as Button;
            if (clickedButton != null)
            {
                if (clickedButton.Name != "up_1")
                {
                    int index = int.Parse(clickedButton.Name.Split('_')[1]) - 1;
                    upIndex(index);
                }
            }
        }

        private void DownButtonClick(object sender, EventArgs e)
        {
            Button clickedButton = sender as Button;
            if (clickedButton != null)
            {
                if (clickedButton.Name != "down_" + points[CurrentArea].Count().ToString())
                {
                    int index = int.Parse(clickedButton.Name.Split('_')[1]) - 1;
                    downIndex(index);
                }
            }
        }

        private void DeleteButtonClick(object sender, EventArgs e)
        {
            Button clickedButton = sender as Button;
            if (clickedButton != null)
            {
                int index = int.Parse(clickedButton.Name.Split('_')[1]) - 1;
                deleteIndex(index);
            }
        }

        private void TextboxTextChange(object sender, KeyEventArgs e)
        {
            TextBox selectedtxt = sender as TextBox;
            if (selectedtxt != null)
            {
                if (e.KeyCode == Keys.Enter)
                {
                    int index = int.Parse(selectedtxt.Name.Split('_')[1]) - 1;
                    if (selectedtxt.Name.Split('_')[0] == "lat")
                    {
                        try
                        {
                            PointLatLng tmp = new PointLatLng(double.Parse(selectedtxt.Text), points[CurrentArea][index].Lng);
                            points[CurrentArea][index] = tmp;
                        }
                        catch { }
                    }
                    else if (selectedtxt.Name.Split('_')[0] == "lon")
                    {
                        try
                        {
                            PointLatLng tmp = new PointLatLng(points[CurrentArea][index].Lat, double.Parse(selectedtxt.Text));
                            points[CurrentArea][index] = tmp;
                        }
                        catch { }
                    }
                    updatePlanScreen();
                    e.Handled = true;
                }
            }
        }

        private void AddPlan(double latitude, double longtitude)
        {
            TextBox quequetxt = new TextBox();
            this.pnlAreaPlan.Controls.Add(quequetxt);
            quequetxt.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            quequetxt.Location = new System.Drawing.Point(0, points[CurrentArea].Count() * 42 + 3);
            quequetxt.Name = "q_" + points[CurrentArea].Count().ToString();
            quequetxt.Size = new System.Drawing.Size(28, 28);
            if (points[CurrentArea].Count() == 1)
            {
                quequetxt.Text = "L";
            }
            else
            {
                quequetxt.Text = (points[CurrentArea].Count() - 1).ToString();
            }
            quequetxt.Enabled = false;
            quequetxt.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;

            TextBox lattxt = new TextBox();
            this.pnlAreaPlan.Controls.Add(lattxt);
            lattxt.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            lattxt.Location = new System.Drawing.Point(34, points[CurrentArea].Count() * 42 + 3);
            lattxt.Name = "lat_" + points[CurrentArea].Count().ToString();
            lattxt.Size = new System.Drawing.Size(110, 28);
            lattxt.Text = latitude.ToString();
            lattxt.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            lattxt.KeyDown += new KeyEventHandler(TextboxTextChange);

            TextBox lngtxt = new TextBox();
            this.pnlAreaPlan.Controls.Add(lngtxt);
            lngtxt.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            lngtxt.Location = new System.Drawing.Point(156, points[CurrentArea].Count() * 42 + 3);
            lngtxt.Name = "lon_" + points[CurrentArea].Count().ToString();
            lngtxt.Size = new System.Drawing.Size(110, 28);
            lngtxt.Text = longtitude.ToString();
            lngtxt.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            lngtxt.KeyDown += new KeyEventHandler(TextboxTextChange);

            List<TextBox> temptxtlist = new List<TextBox>();
            temptxtlist.Add(quequetxt);
            temptxtlist.Add(lattxt);
            temptxtlist.Add(lngtxt);
            txtboxes[CurrentArea].Add(temptxtlist);

            Button up = new Button();
            this.pnlAreaPlan.Controls.Add(up);
            up.Image = global::GTU_AVES.Properties.Resources.miniarrow;
            up.Location = new System.Drawing.Point(272, points[CurrentArea].Count() * 42 + 3);
            up.Name = "up_" + points[CurrentArea].Count().ToString();
            up.Size = new System.Drawing.Size(28, 28);
            up.UseVisualStyleBackColor = true;
            up.Click += new EventHandler(UpButtonClick);

            Button down = new Button();
            this.pnlAreaPlan.Controls.Add(down);
            down.Image = global::GTU_AVES.Properties.Resources.miniarrow1;
            down.Location = new System.Drawing.Point(306, points[CurrentArea].Count() * 42 + 3);
            down.Name = "down_" + points[CurrentArea].Count().ToString();
            down.Size = new System.Drawing.Size(28, 28);
            down.UseVisualStyleBackColor = true;
            down.Click += new EventHandler(DownButtonClick);

            Button delete = new Button();
            this.pnlAreaPlan.Controls.Add(delete);
            delete.Image = global::GTU_AVES.Properties.Resources.carpi;
            delete.Location = new System.Drawing.Point(340, points[CurrentArea].Count() * 42 + 3);
            delete.Name = "delete_" + points[CurrentArea].Count().ToString();
            delete.Size = new System.Drawing.Size(28, 28);
            delete.UseVisualStyleBackColor = true;
            delete.Click += new EventHandler(DeleteButtonClick);

            List<Button> tempbuttonlist = new List<Button>();   
            tempbuttonlist.Add(up);
            tempbuttonlist.Add(down);
            tempbuttonlist.Add(delete);
            buttons[CurrentArea].Add(tempbuttonlist);

        }

        private void map_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {

                if (clickedMarker == null)
                {
                    var point = map.FromLocalToLatLng(e.X, e.Y);
                    PointLatLng markpoint = new PointLatLng(point.Lat, point.Lng);

                    if (points[CurrentArea].Count() == 0)
                    {
                        AddLandMarker(markpoint);
                    }
                    else
                    {
                        AddNumberMarker(markpoint, points[CurrentArea].Count());
                    }
                    points[CurrentArea].Add(markpoint);
                    AddPlan(point.Lat, point.Lng);
                }
                
                if (markers.Markers.Count() >= 3)
                {
                    updatePlanScreen();
                }

            }
        }
        private void map_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                for (int i = 0; i < markers.Markers.Count; i++)
                {
                    GMarkerGoogle marker = (GMarkerGoogle)markers.Markers[i];
                    PointLatLng markerPosition = marker.Position;
                    PointLatLng tmppoint = map.FromLocalToLatLng(e.X, e.Y);

                    if (Math.Abs(markerPosition.Lat - tmppoint.Lat) < toleranceLat &&
                        Math.Abs(markerPosition.Lng - tmppoint.Lng) < toleranceLng)
                    {
                        clickedMarkerIndex = i;
                        clickedMarker = marker;
                    }
                }
            }
        }

        private void map_MouseMove(object sender, MouseEventArgs e)
        {
            if (clickedMarker != null)
            {
                PointLatLng tmppoint = map.FromLocalToLatLng(e.X, e.Y);
                points[CurrentArea][clickedMarkerIndex] = tmppoint;
                clickedMarker.Position = tmppoint;

                updateAreaPlanTexts();

                map.Refresh();
            }
        }

        private void map_MouseUp(object sender, MouseEventArgs e)
        {
            clickedMarker = null;
            clickedMarkerIndex = -1;
        }


        private void btnClearPolygon_Click(object sender, EventArgs e)
        {
            this.pnlAreaPlan.Controls.Clear();
            polygons.Polygons.Clear();
            markers.Markers.Clear();

            if (AreaCounter != 0)
            {
                cmbAreaSelect.Items.RemoveAt(cmbAreaSelect.Items.Count - 1);

                points.RemoveAt(CurrentArea);
                buttons.RemoveAt(CurrentArea);
                txtboxes.RemoveAt(CurrentArea);

                AreaCounter--;
                CurrentArea = AreaCounter;
                cmbAreaSelect.SelectedIndex = cmbAreaSelect.Items.Count - 1;

                updateAreaPlan();
                updatePlanScreen();
            }
            else
            {
                points[CurrentArea].Clear();
                buttons[CurrentArea].Clear();
                txtboxes[CurrentArea].Clear();
            }
        }

        private void btnNewPolygon_Click(object sender, EventArgs e)
        {
            AreaCounter++;
            CurrentArea++;

            points.Add(new List<PointLatLng>());
            buttons.Add(new List<List<Button>>());
            txtboxes.Add(new List<List<TextBox>>());

            this.pnlAreaPlan.Controls.Clear();
            polygons.Polygons.Clear();
            markers.Markers.Clear();

            cmbAreaSelect.Items.Add(AreaCounter + 1);
            cmbAreaSelect.SelectedIndex = cmbAreaSelect.Items.Count - 1;
        }

        private void cmbAreaSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            CurrentArea = int.Parse(cmbAreaSelect.SelectedItem.ToString()) - 1;
            updateAreaPlan();
            updatePlanScreen();
        }

        
    }
}
