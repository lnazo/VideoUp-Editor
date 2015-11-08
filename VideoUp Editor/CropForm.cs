using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace VideoUp
{
    public partial class CropForm : Form
    {
        private Corner _heldCorner = Corner.None;
        private bool _held;

        private bool _insideForm;
        private bool _insideRectangle;
        private Point _mousePos;
        private Point _mouseOffset;

        public static readonly RectangleF FullCrop = new RectangleF(0, 0, 1, 1);
        private RectangleF _rectangle;

        private const int MaxDistance = 6;
        private Font _font = new Font(FontFamily.GenericSansSerif, 11f);

        private FFmpeg _ffmpegProcess;
        private MainForm _owner;

        private string _previewFile = Path.Combine(Environment.CurrentDirectory, "tempPreview.png");
        private bool _generating;
        private string _message;
        private Image _image;

        private enum Corner
        {
            TopLeft,
            TopRight,
            BottomLeft,
            BottomRight,
            None,
        }

        /// <summary>
        /// Fetches the cropping information for a video file from the MainForm.cs class
        /// </summary>
        /// <param name="mainForm">the object that opens this class.>/param>
        public CropForm(MainForm mainForm)
        {
            _owner = mainForm;
            // ensures the user knows where the dots are
            _rectangle = new RectangleF(0.25f, 0.25f, 0.5f, 0.5f);

            InitializeComponent();

            if (mainForm.CroppingRectangle != FullCrop)
                _rectangle = mainForm.CroppingRectangle;
        }

        /// <summary>
        /// Loads the crop form with a preview
        /// </summary>
        private void CropForm_Load(object sender, EventArgs e)
        {
            GeneratePreview();
        }

        /// <summary>
        /// Generates a preview based on user specification
        /// </summary>
        private void GeneratePreview()
        {
            string argument = ConstructArguments();

            if (string.IsNullOrWhiteSpace(argument))
                return;

            _generating = true;
            _ffmpegProcess = new FFmpeg(argument);

            _ffmpegProcess.Process.ErrorDataReceived += (sender, args) => Console.WriteLine(args.Data);
            _ffmpegProcess.Process.OutputDataReceived += (sender, args) => Console.WriteLine(args.Data);

            _ffmpegProcess.Process.Exited += (o, args) => pictureBoxVideo.Invoke((Action)(() =>
            {
                _generating = false;

                int exitCode = _ffmpegProcess.Process.ExitCode;

                if (exitCode != 0)
                {
                    _message = string.Format("An error occurred with the code {0} ", exitCode);
                    return;
                }

                if (!File.Exists(_previewFile))
                {
                    _message = "The preview file did not generate. Please check your inputs";
                    return;
                }

                try
                {
                    using (FileStream stream = new FileStream(_previewFile, FileMode.Open, FileAccess.Read))
                        _image = Image.FromStream(stream);

                    pictureBoxVideo.BackgroundImage = _image;
                    File.Delete(_previewFile);

                    _owner.AssumedInputSize = _image.Size;

                    float aspectRatio = _image.Width / (float)_image.Height;
                    ClientSize = new Size((int)(ClientSize.Height * aspectRatio), ClientSize.Height);
                }
                catch (Exception e)
                {
                    _message = e.ToString();
                }
            }));

            _ffmpegProcess.Process.Start();
        }

        /// <summary>
        /// Constructs arguments based on user specification
        /// </summary>
        private string ConstructArguments()
        {
            string template = "{1} -i \"{0}\" -f image2 -vframes 1 -y \"{2}\"";

            string input = _owner.textBoxIn.Text;
            if (string.IsNullOrWhiteSpace(input))
            {
                _message = "No input file";
                return null;
            }
            if (!File.Exists(input))
            {
                _message = "Input file doesn't exist";
                return null;
            }

            float time = 0.0f;
            try
            {
                time = MainForm.ParseTime(_owner.boxCropFrom.Text);
            }
            catch (Exception)
            {
                
            }

            _message = string.Format("Preview {0}", TimeSpan.FromSeconds(time));
            if (time == 0.0f)
                _message += "\nTo preview at a different time, change trim start time";

            return string.Format(template, input, "-ss " + time, _previewFile);
        }

        /// <summary>
        /// Checks distance from rectangle corner point to the mouse, and then selects the one with the smallest
        /// distance that will be dragged along with the mouse
        /// </summary>
        private void pictureBoxVideo_MouseDown(object sender, MouseEventArgs e)
        {
            var closest = GetClosestPointDistance(new Point(e.X, e.Y));

            // Comparing squared distance
            if (closest.Value < MaxDistance * MaxDistance)
            {
                _heldCorner = closest.Key;
                _held = true;

            }

            // If there's no closest dot and the mouse is inside the cropping rectangle, drag the entire rectangle
            else if (_insideRectangle)
            {
                _mouseOffset = new Point((int)(_rectangle.X * pictureBoxVideo.Width - e.X), (int)(_rectangle.Y * pictureBoxVideo.Height - e.Y));
                _heldCorner  = Corner.None;
                _held = true;
            }


            pictureBoxVideo.Invalidate();
        }

        /// <summary>
        /// Calculates distance between points
        /// </summary>
        private KeyValuePair<Corner, float> GetClosestPointDistance(Point e)
        {
            var distances = new Dictionary<Corner, float>();
            distances[Corner.TopLeft] = (float)(Math.Pow(e.X - _rectangle.Left * pictureBoxVideo.Width, 2) + Math.Pow(e.Y - _rectangle.Top * pictureBoxVideo.Height, 2));
            distances[Corner.TopRight] = (float)(Math.Pow(e.X - _rectangle.Right * pictureBoxVideo.Width, 2) + Math.Pow(e.Y - _rectangle.Top * pictureBoxVideo.Height, 2));
            distances[Corner.BottomLeft] = (float)(Math.Pow(e.X - _rectangle.Left * pictureBoxVideo.Width, 2) + Math.Pow(e.Y - _rectangle.Bottom * pictureBoxVideo.Height, 2));
            distances[Corner.BottomRight] = (float)(Math.Pow(e.X - _rectangle.Right * pictureBoxVideo.Width, 2) + Math.Pow(e.Y - _rectangle.Bottom * pictureBoxVideo.Height, 2));

            return distances.OrderBy(a => a.Value).First();

        }

        /// <summary>
        /// Checks distance from rectangle corner point to the mouse, and then selects the one with the smallest
        /// distance that will be dragged along with the mouse
        /// </summary>
        private void pictureBoxVideo_MouseUp(object sender, MouseEventArgs e)
        {
            _held = false;
            _heldCorner = Corner.None;
            pictureBoxVideo.Invalidate();
        }

        private void pictureBoxVideo_MouseMove(object sender, MouseEventArgs e)
        {
            _mousePos = new Point(e.X, e.Y);
            _insideRectangle = _rectangle.Contains(e.X / (float)pictureBoxVideo.Width, e.Y / (float)pictureBoxVideo.Height);

           if (_held)
            {
                // Change the size of the rectangle if the mouse is actually held down

                // Clamp mouse position to picture box in order to prevent moving the cropping rectangle out of bounds
                Point min = new Point(0, 0);
                Point max = new Point(pictureBoxVideo.Size);
                float clampedMouseX = Math.Max(min.X, Math.Min(max.X, e.X));
                float clampedMouseY = Math.Max(min.Y, Math.Min(max.Y, e.Y));

                float newWidth = 0;
                float newHeight = 0;
                switch (_heldCorner)
                {
                    case Corner.TopLeft:
                        newWidth = _rectangle.Width - (clampedMouseX / (float)pictureBoxVideo.Width - _rectangle.X);
                        newHeight = _rectangle.Height - (clampedMouseY / (float)pictureBoxVideo.Height - _rectangle.Y);
                        _rectangle.X = clampedMouseX / (float)pictureBoxVideo.Width;
                        _rectangle.Y = clampedMouseY / (float)pictureBoxVideo.Height;
                        break;

                    case Corner.TopRight:
                        newWidth = _rectangle.Width + (clampedMouseX / (float)pictureBoxVideo.Width - _rectangle.Right);
                        newHeight = _rectangle.Height - (clampedMouseY / (float)pictureBoxVideo.Height - _rectangle.Y);
                        _rectangle.Y = clampedMouseY / (float)pictureBoxVideo.Height;
                        break;

                    case Corner.BottomLeft:
                        newWidth = _rectangle.Width - (clampedMouseX / (float)pictureBoxVideo.Width - _rectangle.X);
                        newHeight = _rectangle.Height + (clampedMouseY / (float)pictureBoxVideo.Height - _rectangle.Bottom);
                        _rectangle.X = clampedMouseX / (float)pictureBoxVideo.Width;
                        break;

                    case Corner.BottomRight:
                        newWidth = _rectangle.Width + (clampedMouseX / (float)pictureBoxVideo.Width - _rectangle.Right);
                        newHeight = _rectangle.Height + (clampedMouseY / (float)pictureBoxVideo.Height - _rectangle.Bottom);
                        break;

                    // drag entire rectangle
                    case Corner.None:
                        float actualRectW = _rectangle.Width * pictureBoxVideo.Width;
                        float actualRectH = _rectangle.Height * pictureBoxVideo.Height;
                        clampedMouseX = Math.Max(min.X - _mouseOffset.X, Math.Min(max.X - _mouseOffset.X - actualRectW, e.X));
                        clampedMouseY = Math.Max(min.Y - _mouseOffset.Y, Math.Min(max.Y - _mouseOffset.Y - actualRectH, e.Y));
                        _rectangle.X = (clampedMouseX + _mouseOffset.X) / (float)pictureBoxVideo.Width;
                        _rectangle.Y = (clampedMouseY + _mouseOffset.Y) / (float)pictureBoxVideo.Height;
                        break;
                }

                if (newWidth != 0)
                    _rectangle.Width = newWidth;
                if (newHeight != 0)
                    _rectangle.Height = newHeight;
            }
            
            pictureBoxVideo.Invalidate();
        }

        private void pictureBoxVideo_Paint(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;

            var edgePen = new Pen(Color.White, 1f);
            var dotBrush = new SolidBrush(Color.White);
            var outsideBrush = new HatchBrush(HatchStyle.Percent50, Color.Transparent);

            var maxW = pictureBoxVideo.Width;
            var maxH = pictureBoxVideo.Height;
            var x = _rectangle.X * pictureBoxVideo.Width;
            var y = _rectangle.Y * pictureBoxVideo.Height;
            var w = _rectangle.Width * maxW;
            var h = _rectangle.Height * maxH;

            g.FillRectangle(outsideBrush, 0, 0, maxW, y);
            g.FillRectangle(outsideBrush, 0, y, x, h);
            g.FillRectangle(outsideBrush, x + w, y, maxW - (x + w), h);
            g.FillRectangle(outsideBrush, 0, y + h, maxW, maxH);

            g.DrawRectangle(edgePen, x, y, w, h);

            if (_insideForm)
            {
                float diameter = 6;
                float diameterEdge = diameter * 2;

                g.FillEllipse(dotBrush, x - diameter / 2, y - diameter / 2, diameter, diameter);
                g.FillEllipse(dotBrush, x + w - diameter / 2, y - diameter / 2, diameter, diameter);
                g.FillEllipse(dotBrush, x - diameter / 2, y + h - diameter / 2, diameter, diameter);
                g.FillEllipse(dotBrush, x + w - diameter / 2, y + h - diameter / 2, diameter, diameter);

                var closest = GetClosestPointDistance(_mousePos);

                // Comparing squared distance to avoid worthless square roots
                if (closest.Value < MaxDistance * MaxDistance)
                {
                    Cursor = Cursors.Hand;
                    if (closest.Key == Corner.TopLeft) g.DrawEllipse(edgePen, x - diameterEdge / 2, y - diameterEdge / 2, diameterEdge, diameterEdge);
                    if (closest.Key == Corner.TopRight) g.DrawEllipse(edgePen, x + w - diameterEdge / 2, y - diameterEdge / 2, diameterEdge, diameterEdge);
                    if (closest.Key == Corner.BottomLeft) g.DrawEllipse(edgePen, x - diameterEdge / 2, y + h - diameterEdge / 2, diameterEdge, diameterEdge);
                    if (closest.Key == Corner.BottomRight) g.DrawEllipse(edgePen, x + w - diameterEdge / 2, y + h - diameterEdge / 2, diameterEdge, diameterEdge);
                }
                else if (_insideRectangle)
                    Cursor = Cursors.SizeAll;
                else if (Cursor != Cursors.Default)
                    Cursor = Cursors.Default;
            }

            if (_generating)
            {
                for (int i = 0; i < 2; i++)
                    g.DrawString("Generating preview...", _font, new SolidBrush(Color.FromArgb(i * 255, i * 255, i * 255)), 5, 5 - i);
            }
            else if (!string.IsNullOrWhiteSpace(_message))
            {
                for (int i = 0; i < 2; i++)
                    g.DrawString(_message, _font, new SolidBrush(Color.FromArgb(i * 255, i * 255, i * 255)), 5, 5 - i);
            }
        }

        private void pictureBoxVideo_MouseEnter(object sender, EventArgs e)
        {
            _insideForm = true;
            pictureBoxVideo.Invalidate();
        }

        private void pictureBoxVideo_MouseLeave(object sender, EventArgs e)
        {
            _insideForm = false;
            pictureBoxVideo.Invalidate();
        }

        private void pictureBoxVideo_Resize(object sender, EventArgs e)
        {
            pictureBoxVideo.Invalidate();
        }

        private void buttonReset_Click(object sender, EventArgs e)
        {
            _rectangle = FullCrop;
            pictureBoxVideo.Invalidate();
        }

        private void buttonConfirm_Click(object sender, EventArgs e)
        {
            if (_rectangle.Left >= _rectangle.Right || _rectangle.Top >= _rectangle.Bottom)
            {
                MessageBox.Show("Check your crop. Press the reset button and try again.", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            float tolerance = 0.1f;

            if (_rectangle.Left < 0 - tolerance || _rectangle.Top < 0 - tolerance || _rectangle.Right > 1 + tolerance || _rectangle.Bottom > 1 + tolerance)
            {
                MessageBox.Show("Your crop is outside the valid range. Press the reset button and try again.", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            _rectangle.X = Math.Max(0, _rectangle.X);
            _rectangle.Y = Math.Max(0, _rectangle.Y);
            if (_rectangle.Right > 1)
                _rectangle.Width = 1 - _rectangle.X;
            if (_rectangle.Bottom > 1)
                _rectangle.Height = 1 - _rectangle.Y;

            DialogResult = DialogResult.OK;
            _owner.CroppingRectangle = _rectangle;

            Close();
        }
    }
}
