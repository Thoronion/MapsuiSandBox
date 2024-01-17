using Mapsui.Layers;
using Mapsui.Nts.Providers;
using Mapsui.Styles;
using Mapsui.Tiling.Layers;
using Mapsui;
using Microsoft.UI.Xaml;
using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Linq;
using Mapsui.Nts.Extensions;
using Mapsui.Tiling;

namespace MapsuiSandBox;

/// <summary>
/// An empty window that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class MainWindow : Window
{
    private readonly Map _map = new();

    public MainWindow()
    {
        InitializeComponent();

        _map.Layers.Add(OpenStreetMap.CreateTileLayer());
        mapControl.Map = _map;
    }

    public static ILayer CreatePolygonLayer()
    {
        return new Layer("Polygons")
        {
            DataSource = new IndexedMemoryProvider(CreatePolygon().ToFeatures()),
            Style = new VectorStyle
            {
                Fill = new Brush(Color.Red),
            }
        };
    }

    private static List<Polygon> CreatePolygon()
    {
        var result = new List<Polygon>();

        Polygon polygon1;
        int factor;

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
