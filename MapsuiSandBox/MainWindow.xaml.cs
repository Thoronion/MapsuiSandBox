using Mapsui.Layers;
using Mapsui.Nts.Providers;
using Mapsui.Projections;
using Mapsui.Rendering;
using Mapsui.Styles;
using Mapsui.Tiling.Layers;
using Mapsui.UI;
using Mapsui.Widgets;
using Mapsui;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Mapsui.Rendering.Skia;
using Mapsui.Rendering.Skia.Cache;
using Mapsui.Nts.Extensions;
using Mapsui.Tiling;
using Mapsui.UI.WinUI;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace MapsuiSandBox
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();
            Setup(mapControl);
        }


        private Map? _map;

        public void Setup(IMapControl mapControl)
        {
            mapControl.Map = CreateMap();
        }

        public Map CreateMap()
        {
            DefaultRendererFactory.Create = () => new MapRenderer();
            _map?.Dispose();
            _map = new Map();
            _map.Layers.Add(Mapsui.Tiling.OpenStreetMap.CreateTileLayer());

            return _map;
        }

        private void ChangeColor(object? sender, WidgetTouchedEventArgs e)
        {
            var layer = (_map?.Layers)?.First(f => f is RasterizingTileLayer) as RasterizingTileLayer;
            var random = new Random();
            // random color
            Color color = new Color(random.Next(255), random.Next(255), random.Next(255));
            layer!.SourceLayer.Style = new VectorStyle
            {
                Fill = new Mapsui.Styles.Brush(color),
            };
            layer.ClearCache();
        }

        public static ILayer CreatePolygonLayer()
        {
            return new Layer("Polygons")
            {
                DataSource = new IndexedMemoryProvider(CreatePolygon().ToFeatures()),
                Style = new VectorStyle
                {
                    Fill = new Mapsui.Styles.Brush(Color.Red),
                }
            };
        }

        private static List<Polygon> CreatePolygon()
        {
            var result = new List<Polygon>();

            Polygon polygon1;
            int factor = 0;

            for (int i = 0; i < 100000; i++)
            {
                factor = i - 100 * (int)Math.Round((double)(i / 100));
                polygon1 = new Polygon(
                    new LinearRing(new[] {
                    new Coordinate(1000*(factor-1), 1000*(factor-1)-(Math.Round((double)(i/100))*1000)),
                    new Coordinate(1000*(factor-1), 1000*(factor)-(Math.Round((double)(i/100))*1000)),
                    new Coordinate(1000*(factor), 1000*(factor)-(Math.Round((double)(i/100))*1000)),
                    new Coordinate(1000*(factor), 1000*(factor-1)-(Math.Round((double)(i/100))*1000)),
                    new Coordinate(1000*(factor-1), 1000*(factor-1)-(Math.Round((double)(i/100))*1000))
                    }));

                result.Add(polygon1);
            }
            return result;
        }

        public void Dispose()
        {
            _map?.Dispose();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var tileLayer = _map.Layers.FindLayer("Polygons").FirstOrDefault();
            if (tileLayer is not null)
            {
                _map.Layers.Remove(tileLayer);
            }

            _map.Layers.Insert(1, new RasterizingTileLayer(CreatePolygonLayer()));
        }
    }
}
