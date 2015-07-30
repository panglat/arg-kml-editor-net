using SharpKml.Base;
using SharpKml.Dom;
using SharpKml.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ArgKmlEditorNet
{
    public class SelectionChangedEventArgs : EventArgs
    {
        public KMLFeatureTreeViewItem kmlFeatureTreeViewItem { get; set; }
    }

    public class KmlTreeViewController
    {
        KmlFile _kmlFile = null;
        TreeView _treeView = null;

        public event EventHandler<SelectionChangedEventArgs> TreeViewSelectionChanged;

        public void SetKML(KmlFile kmlFile) 
        {
            _kmlFile = kmlFile;
        }

        public void SetTreeView(TreeView treeView)
        {
            _treeView = treeView;
            treeView.SelectedItemChanged += SelectionChanged;
        }

        public void ProcessKml()
        {
            if (_kmlFile == null) /*|| _treeView == null)*/ return;
            Kml kml = _kmlFile.Root as Kml;
            if (kml != null) { 
                Feature feature = kml.Feature;
                TreeViewItem item = ProcessFeature(feature);
                _treeView.Items.Clear();
                _treeView.Items.Add(item);
            }
        }
        
        TreeViewItem ProcessFeature(Feature feature)
        {
            if (feature is Document)
            {
                return ProcessDocument(feature as Document);
            }
            else if (feature is Folder)
            {
                return ProcessFolder(feature as Folder);
            }
            else if (feature is Placemark)
            {
                return ProcessPlacemark(feature as Placemark);
            } 
            else if (feature is Container)
            {
                return ProcessContainer(feature as Container);
            } 
            return null;
        }

        TreeViewItem ProcessContainer(Container container)
        {
            if (container is Document)
            {
                return ProcessDocument(container as Document);
            }
            
            return null;
        }

        TreeViewItem ProcessDocument(Document document)
        {
            string name = document.Name;
            KMLFeatureTreeViewItem item = new KMLFeatureTreeViewItem()
            {
                Header = name,
                Feature = document
            };

            IEnumerable<Feature> features = document.Features;
            features.ToList().ForEach(feature => {
                TreeViewItem node = ProcessFeature(feature);
                if (node != null)
                {
                    item.Items.Add(node);
                }
            });

            return item;
        }

        TreeViewItem ProcessFolder(Folder folder)
        {
            string name = folder.Name;
            KMLFeatureTreeViewItem item = new KMLFeatureTreeViewItem()
            {
                Header = name,
                Feature = folder
            };

            IEnumerable<Feature> features = folder.Features;
            features.ToList().ForEach(feature =>
            {
                TreeViewItem node = ProcessFeature(feature);
                if (node != null)
                {
                    item.Items.Add(node);
                }
            });

            return item;
        }

        Style FindStyleByStyleURL(string styleUrl)
        {
            Style style = null;
            if (!String.IsNullOrEmpty(styleUrl))
            {
                if (styleUrl.StartsWith("#"))
                {
                    styleUrl = styleUrl.Substring(1);
                }

                SharpKml.Dom.StyleSelector styleSelector = _kmlFile.Styles.FirstOrDefault(s => s.Id == styleUrl);
                if (styleSelector != null && styleSelector is StyleMapCollection)
                {
                    StyleMapCollection styleMapCollection = styleSelector as StyleMapCollection;
                    styleMapCollection.ToList().ForEach(element =>
                    {
                        if (element is Pair)
                        {
                            Pair pair = element as Pair;
                            if (pair.State != null && pair.State == StyleState.Highlight)
                            {
                                string styleUrl2 = pair.StyleUrl.OriginalString;
                                if(!String.IsNullOrEmpty(styleUrl2)) {
                                    if (styleUrl2.StartsWith("#"))
                                    {
                                        styleUrl2 = styleUrl2.Substring(1);
                                    }
                                    SharpKml.Dom.StyleSelector styleSelector2 = _kmlFile.Styles.FirstOrDefault(s => s.Id == styleUrl2);
                                    if (styleSelector2 != null && styleSelector2 is Style)
                                    {
                                        style = styleSelector2 as Style;
                                    }
                                }
                            }
                        }
                    });
                }
            }
            return style;
        }

        class ImageByUri
        {
            public BitmapImage Bi { get; set; }
            public Uri Uri { get; set; }
        }

        List<ImageByUri> imageByUriList = new List<ImageByUri>();

        BitmapImage FindImageByUri(Uri uri)
        {
            if (uri != null)
            {
                ImageByUri imageByUri = imageByUriList.FirstOrDefault(iu => iu.Uri.Equals(uri));
                if (imageByUri == null)
                {
                    BitmapImage bi = new BitmapImage();
                    // BitmapImage.UriSource must be in a BeginInit/EndInit block.
                    bi.DownloadFailed += (sender, args) =>
                    {
                        if (sender is BitmapImage)
                        {
                            BitmapImage bit = sender as BitmapImage;
                        }
                    };

                    if (uri != null)
                    {
                        // BitmapImage.UriSource must be in a BeginInit/EndInit block.
                        bi.BeginInit();
                        bi.UriSource = uri;
                        bi.EndInit();
                    }

                    imageByUri = new ImageByUri()
                    {
                        Bi = bi,
                        Uri = uri
                    };
                    imageByUriList.Add(imageByUri);
                }
                return imageByUri.Bi;
            }
            else
            {
                return null;
            }
        }

        TreeViewItem ProcessPlacemark(Placemark placemark)
        {
            string name = placemark.Name;
            StackPanel pan = new StackPanel();
            pan.Orientation = System.Windows.Controls.Orientation.Horizontal;

            Style style = FindStyleByStyleURL(placemark.StyleUrl.OriginalString);

            if (placemark.Geometry is Point)
            {
                Uri uri = null;
                if (style != null && style.Icon != null && style.Icon.Icon != null && style.Icon.Icon.Href != null)
                {
                    uri = style.Icon.Icon.Href;
                }

                Image image = new Image();
                image.Height = 16;
                image.Source = FindImageByUri(uri);
                pan.Children.Add(image);
            }
            else if (placemark.Geometry is LineString)
            {
                GeometryGroup Lines = new GeometryGroup();

                Color32 styleColor = new Color32();
                if (style != null && style.Line != null && style.Line.Color != null)
                {
                    styleColor = (Color32)style.Line.Color;
                }

                // Line
                LineGeometry line = new LineGeometry();
                line.StartPoint = new System.Windows.Point(0, 5);
                line.EndPoint = new System.Windows.Point(10, 5);
                Lines.Children.Add(line);
                GeometryDrawing MyGeometryDrawing = new GeometryDrawing();
                MyGeometryDrawing.Geometry = Lines;
                MyGeometryDrawing.Brush = new SolidColorBrush(Color.FromArgb(styleColor.Alpha, styleColor.Red, styleColor.Green, styleColor.Blue));
                MyGeometryDrawing.Pen = new Pen(MyGeometryDrawing.Brush, 1);
                DrawingImage drawingImage = new DrawingImage(MyGeometryDrawing);
                drawingImage.Freeze();
                Image image = new Image();
                image.Height = 16;
                image.Width = 16;
                image.Source = drawingImage;
                pan.Children.Add(image);
            }

            TextBlock textBlock = new TextBlock();
            textBlock.Text = name;
            textBlock.Margin = new System.Windows.Thickness(4, 0, 0, 0);
            pan.Children.Add(textBlock);

            KMLFeatureTreeViewItem item = new KMLFeatureTreeViewItem()
            {
                Header = pan,
                Feature = placemark
            };
            return item;
        }

        protected virtual void OnTreeViewSelectionChanged(SelectionChangedEventArgs e)
        {
            EventHandler<SelectionChangedEventArgs> handler = TreeViewSelectionChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        private void SelectionChanged(object sender, System.Windows.RoutedPropertyChangedEventArgs<Object> e)
        {
            OnTreeViewSelectionChanged(new SelectionChangedEventArgs() { kmlFeatureTreeViewItem = (KMLFeatureTreeViewItem)((TreeView)sender).SelectedItem });
        }
    }
}
