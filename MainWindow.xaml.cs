using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Diagnostics;
using System.Threading;
using System.Windows.Threading;

namespace obj_loader
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Point3DCollection vertex = new Point3DCollection();
        PointCollection tex_coords = new PointCollection();
        Int32Collection faces = new Int32Collection();
        MaterialGroup mats = new MaterialGroup();
        Vector3DCollection norms = new Vector3DCollection();
        
        Vector3D cam_vec = new Vector3D() {X = 0, Y = 0, Z=-0.1};
        Point3D cam_pos = new Point3D() {X=0, Y=0, Z=3.5};
        PerspectiveCamera cam;
        ModelVisual3D light = new ModelVisual3D() {Content = new DirectionalLight() {Color = Colors.White, Direction = new Vector3D(-1, -1, -2)}};
        ModelVisual3D obj3D;
        TextBlock position = new TextBlock();



         public MainWindow()
        {
            

            InitializeComponent();
            //obj3D_verts.Add(new Point3D(-0.5, 0, 0));
            //obj3D_verts.Add(new Point3D(0, 0.5, 0));
            //obj3D_verts.Add(new Point3D(0.5, 0, 0));
            this.KeyDown += MainWindow_KeyDown;
            LoadConfig("untitled.obj");
            //LoadConfig("461.obj");
            //LoadConfig("906.obj");
            position.Text = cam_pos.ToString();
            obj3D = new ModelVisual3D()
            {
                Content = new GeometryModel3D()
                {
                    Geometry = new MeshGeometry3D(){Positions = vertex, TriangleIndices = faces, Normals = norms, TextureCoordinates = tex_coords}, 
                    Material = mats
                }
            };
            cam = new PerspectiveCamera() {Position = cam_pos, LookDirection = cam_vec };
            vp3D.Camera = cam;
            vp3D.Children.Add(light);
            vp3D.Children.Add(obj3D);
            
            gridok.Children.Add(position);
           
               
              


        }
        public void LoadConfig(string path)
        {
            using (StreamReader sr = new StreamReader($"{Directory.GetCurrentDirectory()}\\{path}", System.Text.Encoding.Default))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                   
                    string[] user_command = line.Split();
                    switch (user_command[0])
                    {
                        case "vt":
                            tex_coords.Add(new Point(double.Parse(user_command[1]), double.Parse(user_command[2])));
                            break;
                        case "v":
                            vertex.Add(new Point3D(double.Parse(user_command[1]), double.Parse(user_command[2]),
                                double.Parse(user_command[3])));
                            break;
                        case "f":

                            foreach (var i in user_command){
                                if (i != "f" && i!="")
                                {
                                    
                                    faces.Add(Int32.Parse(i) - 1);
                                }
                            }
                            
                                break;
                        case "vn":
                            norms.Add(new Vector3D(Int32.Parse(user_command[1]), Int32.Parse(user_command[2]),
                                Int32.Parse(user_command[3])));
                            break;
                        case "dm":
                            if (user_command[1].Contains(".png") || user_command[1].Contains(".jpg"))
                            {
                                mats.Children.Add(new DiffuseMaterial()
                                {
                                    Brush = new ImageBrush(new BitmapImage(
                                        new Uri(@user_command[1], UriKind.RelativeOrAbsolute)))

                                });
                            }
                            else
                            {
                                mats.Children.Add(new DiffuseMaterial()
                                {
                                    Brush = (SolidColorBrush) new BrushConverter().ConvertFromString(user_command[1])
                                });
                            }

                            break;
                        case "sm":
                            if (user_command[1].Contains(".png") || user_command[1].Contains(".jpg"))
                            {
                                mats.Children.Add(new SpecularMaterial()
                                {
                                    Brush  = new ImageBrush(new BitmapImage(
                                        new Uri(@user_command[1], UriKind.RelativeOrAbsolute))),

                                    SpecularPower = Int32.Parse(user_command[2])
                                });
                            }
                            else
                            {
                                mats.Children.Add(new SpecularMaterial()
                                {
                                    Brush = (SolidColorBrush) new BrushConverter().ConvertFromString(user_command[1]),
                                    SpecularPower = Int32.Parse(user_command[2])
                                });
                            }

                            break;
                        case "em":
                            if (user_command[1].Contains(".png") || user_command[1].Contains(".jpg"))
                            {
                                mats.Children.Add(new EmissiveMaterial()
                                {
                                    Brush  = new ImageBrush(new BitmapImage(
                                        new Uri(@user_command[1], UriKind.RelativeOrAbsolute)))

                                });
                            }
                            else
                            {
                                mats.Children.Add(new EmissiveMaterial()
                                {
                                    Brush = (SolidColorBrush) new BrushConverter().ConvertFromString(user_command[1]),

                                });
                            }

                            break;
                    }



                }
            }
        }

        public async void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key==Key.W)
            {
                cam_pos.Y += 0.1;
                cam.Position = cam_pos;
                position.Text = cam_pos.ToString();
            }
            if (e.Key==Key.A)
            {
                cam_pos.X -= 0.1;
                cam.Position = cam_pos;
                position.Text = cam_pos.ToString();
            }
            if (e.Key==Key.S)
            {
                cam_pos.Y -= 0.1;
                cam.Position = cam_pos;
                position.Text = cam_pos.ToString();
            }
            if (e.Key==Key.D)
            {
                cam_pos.X += 0.1;
                cam.Position = cam_pos;
                position.Text = cam_pos.ToString();
            }
            
            if (e.Key==Key.Up)
            {
                cam_vec.Y += 0.01;
                cam.LookDirection = cam_vec;
            }
            if (e.Key==Key.Left)
            {
                cam_vec.X -= 0.01;
                cam.LookDirection = cam_vec;
            }
            if (e.Key==Key.Down)
            {
                cam_vec.Y -= 0.01;
                cam.LookDirection = cam_vec;
            }
            if (e.Key==Key.Right)
            {
              cam_vec.X += 0.01;
              cam.LookDirection = cam_vec;
            }
        }
        }
    }
